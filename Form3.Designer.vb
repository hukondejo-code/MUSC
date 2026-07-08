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
        ProgressBar1 = New ProgressBar()
        LblBetoltes = New Label()
        PictureBox1 = New PictureBox()
        Label1 = New Label()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' ProgressBar1
        ' 
        ProgressBar1.BackColor = Color.WhiteSmoke
        ProgressBar1.ForeColor = Color.DeepPink
        ProgressBar1.Location = New Point(106, 392)
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
        LblBetoltes.Location = New Point(106, 376)
        LblBetoltes.Name = "LblBetoltes"
        LblBetoltes.Size = New Size(54, 13)
        LblBetoltes.TabIndex = 2
        LblBetoltes.Text = "Loading ..."
        LblBetoltes.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackColor = Color.Transparent
        PictureBox1.BackgroundImage = CType(resources.GetObject("PictureBox1.BackgroundImage"), Image)
        PictureBox1.BackgroundImageLayout = ImageLayout.Center
        PictureBox1.Location = New Point(1, 2)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(763, 371)
        PictureBox1.TabIndex = 3
        PictureBox1.TabStop = False
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.BackColor = Color.Transparent
        Label1.Font = New Font("Segoe UI Light", 8.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label1.Location = New Point(605, 242)
        Label1.Name = "Label1"
        Label1.Size = New Size(121, 13)
        Label1.TabIndex = 4
        Label1.Text = "BY: HUKONDEJO - CODE"
        ' 
        ' Form3
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), Image)
        BackgroundImageLayout = ImageLayout.None
        ClientSize = New Size(766, 428)
        Controls.Add(Label1)
        Controls.Add(LblBetoltes)
        Controls.Add(ProgressBar1)
        Controls.Add(PictureBox1)
        FormBorderStyle = FormBorderStyle.None
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "Form3"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Form3"
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents LblBetoltes As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Label1 As Label
End Class
