/********************************************************************
created:    2023-07-04
author:     lixianmin

策划指南:
1. 新建一个ScrollView, 然后将本script加到ScrollView组件节点上. script只使用ScrollRect, 与Scrollbar无关, 可以把其下的2个Scrollbar删除且并不影响功能
2. Viewport实现了遮罩关系, 因此UILoopScrollRect使用Viewport大小计算cell的排版; 初始化时也会将Viewport大小copy到Content节点上, 然后会自动计算滑动方向上Content的实际长度
   可以设置Viewport为stretch mode, 这样就是使用ScrollRect节点的sizeDelta计算cell排版了
3. 一个典型的设置是只设置ScrollView根节点的大小, 而把Viewport与Content均设置为stretch mode (其中Content的对齐方式会在初始化自动设置为左上角对齐)

程序细节:
1. 代码启动时会自动设置Content与cellTransform节点的anchor类型为"左上", 代码以这个为基础计算显隐关系
2. 这个script目前不适用于运行时AddComponent<>到UI上, 因为Awake()方法会使用cellTransform

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.Collections;
using UnityEngine;
using UnityEngine.UI;
using Direction = UnityEngine.UI.Scrollbar.Direction;

namespace Unicorn.UI
{
    [RequireComponent(typeof(UIScrollRect))]
    [DisallowMultipleComponent]
    public partial class UILoopScrollRect : MonoBehaviour
    {
        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _direction = DirBase.Create(direction);

            _InitCellTransform();
            _InitViewportAndContent();
            _InitRank();
            _ResetContentArea();
        }

        private void _InitCellTransform()
        {
            var trans = cellTransform;
            trans.gameObject.SetActive(false);

            // 设置对齐方式为左上角
            trans.anchorMin = Vector2.up;
            trans.anchorMax = Vector2.up;
        }

        private void _InitViewportAndContent()
        {
            var rectContent = _scrollRect.content;
            _contentTransform = rectContent;

            // 设置Content对齐方式为左上角
            rectContent.anchorMin = Vector2.up;
            rectContent.anchorMax = Vector2.up;

            // var rectAncestor = rectContent.parent as RectTransform;
            // // 如果是stretch mode, 就取其父节点
            // while (rectAncestor != null && rectAncestor.anchorMin != rectAncestor.anchorMax)
            // {
            //     rectAncestor = rectAncestor.parent as RectTransform;
            // }
            //
            // if (rectAncestor == null)
            // {
            //     Logo.Warn($"Can not find rectAncestor, root={_contentTransform.root}");
            //     return;
            // }

            // 这个取的应该就是UILoopScrollRect所在的节点, 上一版本按stretch mode取, 如果它自己也设置成了stretch, 就取到上面的节点去了
            var rectAncestor = transform as RectTransform;
            // copy其viewport的sizeDelta到content
            var size = rectAncestor!.sizeDelta;
            if (size.x.IsZero() || size.y.IsZero())
            {
                Logo.Warn($"size={size}, rectAncestor={rectAncestor}, root={_contentTransform.root}");
                return;
            }

            rectContent.sizeDelta = size;

            // 初始化_viewportArea
            _viewportArea = new Rect(0, -size.y, size.x, size.y);
        }

        private void _InitRank()
        {
            var rank = 0;
            var sizeDelta = cellTransform.sizeDelta;
            if ((direction is Direction.LeftToRight or Direction.RightToLeft) && sizeDelta.y > 0)
            {
                rank = (int)(_contentTransform.sizeDelta.y / sizeDelta.y);
            }
            else if (sizeDelta.x > 0)
            {
                rank = (int)(_contentTransform.sizeDelta.x / sizeDelta.x);
            }

            if (rank <= 0)
            {
                rank = 1;
            }

            _rank = rank;
        }

        private Rect _GetRelativeViewportArea()
        {
            var area = _viewportArea;
            var moved = _contentTransform.anchoredPosition;
            area.x -= moved.x;
            area.y -= moved.y;

            return area;
        }

        private void OnDestroy()
        {
            RemoveAllCells();
            _ClearPool();
            // Logo.Info("loop scroll rect is destroying");
        }

        private void Update()
        {
            var anchoredPosition = _contentTransform.anchoredPosition;
            _isDirty |= _lastAnchoredPosition != anchoredPosition;
            _lastAnchoredPosition = anchoredPosition;

            if (_isDirty)
            {
                _isDirty = false;
                _ResetContentArea();
                _RefreshCellsVisibility();
            }
        }

        private void _RefreshCellsVisibility()
        {
            var relativeArea = _GetRelativeViewportArea();
            foreach (Cell cell in _cells)
            {
                var lastVisible = cell!.IsVisible();
                var nextVisible = relativeArea.Overlaps(cell.GetArea());
                if (lastVisible != nextVisible)
                {
                    if (nextVisible)
                    {
                        _ShowCell(cell);
                    }
                    else
                    {
                        _HideCell(cell);
                    }
                }
            }
        }

        private void _ResetContentArea()
        {
            var cellSize = cellTransform!.sizeDelta;
            var nonRankCount = (_cells.Count + _rank - 1) / _rank;

            var dir = _direction.GetDirection();
            if (dir is Direction.LeftToRight or Direction.RightToLeft)
            {
                var totalWidth = nonRankCount * cellSize.x;
                _contentTransform.sizeDelta = new Vector2(totalWidth, _contentTransform.sizeDelta.y);
            }
            else
            {
                var totalHeight = nonRankCount * cellSize.y;
                _contentTransform.sizeDelta = new Vector2(_contentTransform.sizeDelta.x, totalHeight);
            }
        }

        public void AddCell(IWidget widget)
        {
            if (cellTransform == null)
            {
                return;
            }

            var sizeDelta = cellTransform.sizeDelta;
            var index = _cells.Count;
            var areaPos = _direction.GetCellAreaPos(index, _rank, sizeDelta);

            var area = new Rect(areaPos.x, areaPos.y, sizeDelta.x, sizeDelta.y);
            var cell = new Cell(area, widget);

            var relativeArea = _GetRelativeViewportArea();
            var isVisible = relativeArea.Overlaps(area);
            if (isVisible)
            {
                _ShowCell(cell);
            }

            _cells.PushBack(cell);
            _SetDirty();
        }

        public void RemoveCell(int index)
        {
            var count = _cells.Count;
            if (index < 0 || index >= count)
            {
                return;
            }

            var lastIndex = count - 1;
            // 所有cell的transform均匀向前移动一格
            for (var i = lastIndex; i > index; i--)
            {
                var back = _cells[i] as Cell;
                var front = _cells[i - 1] as Cell;
                back!.CopyFrom(front);
            }

            var firstCell = _cells[index] as Cell;
            _HideCell(firstCell);

            _cells.RemoveAt(index);
            _SetDirty();
        }

        public void RemoveAllCells()
        {
            var size = _cells.Count;
            if (size > 0)
            {
                for (var i = 0; i < size; i++)
                {
                    var cell = _cells[i] as Cell;
                    _HideCell(cell);
                }

                _cells.Clear();
                _SetDirty();
            }
        }

        private void _ClearPool()
        {
            var size = _goPool.Count;
            if (size > 0)
            {
                foreach (RectTransform rect in _goPool)
                {
                    Destroy(rect);
                }

                _goPool.Clear();
            }
        }

        private void _ShowCell(Cell cell)
        {
            if (cell != null && !cell.IsVisible())
            {
                cell.SetVisible(true);
                var rect = _SpawnCellTransform(cell);
                cell.SetTransform(rect);

                var widget = cell.GetWidget();
                widget?.OnVisibleChanged(cell);
            }
        }

        private void _HideCell(Cell cell)
        {
            if (cell != null && cell.IsVisible())
            {
                cell.SetVisible(false);
                // 确保所有的OnVisibleChanged事件中, Transform都是可用的, 方便client设置一些东西
                var widget = cell.GetWidget();
                widget?.OnVisibleChanged(cell);

                // 逻辑可保证只有visible的cell才有transform, 才需要回收
                var rect = cell.GetTransform();
                cell.SetTransform(null);
                _RecycleCellTransform(rect);
            }
        }

        /// <summary>
        /// 重置anchoredPosition到初始化的位置
        /// </summary>
        private void _ResetAnchoredPosition()
        {
            _ResetContentArea();
            var dir = _direction.GetDirection();
            switch (dir)
            {
                case Direction.LeftToRight:
                    _contentTransform.anchoredPosition = new Vector2(0, 0);
                    break;
                case Direction.RightToLeft:
                    var posX = -_contentTransform.sizeDelta.x + _viewportArea.width;
                    _contentTransform.anchoredPosition = new Vector2(posX, 0);
                    break;
                case Direction.BottomToTop:
                    _contentTransform.anchoredPosition = new Vector2(0, 0);
                    break;
                case Direction.TopToBottom:
                    var posY = _contentTransform.sizeDelta.y - _viewportArea.height;
                    _contentTransform.anchoredPosition = new Vector2(0, posY);
                    break;
            }
        }

        private void _SetDirty()
        {
            _isDirty = true;
        }

        private RectTransform _SpawnCellTransform(Cell cell)
        {
            if (_goPool.Count > 0)
            {
                var last = _goPool.PopBack() as RectTransform;
                _OnSpawnCellTransform(last, cell);
                return last;
            }

            var go = Instantiate(cellTransform.gameObject, _contentTransform);
            var rect = go.transform as RectTransform;
            _OnSpawnCellTransform(rect, cell);
            return rect;
        }

        private void _OnSpawnCellTransform(RectTransform rect, Cell cell)
        {
            rect!.anchoredPosition = _direction.GetTransformAnchoredPos(cell);
            // Logo.Info($"anchoredPosition={trans.anchoredPosition}");
            rect.gameObject.SetActive(true);
        }

        private void _RecycleCellTransform(RectTransform rect)
        {
            if (rect is not null)
            {
                var go = rect.gameObject;
                go.SetActive(false);
                _goPool.PushBack(rect);
            }
        }

        public Cell GetCell(int index)
        {
            if (index >= 0 && index < _cells.Count)
            {
                var cell = _cells[index] as Cell;
                return cell;
            }

            return null;
        }

        public int GetCellCount()
        {
            return _cells.Count;
        }

        public RectTransform cellTransform;
        public Direction direction = Direction.BottomToTop;

        private ScrollRect _scrollRect;
        private RectTransform _contentTransform;
        private Rect _viewportArea;
        private DirBase _direction;
        private int _rank; // 选定scroll的方向后, 每一排的cell个数

        private bool _isDirty;
        private Vector2 _lastAnchoredPosition;

        private readonly Deque _cells = new();
        private readonly Deque _goPool = new();
    }
}