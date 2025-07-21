# Unicorn - Unity Core Library

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
[![Unity](https://img.shields.io/badge/Unity-2022.3%2B-green.svg)](https://unity3d.com)
[![C#](https://img.shields.io/badge/C%23-9.0-blue.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)

A powerful, production-ready Unity core library providing essential components for building scalable and maintainable Unity applications. Unicorn offers a modern, decoupled architecture with high-performance systems for UI management, resource loading, entity management, and more.

## ✨ Key Features

- **🏗️ Entity Component System (ECS)** - Lightweight, decoupled component architecture
- **🎛️ Kit System** - Modern alternative to MonoBehaviour with reduced coupling
- **🖼️ Advanced UI Management** - Complete UI lifecycle with animations and layering
- **📦 Smart Resource Management** - Async asset loading with automatic memory management
- **⚡ High-Performance Collections** - Optimized data structures (Deque, SortedTable, Slice)
- **🔄 Coroutine Management** - Efficient coroutine system with better performance than Unity's default
- **⚙️ Configuration Management** - Flexible metadata and configuration system
- **🛠️ Utility Extensions** - Comprehensive set of extension methods and tools

## 🚀 Quick Start

### Prerequisites

- Unity 2022.3 LTS or later
- .NET Standard 2.1 compatible
- YooAsset package (for resource management)

### Installation

1. **Via Unity Package Manager**
   ```
   https://github.com/yourusername/unicorn.git
   ```

2. **Via Git Clone**
   ```bash
   git clone https://github.com/yourusername/unicorn.git
   cd unicorn
   ```

3. **Via Unity Package**
   - Download the latest release
   - Import the `.unitypackage` into your project

## 📖 Core Modules

### Entity Component System (ECS)

A lightweight ECS implementation for decoupled component architecture:

```csharp
// Create an entity
var entity = new Entity();

// Add components (Parts)
var movePart = entity.AddPart<MovePart>();
var healthPart = entity.AddPart<HealthPart>();

// Components are automatically updated via PartUpdateSystem
public class MovePart : Part, IExpensiveUpdater
{
    public void ExpensiveUpdate(float deltaTime)
    {
        // Update logic here
        transform.position += velocity * deltaTime;
    }
}
```

### Kit System

Modern replacement for MonoBehaviour with automatic lifecycle management:

```csharp
// Define a Kit
public class PlayerKit : KitBase
{
    protected override void Awake()
    {
        // Initialization logic
    }
    
    protected override void OnEnable()
    {
        // Enable logic
    }
    
    protected override void SlowUpdate(float deltaTime)
    {
        // 10fps update for performance
    }
}

// Use Kit with MbKitProvider in scene
// No direct coupling between scene objects and code
```

### UI Management

Comprehensive UI system with lifecycle management:

```csharp
// Define a UI Window
public class MainMenuWindow : UIWindowBase
{
    public override string GetAssetPath() => "UI/MainMenu";
    
    protected override void OnLoaded()
    {
        // Setup UI components
        var startButton = GetWidget<Button>("StartButton");
        startButton.onClick.AddListener(OnStartGame);
    }
    
    protected override void OnOpened()
    {
        // Window opened with animation complete
    }
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

### High-Performance Collections

Optimized data structures for game development:

```csharp
// Slice<T> - High-performance List<T> alternative
using var slice = SlicePool.Get<Transform>();
slice.Add(transform1);
slice.Add(transform2);
// Automatically returned to pool when disposed

// Deque<T> - Double-ended queue
var deque = new Deque<int>();
deque.PushBack(1);
deque.PushFront(0);
var front = deque.PopFront(); // 0
var back = deque.PopBack();   // 1

// SortedTable<TKey, TValue> - Sorted dictionary
var table = new SortedTable<string, PlayerData>();
table.Add("player1", playerData);
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
│  UI Management  │  Kit System   │  Resource Loading        │
├─────────────────────────────────────────────────────────────┤
│  ECS Framework  │  Collections  │  Coroutine Management    │
├─────────────────────────────────────────────────────────────┤
│  Core Utilities │  Extensions   │  Configuration System    │
├─────────────────────────────────────────────────────────────┤
│                     Unity Engine                           │
└─────────────────────────────────────────────────────────────┘
```

## 📋 API Reference

### UIManager

| Method | Description |
|--------|-------------|
| `OpenWindow(Type)` | Opens a UI window with lifecycle management |
| `CloseWindow(Type)` | Closes a UI window with animation support |
| `GetWindow(Type)` | Gets an existing window instance |

### WebManager

| Method | Description |
|--------|-------------|
| `LoadAsset(WebArgument, Action<IWebNode>)` | Loads an asset asynchronously |
| `LoadPrefab(WebArgument, Action<WebPrefab>)` | Loads a prefab with automatic memory management |

### CoroutineManager

| Method | Description |
|--------|-------------|
| `StartCoroutine(IEnumerator)` | Starts a managed coroutine |
| `StopCoroutine(IEnumerator)` | Stops a running coroutine |

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
- **Dependencies**: 
  - YooAsset 2.1.1+
  - DOTween (included)
  - TextMeshPro
  - Universal Render Pipeline (URP)

## 🐛 Known Issues

- Kit auto code generation may not load all kit classes on first run
- Some reflection-based features require specific managed stripping levels

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
- YooAsset team for asset management solutions
- DOTween for animation support
- All contributors who have helped improve this library

## 📞 Support

- 📖 [Documentation](https://github.com/yourusername/unicorn/wiki)
- 🐛 [Issue Tracker](https://github.com/yourusername/unicorn/issues)
- 💬 [Discussions](https://github.com/yourusername/unicorn/discussions)

---

⭐ **If you find this library useful, please consider giving it a star!** ⭐

