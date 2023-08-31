/********************************************************************
created:    2023-07-04
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;

namespace Unicorn.UI
{
    partial class UILoopScrollRect
    {
        public interface ICellData
        {
            void OnVisibleChanged(Cell cell);
        }

        public class Cell : IHaveTransform
        {
            /// <summary>
            /// 纯数值的cell单元格
            /// </summary>
            /// <param name="index">索引</param>
            /// <param name="area">相对于viewport判定可见性</param>
            /// <param name="data">Cell的附加数据</param>
            public Cell(int index, Rect area, ICellData data)
            {
                _index = index;
                _area = area;
                _data = data;
            }

            public int GetIndex()
            {
                return _index;
            }

            internal Rect GetArea()
            {
                return _area;
            }

            public bool IsVisible()
            {
                return _isVisible;
            }

            internal void SetVisible(bool visible)
            {
                _isVisible = visible;
            }

            internal void SetTransform(RectTransform trans)
            {
                _transform = trans;
            }

            Transform IHaveTransform.GetTransform()
            {
                return _transform;
            }

            public RectTransform GetTransform()
            {
                return _transform;
            }

            public ICellData GetData()
            {
                return _data;
            }

            private readonly int _index;
            private readonly Rect _area; // 相对于viewport判定可见性
            private readonly ICellData _data;

            private bool _isVisible;
            private RectTransform _transform;
        }
    }
}