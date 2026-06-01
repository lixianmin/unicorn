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

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*********************************************************************/

using Unicorn.Collections;
using UnityEngine;
using UnityEngine.UI;
using Direction = UnityEngine.UI.Scrollbar.Direction;

namespace Unicorn.UI
{
    [RequireComponent(typeof(UIScrollRect))]
    [DisallowMultipleComponent]
    [UnityEngine.Scripting.Preserve]
    public partial class UILoopScrollRect : MonoBehaviour
    {
        private void Awake()
        {
            if (cellTransform == null)
            {
                Logo.Error($"[Awake] cellTransform=null, UI={transform.GetFindPath()}");
                return;
            }

            _scrollRect = GetComponent<ScrollRect>();
            _direction = DirBase.Create(direction);

            _InitCellTransform();
            _InitViewportAndContent();
            _InitRank();

            _layoutStrategy = dynamicHeight ? new DynamicLayoutStrategy(this) : new StaticLayoutStrategy(this);
            _layoutStrategy.BuildContentArea();

            Logo.Info(
                $"[Awake] _viewportArea={_viewportArea} _rank={_rank} _contentTransform.sizeDelta={_contentTransform.sizeDelta}");
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
            var contentTransform = _scrollRect.content;
            _contentTransform = contentTransform;

            // 设置Content对齐方式为左上角
            contentTransform.anchorMin = Vector2.up;
            contentTransform.anchorMax = Vector2.up;
            // 设置pivot为 (0, 1), 至少在uishop中需要这样子
            contentTransform.pivot = Vector2.up;

            // selfSize = UILoopScrollRect 节点的实际尺寸，即用户可见区域
            var self = transform as RectTransform;
            var selfSize = self!.sizeDelta;

            // vpSize = ScrollRect viewport 的 rect 尺寸，ScrollRect 用此计算滚动范围
            var vp = _scrollRect.viewport != null ? _scrollRect.viewport : self;
            var vpSize = new Vector2(vp.rect.width, vp.rect.height);

            if (selfSize.x <= 0 || selfSize.y <= 0)
            {
                Logo.Warn($"[_InitViewportAndContent] selfSize={selfSize}, {name}不能是stretch mode");
                return;
            }

            // content 初始大小设为 viewport 大小，确保 ScrollRect 的 bounds 计算正确
            contentTransform.sizeDelta = vpSize;

            // _viewportArea 用 selfSize 管理 cell 可见性（实际可见区域）
            _viewportArea = new Rect(0, -selfSize.y, selfSize.x, selfSize.y);

            // 记录额外的 padding = viewport 超出可见区域的部分
            _extraPaddingY = vpSize.y - selfSize.y;
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
            RemoveAll();
            _ClearPool();
        }

        private void Update()
        {
            if (!_contentTransform)
            {
                Logo.Error($"[Update] _contentTransform=null, UI={transform.GetFindPath()}");
                enabled = false;
                return;
            }

            var anchoredPosition = _contentTransform.anchoredPosition;
            _viewportDirty |= _lastAnchoredPosition != anchoredPosition;
            _lastAnchoredPosition = anchoredPosition;

            if (_contentDirty)
            {
                _contentDirty = false;

                var oldContentY = _contentTransform.sizeDelta.y;
                _layoutStrategy.BuildContentArea();
                var newContentY = _contentTransform.sizeDelta.y;

                if (!Mathf.Approximately(oldContentY, newContentY))
                {
                    _layoutStrategy.OnContentSizeChanged();
                }
            }

            if (_viewportDirty)
            {
                _viewportDirty = false;
                _RefreshCellsVisibility();

                if (_layoutStrategy.NeedsContentRebuildAfterRefresh())
                {
                    _contentDirty = false;
                    _layoutStrategy.BuildContentArea();
                    _layoutStrategy.OnContentSizeChanged();
                }
            }
        }

        private void _RefreshCellsVisibility()
        {
            var relativeArea = _GetRelativeViewportArea();
            foreach (CellBase cell in _cells)
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

        public void AddCell(CellBase cell)
        {
            if (cell == null)
            {
                return;
            }

            const string method = nameof(AddCell);
            if (cellTransform == null)
            {
                Logo.Warn($"[{method}] cellTransform=null");
                return;
            }

            if (_direction == null)
            {
                Logo.Warn($"[{method}] _direction=null, the script should Awake()");
                return;
            }

            var index = _cells.Count;
            var area = _layoutStrategy.ComputeCellArea(index, cell);

            cell.SetArea(area);

            var relativeArea = _GetRelativeViewportArea();
            var isVisible = relativeArea.Overlaps(area);
            if (isVisible)
            {
                _ShowCell(cell);
            }

            _cells.PushBack(cell);
            _SetDirty();

            if (isDebugging)
            {
                Logo.Info($"[{method}] relativeArea={relativeArea} area={area} isVisible={isVisible}");
            }
        }

        public void RemoveCell(int index)
        {
            var count = _cells.Count;
            if (index < 0 || index >= count)
            {
                return;
            }

            var lastIndex = count - 1;
            for (var i = lastIndex; i > index; i--)
            {
                var back = _cells[i] as CellBase;
                var front = _cells[i - 1] as CellBase;
                back!.CopyFrom(front);
            }

            var firstCell = _cells[index] as CellBase;
            _HideCell(firstCell);
            firstCell!.Dispose();

            _cells.RemoveAt(index);
            _SetDirty();
        }

        public void RemoveAll()
        {
            var size = _cells.Count;
            if (size > 0)
            {
                for (var i = 0; i < size; i++)
                {
                    var cell = _cells[i] as CellBase;
                    _HideCell(cell);
                    cell!.Dispose();
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

        private void _ShowCell(CellBase cell)
        {
            if (cell != null && !cell.IsVisible())
            {
                cell.SetVisible(true);
                var rect = _SpawnCellTransform(cell);
                cell.SetTransform(rect);
                cell.InnerOnVisibleChanged();

                _layoutStrategy.OnCellShown();
            }
        }

        private void _HideCell(CellBase cell)
        {
            if (cell != null && cell.IsVisible())
            {
                cell.SetVisible(false);
                cell.InnerOnVisibleChanged();

                var rect = cell.GetTransform();
                cell.SetTransform(null);
                _RecycleCellTransform(rect);
            }
        }

        private void _SetDirty()
        {
            _contentDirty = true;
            _viewportDirty = true;
        }

        private RectTransform _SpawnCellTransform(CellBase cell)
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

        private void _OnSpawnCellTransform(RectTransform rect, CellBase cell)
        {
            rect!.anchoredPosition = _direction.GetTransformAnchoredPos(cell);
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

        public CellBase GetCell(int index)
        {
            if (index >= 0 && index < _cells.Count)
            {
                var cell = _cells[index] as CellBase;
                return cell;
            }

            return null;
        }

        public int GetCount()
        {
            return _cells.Count;
        }

        public RectTransform cellTransform;
        public Direction direction = Direction.BottomToTop;

        /// <summary>
        /// 勾选后启用可变高度模式：使用 DynamicLayoutStrategy，通过 CellBase.GetCellHeight() 获取每个 cell 的实际排版高度。
        /// 不勾选时使用 StaticLayoutStrategy，沿用固定高度 grid/rank 公式。
        /// </summary>
        public bool dynamicHeight;
        
        public bool isDebugging;

        private ScrollRect _scrollRect;
        private RectTransform _contentTransform;
        private Rect _viewportArea;
        private float _extraPaddingY;
        private DirBase _direction;
        private int _rank; // 选定scroll的方向后, 每一排的cell个数

        private LayoutStrategy _layoutStrategy;

        private bool _contentDirty;
        private bool _viewportDirty;
        private Vector2 _lastAnchoredPosition;

        private readonly Deque _cells = new();
        private readonly Deque _goPool = new();
    }
}