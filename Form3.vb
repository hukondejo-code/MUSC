Imports System.IO
Imports System.Threading

Public Class Form3

    Private DatUtvonal As String = Path.Combine(AppContext.BaseDirectory, "Settings.ini")

    Private Sub Form3_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        ' Háttérszál indítása a grafika fagyásának elkerülésére
        Dim bgSzal As New Thread(AddressOf DinamikusBetoltes)
        bgSzal.IsBackground = True
        bgSzal.Start()
    End Sub

    Private Sub DinamikusBetoltes()
        Try
            ' Biztonságos elemkeresés a felületen a deklarációs hiba kiküszöbölésére
            Dim m_ProgressBar As ProgressBar = Nothing
            Dim m_Label As Label = Nothing

            Me.Invoke(Sub()
                          m_ProgressBar = CType(Me.Controls.Find("ProgressBar1", True).FirstOrDefault(), ProgressBar)
                          m_Label = CType(Me.Controls.Find("LblBetoltes", True).FirstOrDefault(), Label)
                      End Sub)

            ' Segédfunkció az elemek frissítésére a háttérszálból
            Dim Frissit As Action(Of String, Integer) =
                Sub(szoveg As String, szazalek As Integer)
                    Me.Invoke(Sub()
                                  If m_Label IsNot Nothing Then m_Label.Text = szoveg
                                  If m_ProgressBar IsNot Nothing Then m_ProgressBar.Value = szazalek
                              End Sub)
                End Sub

            ' 1. FÁZIS: Adatfájl keresése és beolvasása
            Frissit("Searching for settings file (Settings.ini) ...", 10)
            ' Ensure legacy Settings.dat is migrated to Settings.ini early (splash)
            SettingsStore.MigrateDatToIni(AppContext.BaseDirectory)
            Thread.Sleep(300)

            ' Létrehozzuk a Form1 példányát a háttérben
            Dim f1 As Form1 = CType(Application.OpenForms("Form1"), Form1)
            If f1 Is Nothing Then
                ' Ha még nem fut a Form1 (mivel a Form3 az induló ablak), akkor itt nem érjük el közvetlenül a szótárat,
                ' ezért ideiglenesen beolvassuk a fájlt egy helyi listába, amit majd átadunk a Form1-nek az indulásakor.
            End If

            Frissit("Reading configurations ...", 20)
            Thread.Sleep(200)

            ' 2. FÁZIS: A 8 alkalmazás szimulált beolvasása és kiírása
            For i As Integer = 1 To 8
                Dim szoftverNev As String = " ..."

                ' Átnyúlunk a Form1 statikus/alapértelmezett példányába (My.Forms segítségével .NET 10 alatt)
                If Form1.Beallitasok.ContainsKey("NEV_" & i) Then
                    szoftverNev = Form1.Beallitasok("NEV_" & i) & ".exe"
                End If

                Dim aktualisSzazalek As Integer = 20 + (i * 8)
                Frissit($"Checking executables [{i}/8]: {szoftverNev}", aktualisSzazalek)
                Thread.Sleep(150)
            Next

            Frissit("Setting up platform ...", 95)
            Thread.Sleep(400)

            ' Megkérjük a Form1-et a háttérben, hogy frissítse a füleit a beolvasott adatokkal
            Me.Invoke(Sub() Form1.AblakFülNevekFrissitese())

            Frissit("Ready!", 100)
            Thread.Sleep(200)

            ' Bezárjuk a Splash-t és elindítjuk a főablakot
            Me.Invoke(Sub()
                          My.Forms.Form1.Show()
                          Me.Hide() ' Elrejtjük a Splash-t, hogy a főablak átvegye a helyét
                      End Sub)

        Catch ex As Exception
            ' Log full exception to temp for diagnosis
            Try
                Dim tmp = Path.Combine(Path.GetTempPath(), "Form3_error.txt")
                File.WriteAllText(tmp, ex.ToString())
            Catch
            End Try

            Me.Invoke(Sub()
                          MessageBox.Show("Error during loading splashscreen : " & ex.Message)
                          Me.Close()
                      End Sub)
        End Try
    End Sub

End Class
