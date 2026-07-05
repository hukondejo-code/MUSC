# MU SERVER CONTROL (MUSC)

MU SERVER CONTROL (MUSC) is a complete, lightweight, and robust private server manager tool designed primarily for MU ONLINE, but completely adaptable to any server software package thanks to its high modularity. It automates the sequential launching, monitoring, and embedding of external application windows directly into a unified dashboard layout.

---

## 🚀 Key Features

* **Win32 API Windows Embedding**: Seamlessly embeds external `.exe` application windows into individual tab pages inside a unified dashboard using native `SetParent` and `MoveWindow` calls.
* **High Modularity & Scalability**: Abandoned hardcoded elements to support dynamically generated rows, layout forms, and interface tabs based on active configurations.
* **Live Configuration Updates**: Real-time configuration reloading ensures UI elements, tab names, and state-colors update instantly without requiring a application restart.
* **Visual Status Monitoring**: Tab colors dynamically reflect process states in real-time (Red = Unconfigured, Grey = Configured but stopped, Green = Running).
* **Robust Multi-threading**: Handles heavy sequential launch operations on separate background threads to prevent UI freezing and ensure fluid user experience.
* **Safe Working Directories**: Automatically executes programs out of their native directories (`WorkingDirectory`) to maintain SQL, database, and network connectivity dependencies.

---

## 🔄 Changelog: Base Version vs. Refactored GitHub Version

The software has undergone a comprehensive structural rewrite, transitioning from a rigid monolithic prototype into a fully modular, enterprise-grade server manager.

### 🛠️ Configuration & Storage Evolution
* **INI-Backed Storage Engine**: Upgraded the file system from a flat row-based `musc.dat` file to an structured, section-based `Settings.ini` storage format.
* **Automated Data Migration**: Built a safe migration routine that reads legacy data formats on startup and automatically upgrades it to the new configuration standard without data loss.

### ⚙️ Settings Window (Form2) Overhaul
* **Dynamic Row Generation**: Replaced the static layout of 8 hardcoded buttons/labels with an auto-generating row engine. The interface now grows dynamically according to the environment configurations.
* **Enhanced Row Controls**: Every individual software row now features localized controls: **Browse** (select file), **Cfg** (configure parameters), and **Open** (quick launch).
* **Input Validation & Safety**: Added an in-row validation system for the startup delays text box (`VAR_n`). It enforces a minimum value of `1` and prevents unexpected software crashes from empty or invalid string inputs.
* **Delays Management Tab**: Added a dedicated sub-tab for managing application startup delays. Delays stay synced during row deletion, re-indexing, and permanent state persistence.

### 💻 Main Interface (Form1) & Stability Enhancements
* **Dynamic Tab Control**: The primary interface now dynamically generates individual tabs on demand based on configuration depth, removing empty placeholder tabs.
* **Thread-Safe Process Tracking**: Improved how background threads keep track of embedded `.exe` window handles and lifecycle states.
* **Graceful Shutdown Routine**: Added an automated process-cleanup sequence. Closing the main manager gracefully terminates hooks and handles for all running child processes, ensuring no ghost tasks are left behind in the background.
* **IDE & Designer Fixes**: Cleaned up corrupted designer resource references (`.resx` anomalies) inside Visual Studio. Integrated stable visual fallbacks to eliminate environment-specific layout crashes.
