
/********************************************************************
created:    2022-08-15
author:     lixianmin

1. life cycle of a window:

 ----------------------------------------------------------------------
 |  new     --> load   --> open animation  --> open  --> active  -->  |
 |                                                                    |  
 |  dispose <-- unload <-- close animation <-- close <-- deactive <-- |
 ----------------------------------------------------------------------

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.Web;
using UnityEngine;

namespace Unicorn.UI
{
    public abstract partial class UIWindowBase
    {
        /// <summary>
        /// 如果UIRoot的直接子节点存在以assetPath命名的Canvas节点，则直接使用该节点; 否则被当作UI资源的路径从addressable加载
        /// </summary>
        /// <returns>assetPath：UIRoot下的Canvas节点名， 或加载UI的资源路径</returns>
        public abstract string GetAssetPath();
        public virtual RenderQueue GetRenderQueue() { return RenderQueue.Geometry; }
        
        public Transform GetTransform() { return _transform; }
        public Canvas GetCanvas() { return _canvas; }
        public bool IsOpened() { return _fetus.isOpened; }
        
        /// <summary>
        /// 跟踪window加载进度
        /// </summary>
        /// <returns>IWebNode</returns>
        public IWebNode GetWebNode() { return _fetus.GetWebNode(); }
        
        // 事件相关: 以下6个事件, gameObject都是可用的, widget变量都是可用的
        
        /// <summary>
        /// 时机: 加载完成事件, 与OnUnloading()呼应. 此时window可见, widget可用, 但未执行open animation
        /// 用途: 控件数据初始化, register events
        /// </summary>
        protected virtual void OnLoaded() {}
        
        /// <summary>
        /// 时机: 打开完成事件, 与OnClosing呼应. open animation执行完成, window等待玩家输入
        /// 用途: 
        /// </summary>
        protected virtual void OnOpened() {}
        
        /// <summary>
        /// 时机: 获得焦点事件, 与OnDeactivating()呼应. 当通过OpenWindow()打开window时, 该window会被激活并收到OnActivated()事件
        /// 用途: 
        /// </summary>
        protected virtual void OnActivated() {}
        
        /// <summary>
        /// 时机: 准备失去焦点事件, 与OnActivated()呼应. 当通用OpenWindow()打开其它window时, 当前位于前台的window会收到OnDeactivating()事件
        /// 用途: 
        /// </summary>
        protected virtual void OnDeactivating() {}
        
        /// <summary>
        /// 时机: 准备关闭事件, 与OnOpened()呼应. 本事件执行完成后, 会开始close animation 
        /// 用途: 
        /// </summary>
        protected virtual void OnClosing() {}
        
        /// <summary>
        /// 时机: 准备销毁事件, 与OnLoaded()呼应. gameObject与widgets尚未销毁, 都处于可用状态
        /// 用途: unregister在OnLoaded()中注册的events
        /// </summary>
        protected virtual void OnUnloading() {}
        
        /// <summary>
        /// 慢速帧，大概10fps；如果感觉频率不够，可考虑实现IExpensiveUpdater接口
        /// </summary>
        /// <param name="deltaTime">两帧之间的时间间隔，远大于Time.deltaTime</param>
        protected virtual void SlowUpdate(float deltaTime) {}
    }
}