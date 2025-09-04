# Unicorn - Unity 核心库

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
[![Unity](https://img.shields.io/badge/Unity-2022.3%2B-green.svg)](https://unity3d.com)
[![C#](https://img.shields.io/badge/C%23-9.0-blue.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)

一个功能强大、可用于生产环境的 Unity 核心库，为构建可扩展和可维护的 Unity 应用程序提供必要组件。Unicorn 提供现代化架构，配备高性能的 UI 管理、资源加载、网络通信和数据结构系统。

## ✨ 核心特性

- **🖼️ 高级 UI 管理** - 完整的 UI 生命周期管理，支持动画和分层
- **📦 智能资源管理** - 异步资源加载，自动内存管理
- **🌐 游戏网络系统 (Road)** - 实时多人游戏长连接网络系统
- **⚡ 高性能集合类** - 优化的数据结构（Deque、SortedTable、Slice、PriorityQueue）
- **🔄 协程管理** - 高效的协程系统，性能优于 Unity 默认实现
- **⚙️ 配置管理** - 灵活的元数据和配置系统
- **🛠️ 实用扩展** - 全面的扩展方法和工具集

## 🚀 快速开始

### 前置要求

- Unity 2022.3 LTS 或更高版本
- 兼容 .NET Standard 2.1
- TextMeshPro（Unity 内置）
- Universal Render Pipeline (URP)

### 安装方式

**通过下载源码并编译**

1. **Git 克隆**

   ```bash
   git clone https://github.com/lixianmin/unicorn.git
   cd unicorn
   ```

2. **直接下载**

   - 访问 GitHub 仓库：https://github.com/lixianmin/unicorn
   - 点击 "Code" → "Download ZIP" 下载源码包
   - 解压到您的工作目录

3. **导入到 Unity 项目**
   - 使用 Unity 打开下载的项目
   - 或将 `Assets/Code/Unicorn/` 和 `Assets/Standard Assets/Code/Unicorn/` 目录复制到您的项目中

## 📁 项目结构

- **核心库**: `Assets/Code/Unicorn/` 和 `Assets/Standard Assets/Code/Unicorn/` 下的所有模块都是可用于生产环境的库代码
- **示例代码**: `Assets/Code/Client/` 目录包含由 `UNICORN_EDITOR` 宏控制的示例实现
  - 示例代码仅在 Unity 编辑器模式下编译
  - 演示典型的使用模式和集成方法
  - 可以安全地删除或根据您的特定需求进行修改

> **注意**: `UNICORN_EDITOR` 宏确保示例代码不会影响生产构建，同时在开发过程中提供清晰的集成示例。

## 📖 核心模块

### UI 管理

具有生命周期管理的综合 UI 系统：

```csharp
// 定义 UI 窗口
public class MainMenuWindow : UIWindowBase
{
    public override string GetAssetPath() => "UI/MainMenu";

    protected override void OnLoaded()
    {
        // 设置 UI 组件事件监听
        AtUnloading += _startButton.UI.onClick.On(() =>
        {
            // 开始游戏按钮点击事件
            OnStartGame();
        });
    }

    protected override void OnOpened()
    {
        // 窗口打开且动画完成
    }

    private void OnStartGame()
    {
        // 游戏开始逻辑
        Logo.Info("Start Game clicked");
    }

    // 声明 UI 组件
    private readonly UIWidget<UIButton> _startButton = new("StartButton");
}

// 打开/关闭窗口
UIManager.It.OpenWindow(typeof(MainMenuWindow));
UIManager.It.CloseWindow(typeof(MainMenuWindow));
```

### 资源管理

高效的异步资源加载，自动内存管理：

```csharp
// 加载预制体
var webPrefab = WebManager.It.LoadPrefab(new WebArgument
{
    key = "Characters/Player"
}, prefab =>
{
    if (prefab.Asset != null)
    {
        var instance = Instantiate(prefab.Asset);
        // 使用实例化的对象
    }
});

// 资源自动管理和释放
```

### 游戏网络系统 (Road)

为实时多人游戏设计的基于协议的长连接网络系统。具有灵活的 ISerde 序列化接口，JsonSerde 作为无依赖示例。生产项目可以实现自定义序列化器（Protobuf、MessagePack 等）以获得最佳性能。与 [Gonsole](https://github.com/lixianmin/gonsole) 后端框架无缝配合：

```csharp
// 创建网络会话
var session = new Session();

// 使用灵活的序列化连接到游戏服务器
// JsonSerde 作为无依赖示例提供
// 生产项目可以使用 ProtobufSerde、MessagePackSerde 等获得更好的性能
session.Connect("localhost", 8080, session => new JsonSerde(), onHandShaken =>
{
    // 连接建立并握手完成
    Logo.Info("Connected to game server");
}, onClosed =>
{
    // 连接关闭
    Logo.Info("Disconnected from server");
});

// 向服务器发送数据
var message = new { action = "move", x = 10, y = 20 };
session.Call("player.move", message, (response, error) =>
{
    if (error == null)
    {
        // 处理服务器响应
        Logo.Info($"Move result: {response}");
    }
});

// 生产环境自定义序列化示例：
// session.Connect("localhost", 8080, session => new ProtobufSerde(), ...);
// 或
// session.Connect("localhost", 8080, session => new MessagePackSerde(), ...);

// 协议包括握手、心跳和自定义消息类型
// ISerde 接口允许任何序列化方法（JSON、Protobuf、MessagePack 等）
```

**自定义序列化实现：**

```csharp
// 示例：实现 Protobuf 序列化以获得更好的性能
public class ProtobufSerde : ISerde
{
    public byte[] Serialize(object item)
    {
        // 使用 Google.Protobuf 或类似库
        // return ProtobufSerializer.Serialize(item);
        throw new NotImplementedException("在此添加您的 protobuf 序列化代码");
    }

    public T Deserialize<T>(byte[] data) where T : new()
    {
        // 使用 Google.Protobuf 或类似库
        // return ProtobufSerializer.Deserialize<T>(data);
        throw new NotImplementedException("在此添加您的 protobuf 反序列化代码");
    }
}

// 自定义序列化的好处：
// - Protobuf：比 JSON 快约 3 倍，小约 2 倍
// - MessagePack：比 JSON 快约 2 倍，小约 1.5 倍
// - 自定义二进制：针对特定用例的最大性能
```

**关键特性：**

- **基于协议的架构**，结构化数据包处理
- 基于 WebSocket 的长连接，低延迟
- **灵活的序列化**，通过 ISerde 接口（包含 JSON 示例，支持 protobuf、MessagePack 等）
- 内置 gzip 压缩，优化带宽
- 会话管理，支持重连
- RPC 风格的方法调用和响应处理
- 兼容 [Gonsole](https://github.com/lixianmin/gonsole) 游戏服务器框架

### 高性能集合类

为游戏开发优化的数据结构：

```csharp
// Slice<T> - 带对象池的高性能 List<T> 替代品
using var slice = SlicePool.Get<Transform>();
slice.Add(transform1);
slice.Add(transform2);
// 释放时自动返回对象池

// Deque<T> - 双端队列，高效的前端/后端操作
var deque = new Deque<int>();
deque.PushBack(1);
deque.PushFront(0);
var front = deque.PopFront(); // 0
var back = deque.PopBack();   // 1

// SortedTable<TKey, TValue> - 排序字典，快速查找
var table = new SortedTable<string, PlayerData>();
table.Add("player1", playerData);

// PriorityQueue<T> - 高效的基于优先级的操作
var queue = new PriorityQueue<Task>();
queue.Enqueue(highPriorityTask);
var nextTask = queue.Dequeue();

// ThreadSwapper<T> - 线程间安全数据交换
var swapper = new ThreadSwapper<string>();
// 生产者线程
swapper.GetProducer().Add("data");
swapper.Put(true);
// 消费者线程
swapper.Take(false);
var data = swapper.GetConsumer();
```

### 协程管理

轻量级协程系统，性能更优：

```csharp
// 启动协程
CoroutineManager.It.StartCoroutine(MyCoroutine());

private IEnumerator MyCoroutine()
{
    yield return null; // 等待一帧
    yield return new WaitForSeconds(1.0f);
    // 协程逻辑
}
```

## 🏗️ 架构图

```
┌─────────────────────────────────────────────────────────────┐
│                     客户端应用程序                         │
├─────────────────────────────────────────────────────────────┤
│  UI 管理        │  资源加载        │  高性能集合类            │
├─────────────────────────────────────────────────────────────┤
│  网络系统       │  协程管理        │  配置系统                │
│  (Road)         │                  │                        │
├─────────────────────────────────────────────────────────────┤
│  核心工具       │  扩展方法        │  编辑器工具              │
├─────────────────────────────────────────────────────────────┤
│                     Unity 引擎                            │
└─────────────────────────────────────────────────────────────┘
                              ▲
                              │ WebSocket
                              ▼
┌─────────────────────────────────────────────────────────────┐
│         Gonsole 游戏服务器 (后端框架)                       │
│          https://github.com/lixianmin/gonsole               │
│               (协议兼容的后端)                              │
└─────────────────────────────────────────────────────────────┘
```

## 📋 API 参考

### UIManager

| 方法                | 描述                           |
| ------------------- | ------------------------------ |
| `OpenWindow(Type)`  | 打开 UI 窗口，包含生命周期管理 |
| `CloseWindow(Type)` | 关闭 UI 窗口，支持动画         |
| `GetWindow(Type)`   | 获取现有窗口实例               |

### WebManager

| 方法                                         | 描述                     |
| -------------------------------------------- | ------------------------ |
| `LoadAsset(WebArgument, Action<IWebNode>)`   | 异步加载资源             |
| `LoadPrefab(WebArgument, Action<WebPrefab>)` | 加载预制体，自动内存管理 |

### CoroutineManager

| 方法                          | 描述             |
| ----------------------------- | ---------------- |
| `StartCoroutine(IEnumerator)` | 启动托管协程     |
| `StopCoroutine(IEnumerator)`  | 停止运行中的协程 |

### Session (Road 网络系统)

| 方法                                                        | 描述                               |
| ----------------------------------------------------------- | ---------------------------------- |
| `Connect(host, port, serdeBuilder, onHandShaken, onClosed)` | 使用自定义 ISerde 序列化器建立连接 |
| `Call(method, data, callback)`                              | 向服务器发送 RPC 调用，处理响应    |
| `Close()`                                                   | 关闭网络连接                       |

### 集合类 API

| 类                 | 主要方法                                               | 描述                         |
| ------------------ | ------------------------------------------------------ | ---------------------------- |
| `Slice<T>`         | `Add()`, `RemoveAt()`, `Clear()`                       | 带对象池的高性能 List 替代品 |
| `SlicePool`        | `Get<T>()`, `Return<T>()`                              | Slice 实例的对象池           |
| `Deque<T>`         | `PushBack()`, `PopFront()`, `PushFront()`, `PopBack()` | 双端队列                     |
| `SortedTable<K,V>` | `Add()`, `Remove()`, `ContainsKey()`                   | 排序字典                     |
| `PriorityQueue<T>` | `Enqueue()`, `Dequeue()`, `Peek()`                     | 基于优先级的队列             |
| `ThreadSwapper<T>` | `Put()`, `Take()`, `GetProducer()`, `GetConsumer()`    | 线程安全的数据交换           |

### ISerde 接口

| 方法                     | 描述                                 |
| ------------------------ | ------------------------------------ |
| `Serialize(object)`      | 使用自定义序列化将对象转换为字节数组 |
| `Deserialize<T>(byte[])` | 将字节数组转换回 T 类型的对象        |

## 🤝 贡献

我们欢迎贡献！请查看我们的[贡献指南](CONTRIBUTING.md)了解详情。

### 开发设置

1. Fork 仓库
2. 创建功能分支：`git checkout -b feature/amazing-feature`
3. 进行更改并添加测试
4. 提交更改：`git commit -m 'Add amazing feature'`
5. 推送到分支：`git push origin feature/amazing-feature`
6. 提交 pull request

### 代码风格

- 遵循 C# 命名约定
- 使用有意义的变量和方法名
- 为公共 API 添加 XML 文档
- 为新功能包含单元测试

## 📋 系统要求

- **Unity**: 2022.3 LTS 或更高版本
- **核心依赖**:
  - TextMeshPro（Unity 内置）
  - Universal Render Pipeline (URP)
- **可选示例**:
  - YooAsset 2.1.1+（资源加载示例）
  - DOTween（动画示例）
- **生产环境序列化**（可选，获得更好性能）:
  - Google.Protobuf（Protobuf 序列化）
  - MessagePack（MessagePack 序列化）
  - 自定义二进制序列化库

## 🐛 已知问题

- 某些基于反射的功能需要特定的托管代码剥离级别
- UI 序列化在控件名称交换时可能需要手动重新序列化
- `Client/` 目录中的示例代码需要在编辑器模式下定义 `UNICORN_EDITOR` 宏
- Road 网络系统需要稳定的网络连接以获得最佳性能

## 📄 许可证

本项目采用 Apache License 2.0 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

```
Copyright 2024 Unicorn Contributors

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
```

## 🙏 致谢

- Unity Technologies 提供的强大引擎
- TextMeshPro 团队提供的高级文本渲染
- Universal Render Pipeline 团队提供的现代渲染
- [Gonsole](https://github.com/lixianmin/gonsole) 后端框架提供的无缝服务器集成
- 所有帮助改进这个库的贡献者

## 📞 支持

- 📖 [文档](https://github.com/lixianmin/unicorn/wiki)
- 🐛 [问题跟踪](https://github.com/lixianmin/unicorn/issues)
- 💬 [讨论区](https://github.com/lixianmin/unicorn/discussions)

---

⭐ **如果您觉得这个库有用，请考虑给它一个星星！** ⭐
