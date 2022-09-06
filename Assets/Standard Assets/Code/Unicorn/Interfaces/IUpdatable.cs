
/********************************************************************
created:    2013-12-14
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn
{
    public interface IUpdatable
    {
        // 这里有可能需要补上deltaTime这个参数, 因为它位于Standard Assets下面, 没有机会去Time.time拿deltaTime
        // 如果补的话, 会通过UnicornMain把deltaTime传递过来, 但是目前并没有相关需求, 先搁置
        void Update();
    }
}