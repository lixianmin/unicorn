
/********************************************************************
created:    2022-11-12
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