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

            ' Per-tab shutdown button (top-right)
            Dim btnShutdownTab As New Button() With {
                .Name = "BtnShutdown_" & newIndex,
                .Text = "Shutdown",
                .Width = 90,
                .Height = 24,
                .Anchor = AnchorStyles.Top Or AnchorStyles.Right
            }
            ' position near top-right; location will be adjusted by docking/anchoring
            btnShutdownTab.Location = New Point(Math.Max(4, tp.ClientSize.Width - btnShutdownTab.Width - 8), 4)

            Dim idx = newIndex
            AddHandler btnShutdownTab.Click, Sub(s, e) ShutdownSingleApplication(idx)
            tp.Controls.Add(btnShutdownTab)
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
