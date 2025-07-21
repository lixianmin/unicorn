/********************************************************************
created:    2024-03-18
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
    /// 与FetusFlags是同构的
    /// </summary>
    public enum WindowFlags : byte
    {
        None = 0x00,
        Cache = 0x01, // 加载后, 缓存窗体gameObject在内存中, 但会执行完整的从Loaded->Unloading的所有事件流程
    }
}