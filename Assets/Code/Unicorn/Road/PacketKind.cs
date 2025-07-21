/********************************************************************
created:    2023-06-12
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

namespace Unicorn.Road
{
    public enum PacketKind
    {
        Handshake = 1, // 连接建立后, server主动发送handshake
        HandshakeRe = 2, // 连接建立后, client回复server的handshake协议
        Heartbeat = 3, // client定期发送心跳
        Kick = 4, // server踢人
        RouteKind = 5, // 推送route与kind到client
        Echo = 6, // 在服务器注册一个回调方法, 通过客户返回在session的receive的goroutine中调用一次
        UserBase = 10, // 用户自定义的类型, 从这里开始
        // RouteBase = 5000, // 当kind值 >= RouteBase时, 就意味着存储的是route字符串, route字符器长度=(kind - RouteBase)
    }
}