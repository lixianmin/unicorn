# Unicorn - Unity Core Library

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
[![Unity](https://img.shields.io/badge/Unity-2022.3%2B-green.svg)](https://unity3d.com)
[![C#](https://img.shields.io/badge/C%23-9.0-blue.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)

A powerful, production-ready Unity core library providing essential components for building scalable and maintainable Unity applications. Unicorn offers a modern architecture with high-performance systems for UI management, resource loading, networking, and data structures.

## ✨ Key Features

- **🖼️ Advanced UI Management** - Complete UI lifecycle with animations and layering
- **📦 Smart Resource Management** - Async asset loading with automatic memory management
- **🌐 Game Networking (Road)** - Long-connection networking system for real-time games
- **⚡ High-Performance Collections** - Optimized data structures (Deque, SortedTable, Slice, PriorityQueue)
- **🔄 Coroutine Management** - Efficient coroutine system with better performance than Unity's default
- **⚙️ Configuration Management** - Flexible metadata and configuration system
- **🛠️ Utility Extensions** - Comprehensive set of extension methods and tools

## 🚀 Quick Start

### Prerequisites

- Unity 2022.3 LTS or later
- .NET Standard 2.1 compatible
- TextMeshPro (included with Unity)
- Universal Render Pipeline (URP)

### Installation

**Download Source Code and Compile**

1. **Git Clone**

   ```bash
   git clone https://github.com/lixianmin/unicorn.git
   cd unicorn
   ```

2. **Direct Download**

   - Visit GitHub repository: https://github.com/lixianmin/unicorn
   - Click "Code" → "Download ZIP" to download source package
   - Extract to your working directory

3. **Import to Unity Project**
   - Open the downloaded project with Unity
   - Or copy `Assets/Code/Unicorn/` and `Assets/Standard Assets/Code/Unicorn/` directories to your project

## 📁 Project Structure

- **Core Library**: All modules under `Assets/Code/Unicorn/` and `Assets/Standard Assets/Code/Unicorn/` are production-ready library code
- **Example Code**: The `Assets/Code/Client/` directory contains example implementations controlled by the `UNICORN_EDITOR` macro
  - Examples only compile in Unity Editor mode
  - Demonstrates typical usage patterns and integration approaches
  - Safe to remove or modify for your specific needs

> **Note**: The `UNICORN_EDITOR` macro ensures example code doesn't affect production builds while providing clear integration examples during development.

## 📖 Core Modules

### UI Management

Comprehensive UI system with lifecycle management:

```csharp
// Define a UI Window
public class MainMenuWindow : UIWindowBase
{
    public override string GetAssetPath() => "UI/MainMenu";

    protected override void OnLoaded()
    {
        // Setup UI component event listeners
        AtUnloading += _startButton.UI.onClick.On(() =>
        {
            // Start game button click event
            OnStartGame();
        });
    }

    protected override void OnOpened()
    {
        // Window opened with animation complete
    }

    private void OnStartGame()
    {
        // Game start logic
        Logo.Info("Start Game clicked");
    }

    // Declare UI components
    private readonly UIWidget<UIButton> _startButton = new("StartButton");
}

// Open/Close windows
UIManager.It.OpenWindow(typeof(MainMenuWindow));
UIManager.It.CloseWindow(typeof(MainMenuWindow));
```

### Resource Management

Efficient async resource loading with automatic memory management:

```csharp
// Load a prefab
var webPrefab = WebManager.It.LoadPrefab(new WebArgument
{
    key = "Characters/Player"
}, prefab =>
{
    if (prefab.Asset != null)
    {
        var instance = Instantiate(prefab.Asset);
        // Use the instantiated object
    }
});

// Resources are automatically managed and disposed
```

### Game Networking (Road)

Protocol-based long-connection networking system designed for real-time multiplayer games. Features a flexible ISerde serialization interface with JsonSerde as a dependency-free example. Production projects can implement custom serializers (Protobuf, MessagePack, etc.) for optimal performance. Works seamlessly with the [Gonsole](https://github.com/lixianmin/gonsole) backend framework:

```csharp
// Create a network session
var session = new Session();

// Connect to game server with flexible serialization
// JsonSerde is provided as a dependency-free example
// Production projects can use ProtobufSerde, MessagePackSerde, etc. for better performance
session.Connect("localhost", 8080, session => new JsonSerde(), onHandShaken =>
{
    // Connection established and handshake complete
    Logo.Info("Connected to game server");
}, onClosed =>
{
    // Connection closed
    Logo.Info("Disconnected from server");
});

// Send data to server
var message = new { action = "move", x = 10, y = 20 };
session.Call("player.move", message, (response, error) =>
{
    if (error == null)
    {
        // Handle server response
        Logo.Info($"Move result: {response}");
    }
});

// Custom serialization example for production use:
// session.Connect("localhost", 8080, session => new ProtobufSerde(), ...);
// or
// session.Connect("localhost", 8080, session => new MessagePackSerde(), ...);

// Protocol includes handshake, heartbeat, and custom message types
// ISerde interface allows any serialization method (JSON, Protobuf, MessagePack, etc.)
```

**Custom Serialization Implementation:**

```csharp
// Example: Implementing Protobuf serialization for better performance
public class ProtobufSerde : ISerde
{
    public byte[] Serialize(object item)
    {
        // Use Google.Protobuf or similar library
        // return ProtobufSerializer.Serialize(item);
        throw new NotImplementedException("Add your protobuf serialization here");
    }

    public T Deserialize<T>(byte[] data) where T : new()
    {
        // Use Google.Protobuf or similar library
        // return ProtobufSerializer.Deserialize<T>(data);
        throw new NotImplementedException("Add your protobuf deserialization here");
    }
}

// Benefits of custom serialization:
// - Protobuf: ~3x faster, ~2x smaller than JSON
// - MessagePack: ~2x faster, ~1.5x smaller than JSON
// - Custom binary: Maximum performance for specific use cases
```

**Key Features:**

- **Protocol-based architecture** with structured packet handling
- WebSocket-based long connections for low latency
- **Flexible serialization** via ISerde interface (JSON example included, supports protobuf, MessagePack, etc.)
- Built-in gzip compression for bandwidth optimization
- Session management with reconnection support
- RPC-style method calls with response handling
- Compatible with [Gonsole](https://github.com/lixianmin/gonsole) game server framework

### High-Performance Collections

Optimized data structures for game development:

```csharp
// Slice<T> - High-performance List<T> alternative with object pooling
using var slice = SlicePool.Get<Transform>();
slice.Add(transform1);
slice.Add(transform2);
// Automatically returned to pool when disposed

// Deque<T> - Double-ended queue for efficient front/back operations
var deque = new Deque<int>();
deque.PushBack(1);
deque.PushFront(0);
var front = deque.PopFront(); // 0
var back = deque.PopBack();   // 1

// SortedTable<TKey, TValue> - Sorted dictionary with fast lookups
var table = new SortedTable<string, PlayerData>();
table.Add("player1", playerData);

// PriorityQueue<T> - Efficient priority-based operations
var queue = new PriorityQueue<Task>();
queue.Enqueue(highPriorityTask);
var nextTask = queue.Dequeue();

// ThreadSwapper<T> - Safe data exchange between threads
var swapper = new ThreadSwapper<string>();
// Producer thread
swapper.GetProducer().Add("data");
swapper.Put(true);
// Consumer thread
swapper.Take(false);
var data = swapper.GetConsumer();
```

### Coroutine Management

Lightweight coroutine system with better performance:

```csharp
// Start a coroutine
CoroutineManager.It.StartCoroutine(MyCoroutine());

private IEnumerator MyCoroutine()
{
    yield return null; // Wait one frame
    yield return new WaitForSeconds(1.0f);
    // Coroutine logic
}
```

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Client Application                     │
├─────────────────────────────────────────────────────────────┤
│  UI Management  │  Resource Loading │  High-Perf Collections │
├─────────────────────────────────────────────────────────────┤
│  Networking     │  Coroutine Mgmt  │  Configuration System  │
│  (Road)         │                  │                        │
├─────────────────────────────────────────────────────────────┤
│  Core Utilities │  Extensions      │  Editor Tools          │
├─────────────────────────────────────────────────────────────┤
│                     Unity Engine                           │
└─────────────────────────────────────────────────────────────┘
                              ▲
                              │ WebSocket
                              ▼
┌─────────────────────────────────────────────────────────────┐
│         Gonsole Game Server (Backend Framework)            │
│          https://github.com/lixianmin/gonsole               │
│               (Protocol-compatible backend)                │
└─────────────────────────────────────────────────────────────┘
```

## 📋 API Reference

### UIManager

| Method              | Description                                 |
| ------------------- | ------------------------------------------- |
| `OpenWindow(Type)`  | Opens a UI window with lifecycle management |
| `CloseWindow(Type)` | Closes a UI window with animation support   |
| `GetWindow(Type)`   | Gets an existing window instance            |

### WebManager

| Method                                       | Description                                     |
| -------------------------------------------- | ----------------------------------------------- |
| `LoadAsset(WebArgument, Action<IWebNode>)`   | Loads an asset asynchronously                   |
| `LoadPrefab(WebArgument, Action<WebPrefab>)` | Loads a prefab with automatic memory management |

### CoroutineManager

| Method                        | Description                |
| ----------------------------- | -------------------------- |
| `StartCoroutine(IEnumerator)` | Starts a managed coroutine |
| `StopCoroutine(IEnumerator)`  | Stops a running coroutine  |

### Session (Road Networking)

| Method                                                      | Description                                          |
| ----------------------------------------------------------- | ---------------------------------------------------- |
| `Connect(host, port, serdeBuilder, onHandShaken, onClosed)` | Establishes connection with custom ISerde serializer |
| `Call(method, data, callback)`                              | Sends RPC call to server with response handling      |
| `Close()`                                                   | Closes the network connection                        |

### Collections API

| Class              | Key Methods                                            | Description                                    |
| ------------------ | ------------------------------------------------------ | ---------------------------------------------- |
| `Slice<T>`         | `Add()`, `RemoveAt()`, `Clear()`                       | High-performance List alternative with pooling |
| `SlicePool`        | `Get<T>()`, `Return<T>()`                              | Object pool for Slice instances                |
| `Deque<T>`         | `PushBack()`, `PopFront()`, `PushFront()`, `PopBack()` | Double-ended queue                             |
| `SortedTable<K,V>` | `Add()`, `Remove()`, `ContainsKey()`                   | Sorted dictionary                              |
| `PriorityQueue<T>` | `Enqueue()`, `Dequeue()`, `Peek()`                     | Priority-based queue                           |
| `ThreadSwapper<T>` | `Put()`, `Take()`, `GetProducer()`, `GetConsumer()`    | Thread-safe data exchange                      |

### ISerde Interface

| Method                   | Description                                              |
| ------------------------ | -------------------------------------------------------- |
| `Serialize(object)`      | Converts object to byte array using custom serialization |
| `Deserialize<T>(byte[])` | Converts byte array back to object of type T             |

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

### Development Setup

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Make your changes and add tests
4. Commit your changes: `git commit -m 'Add amazing feature'`
5. Push to the branch: `git push origin feature/amazing-feature`
6. Submit a pull request

### Code Style

- Follow C# naming conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Include unit tests for new features

## 📋 Requirements

- **Unity**: 2022.3 LTS or later
- **Core Dependencies**:
  - TextMeshPro (included with Unity)
  - Universal Render Pipeline (URP)
- **Optional Examples**:
  - YooAsset 2.1.1+ (for resource loading examples)
  - DOTween (for animation examples)
- **Production Serialization** (optional, for better performance):
  - Google.Protobuf (for Protobuf serialization)
  - MessagePack (for MessagePack serialization)
  - Custom binary serialization libraries

## 🐛 Known Issues

- Some reflection-based features require specific managed stripping levels
- UI serialization may require manual re-serialization when control names are exchanged
- Example code in `Client/` directory requires `UNICORN_EDITOR` macro to be defined in Editor mode
- Road networking requires stable internet connection for optimal performance

## 📄 License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

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

## 🙏 Acknowledgments

- Unity Technologies for the amazing engine
- TextMeshPro team for advanced text rendering
- Universal Render Pipeline team for modern rendering
- [Gonsole](https://github.com/lixianmin/gonsole) backend framework for seamless server integration
- All contributors who have helped improve this library

## 📞 Support

- 📖 [Documentation](https://github.com/lixianmin/unicorn/wiki)
- 🐛 [Issue Tracker](https://github.com/lixianmin/unicorn/issues)
- 💬 [Discussions](https://github.com/lixianmin/unicorn/discussions)

---

⭐ **If you find this library useful, please consider giving it a star!** ⭐
