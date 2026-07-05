<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form3
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form3))
        Label1 = New Label()
        ProgressBar1 = New ProgressBar()
        LblBetoltes = New Label()
        SuspendLayout()
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.BackColor = Color.FromArgb(CByte(30), CByte(30), CByte(30))
        Label1.Font = New Font("Segoe UI Black", 36F, FontStyle.Bold, GraphicsUnit.Point, CByte(238))
        Label1.ForeColor = Color.DeepPink
        Label1.Image = CType(resources.GetObject("Label1.Image"), Image)
        Label1.Location = New Point(99, 174)
        Label1.Name = "Label1"
        Label1.Size = New Size(549, 65)
        Label1.TabIndex = 0
        Label1.Text = "MU SERVER CONTROL"
        ' 
        ' ProgressBar1
        ' 
        ProgressBar1.BackColor = Color.WhiteSmoke
        ProgressBar1.ForeColor = Color.DeepPink
        ProgressBar1.Location = New Point(99, 431)
        ProgressBar1.Name = "ProgressBar1"
        ProgressBar1.Size = New Size(549, 13)
        ProgressBar1.Style = ProgressBarStyle.Continuous
        ProgressBar1.TabIndex = 1
        ' 
        ' LblBetoltes
        ' 
        LblBetoltes.AutoSize = True
        LblBetoltes.BackColor = Color.Transparent
        LblBetoltes.Font = New Font("Segoe UI", 8.25F, FontStyle.Italic, GraphicsUnit.Point, CByte(238))
        LblBetoltes.Location = New Point(99, 415)
        LblBetoltes.Name = "LblBetoltes"
        LblBetoltes.Size = New Size(54, 13)
        LblBetoltes.TabIndex = 2
        LblBetoltes.Text = "Loading ..."
        LblBetoltes.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' Form3
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), Image)
        BackgroundImageLayout = ImageLayout.None
        ClientSize = New Size(814, 477)
        Controls.Add(LblBetoltes)
        Controls.Add(ProgressBar1)
        Controls.Add(Label1)
        FormBorderStyle = FormBorderStyle.None
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "Form3"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Form3"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents LblBetoltes As Label
End Class
