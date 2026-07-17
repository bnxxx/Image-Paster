# Clipboard Image Paster (Windows Context Menu Utility)

A lightweight, native C# (.NET/Windows) utility that adds a **"Paste clipboard image"** option directly to your Windows Explorer right-click context menu (both when right-clicking on folder icons AND on the empty background inside open folders). Simply copy any screenshot, image file, or picture from your browser or editor, right-click on any folder icon or open folder background in Windows Explorer, and select **"Paste clipboard image"** to save it directly into that folder.

---

## Key Features

- **Instant & Silent Saving**: By default, images on the clipboard are automatically saved as high-quality PNG files (`Image_YYYY-MM-DD_HHMMSS.png`) directly inside the clicked directory without any interrupting dialogs or notifications.
- **Custom Save Dialog (SHIFT + Click)**: Need a custom filename or a different format (JPG, BMP, PNG)? Hold down the **SHIFT** key while clicking **"Paste clipboard image"** in the context menu to launch a quick Save As dialog.
- **Smart Clipboard Detection**: Works effortlessly with raw clipboard bitmaps (e.g., from **Snipping Tool / Win + Shift + S**, browser copy, PrintScreen, Photoshop) as well as copied image files from Windows Explorer (`.png`, `.jpg`, `.jpeg`, `.bmp`, `.gif`, `.tiff`).
- **Zero External Dependencies**: Built using native C# (.NET Framework / WinForms), resulting in a tiny, standalone executable (~20 KB) that runs instantly on Windows 7, 8, 10, and 11 without requiring external runtime installers or libraries.
- **Built-in Setup & Installer GUI**: Launching the executable (`ClipboardPaster.exe`) directly by double-clicking opens a clean, modern installer graphical interface to easily **Install** or **Uninstall** the context menu entry with a single click.
- **Command-Line & Automation Support**: Includes command-line arguments (`--install` and `--uninstall`) for scripted deployments, winget packaging, and IT administration.

---

## Quick Start & Installation

### Option 1: GUI Setup (Recommended)
1. Double-click `bin\ClipboardPaster.exe` (or run it without arguments).
2. The **Clipboard Image Paster Setup** dialog will open.
3. Click **Install to Context Menu**.
4. You are done! Open Windows Explorer, right-click on any folder icon or inside any open folder background, and you will see **"Paste clipboard image"**.

### Option 2: Command-Line / Scripted Installation
Open a terminal (Command Prompt or PowerShell) and run:
```cmd
bin\ClipboardPaster.exe --install
```

To uninstall at any time:
```cmd
bin\ClipboardPaster.exe --uninstall
```

---

## Usage Guide

### 1. Automatic Silent Paste
1. Copy an image to your clipboard using any tool (e.g., press `Win + Shift + S` to take a screenshot with Windows Snipping Tool, or right-click an image on the web and click *Copy Image*).
2. Navigate to any folder in Windows Explorer.
3. Right-click the folder icon OR the empty background inside the open folder, and select **Paste clipboard image**.
4. The image will be silently saved inside the folder as `Image_YYYY-MM-DD_HHMMSS.png`.

### 2. Custom Filename / Format Selection
1. Copy an image to your clipboard.
2. Right-click the folder icon or inside the folder where you want to save it.
3. Hold the **SHIFT** key on your keyboard and click **Paste clipboard image**.
4. A Save As dialog will appear, allowing you to choose a custom file name or change the format between PNG, JPG, or BMP before saving.

---

## Building from Source

You do **not** need Visual Studio or heavy SDKs installed to compile this project. It uses the pre-installed Microsoft `.NET Framework` C# compiler (`csc.exe`) included out-of-the-box with Windows (`C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe`).

### Compile using Batch (`build.bat`):
Open Command Prompt inside the project directory and run:
```cmd
build.bat
```

### Compile using PowerShell (`build.ps1`):
Open PowerShell inside the project directory and run:
```powershell
.\build.ps1
```

The compiled binary will be placed inside `bin\ClipboardPaster.exe`.

---

## Architecture & File Summary

- `src/Program.cs`: Application entry point. Parses command-line arguments to handle `--install`, `--uninstall`, `--gui`, or right-click context menu invocations (`%V`).
- `src/RegistryManager.cs`: Manages registry modifications under `HKCU\Software\Classes\Directory\shell\ClipboardPaster` and `HKCU\Software\Classes\Directory\Background\shell\ClipboardPaster` without requiring Administrator privileges.
- `src/ClipboardHandler.cs`: Handles clipboard inspection (`ContainsImage()` / `ContainsFileDropList()`), SHIFT key detection (`GetAsyncKeyState`), unique timestamp filename generation, and image saving (`STAThread`).
- `src/InstallerForm.cs`: Modern WinForms graphical interface for checking installation status and managing context menu registration.
- `build.bat` / `build.ps1`: Zero-dependency build scripts for Windows.
