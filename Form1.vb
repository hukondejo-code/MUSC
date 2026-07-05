Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Threading
Imports System.Text

Public Class Form1

    ' Tracks if we've attached the VisibleChanged handler to the Form2 instance
    Private form2VisibleHandlerAttached As Boolean = False

    ' --- Win32 API funkciók az ablakok átrakásához és stílusához ---
    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetParent(hWndChild As IntPtr, hWndNewParent As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function MoveWindow(hWnd As IntPtr, X As Integer, Y As Integer, nWidth As Integer, nHeight As Integer, bRepaint As Boolean) As Boolean
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function GetWindowLong(hWnd As IntPtr, nIndex As Integer) As Integer
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetWindowLong(hWnd As IntPtr, nIndex As Integer, dwNewLong As Integer) As Integer
    End Function

    Private Const GWL_STYLE As Integer = -16
    Private Const WS_CHILD As Integer = &H40000000

    ' Beállításokat tároló szótár (Dictionary)
    Public Beallitasok As New Dictionary(Of String, String)()
    ' Track started processes by index
    Private StartedProcesses As New Dictionary(Of Integer, Process)()
    ' Logging
    Private ReadOnly logLock As New Object()
    Private ReadOnly LogFilePath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "monitor.log")
    Private LoggingEnabled As Boolean = False
    Private Sub Log(message As String)
        Try
            If Not LoggingEnabled Then Return
            SyncLock logLock
                Try
                    EnsureLogRotation()
                Catch
                End Try
                Dim entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}"
                File.AppendAllText(LogFilePath, entry, Encoding.UTF8)
            End SyncLock
        Catch
        End Try
    End Sub

    Private Sub EnsureLogRotation()
        Try
            Dim maxBytes As Long = 5L * 1024L * 1024L ' 5 MB
            If File.Exists(LogFilePath) Then
                Dim fi As New FileInfo(LogFilePath)
                If fi.Length >= maxBytes Then
                    Dim rotated As String = Path.Combine(Path.GetDirectoryName(LogFilePath), "monitor.log.1")
                    Try
                        If File.Exists(rotated) Then File.Delete(rotated)
                    Catch
                    End Try
                    File.Move(LogFilePath, rotated)
                End If
            End If
        Catch
        End Try
    End Sub
    ' Monitor cancellation and in-progress starts
    Private monitorCts As Threading.CancellationTokenSource = Nothing
    Private monitorLock As New Object()
    Private startingSlots As New HashSet(Of Integer)()

    ' Return highest indexed NEV_ entry (number of app rows)
    Private Function GetSettingsCount() As Integer
        Dim maxIdx As Integer = 0
        For Each k In Beallitasok.Keys
            If k.StartsWith("NEV_") Then
                Dim parts = k.Split("_"c)
                Dim idx As Integer
                If parts.Length > 1 AndAlso Integer.TryParse(parts(1), idx) Then
                    If idx > maxIdx Then maxIdx = idx
                End If
            End If
        Next
        Return maxIdx
    End Function

    ' Ensure TabControl has exactly n TabPages; each tab gets a Panel named Panel{index}
    Private Sub EnsureTabsMatchCount(n As Integer)
        If TabControl1 Is Nothing Then Return
        If n < 1 Then n = 1

        ' Add missing tabs
        While TabControl1.TabPages.Count < n
            Dim newIndex = TabControl1.TabPages.Count + 1
            Dim tp As New TabPage("Empty")
            tp.BackColor = Color.Transparent
            tp.Padding = New Padding(3)

            Dim pnl As New Panel() With {
                .Name = "Panel" & newIndex,
                .Dock = DockStyle.Fill,
                .BorderStyle = BorderStyle.None
            }

            ' Previously we added a per-tab Shutdown button here; removed because auto-restart
            ' now manages lifecycle and the per-tab button caused clutter.
            tp.Controls.Add(pnl)
            TabControl1.TabPages.Add(tp)
        End While

        ' Remove extra tabs
        While TabControl1.TabPages.Count > n
            TabControl1.TabPages.RemoveAt(TabControl1.TabPages.Count - 1)
        End While
    End Sub



    ' --- Amikor a Visual Basic program elindul (Most már csak a fájlt olvassa be!) ---
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' A Form1 betöltésekor csak a memóriában lévő beállításokat érvényesítjük, 
        ' mert a tényleges beolvasást már a Splash ablak elvégezte előttünk!
        BeallitasokBetoltese()
        AblakFülNevekFrissitese()

        ' Populate StartedProcesses from currently running system processes that match configured EXE names
        Try
            SyncStartedProcessesFromSystem()
        Catch
        End Try

        ' Initialize logging enabled from settings if present
        Try
            If Beallitasok.ContainsKey("LOGGING_ENABLED") Then
                Dim raw = Beallitasok("LOGGING_ENABLED")
                If String.Equals(raw, "1") OrElse String.Equals(raw, "true", StringComparison.OrdinalIgnoreCase) Then
                    LoggingEnabled = True
                Else
                    Boolean.TryParse(raw, LoggingEnabled)
                End If
            End If
        Catch
            LoggingEnabled = False
        End Try

        ' Add File -> Enable Logging menu item (toggle)
        Try
            Dim ms As MenuStrip = Nothing
            Try
                ms = Me.MenuStrip1
            Catch
            End Try
            If ms Is Nothing Then
                ms = Me.Controls.OfType(Of MenuStrip)().FirstOrDefault()
            End If

            If ms IsNot Nothing Then
                Dim fileItem = ms.Items.OfType(Of ToolStripMenuItem)().FirstOrDefault(Function(t) t.Name = "FileToolStripMenuItem")
                If fileItem Is Nothing Then
                    fileItem = ms.Items.OfType(Of ToolStripMenuItem)().FirstOrDefault()
                End If

                If fileItem IsNot Nothing Then
                    Dim restartEnabled As Boolean = False
                    Try
                        If Beallitasok.ContainsKey("PRCEN_GLOBAL") Then
                            Dim raw = Beallitasok("PRCEN_GLOBAL")
                            If String.Equals(raw, "1") OrElse String.Equals(raw, "true", StringComparison.OrdinalIgnoreCase) Then
                                restartEnabled = True
                            Else
                                Boolean.TryParse(raw, restartEnabled)
                            End If
                        End If
                    Catch
                        restartEnabled = False
                    End Try

                    Dim existingRestartItem = fileItem.DropDownItems.OfType(Of ToolStripMenuItem)().FirstOrDefault(Function(t) t.Name = "EnableAutoRestartToolStripMenuItem")
                    Dim existingLogItem = fileItem.DropDownItems.OfType(Of ToolStripMenuItem)().FirstOrDefault(Function(t) t.Name = "EnableLoggingToolStripMenuItem")
                    If existingRestartItem Is Nothing Then
                        Dim restartItem As New ToolStripMenuItem("Enable Auto-Restart") With {
                            .CheckOnClick = True,
                            .Checked = restartEnabled,
                            .Name = "EnableAutoRestartToolStripMenuItem"
                        }
                        AddHandler restartItem.Click, Sub(s, ev)
                                                           Try
                                                               Dim itm = CType(s, ToolStripMenuItem)
                                                               Dim enabled = itm.Checked
                                                               Try
                                                                   Dim iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.ini")
                                                                   Dim existing = If(File.Exists(iniPath), SettingsStore.ReadSettingsFromFile(iniPath), New Dictionary(Of String, String)())
                                                                   existing("PRCEN_GLOBAL") = If(enabled, "1", "0")
                                                                   SettingsStore.WriteSettingsToFile(iniPath, existing)
                                                                   If Beallitasok Is Nothing Then Beallitasok = New Dictionary(Of String, String)()
                                                                   Beallitasok("PRCEN_GLOBAL") = If(enabled, "1", "0")
                                                               Catch
                                                               End Try
                                                               Log("Auto-Restart toggled: " & enabled.ToString())
                                                           Catch
                                                           End Try
                                                       End Sub
                        fileItem.DropDownItems.Insert(0, restartItem)
                    Else
                        existingRestartItem.Checked = restartEnabled
                    End If

                    If existingLogItem Is Nothing Then
                        Dim logItem As New ToolStripMenuItem("Enable Logging") With {
                            .CheckOnClick = True,
                            .Checked = LoggingEnabled,
                            .Name = "EnableLoggingToolStripMenuItem"
                        }
                        AddHandler logItem.Click, Sub(s, ev)
                                                       Try
                                                           Dim itm = CType(s, ToolStripMenuItem)
                                                           LoggingEnabled = itm.Checked
                                                           ' persist to INI immediately
                                                           Try
                                                               Dim iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.ini")
                                                               Dim existing = If(File.Exists(iniPath), SettingsStore.ReadSettingsFromFile(iniPath), New Dictionary(Of String, String)())
                                                               existing("LOGGING_ENABLED") = If(LoggingEnabled, "1", "0")
                                                               SettingsStore.WriteSettingsToFile(iniPath, existing)
                                                               ' update in-memory map
                                                               If Beallitasok Is Nothing Then Beallitasok = New Dictionary(Of String, String)()
                                                               Beallitasok("LOGGING_ENABLED") = If(LoggingEnabled, "1", "0")
                                                           Catch
                                                           End Try
                                                           Log("Logging toggled: " & LoggingEnabled.ToString())
                                                       Catch
                                                       End Try
                                                   End Sub
                        Dim restartIndex As Integer = fileItem.DropDownItems.IndexOfKey("EnableAutoRestartToolStripMenuItem")
                        If restartIndex >= 0 Then
                            fileItem.DropDownItems.Insert(restartIndex + 1, logItem)
                        Else
                            fileItem.DropDownItems.Insert(0, logItem)
                        End If
                    Else
                        existingLogItem.Checked = LoggingEnabled
                    End If
                End If
            End If
        Catch
        End Try

        ' Start background process monitor if enabled by settings
        Try
            StartProcessMonitor()
        Catch
        End Try
    End Sub

    ' Scan running processes and populate StartedProcesses for indices matching configured EXE_ entries
    Private Sub SyncStartedProcessesFromSystem()
        Try
            Dim maxIdx = GetSettingsCount()
            Log($"SyncStartedProcessesFromSystem: scanning up to {maxIdx} slots")
            For i As Integer = 1 To maxIdx
                Try
                    Dim keyExe = "EXE_" & i
                    Dim keyMap = "MAPPA_" & i
                    Dim keyNev = "NEV_" & i
                    If Not Beallitasok.ContainsKey(keyExe) Then Continue For

                    Dim exeName = Beallitasok(keyExe)
                    If String.IsNullOrWhiteSpace(exeName) Then Continue For

                    Dim procName = Path.GetFileNameWithoutExtension(exeName)
                    If String.IsNullOrWhiteSpace(procName) Then Continue For

                    Dim procs = Process.GetProcessesByName(procName)
                    If procs Is Nothing OrElse procs.Length = 0 Then Continue For

                    Dim p As Process = procs(0)

                    SyncLock StartedProcesses
                        If Not StartedProcesses.ContainsKey(i) Then
                            StartedProcesses.Add(i, p)
                        Else
                            StartedProcesses(i) = p
                        End If
                    End SyncLock

                    ' Try to embed window into the corresponding panel (similar to startup embedding)
                    Me.Invoke(Sub()
                                  Try
                                      Dim celPanel As Panel = Nothing
                                      If TabControl1 IsNot Nothing AndAlso TabControl1.TabPages.Count >= i Then
                                          Dim tp As TabPage = TabControl1.TabPages(i - 1)
                                          celPanel = tp.Controls.OfType(Of Panel)().FirstOrDefault(Function(pl) pl.Name = ("Panel" & i))
                                          If celPanel Is Nothing Then celPanel = tp.Controls.OfType(Of Panel)().FirstOrDefault()
                                      Else
                                          Dim panelNev As String = "Panel" & i
                                          celPanel = TryCast(Me.Controls.Find(panelNev, True).FirstOrDefault(), Panel)
                                      End If

                                      If celPanel IsNot Nothing Then
                                          Dim handle As IntPtr = IntPtr.Zero
                                          Dim attempts As Integer = 0
                                          Log($"Slot {i}: polling for MainWindowHandle (up to 10s) for process Id={p.Id}")
                                          While attempts < 100 AndAlso handle = IntPtr.Zero
                                              Try
                                                  p.Refresh()
                                                  Try
                                                      p.WaitForInputIdle(200)
                                                  Catch
                                                  End Try
                                              Catch
                                              End Try
                                              handle = p.MainWindowHandle
                                              If handle = IntPtr.Zero Then
                                                  Threading.Thread.Sleep(100)
                                                  attempts += 1
                                              End If
                                          End While
                                          If handle <> IntPtr.Zero Then
                                              Try
                                                  SetWindowLong(handle, GWL_STYLE, WS_CHILD Or Visible)
                                                  SetParent(handle, celPanel.Handle)
                                                  MoveWindow(handle, 0, 0, celPanel.Width, celPanel.Height, True)
                                                  Log($"Slot {i}: embedded process Id={p.Id} handle={handle} into Panel{ i }")
                                              Catch exEmbed As Exception
                                                  Log($"Slot {i}: embed failed for process Id={p.Id} - " & exEmbed.Message)
                                              End Try
                                          Else
                                              Log($"Slot {i}: main window handle not available after polling for process Id={p.Id}")
                                          End If
                                      Else
                                          Log($"Slot {i}: target panel not found for embedding")
                                      End If
                                  Catch
                                  End Try
                              End Sub)
                Catch
                End Try
            Next
        Catch
        End Try
    End Sub

    ' Készítsünk egy külön nyilvános segédfunkciót a Form1-ben, amit a Splash meg tud hívni a fülek átnevezéséhez:
    Public Sub AblakFülNevekFrissitese()
        If TabControl1 Is Nothing Then Return

        Dim count = GetSettingsCount()
        If count < 1 Then count = 1

        EnsureTabsMatchCount(count)

        For i As Integer = 1 To TabControl1.TabPages.Count
            Dim kulcsNev As String = "NEV_" & i
            If Beallitasok.ContainsKey(kulcsNev) Then
                TabControl1.TabPages(i - 1).Text = Beallitasok(kulcsNev)
            Else
                TabControl1.TabPages(i - 1).Text = "NOT DEFINED"
            End If
        Next

        TabControl1.Invalidate()
    End Sub



    Public Sub BeallitasokBetoltese()
        Try
            Beallitasok.Clear()
            ' Ensure legacy Settings.dat is migrated to Settings.ini
            SettingsStore.MigrateDatToIni(AppDomain.CurrentDomain.BaseDirectory)

            Dim iniUtvonal As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.ini")

            If File.Exists(iniUtvonal) Then
                Beallitasok = SettingsStore.ReadSettingsFromFile(iniUtvonal)
            Else
                MessageBox.Show("Can't find the settings file (Settings.ini). It will be created after your first save in Settings.")
                Exit Sub
            End If

            ' Normalize VAR_ units: if VAR appears to be milliseconds (>=1000), convert to seconds
            Try
                Dim migrated As Boolean = False
                Dim keys = New List(Of String)(Beallitasok.Keys)
                For Each k In keys
                    If k.StartsWith("VAR_") Then
                        Dim raw = Beallitasok(k)
                        Dim v As Integer = 0
                        If Integer.TryParse(raw, v) Then
                            If v >= 1000 Then
                                ' assume milliseconds, convert to seconds (integer)
                                Dim sec = Math.Max(1, v \ 1000)
                                Beallitasok(k) = sec.ToString()
                                migrated = True
                                Log($"Migration: converted {k} from {v} (ms) to {sec} (s)")
                            ElseIf v < 1 Then
                                Beallitasok(k) = "1"
                                migrated = True
                                Log($"Migration: normalized {k} from {v} to 1 (s)")
                            End If
                        End If
                    End If
                Next
                If migrated Then
                    Try
                        SettingsStore.WriteSettingsToFile(iniUtvonal, Beallitasok)
                        Log("Migration: updated Settings.ini with normalized VAR_ values")
                    Catch
                    End Try
                End If
            Catch
            End Try

            ' Ensure tabs match settings count and update their names
            Dim n As Integer = GetSettingsCount()
            If n < 1 Then n = 1
            EnsureTabsMatchCount(n)
            AblakFülNevekFrissitese()

        Catch ex As Exception
            MessageBox.Show("Loading error or tabname error: " & ex.Message)
        End Try
        If TabControl1 IsNot Nothing Then TabControl1.Invalidate()

    End Sub


    ' --- Ezt a szubrutint hívja meg a Futtatás gomb ---
    Private Sub InditasFolyamata()
        ' Biztonság kedvéért újra beolvassuk, hátha módosítottad a Settings-ben
        BeallitasokBetoltese()

        Dim inditoSzal As New Thread(AddressOf SzoftvercsomagInditasa)
        inditoSzal.IsBackground = True
        inditoSzal.Start()
    End Sub


    ' --- A 8 alkalmazás egymás utáni indítása, beágyazása és DUPLIKÁCIÓ ELLENŐRZÉSE ---
    Private Sub SzoftvercsomagInditasa()
        Dim total As Integer = GetSettingsCount()
        If total < 1 Then total = 1

        For i As Integer = 1 To total
            Try
                ' Változók kinyerése a beállításokból a sorszám alapján
                Dim kulcsMappa As String = "MAPPA_" & i
                Dim kulcsExe As String = "EXE_" & i
                Dim kulcsVar As String = "VAR_" & i
                Dim kulcsNev As String = "NEV_" & i
                If Beallitasok.ContainsKey(kulcsMappa) AndAlso Beallitasok.ContainsKey(kulcsExe) AndAlso Beallitasok.ContainsKey(kulcsNev) Then
                    Dim mappa As String = Beallitasok(kulcsMappa)
                    Dim exe As String = Beallitasok(kulcsExe)
                    Dim psNev As String = Beallitasok(kulcsNev)
                    Dim szoftverNev As String = Beallitasok(kulcsNev)
                    Dim varakozas As Integer = Integer.Parse(Beallitasok(kulcsVar)) * 1000

                    ' -----------------------------------------------------------------
                    ' BUG JAVÍTÁS: Ellenőrizzük, hogy fut-e már ez a folyamat a rendszerben
                    ' -----------------------------------------------------------------
                    Dim futoFolyamatok As Process() = Process.GetProcessesByName(psNev)

                    If futoFolyamatok.Length > 0 Then
                        ' Ha a lista nem üres, a program már fut. Kiíratjuk és átugorjuk.
                        Me.Invoke(Sub()
                                      MessageBox.Show($"The '{szoftverNev}' executable is running. Skipping.", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                  End Sub)
                        Continue For ' Azonnal ugrik a ciklus a következő (i+1) alkalmazásra
                    End If
                    ' -----------------------------------------------------------------

                    ' Ha nem fut, akkor elindítjuk a megszokott módon
                    Dim psi As New ProcessStartInfo()
                    psi.FileName = Path.Combine(mappa, exe)
                    psi.WorkingDirectory = mappa

                    Dim p As Process = Process.Start(psi)

                    ' track started process so we can shut it down later
                    Try
                        SyncLock StartedProcesses
                            If StartedProcesses.ContainsKey(i) Then
                                StartedProcesses(i) = p
                            Else
                                StartedProcesses.Add(i, p)
                            End If
                        End SyncLock
                    Catch
                    End Try

                    ' Várunk, amíg az ablak létrejön a Windowsban
                    Dim ablakHandle As IntPtr = IntPtr.Zero
                    Dim szamlalo As Integer = 0
                    While ablakHandle = IntPtr.Zero And szamlalo < 40
                        Thread.Sleep(100)
                        p.Refresh()
                        ablakHandle = p.MainWindowHandle
                        szamlalo += 1
                    End While

                    ' Ha megvan az ablak, beágyazzuk a megfelelő sorszámú Panelbe
                    If ablakHandle <> IntPtr.Zero Then
                        Me.Invoke(Sub()
                                      ' Find the target panel inside the corresponding TabPage
                                      Dim celPanel As Panel = Nothing
                                      If TabControl1 IsNot Nothing AndAlso TabControl1.TabPages.Count >= i Then
                                          Dim tp As TabPage = TabControl1.TabPages(i - 1)
                                          Dim predicate As Func(Of Panel, Boolean) = Function(pl) pl.Name = ("Panel" & i)
                                          celPanel = tp.Controls.OfType(Of Panel)().FirstOrDefault(predicate:=predicate)
                                          If celPanel Is Nothing Then
                                              ' fallback: first panel in tab
                                              celPanel = tp.Controls.OfType(Of Panel)().FirstOrDefault()
                                          End If
                                      Else
                                          ' fallback: search globally for PanelN
                                          Dim panelNev As String = "Panel" & i
                                          celPanel = CType(Me.Controls.Find(panelNev, True).FirstOrDefault(), Panel)
                                      End If

                                      If celPanel IsNot Nothing Then
                                          SetWindowLong(ablakHandle, GWL_STYLE, WS_CHILD Or Visible)
                                          SetParent(ablakHandle, celPanel.Handle)
                                          MoveWindow(ablakHandle, 0, 0, celPanel.Width, celPanel.Height, True)
                                      End If
                                  End Sub)
                    End If

                    ' Várunk a beállításokban megadott másodpercig (Pl. 2 másodperc)
                    Thread.Sleep(varakozas)

                    ' -----------------------------------------------------------------
                    ' JAVÍTÁS: A várakozás UTÁN újra frissítjük a grafikát!
                    ' Ekkorra a folyamat már biztosan beágyazódott és elindult az SQL/NET.
                    ' -----------------------------------------------------------------
                    Me.Invoke(Sub() TabControl1.Invalidate())
                    ' -----------------------------------------------------------------
                End If

            Catch ex As Exception
                Continue For
            End Try
        Next
        Me.Invoke(Sub() TabControl1.Invalidate())

    End Sub


    Private Sub ServerStartupToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ServerStartupToolStripMenuItem.Click
        InditasFolyamata()
    End Sub

    Private Sub SettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SettingsToolStripMenuItem.Click
        Try
            ' Disable the menu while the settings form is visible to prevent duplicates
            SettingsToolStripMenuItem.Enabled = False

            ' Attach a VisibleChanged handler once so we can re-enable the menu when the form is hidden
            If Not form2VisibleHandlerAttached Then
                Try
                    AddHandler Form2.VisibleChanged, AddressOf Form2_VisibleChangedHandler
                    form2VisibleHandlerAttached = True
                Catch
                End Try
            End If

            ' Show or restore the settings form
            If Form2 IsNot Nothing AndAlso Form2.Visible Then
                If Form2.WindowState = FormWindowState.Minimized Then
                    Form2.WindowState = FormWindowState.Normal
                End If
                Form2.BringToFront()
                Form2.Activate()
            Else
                Form2.Show()
                Form2.BringToFront()
                Form2.Activate()
            End If
        Catch ex As Exception
            ' Fallback: try to show the default instance and ensure menu state
            Try
                Form2.Show()
                SettingsToolStripMenuItem.Enabled = False
            Catch
            End Try
        End Try
    End Sub

    Private Sub Form2_VisibleChangedHandler(sender As Object, e As EventArgs)
        Try
            ' Re-enable the Settings menu when the settings form is no longer visible
            Dim frm = TryCast(sender, Form)
            If frm Is Nothing Then
                SettingsToolStripMenuItem.Enabled = True
            Else
                ' Enable the menu if the form is hidden OR minimized; disable while visible and normal/maximized
                Dim isMinimized As Boolean = (frm.WindowState = FormWindowState.Minimized)
                SettingsToolStripMenuItem.Enabled = (Not frm.Visible) Or isMinimized
            End If
        Catch
            SettingsToolStripMenuItem.Enabled = True
        End Try
    End Sub

    Private Sub ExitServerShutdownToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitServerShutdownToolStripMenuItem.Click
        Dim resp = MessageBox.Show("Shutdown all started embedded applications and exit?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If resp <> DialogResult.Yes Then Return

        ' First try to shut down any started embedded applications
        ShutdownStartedApplications()

        ' Then exit the application
        Application.Exit()
    End Sub

    Private Sub BtnShutdownApps_Click(sender As Object, e As EventArgs) Handles BtnShutdownApps.Click
        Dim resp = MessageBox.Show("Shutdown all started applications?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If resp <> DialogResult.Yes Then Return

        ShutdownStartedApplications()
    End Sub

    Private Sub ShutdownStartedApplications()
        Dim procs As New List(Of Process)()
        SyncLock StartedProcesses
            For Each kvp In StartedProcesses
                If kvp.Value IsNot Nothing AndAlso Not kvp.Value.HasExited Then
                    procs.Add(kvp.Value)
                End If
            Next
            StartedProcesses.Clear()
        End SyncLock

        For Each proc In procs
            Try
                If Not proc.HasExited Then
                    ' try graceful close
                    Try
                        proc.CloseMainWindow()
                        If Not proc.WaitForExit(3000) Then
                            proc.Kill()
                        End If
                    Catch
                        Try
                            proc.Kill()
                        Catch
                        End Try
                    End Try
                End If
            Catch
            End Try
        Next
    End Sub

    ' --- Process monitor: periodically check and restart stopped apps when enabled ---
    Private Sub StartProcessMonitor()
        If monitorCts IsNot Nothing Then Return

        monitorCts = New Threading.CancellationTokenSource()
        Dim token = monitorCts.Token

        Task.Run(Sub()
                     Try
                         While Not token.IsCancellationRequested
                             Try
                                 ' Check only PRCEN_GLOBAL (enabled flag). The monitor will rely on per-row VAR_ delays instead of a global timer.
                                 Dim prcEnabled As Boolean = False
                                 Try
                                     If Beallitasok.ContainsKey("PRCEN_GLOBAL") Then
                                         Dim raw = Beallitasok("PRCEN_GLOBAL")
                                         If String.Equals(raw, "1") OrElse String.Equals(raw, "true", StringComparison.OrdinalIgnoreCase) Then
                                             prcEnabled = True
                                         Else
                                             Boolean.TryParse(raw, prcEnabled)
                                         End If
                                     End If
                                     If Not prcEnabled Then
                                         ' global restart disabled, skip scanning restarts
                                         Continue While
                                     End If
                                 Catch
                                     prcEnabled = False
                                 End Try
                                 Dim maxIdx = GetSettingsCount()
                                 For i As Integer = 1 To maxIdx
                                     If token.IsCancellationRequested Then Exit For
                                     Log("Monitor: evaluating slot " & i)
                                     ' unified global restart cadence: prcSeconds already computed

                                     ' Skip if another start is in progress
                                     SyncLock monitorLock
                                         If startingSlots.Contains(i) Then
                                             Log("Monitor: slot " & i & " skip - start already in progress")
                                             Continue For
                                         End If
                                     End SyncLock

                                     Dim needRestart As Boolean = False
                                     Dim existingProc As Process = Nothing
                                     SyncLock StartedProcesses
                                         If StartedProcesses.ContainsKey(i) Then
                                             existingProc = StartedProcesses(i)
                                             If existingProc Is Nothing Then
                                                 Log("Monitor: slot " & i & " tracked process is null -> will attempt restart")
                                                 needRestart = True
                                             ElseIf existingProc.HasExited Then
                                                 Log("Monitor: slot " & i & " tracked process Id=" & existingProc.Id & " has exited -> will attempt restart")
                                                 needRestart = True
                                             Else
                                                 Log("Monitor: slot " & i & " tracked process Id=" & existingProc.Id & " is running -> skip restart")
                                             End If
                                         Else
                                             ' Do not auto-start slots that were not previously started by this manager session
                                             Log("Monitor: slot " & i & " not started in this session -> skip auto-start")
                                             needRestart = False
                                         End If
                                     End SyncLock

                                     If needRestart Then
                                         ' mark starting
                                         SyncLock monitorLock
                                             startingSlots.Add(i)
                                         End SyncLock

                                         ' wait per-row VAR delay (in seconds) before restart
                                         Dim varKey = "VAR_" & i
                                         Dim varSeconds As Integer = 2
                                         Try
                                             If Beallitasok.ContainsKey(varKey) Then
                                                 Integer.TryParse(Beallitasok(varKey), varSeconds)
                                                 If varSeconds < 1 Then varSeconds = 2
                                             End If
                                         Catch
                                             varSeconds = 2
                                         End Try
                                         varSeconds = varSeconds ' VAR stored as seconds already when multiplied in SzoftvercsomagInditasa
                                         Log("Monitor: slot " & i & " will wait " & varSeconds & " second(s) before attempting restart")
                                         Dim waited As Integer = 0
                                         While waited < varSeconds AndAlso Not token.IsCancellationRequested
                                             Threading.Thread.Sleep(1000)
                                             waited += 1
                                         End While

                                         ' double-check under lock then start if still needed
                                         Dim shouldStart As Boolean = False
                                         SyncLock StartedProcesses
                                             If Not StartedProcesses.ContainsKey(i) OrElse StartedProcesses(i) Is Nothing OrElse StartedProcesses(i).HasExited Then
                                                 shouldStart = True
                                             End If
                                         End SyncLock
                                         If shouldStart Then
                                             Log("Monitor: slot " & i & " confirmed need to start")
                                             Try
                                                 ' read settings for this slot
                                                 Dim keyMap = "MAPPA_" & i
                                                 Dim keyExe = "EXE_" & i
                                                 Dim keyNev = "NEV_" & i
                                                 If Beallitasok.ContainsKey(keyMap) AndAlso Beallitasok.ContainsKey(keyExe) AndAlso Beallitasok.ContainsKey(keyNev) Then
                                                     Dim mappa = Beallitasok(keyMap)
                                                     Dim exe = Beallitasok(keyExe)
                                                     Dim psi As New ProcessStartInfo()
                                                     psi.FileName = Path.Combine(mappa, exe)
                                                     psi.WorkingDirectory = mappa

                                                     Dim p As Process = Nothing
                                                     Try
                                                         p = Process.Start(psi)
                                                         Log("Monitor: slot " & i & " Process.Start returned Id=" & If(p IsNot Nothing, p.Id, -1))
                                                     Catch exStart As Exception
                                                         Log("Monitor: slot " & i & " Process.Start failed - " & exStart.Message)
                                                     End Try

                                                     If p IsNot Nothing Then
                                                         SyncLock StartedProcesses
                                                             If StartedProcesses.ContainsKey(i) Then StartedProcesses(i) = p Else StartedProcesses.Add(i, p)
                                                         End SyncLock
                                                         ' embed window on UI thread similar to SzoftvercsomagInditasa
                                                         Log($"Monitor: started process Id={If(p IsNot Nothing, p.Id, -1)} for slot {i}")
                                                         Me.Invoke(Sub()
                                                                       Try
                                                                           Dim celPanel As Panel = Nothing
                                                                           If TabControl1 IsNot Nothing AndAlso TabControl1.TabPages.Count >= i Then
                                                                               Dim tp As TabPage = TabControl1.TabPages(i - 1)
                                                                               celPanel = tp.Controls.OfType(Of Panel)().FirstOrDefault(Function(pl) pl.Name = ("Panel" & i))
                                                                               If celPanel Is Nothing Then celPanel = tp.Controls.OfType(Of Panel)().FirstOrDefault()
                                                                           Else
                                                                               Dim panelNev As String = "Panel" & i
                                                                               celPanel = TryCast(Me.Controls.Find(panelNev, True).FirstOrDefault(), Panel)
                                                                           End If

                                                                           If celPanel IsNot Nothing Then
                                                                               Dim handle As IntPtr = IntPtr.Zero
                                                                               Dim attempts As Integer = 0
                                                                               Log($"Monitor: polling for MainWindowHandle (up to 10s) for started process Id={p.Id} slot={i}")
                                                                               While attempts < 100 AndAlso handle = IntPtr.Zero
                                                                                   Try
                                                                                       p.Refresh()
                                                                                       Try
                                                                                           p.WaitForInputIdle(200)
                                                                                       Catch
                                                                                       End Try
                                                                                   Catch
                                                                                   End Try
                                                                                   handle = p.MainWindowHandle
                                                                                   If handle = IntPtr.Zero Then
                                                                                       Threading.Thread.Sleep(100)
                                                                                       attempts += 1
                                                                                   End If
                                                                               End While
                                                                               If handle <> IntPtr.Zero Then
                                                                                   Try
                                                                                       SetWindowLong(handle, GWL_STYLE, WS_CHILD Or Visible)
                                                                                       SetParent(handle, celPanel.Handle)
                                                                                       MoveWindow(handle, 0, 0, celPanel.Width, celPanel.Height, True)
                                                                                       Log($"Monitor: embedded started process Id={p.Id} handle={handle} into Panel{ i }")
                                                                                   Catch exEmbed As Exception
                                                                                       Log($"Monitor: embed failed for started process Id={p.Id} slot={i} - " & exEmbed.Message)
                                                                                   End Try
                                                                               Else
                                                                                   Log($"Monitor: main window handle not available after polling for started process Id={p.Id} slot={i}")
                                                                               End If
                                                                           Else
                                                                               Log($"Monitor: target panel not found for slot {i} when embedding started process Id={p.Id}")
                                                                           End If
                                                                       Catch ex As Exception
                                                                           Log($"Monitor: exception embedding started process Id={If(p IsNot Nothing, p.Id, -1)} slot={i} - " & ex.Message)
                                                                       End Try
                                                                   End Sub)
                                                     End If
                                                 End If
                                             Catch
                                             End Try
                                         End If

                                         SyncLock monitorLock
                                             startingSlots.Remove(i)
                                         End SyncLock
                                     End If
                                 Next
                             Catch
                             End Try

                             ' Sleep between full scans (use small granularity to be responsive to cancellation)
                             Dim sleepMs As Integer = 1000 * 60 * 5 ' default 5 minutes between checks
                             Dim slept As Integer = 0
                             While slept < sleepMs AndAlso Not token.IsCancellationRequested
                                 Threading.Thread.Sleep(1000)
                                 slept += 1000
                             End While
                         End While
                     Catch
                     End Try
                 End Sub, token)
    End Sub

    Private Sub StopProcessMonitor()
        Try
            If monitorCts IsNot Nothing Then
                monitorCts.Cancel()
                monitorCts.Dispose()
                monitorCts = Nothing
            End If
        Catch
        End Try
    End Sub

    Private Sub ShutdownSingleApplication(index As Integer)
        Try
            Dim proc As Process = Nothing
            SyncLock StartedProcesses
                If StartedProcesses.ContainsKey(index) Then
                    proc = StartedProcesses(index)
                    StartedProcesses.Remove(index)
                End If
            End SyncLock

            If proc Is Nothing Then
                MessageBox.Show($"No tracked process for slot {index}.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            If proc IsNot Nothing AndAlso Not proc.HasExited Then
                Try
                    proc.CloseMainWindow()
                    If Not proc.WaitForExit(3000) Then
                        proc.Kill()
                    End If
                Catch
                    Try
                        proc.Kill()
                    Catch
                    End Try
                End Try
            End If
        Catch ex As Exception
            MessageBox.Show("Error shutting down application: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    ' --- ÚJRAÉLESZTVE: A fülek sötétítése és dinamikus színezése állapot szerint ---
    Private Sub TabControl1_DrawItem(sender As Object, e As DrawItemEventArgs) Handles TabControl1.DrawItem
        ' Ha üres a TabControl, nincs mit rajzolni
        If TabControl1.TabPages.Count = 0 OrElse e.Index < 0 Then Exit Sub

        Dim g As Graphics = e.Graphics
        Dim tp As TabPage = TabControl1.TabPages(e.Index)
        Dim fülTeglalap As Rectangle = e.Bounds

        ' 1. ALAPÉRTELMEZETT STÁTUSZ: PIROS (Ha nincs betallózva semmi)
        Dim aktualisHatterSzin As Color = Color.FromArgb(160, 40, 40) ' Elegáns sötétpiros

        ' Megnézzük a sorszámhoz tartozó kulcsokat a memóriában
        Dim sorszam As Integer = e.Index + 1
        Dim kulcsExe As String = "EXE_" & sorszam
        Dim kulcsNev As String = "NEV_" & sorszam

        If Beallitasok.ContainsKey(kulcsExe) AndAlso Beallitasok.ContainsKey(kulcsNev) Then
            Dim exeNev As String = Beallitasok(kulcsExe)
            Dim szoftverNev As String = Beallitasok(kulcsNev)

            If String.IsNullOrEmpty(exeNev) Then
                ' Ha a fájlnév üres -> PIROS
                aktualisHatterSzin = Color.FromArgb(160, 40, 40)
            Else
                ' Megnézzük, hogy fut-e a folyamat a háttérben
                Dim futoFolyamatok As Process() = Process.GetProcessesByName(szoftverNev)

                If futoFolyamatok.Length > 0 Then
                    ' Ha fut -> ZÖLD (Működő alkalmazás)
                    aktualisHatterSzin = Color.FromArgb(40, 130, 40) ' Elegáns sötétzöld
                Else
                    ' Ha be van tallózva, de NEM fut -> SZÜRKE (Sötét mód kompatibilis)
                    aktualisHatterSzin = Color.FromArgb(65, 65, 65) ' Sötétszürke háttér
                End If
            End If
        End If

        ' 2. FÓKUSZ/KIJELÖLÉS: Ha épp ezen a fülön áll a felhasználó, kicsit megvilágítjuk a keretét/színét
        If e.State = DrawItemState.Selected Then
            aktualisHatterSzin = Color.FromArgb(Math.Min(aktualisHatterSzin.R + 25, 255),
                                                 Math.Min(aktualisHatterSzin.G + 25, 255),
                                                 Math.Min(aktualisHatterSzin.B + 25, 255))
        End If

        ' 3. A fül tényleges kirajzolása a kiválasztott színnel és fehér szöveggel
        Using hatterEcset As New SolidBrush(aktualisHatterSzin)
            Using szovegEcset As New SolidBrush(Color.White)
                g.FillRectangle(hatterEcset, fülTeglalap)

                ' Szöveg középre igazítása a fülön belül
                Dim sf As New StringFormat()
                sf.Alignment = StringAlignment.Center
                sf.LineAlignment = StringAlignment.Center

                g.DrawString(tp.Text, e.Font, szovegEcset, fülTeglalap, sf)
            End Using
        End Using
    End Sub
    ' --- JAVÍTÁS: Ha a főablakot bezárják, az egész alkalmazás azonnal tűnjön el a Feladatkezelőből ---
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' A legdrasztikusabb és legtisztább parancs, ami az összes háttérszálat és ablakot egyszerre lövi le
        Application.Exit()
    End Sub

End Class
