Imports System.IO

Partial Public Class Form2
    ' Designer requirements:
    ' - FlowLayoutPanel named: flowRows (AutoScroll=True, FlowDirection=TopDown, WrapContents=False)
    ' - Button named: btnAddRow (text: "+")
    ' - Button named: BtnMentes (already present in designer)

    Private ReadOnly IniPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.ini")

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' Migrate legacy file if present
            SettingsStore.MigrateDatToIni(AppDomain.CurrentDomain.BaseDirectory)

            ' Ensure we have a TabControl to separate Applications and Delays
            If Not Me.Controls.ContainsKey("tabSettings") Then
                Dim tab As New TabControl() With {
                    .Name = "tabSettings",
                    .Dock = DockStyle.Fill
                }
                Dim tpApps As New TabPage("Applications")
                Dim tpDelays As New TabPage("Delays")
                tab.TabPages.Add(tpApps)
                tab.TabPages.Add(tpDelays)

                ' Move existing flowRows into the Applications tab if present
                If Me.Controls.ContainsKey("flowRows") Then
                    Dim flow = CType(Me.Controls("flowRows"), FlowLayoutPanel)
                    Me.Controls.Remove(flow)
                    flow.Dock = DockStyle.Fill
                    tpApps.Controls.Add(flow)
                End If

                ' Create delays FlowLayoutPanel in Delays tab
                Dim flowDelays As New FlowLayoutPanel() With {
                    .Name = "flowDelays",
                    .AutoScroll = True,
                    .FlowDirection = FlowDirection.TopDown,
                    .WrapContents = False,
                    .Dock = DockStyle.Fill
                }
                tpDelays.Controls.Add(flowDelays)

                ' Insert the tab control into the form
                Me.Controls.Add(tab)
                ' Ensure tab is behind any toolstrip or other top-level controls
                tab.SendToBack()
            End If

            ' Populate UI rows from settings
            Dim data = If(File.Exists(IniPath), SettingsStore.ReadSettingsFromFile(IniPath), New Dictionary(Of String, String)())

            Dim i As Integer = 1
            Dim found As Boolean = False
            While data.ContainsKey($"NEV_{i}") OrElse data.ContainsKey($"MAPPA_{i}") OrElse data.ContainsKey($"EXE_{i}") OrElse data.ContainsKey($"CFG_{i}") OrElse data.ContainsKey($"VAR_{i}") OrElse data.ContainsKey($"PORT_{i}")
                Dim name = If(data.ContainsKey($"NEV_{i}"), data($"NEV_{i}"), String.Empty)
                Dim mappa = If(data.ContainsKey($"MAPPA_{i}"), data($"MAPPA_{i}"), String.Empty)
                Dim exe = If(data.ContainsKey($"EXE_{i}"), data($"EXE_{i}"), String.Empty)
                Dim cfg = If(data.ContainsKey($"CFG_{i}"), data($"CFG_{i}"), String.Empty)
                Dim vr = If(data.ContainsKey($"VAR_{i}"), data($"VAR_{i}"), "1")
                ' ÚJ: Port beolvasása az INI-ből, ha nem létezik, alapértelmezetten "55901"
                Dim prt = If(data.ContainsKey($"PORT_{i}"), data($"PORT_{i}"), "55901")

                ' PRC per-row keys are no longer used; pass empty defaults, és végén az új prt változó
                AddRow(name, mappa, exe, cfg, vr, String.Empty, "0", prt)
                found = True
                i += 1
            End While

            If Not found Then
                ' start with one empty row, alapértelmezett "55901" porttal
                AddRow(String.Empty, String.Empty, String.Empty, String.Empty, "1", String.Empty, "0", "55901")
            End If

            ' Ensure CFG filenames are reflected in row labels even if indexes or data were inconsistent
            Try
                If Me.Controls.ContainsKey("flowRows") Then
                    Dim container = CType(Me.Controls("flowRows"), FlowLayoutPanel)
                    Dim idx2 As Integer = 1
                    For Each ctrl As Control In container.Controls
                        Dim p = TryCast(ctrl, Panel)
                        If p Is Nothing Then Continue For
                        Dim info = TryCast(p.Tag, Dictionary(Of String, String))
                        If info Is Nothing Then Continue For

                        Dim key = $"CFG_{idx2}"
                        If data.ContainsKey(key) AndAlso Not String.IsNullOrEmpty(data(key)) Then
                            Dim cfgVal = data(key)
                            info("CFG") = cfgVal
                            ' update label
                            Dim lbl = p.Controls.OfType(Of Label)().FirstOrDefault()
                            If lbl IsNot Nothing Then
                                Try
                                    Dim cfgShort = Path.GetFileName(cfgVal)
                                    If Not String.IsNullOrEmpty(lbl.Text) AndAlso Not lbl.Text.StartsWith("Not Defined") Then
                                        If lbl.Text.Contains("(Cfg:") Then
                                            ' replace existing Cfg portion
                                            lbl.Text = System.Text.RegularExpressions.Regex.Replace(lbl.Text, "\(Cfg:.*\)", "(Cfg: " & cfgShort & ")")
                                        Else
                                            lbl.Text &= " (Cfg: " & cfgShort & ")"
                                        End If
                                    Else
                                        lbl.Text = "Cfg: " & cfgShort
                                    End If
                                    lbl.ForeColor = Color.Green
                                Catch
                                End Try
                            End If
                        End If

                        idx2 += 1
                    Next
                End If
            Catch
            End Try

            ' Resize handler to adapt row widths
            AddHandler Me.Resize, Sub() AdjustRowWidths()
            AdjustRowWidths()

        Catch ex As Exception
            MessageBox.Show("Load error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub AdjustRowWidths()
        Dim container = TryCast(Me.Controls.Find("flowRows", True).FirstOrDefault(), FlowLayoutPanel)
        If container Is Nothing Then Return
        For Each ctrl As Control In container.Controls
            ctrl.Width = Math.Max(200, container.ClientSize.Width - 25)
        Next
    End Sub

    ' Ensure PrcTimer accepts only digits and normalizes value on changes/leave
    Private Sub PrcTimer_KeyPress(sender As Object, e As KeyPressEventArgs)
        ' Allow control keys (backspace, etc.) and digits only
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub PrcTimer_TextChanged(sender As Object, e As EventArgs)
        Try
            Dim tb = CType(sender, TextBox)
            Dim original = tb.Text
            If String.IsNullOrEmpty(original) Then Return
            ' Remove any non-digit characters (covers paste)
            Dim cleaned = System.Text.RegularExpressions.Regex.Replace(original, "\D", "")
            If cleaned <> original Then
                Dim sel = tb.SelectionStart
                tb.Text = cleaned
                tb.SelectionStart = Math.Min(cleaned.Length, sel)
            End If
        Catch
        End Try
    End Sub

    Private Sub PrcTimer_Leave(sender As Object, e As EventArgs)
        Try
            Dim tb = CType(sender, TextBox)
            If String.IsNullOrEmpty(tb.Text) Then
                tb.Text = "30"
                Return
            End If
            Dim v = 0
            If Not Integer.TryParse(tb.Text, v) OrElse v < 1 Then
                tb.Text = "30"
            End If
        Catch
        End Try
    End Sub

    ' Add a new row to the FlowLayoutPanel - KIEGÉSZÍTVE az initialPort paraméterrel
    Private Sub AddRow(Optional initialName As String = "", Optional initialPath As String = "", Optional initialExe As String = "", Optional initialCfg As String = "", Optional initialVar As String = "1", Optional initialPrc As String = "", Optional initialPrcEn As String = "0", Optional initialPort As String = "55901")
        Dim container = TryCast(Me.Controls.Find("flowRows", True).FirstOrDefault(), FlowLayoutPanel)
        If container Is Nothing Then
            MessageBox.Show("flowRows panel is missing in the designer. Add a FlowLayoutPanel named 'flowRows'.")
            Return
        End If

        Dim p As New Panel() With {
         .Height = 34,
         .Width = Math.Max(200, container.ClientSize.Width - 25),
         .Padding = New Padding(2)
     }

        Dim lbl As New Label() With {
         .AutoSize = False,
         .Width = 300,
         .Height = 28,
         .Left = 4,
         .Top = 4,
         .TextAlign = ContentAlignment.MiddleLeft
     }

        Dim btnBrowse As New Button() With {
         .Text = "Browse",
         .Width = 70,
         .Left = lbl.Right + 6,
         .Top = 4
     }

        Dim btnCfg As New Button() With {
         .Text = "Cfg",
         .Width = 50,
         .Left = btnBrowse.Right + 6,
         .Top = 4
     }

        Dim btnOpenCfg As New Button() With {
         .Text = "Open",
         .Width = 60,
         .Left = btnCfg.Right + 6,
         .Top = 4
     }

        ' VAR textbox inside the row (editable directly)
        Dim idxDisplayRow = container.Controls.Count + 1
        Dim txtVarRow As New TextBox() With {
         .Name = "txtVARRow_" & idxDisplayRow,
         .Width = 60,
         .Left = btnOpenCfg.Right + 6,
         .Top = 6,
         .Text = initialVar
     }

        ' ÚJ: Port TextBox létrehozása a Startup Delay (txtVarRow) után
        Dim txtPortRow As New TextBox() With {
         .Name = "txtPortRow_" & idxDisplayRow,
         .Width = 60,
         .Left = txtVarRow.Right + 6,
         .Top = 6,
         .Text = initialPort,
         .MaxLength = 5 ' Maximális karakterszám a szabványos portokhoz (0-65535)
     }
        ' Újrahasznosítjuk az input szűrő eseményeket, hogy ne pazaroljunk memóriát
        AddHandler txtPortRow.KeyPress, AddressOf PrcTimer_KeyPress
        AddHandler txtPortRow.TextChanged, AddressOf PrcTimer_TextChanged

        ' MÓDOSÍTVA: A Remove gomb az új txtPortRow után csúszik
        Dim btnRemove As New Button() With {
         .Text = "Remove",
         .Width = 70,
         .Left = txtPortRow.Right + 6,
         .Top = 4
     }

        ' Store data in Tag - BŐVÍTVE a Port értékkel
        Dim info As New Dictionary(Of String, String)()
        info("NAME") = initialName
        info("PATH") = initialPath
        info("EXE") = initialExe
        info("CFG") = initialCfg
        info("VAR") = initialVar
        info("PRC") = initialPrc
        info("PRCEN") = initialPrcEn
        info("PORT") = initialPort
        p.Tag = info

        ' Set label to show the application name and optionally the config filename
        If Not String.IsNullOrEmpty(initialName) Then
            lbl.Text = "Selected: " & initialName
            lbl.ForeColor = Color.Green
            If Not String.IsNullOrEmpty(initialCfg) Then
                Try
                    Dim cfgShort = Path.GetFileName(initialCfg)
                    lbl.Text &= " (Cfg: " & cfgShort & ")"
                Catch
                End Try
            End If
        ElseIf Not String.IsNullOrEmpty(initialCfg) Then
            ' No app name, but config exists
            Try
                Dim cfgShort = Path.GetFileName(initialCfg)
                lbl.Text = "Cfg: " & cfgShort
                lbl.ForeColor = Color.Green
            Catch
                lbl.Text = "Cfg: (unknown)"
                lbl.ForeColor = Color.Green
            End Try
        Else
            lbl.Text = "Not Defined!"
            lbl.ForeColor = Color.Red
        End If

        AddHandler btnBrowse.Click, Sub(s, e)
                                        Using ofd As New OpenFileDialog()
                                            ofd.Filter = "Executables (*.exe)|*.exe|All Filetypes (*.*)|*.*"
                                            If ofd.ShowDialog() = DialogResult.OK Then
                                                Dim full = ofd.FileName
                                                Dim folder = Path.GetDirectoryName(full)
                                                Dim exeName = Path.GetFileName(full)
                                                Dim shortName = Path.GetFileNameWithoutExtension(full)

                                                info("NAME") = shortName
                                                info("PATH") = folder
                                                info("EXE") = exeName

                                                lbl.Text = "Selected: " & shortName
                                                lbl.ForeColor = Color.Green
                                            End If
                                        End Using
                                    End Sub

        ' Browse for config file (ini/cfg/xml)
        AddHandler btnCfg.Click, Sub(s, e)
                                     Using ofd As New OpenFileDialog()
                                         ofd.Filter = "Config Files (*.ini;*.cfg;*.xml)|*.ini;*.cfg;*.xml|All Files (*.*)|*.*"
                                         ofd.Title = "Select configuration file for this application"
                                         ' Start browsing in the application's folder if available
                                         Dim startDir As String = String.Empty
                                         If info.ContainsKey("PATH") Then startDir = info("PATH")
                                         If String.IsNullOrEmpty(startDir) OrElse Not Directory.Exists(startDir) Then
                                             startDir = AppDomain.CurrentDomain.BaseDirectory
                                         End If
                                         Try
                                             ofd.InitialDirectory = startDir
                                         Catch
                                         End Try

                                         If ofd.ShowDialog() = DialogResult.OK Then
                                             Dim cfgFull = ofd.FileName
                                             info("CFG") = cfgFull
                                             ' Update label to show cfg filename (short)
                                             Dim cfgShort = Path.GetFileName(cfgFull)
                                             If String.IsNullOrEmpty(lbl.Text) OrElse lbl.Text.StartsWith("Not Defined") Then
                                                 lbl.Text = "Cfg: " & cfgShort
                                                 lbl.ForeColor = Color.Green
                                             Else
                                                 lbl.Text = lbl.Text & " (Cfg: " & cfgShort & ")"
                                             End If
                                         End If
                                     End Using
                                 End Sub

        ' Open the configured config file with default editor
        AddHandler btnOpenCfg.Click, Sub(s, e)
                                         Dim cfgPath As String = If(info.ContainsKey("CFG"), info("CFG"), String.Empty)
                                         If String.IsNullOrEmpty(cfgPath) Then
                                             MessageBox.Show("No configuration file selected for this row.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                             Return
                                         End If
                                         If Not Path.IsPathRooted(cfgPath) Then
                                             cfgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cfgPath)
                                         End If
                                         If Not File.Exists(cfgPath) Then
                                             MessageBox.Show($"Config file not found: {cfgPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                             Return
                                         End If

                                         ' Hide the settings form while the external editor is open, then restore it when the editor exits
                                         Try
                                             Me.Hide()
                                         Catch
                                         End Try

                                         Dim startedProc As Process = Nothing
                                         Try
                                             Dim psi As New ProcessStartInfo(cfgPath) With {.UseShellExecute = True}
                                             startedProc = Process.Start(psi)
                                         Catch ex As Exception
                                             MessageBox.Show("Failed to open config: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                         End Try

                                         ' Monitor the external editor: if we have a process object, wait for exit; otherwise poll file lock
                                         Task.Run(Sub()
                                                      Try
                                                          If startedProc IsNot Nothing Then
                                                              Try
                                                                  startedProc.WaitForExit()
                                                              Catch
                                                              End Try
                                                          Else
                                                              ' Poll until the file is writable (editor closed) or until timeout
                                                              Dim timeout = DateTime.Now.AddMinutes(10)
                                                              While DateTime.Now < timeout
                                                                  Try
                                                                      Using fs = File.Open(cfgPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
                                                                          fs.Close()
                                                                          Exit While
                                                                      End Using
                                                                  Catch
                                                                      Threading.Thread.Sleep(500)
                                                                  End Try
                                                              End While
                                                          End If
                                                      Catch
                                                      End Try
                                                      Try
                                                          Me.Invoke(Sub()
                                                                        If Me.WindowState = FormWindowState.Minimized Then Me.WindowState = FormWindowState.Normal
                                                                        Me.Show()
                                                                        Me.BringToFront()
                                                                        Me.Activate()
                                                                    End Sub)
                                                      Catch
                                                      End Try
                                                  End Sub)
                                     End Sub

        ' ÚJ: Szinkronizáljuk a közvetlenül szerkeszthető szövegmezők változásait a Tag (info) szótárral
        AddHandler txtVarRow.TextChanged, Sub(s, e)
                                              info("VAR") = txtVarRow.Text
                                          End Sub

        AddHandler txtPortRow.TextChanged, Sub(s, e)
                                               info("PORT") = txtPortRow.Text
                                           End Sub

        ' VAR textbox inside the row (editable directly)
        AddHandler txtVarRow.TextChanged, Sub(s, e)
                                              Try
                                                  Dim txt = txtVarRow.Text
                                                  If String.IsNullOrEmpty(txt) Then
                                                      txtVarRow.Text = "1"
                                                      txtVarRow.SelectionStart = txtVarRow.Text.Length
                                                      Return
                                                  End If

                                                  Dim sb As New System.Text.StringBuilder()
                                                  For Each ch As Char In txt
                                                      If Char.IsDigit(ch) Then sb.Append(ch)
                                                  Next
                                                  Dim digits = sb.ToString()
                                                  If String.IsNullOrEmpty(digits) Then digits = "1"

                                                  Dim parsed As Integer
                                                  If Not Integer.TryParse(digits, parsed) Then parsed = 1
                                                  If parsed < 1 Then parsed = 1

                                                  Dim newText = parsed.ToString()
                                                  If newText <> txtVarRow.Text Then
                                                      txtVarRow.Text = newText
                                                      txtVarRow.SelectionStart = txtVarRow.Text.Length
                                                  End If

                                                  ' update Tag
                                                  TryCast(p.Tag, Dictionary(Of String, String))("VAR") = txtVarRow.Text

                                                  ' update corresponding delay textbox if present
                                                  Try
                                                      Dim flowDelays = TryCast(Me.Controls.Find("flowDelays", True).FirstOrDefault(), FlowLayoutPanel)
                                                      If flowDelays IsNot Nothing Then
                                                          Dim idxRow = container.Controls.IndexOf(p)
                                                          If idxRow >= 0 AndAlso idxRow < flowDelays.Controls.Count Then
                                                              Dim delayPanel = TryCast(flowDelays.Controls(idxRow), Panel)
                                                              If delayPanel IsNot Nothing Then
                                                                  Dim tb = delayPanel.Controls.OfType(Of TextBox)().FirstOrDefault()
                                                                  If tb IsNot Nothing AndAlso tb.Text <> txtVarRow.Text Then tb.Text = txtVarRow.Text
                                                              End If
                                                          End If
                                                      End If
                                                  Catch
                                                  End Try
                                              Catch
                                              End Try
                                          End Sub
        ' Restrict input to digits only on keypress for row textbox
        AddHandler txtVarRow.KeyPress, Sub(senderLocal, ke)
                                           If Not Char.IsControl(ke.KeyChar) AndAlso Not Char.IsDigit(ke.KeyChar) Then
                                               ke.Handled = True
                                           End If
                                       End Sub

        ' ÚJ: txtPortRow precíz TextChanged eseménykezelője a vágólapos beillesztések tisztítására és a Tag frissítésére
        AddHandler txtPortRow.TextChanged, Sub(s, e)
                                               Try
                                                   Dim txt = txtPortRow.Text
                                                   If String.IsNullOrEmpty(txt) Then
                                                       ' Ha törli a felhasználó, ne hagyjuk üresen, kapja vissza a default portot
                                                       txtPortRow.Text = "55901"
                                                       txtPortRow.SelectionStart = txtPortRow.Text.Length
                                                       Return
                                                   End If

                                                   Dim sb As New System.Text.StringBuilder()
                                                   For Each ch As Char In txt
                                                       If Char.IsDigit(ch) Then sb.Append(ch)
                                                   Next
                                                   Dim digits = sb.ToString()
                                                   If String.IsNullOrEmpty(digits) Then digits = "55901"

                                                   Dim parsed As Integer
                                                   If Not Integer.TryParse(digits, parsed) Then parsed = 55901
                                                   ' Szabványos TCP port tartomány ellenőrzés (1 - 65535)
                                                   If parsed < 1 Then parsed = 1
                                                   If parsed > 65535 Then parsed = 65535

                                                   Dim newText = parsed.ToString()
                                                   If newText <> txtPortRow.Text Then
                                                       txtPortRow.Text = newText
                                                       txtPortRow.SelectionStart = txtPortRow.Text.Length
                                                   End If

                                                   ' update Tag szótár a legfrissebb port értékkel
                                                   TryCast(p.Tag, Dictionary(Of String, String))("PORT") = txtPortRow.Text
                                               Catch
                                               End Try
                                           End Sub

        ' ÚJ: txtPortRow KeyPress korlátozás (csak számok és backspace)
        AddHandler txtPortRow.KeyPress, Sub(senderLocal, ke)
                                            If Not Char.IsControl(ke.KeyChar) AndAlso Not Char.IsDigit(ke.KeyChar) Then
                                                ke.Handled = True
                                            End If
                                        End Sub

        AddHandler btnRemove.Click, Sub(s, e)
                                        ' Remove corresponding delay control if present
                                        Dim delayPanel = TryCast(Me.Controls.Find("flowDelays", True).FirstOrDefault(), FlowLayoutPanel)
                                        Dim removeIndex = container.Controls.IndexOf(p)
                                        If delayPanel IsNot Nothing AndAlso removeIndex >= 0 AndAlso removeIndex < delayPanel.Controls.Count Then
                                            delayPanel.Controls.RemoveAt(removeIndex)
                                        End If
                                        container.Controls.Remove(p)

                                        ' Reindex remaining rows and delays and persist settings
                                        Try
                                            ReindexAndSyncDelays()
                                            SaveSettingsToIni()
                                        Catch
                                        End Try
                                    End Sub

        p.Controls.Add(lbl)
        p.Controls.Add(btnBrowse)
        p.Controls.Add(btnCfg)
        p.Controls.Add(btnOpenCfg)
        p.Controls.Add(txtVarRow)
        p.Controls.Add(txtPortRow) ' ÚJ: Az új Port mező hozzáadása a panelhez a megfelelő vizuális sorrendben!
        p.Controls.Add(btnRemove)

        container.Controls.Add(p)

        ' Also add a corresponding delay textbox in the Delays tab (flowDelays)
        Try
            Dim flowDelays = TryCast(Me.Controls.Find("flowDelays", True).FirstOrDefault(), FlowLayoutPanel)
            If flowDelays IsNot Nothing Then
                Dim delayPanel As New Panel() With {
                .Height = 34,
                .Width = Math.Max(200, flowDelays.ClientSize.Width - 25),
                .Padding = New Padding(2)
            }

                Dim lblIdx As New Label() With {
                .AutoSize = False,
                .Width = 200,
                .Height = 28,
                .Left = 4,
                .Top = 4,
                .TextAlign = ContentAlignment.MiddleLeft
            }
                Dim idxDisplay As Integer = container.Controls.IndexOf(p) + 1
                If Not String.IsNullOrEmpty(initialName) Then
                    lblIdx.Text = "#" & idxDisplay & " - " & initialName
                Else
                    lblIdx.Text = "#" & idxDisplay & ""
                End If

                Dim txtVar As New TextBox() With {
                .Name = "txtVAR_" & idxDisplay,
                .Width = 60,
                .Left = lblIdx.Right + 6,
                .Top = 6,
                .Text = initialVar
            }
                ' Keep VAR in sync with row Tag and validate value (min 1). Also sanitize pasted text.
                AddHandler txtVar.TextChanged, Sub(s, e)
                                                   Try
                                                       Dim txt = txtVar.Text
                                                       If String.IsNullOrEmpty(txt) Then
                                                           txtVar.Text = "1"
                                                           txtVar.SelectionStart = txtVar.Text.Length
                                                           Return
                                                       End If

                                                       ' remove non-digit characters
                                                       Dim sb As New System.Text.StringBuilder()
                                                       For Each ch As Char In txt
                                                           If Char.IsDigit(ch) Then sb.Append(ch)
                                                       Next
                                                       Dim digits = sb.ToString()
                                                       If String.IsNullOrEmpty(digits) Then digits = "1"

                                                       Dim parsed As Integer
                                                       If Not Integer.TryParse(digits, parsed) Then parsed = 1
                                                       If parsed < 1 Then parsed = 1

                                                       Dim newText = parsed.ToString()
                                                       If newText <> txtVar.Text Then
                                                           txtVar.Text = newText
                                                           txtVar.SelectionStart = txtVar.Text.Length
                                                       End If

                                                       TryCast(p.Tag, Dictionary(Of String, String))("VAR") = txtVar.Text
                                                   Catch
                                                   End Try
                                               End Sub
                ' Restrict input to digits only on keypress
                AddHandler txtVar.KeyPress, Sub(senderLocal, ke)
                                                If Not Char.IsControl(ke.KeyChar) AndAlso Not Char.IsDigit(ke.KeyChar) Then
                                                    ke.Handled = True
                                                End If
                                            End Sub

                delayPanel.Controls.Add(lblIdx)
                delayPanel.Controls.Add(txtVar)
                flowDelays.Controls.Add(delayPanel)
            End If
        Catch
        End Try
    End Sub

    Private Sub btnAddRow_Click(sender As Object, e As EventArgs) Handles btnAddRow.Click
        ' Use global PrcTimer and PrcRestart defaults when adding a new row
        Dim prcDefault As String = String.Empty
        Dim prcEnDefault As String = "0"
        Try
            Dim txt = Me.Controls.Find("PrcTimer", True).FirstOrDefault()
            If txt IsNot Nothing AndAlso TypeOf txt Is TextBox Then
                prcDefault = CType(txt, TextBox).Text
            End If
            Dim chk = Me.Controls.Find("PrcRestart", True).FirstOrDefault()
            If chk IsNot Nothing AndAlso TypeOf chk Is CheckBox Then
                prcEnDefault = If(CType(chk, CheckBox).Checked, "1", "0")
            End If
        Catch
        End Try
        ' ÚJ: A legújabb default porttal indítunk ("55901")
        AddRow(initialPrc:=prcDefault, initialPrcEn:=prcEnDefault, initialPort:="55901")
    End Sub

    Private Sub BtnMentes_Click(sender As Object, e As EventArgs) Handles BtnMentes.Click
        Try
            ' Read existing non-row settings to preserve them
            Dim existing As New Dictionary(Of String, String)()
            If File.Exists(IniPath) Then existing = SettingsStore.ReadSettingsFromFile(IniPath)

            Dim result As New Dictionary(Of String, String)()
            ' preserve other keys (exclude row keys NEV_/MAPPA_/EXE_/VAR_/CFG_ and now PORT_)
            For Each kvp In existing
                If Not kvp.Key.StartsWith("NEV_") AndAlso Not kvp.Key.StartsWith("MAPPA_") AndAlso Not kvp.Key.StartsWith("EXE_") AndAlso Not kvp.Key.StartsWith("VAR_") AndAlso Not kvp.Key.StartsWith("CFG_") AndAlso Not kvp.Key.StartsWith("PORT_") Then
                    result(kvp.Key) = kvp.Value
                End If
            Next

            ' Enumerate rows in order
            Dim container = TryCast(Me.Controls.Find("flowRows", True).FirstOrDefault(), FlowLayoutPanel)
            If container Is Nothing Then
                MessageBox.Show("flowRows panel is missing in the designer. Add a FlowLayoutPanel named 'flowRows'.")
                Return
            End If

            Dim index As Integer = 1
            For Each ctrl As Control In container.Controls
                Dim p = TryCast(ctrl, Panel)
                If p Is Nothing Then Continue For
                Dim info = TryCast(p.Tag, Dictionary(Of String, String))
                If info Is Nothing Then Continue For

                Dim name = If(info.ContainsKey("NAME"), info("NAME"), String.Empty)
                Dim path = If(info.ContainsKey("PATH"), info("PATH"), String.Empty)
                Dim exe = If(info.ContainsKey("EXE"), info("EXE"), String.Empty)
                Dim cfg = If(info.ContainsKey("CFG"), info("CFG"), String.Empty)
                ' ÚJ: Port kinyerése a Panel Tag-ből
                Dim prt = If(info.ContainsKey("PORT"), info("PORT"), String.Empty)

                If String.IsNullOrEmpty(name) AndAlso String.IsNullOrEmpty(exe) Then
                    ' skip empty rows
                    Continue For
                End If

                result($"NEV_{index}") = name
                result($"MAPPA_{index}") = path
                result($"EXE_{index}") = exe
                result($"VAR_{index}") = If(info.ContainsKey("VAR"), info("VAR"), "1")

                ' INTELLIGENS VÉDŐHÁLÓ: Ha a kinyert port üres vagy hibás, név alapján adunk neki egy gyári defaultot
                Dim exeNev As String = exe.ToLower()
                Dim fNev As String = name.ToLower()
                If String.IsNullOrEmpty(prt) OrElse prt = "0" Then
                    Dim defaultPort As Integer = 55901 ' Abszolút alapértelmezett
                    If exeNev.Contains("connect") Then defaultPort = 44405
                    If exeNev.Contains("login") Then defaultPort = 55970
                    If fNev.Contains("link") Then defaultPort = 55960
                    If exeNev.Contains("gameserver") Then
                        If index = 4 Then defaultPort = 55901
                        If index = 5 Then defaultPort = 55903
                        If index = 6 Then defaultPort = 55905
                        If index = 7 Then defaultPort = 55907
                    End If
                    prt = defaultPort.ToString()
                End If

                ' Mentjük a portot a dinamikus indexhez kapcsolva!
                result($"PORT_{index}") = prt

                ' Store CFG path relative to app base when possible
                If Not String.IsNullOrEmpty(cfg) Then
                    Try
                        Dim baseDir = System.IO.Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory)
                        If Not baseDir.EndsWith(System.IO.Path.DirectorySeparatorChar) Then baseDir &= System.IO.Path.DirectorySeparatorChar
                        Dim fullCfg = System.IO.Path.GetFullPath(cfg)
                        If fullCfg.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase) Then
                            ' store relative path
                            result($"CFG_{index}") = fullCfg.Substring(baseDir.Length)
                        Else
                            result($"CFG_{index}") = fullCfg
                        End If
                    Catch
                        result($"CFG_{index}") = cfg
                    End Try
                End If
                index += 1
            Next

            ' Write to INI
            ' Persist global PRCEN_GLOBAL (enabled) from PrcRestart control
            Try
                Dim prcEnGlobal As String = "0"
                Dim chk = Me.Controls.Find("PrcRestart", True).FirstOrDefault()
                If chk IsNot Nothing Then
                    If TypeOf chk Is CheckBox Then prcEnGlobal = If(CType(chk, CheckBox).Checked, "1", "0")
                    If TypeOf chk Is RadioButton Then prcEnGlobal = If(CType(chk, RadioButton).Checked, "1", "0")
                End If
                result("PRCEN_GLOBAL") = prcEnGlobal
            Catch
            End Try

            ' A RÉGI MEREV FOR I = 1 TO 7 CIKLUS ELTÁVOLÍTVA – a fenti dinamikus indexelés teljesen kiváltotta!

            SettingsStore.WriteSettingsToFile(IniPath, result)

            ' Refresh Form1 and hide (do not dispose so user can re-open settings)
            Form1.BeallitasokBetoltese()
            MessageBox.Show("Settings saved successfully!", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Hide()

            ' BIZTONSÁGOS ÉS AGRESSZÍV FLUSH AZ ELREJTÉS UTÁN:
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
        Catch ex As Exception
            MessageBox.Show("Save error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Reindex delay labels and textbox names to match current application rows
    Private Sub ReindexAndSyncDelays()
        Try
            Dim flowRows = TryCast(Me.Controls.Find("flowRows", True).FirstOrDefault(), FlowLayoutPanel)
            Dim flowDelays = TryCast(Me.Controls.Find("flowDelays", True).FirstOrDefault(), FlowLayoutPanel)
            If flowRows Is Nothing OrElse flowDelays Is Nothing Then Return

            ' Rebuild delay controls to match rows
            Dim newDelays As New List(Of Tuple(Of String, String))()
            For i As Integer = 0 To flowRows.Controls.Count - 1
                Dim p = TryCast(flowRows.Controls(i), Panel)
                If p Is Nothing Then Continue For
                Dim info = TryCast(p.Tag, Dictionary(Of String, String))
                Dim name = If(info.ContainsKey("NAME"), info("NAME"), String.Empty)
                Dim varv = If(info.ContainsKey("VAR"), info("VAR"), "2")
                newDelays.Add(Tuple.Create(name, varv))
            Next

            ' Clear and recreate flowDelays
            flowDelays.Controls.Clear()
            For idx = 0 To newDelays.Count - 1
                ' CLOSURE FIX: Lokális változóba mentjük az indexet, hogy a lambda ne csússzon el!
                Dim localIdx As Integer = idx

                Dim delayPanel As New Panel() With {
                 .Height = 34,
                 .Width = Math.Max(200, flowDelays.ClientSize.Width - 25),
                 .Padding = New Padding(2)
             }

                Dim lblIdx As New Label() With {
                 .AutoSize = False,
                 .Width = 200,
                 .Height = 28,
                 .Left = 4,
                 .Top = 4,
                 .TextAlign = ContentAlignment.MiddleLeft,
                 .Text = "#" & (localIdx + 1) & If(String.IsNullOrEmpty(newDelays(localIdx).Item1), "", " - " & newDelays(localIdx).Item1)
             }

                Dim txtVar As New TextBox() With {
                 .Name = "txtVAR_" & (localIdx + 1),
                 .Width = 60,
                 .Left = lblIdx.Right + 6,
                 .Top = 6,
                 .Text = newDelays(localIdx).Item2
             }

                ' Keep VAR in sync with corresponding row Tag (Using localIdx)
                AddHandler txtVar.TextChanged, Sub(s, e)
                                                   Try
                                                       If flowRows.Controls.Count > localIdx Then
                                                           Dim rowPanel = TryCast(flowRows.Controls(localIdx), Panel)
                                                           If rowPanel IsNot Nothing Then
                                                               Dim info = TryCast(rowPanel.Tag, Dictionary(Of String, String))
                                                               If info IsNot Nothing Then info("VAR") = txtVar.Text
                                                           End If
                                                       End If
                                                   Catch
                                                   End Try
                                               End Sub

                ' FIX: Kiszabadítottuk a TextChanged fogságából, így csak egyszer regisztrálódik a memóriában!
                AddHandler txtVar.KeyPress, Sub(senderLocal, ke)
                                                If Not Char.IsControl(ke.KeyChar) AndAlso Not Char.IsDigit(ke.KeyChar) Then
                                                    ke.Handled = True
                                                End If
                                            End Sub

                delayPanel.Controls.Add(lblIdx)
                delayPanel.Controls.Add(txtVar)
                flowDelays.Controls.Add(delayPanel)
            Next
        Catch
        End Try
    End Sub


    ' Helper to persist current form rows into Settings.ini (used by removal/renumber)
    Private Sub SaveSettingsToIni()
        Try
            Dim ini As New Dictionary(Of String, String)()
            ' Preserve other keys (BŐVÍTVE: PORT_ kulcsok kizárásával az újraindexeléshez)
            If File.Exists(IniPath) Then
                Dim existing = SettingsStore.ReadSettingsFromFile(IniPath)
                For Each kvp In existing
                    If Not kvp.Key.StartsWith("NEV_") AndAlso Not kvp.Key.StartsWith("MAPPA_") AndAlso Not kvp.Key.StartsWith("EXE_") AndAlso Not kvp.Key.StartsWith("VAR_") AndAlso Not kvp.Key.StartsWith("CFG_") AndAlso Not kvp.Key.StartsWith("PORT_") Then
                        ini(kvp.Key) = kvp.Value
                    End If
                Next
            End If

            Dim flowRows = TryCast(Me.Controls.Find("flowRows", True).FirstOrDefault(), FlowLayoutPanel)
            If flowRows Is Nothing Then Return
            Dim idx As Integer = 1
            For Each ctrl As Control In flowRows.Controls
                Dim p = TryCast(ctrl, Panel)
                If p Is Nothing Then Continue For
                Dim info = TryCast(p.Tag, Dictionary(Of String, String))
                If info Is Nothing Then Continue For

                Dim name = If(info.ContainsKey("NAME"), info("NAME"), String.Empty)
                Dim path = If(info.ContainsKey("PATH"), info("PATH"), String.Empty)
                Dim exe = If(info.ContainsKey("EXE"), info("EXE"), String.Empty)
                Dim varv = If(info.ContainsKey("VAR"), info("VAR"), "2")
                Dim cfg = If(info.ContainsKey("CFG"), info("CFG"), String.Empty)
                ' ÚJ: Port kinyerése a Panel Tag-ből törlés/újraindexelés során
                Dim prt = If(info.ContainsKey("PORT"), info("PORT"), "55901")

                If String.IsNullOrEmpty(name) AndAlso String.IsNullOrEmpty(exe) Then Continue For

                ini($"NEV_{idx}") = name
                ini($"MAPPA_{idx}") = path
                ini($"EXE_{idx}") = exe
                ini($"VAR_{idx}") = varv
                ini($"PORT_{idx}") = prt ' ÚJ: Port mentése a megfelelő dinamikus indexhez

                If Not String.IsNullOrEmpty(cfg) Then
                    ' normalize relative as before
                    Try
                        Dim baseDir = System.IO.Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory)
                        If Not baseDir.EndsWith(System.IO.Path.DirectorySeparatorChar) Then baseDir &= System.IO.Path.DirectorySeparatorChar
                        Dim fullCfg = System.IO.Path.GetFullPath(cfg)
                        If fullCfg.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase) Then
                            ini($"CFG_{idx}") = fullCfg.Substring(baseDir.Length)
                        Else
                            ini($"CFG_{idx}") = fullCfg
                        End If
                    Catch
                        ini($"CFG_{idx}") = cfg
                    End Try
                End If

                ' Also include global PRCEN_GLOBAL in the rebuilt INI (enabled flag only)
                Try
                    Dim prcEnGlobal As String = "0"
                    Dim chk = Me.Controls.Find("PrcRestart", True).FirstOrDefault()
                    If chk IsNot Nothing Then
                        If TypeOf chk Is CheckBox Then prcEnGlobal = If(CType(chk, CheckBox).Checked, "1", "0")
                        If TypeOf chk Is RadioButton Then prcEnGlobal = If(CType(chk, RadioButton).Checked, "1", "0")
                    End If
                    ini("PRCEN_GLOBAL") = prcEnGlobal
                Catch
                End Try

                idx += 1
            Next

            SettingsStore.WriteSettingsToFile(IniPath, ini)
            Form1.BeallitasokBetoltese()
        Catch
        End Try
    End Sub

    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub
End Class
