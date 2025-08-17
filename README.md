# AIPaste

AIPaste is a Windows application for editing clipboard text with AI. Simply summon the app with a hotkey and provide instructions to easily modify your copied text. The result is then copied back to your clipboard for immediate use.



## 📝 Overview

This tool uses a large language model (LLM) to revise and proofread text copied to your clipboard. It streamlines tasks like converting a casual message to a friend into a business email or correcting typos and grammatical errors in a document.

It runs in the background and can be called up at any time with a hotkey, so it doesn't interrupt your workflow. It supports both locally running LLMs and Google's Gemini model, allowing users to choose the AI model that best suits their needs.

---

## ✨ Key Features

* **Clipboard Integration**: Automatically reads text from the clipboard to be edited by the AI.
* **Hotkey Support**: Set a global hotkey to call the application at any time.
* **AI-Powered Text Editing**: The AI revises, translates, summarizes, and performs other text edits based on user instructions.
* **Model Selection**: Switch between a locally running LLM and the Google Gemini API.
* **Settings Menu**: Easily configure hotkey combinations, API keys for AI models, and other settings through a GUI.
* **Auto-Startup**: Configure the app to launch automatically when your PC starts.

---

## 🛠️ System Requirements

* **OS**: Windows 11 or later
* **Framework**: .NET 8.0
* **Platform**: WinUI 3 / Windows App SDK

---

## 🚀 Installation

### 1. Manual Placement of LLamaSharp Backend

This application uses the `LLamaSharp` library, but some of its binary files must be placed manually. This step is necessary to work around a known issue with the library ([issue #382](https://github.com/SciSharp/LLamaSharp/issues/382)).

Please download the binary files for the following libraries and place them within the `Binary/LLamaSharpBackend` directory according to the specified structure.

* LLamaSharp.Backend.Cpu
* LLamaSharp.Backend.Cuda11.Windows
* LLamaSharp.Backend.Cuda12.Windows
* LLamaSharp.Backend.Vulkan.Windows

**Directory Structure:**
Binary
 └── LLamaSharpBackend
   └── win-x64
     └── native
       ├── avx
       │   ├── ggml.dll
       │   └── ...
       ├── avx2
       ├── cuda11
       └── ...

### 2. Build

Open the `AIPaste.sln` file in Visual Studio 2022 or later and build the project.

---

## How to Use

1.  Copy the text you want to edit to your clipboard.
2.  Press the configured hotkey (default is `Ctrl + Alt + C`) to bring up the AIPaste window.
3.  In the input box at the bottom of the window, type your instructions for the AI (e.g., "Rewrite this as a business email") and press the execute button.
4.  The AI-generated text will appear in the "Generated Result" field.
5.  Press the "Copy" button to copy the generated text to your clipboard, ready to be pasted into other applications.

---

## ⚙️ Settings

On the settings page, you can customize the following options:

* **Hotkey**: Change the key combination to summon the application.
* **Auto-Startup**: Set whether AIPaste should start automatically when Windows boots up.
* **Model Selection**:
    * **Local LLM**: Set the path to your local model file and configure settings like GPU usage.
    * **Gemini**: Set the API key for using the Google Gemini API.

---

## 📄 License

This repository is licensed under the **MIT License**. See `LICENSE.txt` for details.
