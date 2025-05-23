// /********************************************************************
// created:    2022-08-26
// author:     lixianmin
//
// Copyright (C) - All Rights Reserved
// *********************************************************************/
//
// using UnityEngine;
// using UObject = UnityEngine.Object;
//
// namespace Unicorn.Kit
// {
//     public partial class KitBase : IHaveTransform, IIsDisposed
//     {
//         /// <summary>
//         /// 对标MonoBehaviour的Awake()方法。设计成protected是为了防止client自己主动调用这些方法
//         /// </summary>
//         protected virtual void Awake() { }
//
//         /// <summary>
//         /// gameObject销毁事件, 对标MonoBehaviour的OnDestroy()
//         /// </summary>
//         protected virtual void OnDestroy() { }
//
//         /// <summary>
//         /// 在 gameObject.SetActive(true) 或 script.enabled=true 时都会触发OnEnable()事件
//         /// </summary>
//         protected virtual void OnEnable() { }
//         
//         /// <summary>
//         /// 在 gameObject.SetActive(false) 或 script.enabled=false 时都会触发OnDisable()事件
//         /// </summary>
//         protected virtual void OnDisable() { }
//
//         /// <summary>
//         /// 被触碰的商店需要设置isTrigger=true
//         /// </summary>
//         /// <param name="other">如果是player进入触碰商店的collider，则other就是player</param>
//         protected virtual void OnTriggerEnter(Collider other) { }
//         
//         /// <summary>
//         /// 只所以把这些事件都设置为virtual方法，而不是昨天挂接MbKitOnTriggerEnterExit脚本的方式，原因是这些方法的调用频率并不高，预计这种设计带来的额外开销不会很大，但使用方便性上比如实现IOnTriggerExit接口的方式要简单不少
//         /// </summary>
//         /// <param name="other"></param>
//         protected virtual void OnTriggerExit(Collider other) { }
//
//         /// <summary>
//         /// 慢速帧，大概10fps；如果感觉频率不够，可考虑实现IExpensiveUpdater接口
//         /// </summary>
//         /// <param name="deltaTime">两帧之间的时间间隔，远大于Time.deltaTime</param>
//         protected virtual void SlowUpdate(float deltaTime) { }
//
//         // /// <summary>
//         // /// AddListener()这个方法是送的, 子类可以用, 也可以不用
//         // /// </summary>
//         // /// <param name="evt"></param>
//         // /// <param name="handler"></param>
//         // public void AddListener(UnityEvent evt, UnityAction handler)
//         // {
//         //     _listener.AddListener(evt, handler);
//         // }
//         //
//         // public void AddListener<T>(UnityEvent<T> evt, UnityAction<T> handler)
//         // {
//         //     _listener.AddListener(evt, handler);
//         // }
//
//         bool IIsDisposed.IsDisposed()
//         {
//             return _isDisposed;
//         }
//
//         public Transform GetTransform()
//         {
//             return _transform;
//         }
//
//         public UObject[] GetAssets()
//         {
//             return _assets;
//         }
//
//         
//         /// <summary>
//         /// 与MbKitOnEnable的同名属性同步
//         /// </summary>
//         public bool isActiveAndEnabled { get; private set; }
//
//         /// <summary>
//         /// Kit对象的Update()顺序, 相同class的kit对象同一批调用Update().
//         /// 如何通过sort控制Update()顺序呢? 可以在client代码中, 找一个地方使用以下方式初始化:
//         ///   TypeTools.SetDefaultTypeIndex(typeof(Client.BowlKit));
//         ///   TypeTools.SetDefaultTypeIndex(typeof(Client.PlayerMoveKit));
//         /// </summary>
//         internal int sort;
//
//         private Transform _transform;
//
//         private UObject[] _assets;
//
//         // private readonly EventListener _listener = new();
//         private bool _isDisposed;
//     }
// }