/********************************************************************
created:    2026-05-30
author:     agent

设计目标：将 cell 排版逻辑抽取为策略模式。
         FixedLayoutStrategy 保持原有固定高度逻辑不变。
         VariableLayoutStrategy 处理可变高度场景，通过 CellBase.GetCellHeight()
         获取每个 cell 的实际排版高度。

UILoopScrollRect 根据 variableHeight 字段自动选择策略。
*********************************************************************/

using Unicorn.Collections;
using UnityEngine;
using Direction = UnityEngine.UI.Scrollbar.Direction;

namespace Unicorn.UI
{
    partial class UILoopScrollRect
    {
        /// <summary>
        /// cell 排版策略抽象。子类实现固定高度 / 可变高度两种排版算法。
        /// </summary>
        internal abstract class LayoutStrategy
        {
            protected LayoutStrategy(UILoopScrollRect owner)
            {
                _owner = owner;
            }

            /// <summary>AddCell 时计算新 cell 的 area</summary>
            public abstract Rect ComputeCellArea(int index, CellBase cell);

            /// <summary>_ResetContentArea：重算所有 cell area 并设置 content.sizeDelta</summary>
            public abstract void BuildContentArea();

            /// <summary>_ShowCell 后的回调，策略可在此触发额外逻辑</summary>
            public virtual void OnCellShown()
            {
            }

            /// <summary>_RefreshCellsVisibility 完成后判断是否需要再跑一次 BuildContentArea</summary>
            public virtual bool NeedsContentRebuildAfterRefresh()
            {
                return false;
            }

            /// <summary>content height 变化后的回调，用于重定位可见 cell</summary>
            public virtual void OnContentSizeChanged()
            {
            }

            protected readonly UILoopScrollRect _owner;
        }

        /// <summary>固定高度策略：沿用原始的 rank * cellSize 公式，完全不调用 GetCellHeight()</summary>
        internal sealed class StaticLayoutStrategy : LayoutStrategy
        {
            public StaticLayoutStrategy(UILoopScrollRect owner) : base(owner)
            {
            }

            public override Rect ComputeCellArea(int index, CellBase cell)
            {
                var sizeDelta = _owner.cellTransform.sizeDelta;
                var areaPos = _owner._direction.GetCellAreaPos(index, _owner._rank, sizeDelta);
                return new Rect(areaPos.x, areaPos.y, sizeDelta.x, sizeDelta.y);
            }

            public override void BuildContentArea()
            {
                var cellSize = _owner.cellTransform.sizeDelta;
                var dir = _owner._direction.GetDirection();
                var isVertical = dir is Direction.BottomToTop or Direction.TopToBottom;
                var nonRankCount = (_owner._cells.Count + _owner._rank - 1) / _owner._rank;

                float contentSize;
                string k;
                if (isVertical)
                {
                    contentSize = nonRankCount * cellSize.y;
                    _owner._contentTransform.sizeDelta = new Vector2(_owner._contentTransform.sizeDelta.x, contentSize + _owner._extraPaddingY);
                    k = "totalHeight";
                }
                else
                {
                    contentSize = nonRankCount * cellSize.x;
                    _owner._contentTransform.sizeDelta = new Vector2(contentSize, _owner._contentTransform.sizeDelta.y);
                    k = "totalWidth";
                }

                if (_owner.isDebugging)
                {
                    const string method = nameof(BuildContentArea);
                    Logo.Info(
                        $"[{method}] {k}={contentSize} dir={dir} _cells.Count={_owner._cells.Count} _rank={_owner._rank} cellTransform.sizeDelta={cellSize}");
                }
            }
        }

        /// <summary>
        /// 可变高度策略：通过 CellBase.GetCellHeight() 获取每个 cell 的实际排版高度。
        /// AddCell 和 BuildContentArea 均按实际高度累积计算，不使用 rank 公式。
        /// </summary>
        internal sealed class DynamicLayoutStrategy : LayoutStrategy
        {
            public DynamicLayoutStrategy(UILoopScrollRect owner) : base(owner)
            {
            }

            public override Rect ComputeCellArea(int index, CellBase cell)
            {
                var sizeDelta = _owner.cellTransform.sizeDelta;
                var offsetY = 0f;
                for (var i = 0; i < index; i++)
                {
                    var h = ((CellBase)_owner._cells[i]).GetCellHeight();
                    offsetY += h > 0 ? h : sizeDelta.y;
                }

                var cellHeight = cell.GetCellHeight();
                if (cellHeight <= 0)
                {
                    cellHeight = sizeDelta.y;
                }

                return new Rect(0, -offsetY - cellHeight, sizeDelta.x, cellHeight);
            }

            public override void BuildContentArea()
            {
                var cellSize = _owner.cellTransform.sizeDelta;
                var dir = _owner._direction.GetDirection();
                var isVertical = dir is Direction.BottomToTop or Direction.TopToBottom;

                var contentSize = 0f;
                var offsetY = 0f;
                foreach (CellBase cell in _owner._cells)
                {
                    var h = cell.GetCellHeight();
                    var cellHeight = h > 0 ? h : cellSize.y;

                    cell.SetArea(new Rect(0, -offsetY - cellHeight, cellSize.x, cellHeight));
                    offsetY += cellHeight;
                    contentSize += cellHeight;
                }

                _owner._contentTransform.sizeDelta = isVertical
                    ? new Vector2(_owner._contentTransform.sizeDelta.x, contentSize + _owner._extraPaddingY)
                    : new Vector2(contentSize, _owner._contentTransform.sizeDelta.y);

                if (_owner.isDebugging)
                {
                    var k = isVertical ? "totalHeight" : "totalWidth";
                    Logo.Info(
                        $"[BuildContentArea] {k}={contentSize} dir={dir} _cells.Count={_owner._cells.Count} cellTransform.sizeDelta={cellSize}");
                }
            }

            /// <summary>
            /// cell 被 _ShowCell 激活后，OnBecameVisible 可能更新了实际高度。
            /// 标记 content 需要重算以同步 area。
            /// </summary>
            public override void OnCellShown()
            {
                _owner._contentDirty = true;
            }

            /// <summary>
            /// _RefreshCellsVisibility 后需要检查是否因 cell 高度更新需要重算 area。
            /// </summary>
            public override bool NeedsContentRebuildAfterRefresh()
            {
                return _owner._contentDirty;
            }

            /// <summary>
            /// content height 变化后，重新定位所有可见 cell 的 anchoredPosition。
            /// </summary>
            public override void OnContentSizeChanged()
            {
                foreach (CellBase cell in _owner._cells)
                {
                    if (cell.IsVisible())
                    {
                        var rect = cell.GetTransform();
                        if (rect != null)
                        {
                            rect.anchoredPosition = _owner._direction.GetTransformAnchoredPos(cell);
                        }
                    }
                }
            }
        }
    }
}
