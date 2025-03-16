# Unity Case Study Project

This project is a **Unity case study** focusing on scalable and efficient event-driven architecture.  
It utilizes **Zenject-based dependency injection**, **MessagePipe for high-performance message handling**,  
and **UniTask for optimized asynchronous programming**.

## 🚀 **Technologies & 3rd Party Tools**
The project integrates the following **third-party tools** for better performance and maintainability:

### **1️⃣ [MessagePipe](https://github.com/Cysharp/MessagePipe)**
- A high-performance **message/event-passing** library for Unity and .NET.  
- Provides **fast, type-safe** event dispatching with **minimal garbage allocation**.  
- Supports **Pub/Sub, async event handling**, and **multi-threaded execution**.

### **2️⃣ [MessagePipe.Zenject](https://github.com/Cysharp/MessagePipe)**
- Seamlessly integrates **MessagePipe** with **Zenject's Dependency Injection (DI)** system.
- Enables **event-driven architecture** while keeping **loose coupling** between components.
- Supports **scoped, global, and local message buses**.

### **3️⃣ [UniTask](https://github.com/Cysharp/UniTask)**
- A **high-performance async/await solution** for Unity.
- Optimized for **low-GC (Garbage Collection) cost** and **lightweight coroutines**.
- Supports **async event handling**, **async LINQ**, and **custom awaiters**.

### 4️⃣ UniRX
- Reactive Extensions for Unity, enabling a declarative programming style.**
- Provides a powerful event-driven model for handling UI interactions and game logic.**
- Supports efficient observables and reactive state management.**

## 🛠 **Project Setup & Dependencies**
To clone and run this project, ensure you have the following dependencies:

1. **Unity 2021.3+** (Recommended LTS version)
2. Install the required **Unity packages** using the Unity Package Manager:
   ```json
   "dependencies": {
    "com.cysharp.messagepipe": "https://github.com/Cysharp/MessagePipe.git?path=src/MessagePipe.Unity/Assets/Plugins/MessagePipe",
    "com.cysharp.messagepipe.zenject": "https://github.com/Cysharp/MessagePipe.git?path=src/MessagePipe.Unity/Assets/Plugins/MessagePipe.Zenject",
    "com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
   }

   

