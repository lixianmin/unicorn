/********************************************************************
created:    2023-06-10
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn.Road
{
    public class Error
    {
        public string Code;
        public string Message;

        public override string ToString()
        {
            return $"Code={Code}, Message={Message}";
        }
    }
}