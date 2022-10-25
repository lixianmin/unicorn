
/********************************************************************
created:    2013-12-14
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn
{
    public interface IExpensiveUpdater
    {
        /// <summary>
        /// 实际上就是Update()，只所以起名ExpensiveUpdate()，是为了让使用者郑重考虑是否启用这个可能会比较费的更新逻辑
        /// </summary>
        /// <param name="deltaTime">需要补上deltaTime这个参数, 因为它位于Standard Assets下面, 没有机会去Time.time拿deltaTime</param>
        void ExpensiveUpdate(float deltaTime);
    }
}