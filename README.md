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
# 🔥 Changelog - v1.3.0 (Dynamic Port Upgrade)
Previously, MUSC relied on static, hardcoded port assignments based on process names. Version 1.3.0 completely breaks these boundaries:

### Added
- **Dynamic Per-Row Port Configuration:** Every application row slot inside the Applications tab now features a dedicated, editable Port TextBox. You no longer need to modify INI files manually to tweak connection hooks.
- **Clipboard-Safe TCP Range Validation:** The new Port input is strictly validated on-the-fly (MaxLength = 5). It filters out non-digit characters (even during clipboard paste actions) and automatically forces standard TCP boundaries (between 1 and 65535).

### Refactored
- **Bulletproof Save & Reindex Engine:** Rewrote the background SaveSettingsToIni() and ReindexAndSyncDelays() routines. Eliminated critical closure variable capture bugs and localized event handlers to guarantee zero memory leaks during row removal.

###Improved
- **Visual Layout Adaptability:** Polished control anchors and layout widths within the dynamic FlowLayoutPanel container. Labels and text alignment are perfectly normalized across different Windows scaling modes.
###Maintained###
- **19.3MB Idle Footprint:** Despite adding heavy dynamic UI handling, internal memory optimization keeps the idle footprint at a record-breaking 19.3 MB with 0% CPU utilization.


---

# 🛡️ Changelog - v1.2.0 (Stable)

### Added
- **Pre-Launch Port Availability Shield**: An asynchronous, thread-safe network inspection system that intercepts zombie processes and port conflicts before any executable launches.
- **Dynamic Amber/Warning Layout**: Implemented an automated fallback where affected TabControl tabs switch to an Amber warning color if a port conflict is detected.
- **Smart Port Scanner**: Integrated an automated `TryGetConfiguredPort` logic that reads target ports directly from the sequential `Settings.ini` configuration.
- **Formatted About Dialog**: Added a clean, RichTextBox-based, center-aligned About window featuring native Win32 URL execution with 0 MB internal RAM overhead.

### Performance & Optimization
- **Aggressive Resource Flush Engine**: Deployed explicit `GC.Collect` and component `Dispose` protocols triggered immediately after hiding or closing the SplashScreen (Form3) and Settings (Form2).
- **Record-Breaking RAM Footprint**: Successfully reduced idle memory utilization from ~19.3 MB down to an absolute record low of **~14.7 MB**.

### UI/UX & Graphics
- Completely redesigned, modern **SplashScreen** boot graphic.
- Embedded a high-resolution **MUSC software logo** and a custom application icon for professional desktop presentation.

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
