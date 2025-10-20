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

using System;
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
            if (cellTransform == null)
            {
                Logo.Error($"cellTransform is null, UI={transform.GetFindPath()}");
                return;
            }
            
            _scrollRect = GetComponent<ScrollRect>();
            _direction = DirBase.Create(direction);

            _InitCellTransform();
            _InitViewportAndContent();
            _InitRank();
            _ResetContentArea();
            
            Logo.Info($"[{name}] _viewportArea={_viewportArea} _rank={_rank} _contentTransform.sizeDelta={_contentTransform.sizeDelta}");
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
            if (size.x <= 0 || size.y <= 0)
            {
                Logo.Warn($"[_InitViewportAndContent] size={size}, {name}不能是streth mode");
                return;
            }

            contentTransform.sizeDelta = size;

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
            RemoveAll();
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

        private void _ResetContentArea()
        {
            var cellSize = cellTransform!.sizeDelta;
            var nonRankCount = (_cells.Count + _rank - 1) / _rank;

            string k;
            float v;

            var dir = _direction.GetDirection();
            if (dir is Direction.LeftToRight or Direction.RightToLeft)
            {
                v = nonRankCount * cellSize.x;
                _contentTransform.sizeDelta = new Vector2(v, _contentTransform.sizeDelta.y);
                k = "totalWidth";
            }
            else
            {
                v = nonRankCount * cellSize.y;
                _contentTransform.sizeDelta = new Vector2(_contentTransform.sizeDelta.x, v);
                k = "totalHeight";
            }

            if (isDebugging)
            {
                const string method = nameof(_ResetContentArea);
                Logo.Info($"[{method}] {k}={v} dir={dir} _cells.Count={_cells.Count} _rank={_rank} nonRankCount={nonRankCount} cellSize={cellSize}");
                _PrintDirection(dir);
            }
        }

        private static void _PrintDirection(Direction direction)
        {
            const string method = nameof(_PrintDirection);
            switch (direction)
            {
                case Direction.BottomToTop:
                    Logo.Info($"[{method}] BottomToTop => 向下拉scrollbar的时候, 内容从bottom到top走");
                    break;
                case Direction.TopToBottom:
                    Logo.Info($"[{method}] TopToBottom => 向下拉scrollbar的时候, 内容从top到bottom走");
                    break;
                case Direction.LeftToRight:
                    Logo.Info($"[{method}] LeftToRight => 向右拉scrollbar的时候, 内容从left到right走");
                    break;
                case Direction.RightToLeft:
                    Logo.Info($"[{method}] RightToLeft => 向右拉scrollbar的时候, 内容从right到left走");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public void AddCell(CellBase cell)
        {
            if (cell == null)
            {
                return;
            }
            
            const string method = nameof(AddCell);
            // widget看起来可以是null
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

            var sizeDelta = cellTransform.sizeDelta;
            var index = _cells.Count;
            var areaPos = _direction.GetCellAreaPos(index, _rank, sizeDelta);

            // 1. cell就是数据层, 数量会很大. 因此cell使用哪一个transform是不确定的, 是显示的那一刻从pool中挖出来的
            var area = new Rect(areaPos.x, areaPos.y, sizeDelta.x, sizeDelta.y);
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
            // 所有cell的transform均匀向前移动一格
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
            }
        }

        private void _HideCell(CellBase cell)
        {
            if (cell != null && cell.IsVisible())
            {
                cell.SetVisible(false);
                // 确保所有的OnVisibleChanged事件中, Transform都是可用的, 方便client设置一些东西
                cell.InnerOnVisibleChanged();

                // 逻辑可保证只有visible的cell才有transform, 才需要回收
                var rect = cell.GetTransform();
                cell.SetTransform(null);
                _RecycleCellTransform(rect);
            }
        }

        /// <summary>
        /// 重置anchoredPosition到初始化的位置
        /// </summary>
        // private void _ResetAnchoredPosition()
        // {
        //     _ResetContentArea();
        //     var dir = _direction.GetDirection();
        //     switch (dir)
        //     {
        //         case Direction.LeftToRight:
        //             _contentTransform.anchoredPosition = new Vector2(0, 0);
        //             break;
        //         case Direction.RightToLeft:
        //             var posX = -_contentTransform.sizeDelta.x + _viewportArea.width;
        //             _contentTransform.anchoredPosition = new Vector2(posX, 0);
        //             break;
        //         case Direction.BottomToTop:
        //             _contentTransform.anchoredPosition = new Vector2(0, 0);
        //             break;
        //         case Direction.TopToBottom:
        //             var posY = _contentTransform.sizeDelta.y - _viewportArea.height;
        //             _contentTransform.anchoredPosition = new Vector2(0, posY);
        //             break;
        //     }
        // }

        private void _SetDirty()
        {
            _isDirty = true;
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
        public bool isDebugging;

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