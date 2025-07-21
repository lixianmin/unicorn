/********************************************************************
created:    2022-08-12
author:     lixianmin

1. UNICORN_EDITOR 这个宏定义在 mcs.rsp 文件中

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

#if UNICORN_EDITOR

using System.Collections;
using Clients.Web;
using UnityEngine;
using Unicorn;
using Metadata;
using Unicorn.Collections;
using Unicorn.UI;

public class MbUnicornGame : MonoBehaviour
{
    class GameMetadataManager : MetadataManager
    {
        public new static readonly GameMetadataManager It = new();
    }

    private IEnumerator Start()
    {
        // 避免Game对象在场景切换的时候被干掉
        DontDestroyOnLoad(gameObject);

        // UnicornMain.It.Init();
        // 启用每次输出log, 方便开发过程中调试
        Logo.Flags |= LogoFlags.FlushOnWrite;
        
        yield return _webManager.InitPackage();

        UIManager.It.OpenWindow(typeof(Clients.UI.UIMain));
    }

    // Update is called once per frame
    private void Update()
    {
        using var list = SlicePool.Get<string>();
        list.Add("hello");
        list.Add("world");
        
        // 主动调用SlicePool.Return()也是可以的
        SlicePool.Return(list);
        SlicePool.Return(list);
        SlicePool.Return(list);
    }

    private readonly GameWebManager _webManager = GameWebManager.It; // 初始化基类中的Instance引用
}

#endif