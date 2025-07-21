/********************************************************************
created:    2023-09-01
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

using UnityEngine.Events;

#if UNICORN_EDITOR

namespace Clients.UI
{
    public class ShopGood
    {
        public ShopGood(int tid, string name)
        {
            _tid = tid;
            _name = name;
        }

        public int GetTemplateId()
        {
            return _tid;
        }

        public void SetName(string name)
        {
            if (_name != name)
            {
                _name = name;
                OnUpdateGoods.Invoke(this);
            }
        }

        public string GetName()
        {
            return _name;
        }

        public readonly UnityEvent<ShopGood> OnUpdateGoods = new();

        private int _tid;
        private string _name;
    }
}

#endif