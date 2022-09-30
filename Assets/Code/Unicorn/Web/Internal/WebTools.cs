
/********************************************************************
created:    2022-08-13
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
namespace Unicorn.Web.Internal
{
    internal static class WebTools
    {
        public static int GetNextId()
        {
            return ++_idGenerator;
        }
        
        private static int _idGenerator;
    }
}