Public Class Form4

    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' 1. Szöveg feltöltése (0 MB belső RAM overhead)
        rtbAbout.Text = "MU Server Control (MUSC)        " & vbCrLf & vbCrLf &
                        "An ultra-lightweight, resource-friendly private server manager sofware," & vbCrLf &
                        "optimized for minimal memory usage and maximal stability." & vbCrLf & vbCrLf &
                        "Main features:" & vbCrLf &
                        "• Ultra-lightweight hardware recommendation (~19.3 MB idle RAM footprint)" & vbCrLf &
                        "• Bulletproof Win32 window hooking and embeding engine" & vbCrLf &
                        "• Asyncronus Port Availability Shield countering dead-locks" & vbCrLf &
                        "• Built-in Universal Server Log Vacuum automatic cleaner" & vbCrLf &
                        "• Live configuration handling with 0 MB inside RAM overhead" & vbCrLf & vbCrLf &
                         "Developers: hukondejo-code (Lead Developer) & Gemini AI (Technical Architect)" & vbCrLf &
                         "Licence: GNU General Public License Version 3 (GPLv3)" & vbCrLf & vbCrLf &
                         "For more information and updates, please visit:" & vbCrLf &
                         "https://github.com/hukondejo-code/MUSC"

        ' 2. A TELJES szöveg kijelölése és KÖZÉPRE rendezése
        rtbAbout.SelectAll()
        rtbAbout.SelectionAlignment = HorizontalAlignment.Center

        ' 3. Csak az ELSŐ SOR kijelölése és FÉLKÖVÉRRÉ tétele
        ' (A "MU Server Control (MUSC) v1.2.0" pontosan 31 karakter)
        rtbAbout.Select(0, 31)
        rtbAbout.SelectionFont = New Font(rtbAbout.Font, FontStyle.Bold)

        ' 4. Kijelölés megszüntetése, hogy ne legyen kék/kijelölt a szöveg indításkor
        rtbAbout.SelectionLength = 0
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        ' Kiürítjük a RichTextBoxot, hogy elengedje a szövegformázási memóriát
        rtbAbout.Clear()
        rtbAbout.Dispose()

        ' Bezárjuk és megsemmisítjük a Formot
        Me.Close()
        Me.Dispose()
    End Sub

    ' A GitHub link megnyitása a gyári Win32 UseShellExecute trükkel (0 MB belső RAM overhead)
    Private Sub rtbAbout_LinkClicked(sender As Object, e As LinkClickedEventArgs) Handles rtbAbout.LinkClicked
        Try
            Dim psi As New ProcessStartInfo() With {
            .FileName = e.LinkText, ' Automatikusan a kattintott URL-t nyitja meg
            .UseShellExecute = True
        }
            Process.Start(psi)
        Catch ex As Exception
            MessageBox.Show("Can't open URL: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Form4_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        rtbAbout.Clear()
        rtbAbout.Dispose()
        Me.Dispose()
    End Sub
End Class