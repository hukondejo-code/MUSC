Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Threading

Public Class Form1

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

    ' --- Amikor a Visual Basic program elindul (Most már csak a fájlt olvassa be!) ---
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' A Form1 betöltésekor csak a memóriában lévő beállításokat érvényesítjük, 
        ' mert a tényleges beolvasást már a Splash ablak elvégezte előttünk!
        BeallitasokBetoltese()
        AblakFülNevekFrissitese()
    End Sub

    ' Készítsünk egy külön nyilvános segédfunkciót a Form1-ben, amit a Splash meg tud hívni a fülek átnevezéséhez:
    Public Sub AblakFülNevekFrissitese()
        If TabControl1 IsNot Nothing Then
            For i As Integer = 1 To 8
                Dim kulcsNev As String = "NEV_" & i
                If Beallitasok.ContainsKey(kulcsNev) Then
                    If TabControl1.TabPages.Count >= i Then
                        TabControl1.TabPages(i - 1).Text = Beallitasok(kulcsNev)
                    End If
                End If
            Next
            TabControl1.Invalidate()
        End If
    End Sub



    Public Sub BeallitasokBetoltese()
        Try
            Beallitasok.Clear()
            Dim txtUtvonal As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.dat")

            If Not File.Exists(txtUtvonal) Then
                MessageBox.Show("Can't find the settings file. Will be created after your first (File>Settings>Save Settings) save.")
                Exit Sub
            End If

            ' 1. Fájl soronkénti beolvasása és mentése a memóriába
            For Each sor As String In File.ReadLines(txtUtvonal)
                If sor.Contains("=") AndAlso Not sor.StartsWith("::") AndAlso Not sor.Trim() = "" Then
                    Dim reszek = sor.Split("="c)
                    Dim kulcs As String = reszek(0).Trim()
                    Dim ertek As String = reszek(1).Trim()
                    If Not Beallitasok.ContainsKey(kulcs) Then
                        Beallitasok.Add(kulcs, ertek)
                    End If
                End If
            Next

            ' 2. ÚJ FUNKCIÓ: A fülek (TabPage-ek) neveinek dinamikus átírása
            ' Ellenőrizzük, hogy a TabControl1 létezik-e az ablakon
            If TabControl1 IsNot Nothing Then
                For i As Integer = 1 To 8
                    Dim kulcsNev As String = "NEV_" & i

                    ' Csak akkor írjuk át, ha a musc.txt-ben létezik az adott sorszámú név
                    If Beallitasok.ContainsKey(kulcsNev) Then
                        Dim szoftverNev As String = Beallitasok(kulcsNev)

                        ' A .NET-ben a fülek indexelése 0-tól indul, így az i-1. fület módosítjuk
                        If TabControl1.TabPages.Count >= i Then
                            TabControl1.TabPages(i - 1).Text = szoftverNev
                        End If
                    End If
                Next
            End If

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
        For i As Integer = 1 To 8
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
                        Dim panelNev As String = "Panel" & i

                        Me.Invoke(Sub()
                                      Dim celPanel As Panel = CType(Me.Controls.Find(panelNev, True).FirstOrDefault(), Panel)
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
        Form2.Show()
    End Sub

    Private Sub ExitServerShutdownToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitServerShutdownToolStripMenuItem.Click
        Application.Exit()
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
