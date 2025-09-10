/********************************************************************
created:    2023-07-04
author:     lixianmin

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

using UnityEngine;

namespace Unicorn.UI
{
    partial class UILoopScrollRect
    {
        public interface IRecord
        {
            void OnVisibleChanged(Cell cell);
        }

        public class Cell : IHaveTransform
        {
            public Cell(Rect area, IRecord record)
            {
                _area = area;
                _record = record;
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

            /// <summary>
            /// 如果record一开始传进来的就是null, 那么它就只能是null了
            /// </summary>
            /// <returns></returns>
            public IRecord GetRecord()
            {
                return _record;
            }

            internal void CopyFrom(Cell other)
            {
                if (other != null)
                {
                    _area = other._area;
                    _isVisible = other._isVisible;
            
                    // 不能直接替换transform, 而是只能复制transform的位置. 因为transform上已经设置了很多玩家数据, 这个是不能直接替换的
                    // _transform = other._transform;
            
                    if (other._transform is not null)
                    {
                        if (_transform is not null)
                        {
                            _transform.anchoredPosition = other._transform.anchoredPosition;
                        }
                        else // 如果是在背包尾部的交界处, other是可见的, 而this是不可见的. 这时在_area复制后, 需要通过UILoopScrollRect的Update()更新可见性
                        {
                            _isVisible = false;
                        }
                    }
                }
            }

            private Rect _area; // 相对于viewport判定可见性
            private readonly IRecord _record;

            private bool _isVisible;
            private RectTransform _transform;
        }
    }
}