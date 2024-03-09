/********************************************************************
created:    2023-07-04
author:     lixianmin

1. 无论是BottomToTop还是TopToBottom, 一开始的时候scrollbar的位置都在最上面
2. BottomToTop => 向下拉scrollbar的时候, 内容从bottom到top走
3. TopToBottom => 向下拉scrollbar的时候, 内容从top到bottom走

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using Direction = UnityEngine.UI.Scrollbar.Direction;

namespace Unicorn.UI
{
    partial class UILoopScrollRect
    {
        public class DirBase
        {
            public static DirBase Create(Direction direction)
            {
                var dir = direction switch
                {
                    Direction.LeftToRight => new HorizontalDir(),
                    Direction.RightToLeft => new HorizontalDir(),
                    Direction.BottomToTop => new VerticalDir(),
                    Direction.TopToBottom => new VerticalDir(),
                    _ => new DirBase(),
                };

                dir._direction = direction;
                return dir;
            }

            protected DirBase()
            {
            }

            public Direction GetDirection()
            {
                return _direction;
            }

            public virtual Vector2 GetTransformAnchoredPos(Cell cell)
            {
                return Vector2.zero;
            }

            public virtual Vector2 GetCellAreaPos(int index, int rank, Vector2 sizeDelta)
            {
                return Vector2.zero;
            }

            private Direction _direction = (Direction)(-1);
        }

        public class HorizontalDir : DirBase
        {
            public override Vector2 GetTransformAnchoredPos(Cell cell)
            {
                var area = cell.GetArea();
                return new Vector2(area.width * 0.5f + area.x, area.y);
            }

            public override Vector2 GetCellAreaPos(int index, int rank, Vector2 sizeDelta)
            {
                var indexX = index / rank;
                var indexY = index % rank;
                return new Vector2(indexX * sizeDelta.x, -(indexY + 0.5f) * sizeDelta.y);
            }
        }

        public class VerticalDir : DirBase
        {
            public override Vector2 GetTransformAnchoredPos(Cell cell)
            {
                var area = cell.GetArea();
                return new Vector2(area.width * .5f + area.x, area.height * .5f + area.y);
            }

            public override Vector2 GetCellAreaPos(int index, int rank, Vector2 sizeDelta)
            {
                var indexX = index % rank;
                var indexY = index / rank;
                return new Vector2(indexX * sizeDelta.x, -(indexY + 1) * sizeDelta.y);
            }
        }
    }
}