<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form4
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form4))
        PictureBox1 = New PictureBox()
        rtbAbout = New RichTextBox()
        BtnOK = New Button()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackColor = Color.Transparent
        PictureBox1.BackgroundImage = CType(resources.GetObject("PictureBox1.BackgroundImage"), Image)
        PictureBox1.BackgroundImageLayout = ImageLayout.Stretch
        PictureBox1.Location = New Point(207, 12)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(77, 78)
        PictureBox1.TabIndex = 0
        PictureBox1.TabStop = False
        ' 
        ' rtbAbout
        ' 
        rtbAbout.BackColor = SystemColors.Control
        rtbAbout.BorderStyle = BorderStyle.None
        rtbAbout.Font = New Font("Segoe UI Light", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(238))
        rtbAbout.ForeColor = Color.FromArgb(CByte(30), CByte(30), CByte(30))
        rtbAbout.Location = New Point(12, 99)
        rtbAbout.Name = "rtbAbout"
        rtbAbout.ReadOnly = True
        rtbAbout.ScrollBars = RichTextBoxScrollBars.None
        rtbAbout.Size = New Size(468, 263)
        rtbAbout.TabIndex = 1
        rtbAbout.Text = "https://github.com/hukondejo-code/MUSC"
        rtbAbout.WordWrap = False
        ' 
        ' BtnOK
        ' 
        BtnOK.BackColor = Color.FromArgb(CByte(30), CByte(30), CByte(30))
        BtnOK.ForeColor = SystemColors.Control
        BtnOK.Location = New Point(358, 373)
        BtnOK.Name = "BtnOK"
        BtnOK.Size = New Size(122, 32)
        BtnOK.TabIndex = 3
        BtnOK.Text = "OK"
        BtnOK.UseVisualStyleBackColor = False
        ' 
        ' Form4
        ' 
        AcceptButton = BtnOK
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        CancelButton = BtnOK
        ClientSize = New Size(496, 417)
        Controls.Add(BtnOK)
        Controls.Add(rtbAbout)
        Controls.Add(PictureBox1)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "Form4"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents rtbAbout As RichTextBox
    Friend WithEvents BtnOK As Button
End Class
