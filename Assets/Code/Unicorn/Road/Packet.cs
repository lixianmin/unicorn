/********************************************************************
created:    2023-06-10
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn.Road
{
    public struct Packet
    {
        public int Kind; // 自定义的类型从UserDefined开始

        // public byte[] Route; // kind >= RouteBase <=> Route有值
        public int RequestId; // 请求的rid, 用于client请求时定位response的handler
        public byte[] Code; // error code
        public byte[] Data; // 如果有error code, 则Data是error message; 否则Data是数据payload
    }

    // server握手协议, 使用json序列化
    public struct JsonHandshake
    {
        public int nonce;
        public int heartbeat; // 心跳间隔. 单位: 秒
        // public string gid; // client断线重连时, 基于此判断client重连的是不是上一次的同一个server进程

        // 有序的routes, 其kinds值从Userdata(1000)有序增加; 只所以这么做并不是为了省流量, 而是unity3d的JsonUtility不支持反序列化Dictionary
        public string routes;
        public long sid;
    }

    // 回复server的握手协议, 使用json序列化
    public struct JsonHandshakeRe
    {
        public string serde;
    }

    public struct JsonRouteKind
    {
        public int kind;
        public string route;
    }
}