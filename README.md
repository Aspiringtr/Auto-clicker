# 🖱️ Avalonia Auto Clicker

A minimal, cross-platform **Auto Clicker** built using [Avalonia UI](https://avaloniaui.net/) and C#. This tool allows users to:
- Monitor real-time mouse coordinates and capture positions
- Start clicking with a button
- Automatically move the mouse to a defined position
- Perform repeated clicks in a loop

---

## 🚀 Features

- ✅ 200x230 pixel clean Avalonia UI window
- ✅ Start mouse tracking with a button
- ✅ Press `Q` to capture current mouse position
- ✅ Press `Start` to start auto-clicking at the position
- ✅ Live mouse position shown in a `TextBlock`
- ✅ Acrylic ui because I like it
---

## 🧰 Tech Stack

- [.NET 7.0+](https://dotnet.microsoft.com/)
- [Python](https://www.python.org/)
- [Avalonia UI](https://avaloniaui.net/) (Cross-platform UI)
- Windows API via `user32.dll` for native mouse control (Windows only)

---

## 📦 Requirements

- Windows OS (due to native `SetCursorPos`, `GetCursorPos`, and `mouse_event`)
- .NET 7 SDK or higher
- Avalonia UI NuGet packages

---

## 🔧 Build & Run

1. **Clone the repository**
   ```bash
   git clone https://github.com/Aspiringtr/Auto-clicker.git
   cd Auto-clicker
   dotnet run
