
/********************************************************************
created:    2022-11-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
namespace Unicorn.UI
{
    /// <summary>
    /// 有时需要把UI的主代码拉到client去测试, 需要IRemoveAllListeners是public的
    /// </summary>
    public interface IRemoveAllListeners
    {
        void RemoveAllListeners();
    }
}