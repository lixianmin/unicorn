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
using UnityEngine.Events;
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

            var rectAncestor = rectContent.parent as RectTransform;
            // 如果是stretch mode, 就取其父节点
            while (rectAncestor != null && rectAncestor.anchorMin != rectAncestor.anchorMax)
            {
                rectAncestor = rectAncestor.parent as RectTransform;
            }

            if (rectAncestor != null)
            {
                // copy其viewport的sizeDelta到content
                var size = rectAncestor.sizeDelta;
                rectContent.sizeDelta = size;

                // 初始化_viewportArea
                _viewportArea = new Rect(0, -size.y, size.x, size.y);
            }
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
            _RemoveAllCells();
            Logo.Info("loop scroll rect is destroying");
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
                var currentVisible = relativeArea.Overlaps(cell.GetArea());
                if (lastVisible != currentVisible)
                {
                    cell.SetVisible(currentVisible);
                    if (currentVisible)
                    {
                        var go = _SpawnCellGameObject(cell);
                        cell.SetTransform(go.transform as RectTransform);
                        _OnCellVisibleChanged(cell);
                    }
                    else
                    {
                        // 这样可确保所有的OnVisibleChanged事件中, Transform都是可用的, 方便client设置一些东西
                        _OnCellVisibleChanged(cell);
                        var trans = cell.GetTransform();
                        cell.SetTransform(null);
                        _RecycleCellGameObject(trans.gameObject);
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
            var cell = new Cell(index, area, widget);

            var relativeArea = _GetRelativeViewportArea();
            var isVisible = relativeArea.Overlaps(area);
            if (isVisible)
            {
                var go = _SpawnCellGameObject(cell);
                cell.SetTransform(go.transform as RectTransform);
                cell.SetVisible(true);
                _OnCellVisibleChanged(cell);
            }

            _cells.PushBack(cell);
            _SetDirty();
        }

        public void RemoveCell(int index)
        {
            if (index >= 0 && index < _cells.Count)
            {
                var lastCell = _cells[index] as Cell;
                _cells.RemoveAt(index);

                var trans = lastCell!.GetTransform();
                if (trans != null)
                {
                    _RecycleCellGameObject(trans.gameObject);
                }

                _SetDirty();
            }
        }

        private void _RemoveAllCells()
        {
            var size = _cells.Count;
            if (size > 0)
            {
                for (var i = 0; i < size; i++)
                {
                    var cell = _cells[i] as Cell;
                    var trans = cell!.GetTransform();
                    if (trans != null)
                    {
                        // todo 不知道这里, 会不会有可能 gameObject被destroy掉的问题, 需要测试. RemoveCell()有相同的风险
                        _RecycleCellGameObject(trans.gameObject);
                    }
                }

                _cells.Clear();
                _SetDirty();
            }
        }

        /// <summary>
        /// 重置anchoredPosition到初始化的位置
        /// </summary>
        public void ResetAnchoredPosition()
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

        private GameObject _SpawnCellGameObject(Cell cell)
        {
            if (_goPool.Count > 0)
            {
                var last = _goPool.PopBack() as GameObject;
                _OnSpawnCellGameObject(last, cell);
                return last;
            }

            var go = Instantiate(cellTransform.gameObject, _contentTransform);
            _OnSpawnCellGameObject(go, cell);
            return go;
        }

        private void _OnSpawnCellGameObject(GameObject go, Cell cell)
        {
            var trans = go.transform as RectTransform;
            trans!.anchoredPosition = _direction.GetTransformAnchoredPos(cell);
            go.SetActive(true);
        }

        private void _RecycleCellGameObject(GameObject go)
        {
            if (null != go)
            {
                go.SetActive(false);
                _goPool.PushBack(go);
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

        private void _OnCellVisibleChanged(Cell cell)
        {
            var widget = cell.GetWidget();
            widget?.OnVisibleChanged(cell);
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