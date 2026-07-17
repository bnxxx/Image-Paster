# Clipboard Image Paster (Windows Context Menu Utility)

A lightweight, native C# (.NET/Windows) utility that adds **"Paste clipboard image"** and **"Paste image & transcribe text (OCR)"** options directly to your Windows Explorer right-click context menu (both when right-clicking on folder icons AND on the empty background inside open folders). Simply copy any screenshot, image file, or picture from your browser or editor, right-click on any folder icon or open folder background in Windows Explorer, and select either option to instantly save the image (and optionally extract its text) directly into that folder.

---

## Key Features

- **Instant & Silent Saving**: By default, images on the clipboard are automatically saved as high-quality PNG files (`Image_YYYY-MM-DD_HHMMSS.png`) directly inside the clicked directory without any interrupting dialogs or notifications.
- **Built-in Optical Character Recognition (OCR)**: Select **"Paste image & transcribe text (OCR)"** from the context menu to automatically run native Windows AI text recognition on the pasted image. The application extracts all text (`Text_YYYY-MM-DD_HHMMSS.txt`) while preserving recognized line breaks (`\r\n`), saving **only the text file** (no image file is created).
- **Custom Save Dialog (SHIFT + Click)**: Need a custom filename or a different format (JPG, BMP, PNG)? Hold down the **SHIFT** key while clicking either context menu option to launch a quick Save As dialog.
- **Smart Clipboard Detection**: Works effortlessly with raw clipboard bitmaps (e.g., from **Snipping Tool / Win + Shift + S**, browser copy, PrintScreen, Photoshop) as well as copied image files from Windows Explorer (`.png`, `.jpg`, `.jpeg`, `.bmp`, `.gif`, `.tiff`).
- **Zero External Dependencies**: Built using native C# (.NET Framework / WinRT / WinForms), resulting in a tiny, standalone executable that runs instantly on Windows 10 and 11 without requiring external runtime installers or heavy libraries.
- **Built-in Setup & Installer GUI**: Launching the executable (`ClipboardPaster.exe`) directly by double-clicking opens a clean, modern installer graphical interface to easily **Install** or **Uninstall** all context menu entries with a single click.
- **Command-Line & Automation Support**: Includes command-line arguments (`--install`, `--uninstall`, `--ocr`) for scripted deployments, winget packaging, and IT administration.

---

## Quick Start & Installation

### Option 1: One-Line PowerShell Install (Recommended - No Approval Required)
The fastest way to install the application instantly right now. Open PowerShell and run this single line:
```powershell
irm https://raw.githubusercontent.com/bnxxx/Image-Paster/main/install.ps1 | iex
```
This automatically downloads the latest release, places it in `%LOCALAPPDATA%\Programs\ImagePaster`, and silently registers the context menu options without interrupting you.

To uninstall anytime using a single command, run:
```powershell
irm https://raw.githubusercontent.com/bnxxx/Image-Paster/main/uninstall.ps1 | iex
```


### Option 2: Winget (Windows Package Manager)
If installed via the official Winget Community Repository (`microsoft/winget-pkgs`), open PowerShell or Command Prompt and run:
```powershell
winget install bnxxx.ImagePaster
```


### Option 3: Manual GUI Setup / Portable Download
If you prefer not to use terminal commands or want a standalone portable setup:
1. Download `ClipboardPaster.exe` from the [Releases](https://github.com/bnxxx/Image-Paster/releases) page.
2. Double-click `ClipboardPaster.exe` (or run it without arguments).
3. The **Clipboard Image Paster Setup** dialog will open.
4. Click **Install to Context Menu**.
5. You are done! Open Windows Explorer, right-click on any folder icon or inside any open folder background, and you will see **"Paste clipboard image"** and **"Paste image & transcribe text (OCR)"**.

To uninstall using the GUI, simply run `ClipboardPaster.exe` again and click **Uninstall from Context Menu**.

---

## Usage Guide

### 1. Automatic Silent Paste
1. Copy an image to your clipboard using any tool (e.g., press `Win + Shift + S` to take a screenshot with Windows Snipping Tool, or right-click an image on the web and click *Copy Image*).
2. Navigate to any folder in Windows Explorer.
3. Right-click the folder icon OR the empty background inside the open folder, and select **Paste clipboard image**.
4. The image will be silently saved inside the folder as `Image_YYYY-MM-DD_HHMMSS.png`.

### 2. Paste + OCR Text Extraction
1. Copy an image containing text (such as a receipt, invoice, code snippet, or scanned document) to your clipboard.
2. Right-click the folder icon OR the empty background inside any open folder.
3. Select **Paste image & transcribe text (OCR)**.
4. The application extracts the text using native Windows 10/11 OCR, saving **only the transcribed text** (`Text_YYYY-MM-DD_HHMMSS.txt`) directly inside the folder while preserving all recognized line breaks (`\r\n`). No image (`.png`) file is written to disk in this mode.

### 3. Custom Filename / Format Selection
1. Copy an image to your clipboard.
2. Right-click the folder icon or inside the folder where you want to save it.
3. Hold the **SHIFT** key on your keyboard and click either **Paste clipboard image** or **Paste image & transcribe text (OCR)**.
4. A Save As dialog will appear, allowing you to choose a custom file name or format. If OCR was selected, you will be prompted to choose a destination and filename for the transcribed `.txt` file.

---

## Architecture & File Summary

- `src/Program.cs`: Application entry point. Parses command-line arguments to handle `--install`, `--uninstall`, `--gui`, `--ocr`, or right-click context menu invocations (`%V`).
- `src/RegistryManager.cs`: Manages registry modifications under `HKCU\Software\Classes\Directory\shell` and `Directory\Background\shell` for both standard paste (`ClipboardPaster`) and OCR transcription (`ClipboardPasterOCR`).
- `src/ClipboardHandler.cs`: Handles clipboard inspection (`ContainsImage()` / `ContainsFileDropList()`), SHIFT key detection (`GetAsyncKeyState`), unique timestamp filename generation, and image saving (`STAThread`).
- `src/OcrTranscriber.cs`: Interfaces with native Windows 10/11 WinRT OCR APIs (`Windows.Media.Ocr.OcrEngine`) using synchronous polling loops (`WaitForResult`) to convert images into text files without external DLLs.
- `src/InstallerForm.cs`: Modern WinForms graphical interface for checking installation status and managing context menu registration.
- `build.bat` / `build.ps1`: Zero-dependency build scripts for Windows.
