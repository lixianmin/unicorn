/********************************************************************
created:    2023-07-04
author:     lixianmin

策划指南:
1. 新建一个ScrollView, 然后将本script加到ScrollView组件节点上. script只使用ScrollRect, 与Scrollbar无关, 可以把其下的2个Scrollbar删除且并不影响功能
2. ScrollView/ScrollRect的大小为viewport的大小, 因为ScrollView的大小在Scene窗口中是可见的, 方便策划编辑

程序细节:
1. Content节点初始化时会copy其ScrollRect节点的的Width与Height, 然后自动计算滑动方向上的实际长度
2. 代码启动时会自动设置Content与cellTransform节点的anchor类型为"左上", 代码以这个为基础计算显隐关系
3. 这个script目前不适用于运行时AddComponent<>到UI上, 因为Awake()方法会使用cellTransform

Copyright (C) - All Rights Reserved
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
            _scrollRect = GetComponent<ScrollRect>();
            _direction = DirBase.Create(direction);

            _InitCellTransform();
            _InitContentTransform();
            _InitRank();
            _InitViewport();
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

        private void _InitContentTransform()
        {
            var trans = _scrollRect.content;
            _contentTransform = trans;

            // 设置对齐方式为左上角
            trans.anchorMin = Vector2.up;
            trans.anchorMax = Vector2.up;

            // copy其scrollRect根节点的sizeDelta
            var ancestor = _scrollRect.transform as RectTransform;
            trans.sizeDelta = ancestor!.sizeDelta;
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

        private void _InitViewport()
        {
            var rect = transform as RectTransform;
            var size = rect!.sizeDelta;
            _viewportArea = new Rect(0, -size.y, size.x, size.y);
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
            OnVisibleChanged = null;
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
                        OnVisibleChanged?.Invoke(cell);
                    }
                    else
                    {
                        // 这样可确保所有的OnVisibleChanged事件中, Transform都是可用的, 方便client设置一些东西
                        OnVisibleChanged?.Invoke(cell);
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

        public void AddCell(object data)
        {
            if (cellTransform == null)
            {
                return;
            }

            var sizeDelta = cellTransform.sizeDelta;
            var index = _cells.Count;
            var areaPos = _direction.GetCellAreaPos(index, _rank, sizeDelta);

            var area = new Rect(areaPos.x, areaPos.y, sizeDelta.x, sizeDelta.y);
            var item = new Cell(index, area, data);

            var relativeArea = _GetRelativeViewportArea();
            var isVisible = relativeArea.Overlaps(area);
            if (isVisible)
            {
                var go = _SpawnCellGameObject(item);
                item.SetTransform(go.transform as RectTransform);
                item.SetVisible(true);
                OnVisibleChanged?.Invoke(item);
            }

            _cells.PushBack(item);
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

        public void RemoveAllCells()
        {
            var size = _cells.Count;
            if (size > 0)
            {
                for (int i = 0; i < size; i++)
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

        private GameObject _SpawnCellGameObject(Cell item)
        {
            if (_goPool.Count > 0)
            {
                var last = _goPool.PopBack() as GameObject;
                _OnSpawnCellGameObject(last, item);
                return last;
            }

            var go = Instantiate(cellTransform.gameObject, _contentTransform);
            _OnSpawnCellGameObject(go, item);
            return go;
        }

        private void _OnSpawnCellGameObject(GameObject go, Cell item)
        {
            var trans = go.transform as RectTransform;
            trans!.anchoredPosition = _direction.GetTransformAnchoredPos(item);
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

        public RectTransform cellTransform;
        public Direction direction = Direction.BottomToTop;

        public event Action<Cell> OnVisibleChanged;

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