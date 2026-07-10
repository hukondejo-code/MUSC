<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form2
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form2))
        btnAddRow = New Button()
        BtnMentes = New Button()
        Panel1 = New Panel()
        Label9 = New Label()
        Label8 = New Label()
        Label7 = New Label()
        Label6 = New Label()
        Label5 = New Label()
        Label4 = New Label()
        Label3 = New Label()
        Label2 = New Label()
        Label1 = New Label()
        flowRows = New FlowLayoutPanel()
        PictureBox1 = New PictureBox()
        Panel1.SuspendLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' btnAddRow
        ' 
        btnAddRow.Location = New Point(737, 32)
        btnAddRow.Name = "btnAddRow"
        btnAddRow.Size = New Size(60, 28)
        btnAddRow.TabIndex = 1
        btnAddRow.Text = "+"
        btnAddRow.UseVisualStyleBackColor = True
        ' 
        ' BtnMentes
        ' 
        BtnMentes.BackColor = Color.FromArgb(CByte(30), CByte(30), CByte(30))
        BtnMentes.ForeColor = SystemColors.Control
        BtnMentes.Location = New Point(737, 451)
        BtnMentes.Name = "BtnMentes"
        BtnMentes.Size = New Size(122, 32)
        BtnMentes.TabIndex = 2
        BtnMentes.Text = "SAVE SETTINGS"
        BtnMentes.UseVisualStyleBackColor = False
        ' 
        ' Panel1
        ' 
        Panel1.BackColor = SystemColors.ControlLight
        Panel1.BackgroundImageLayout = ImageLayout.None
        Panel1.BorderStyle = BorderStyle.FixedSingle
        Panel1.Controls.Add(Label9)
        Panel1.Controls.Add(Label8)
        Panel1.Controls.Add(Label7)
        Panel1.Controls.Add(Label6)
        Panel1.Controls.Add(Label5)
        Panel1.Controls.Add(Label4)
        Panel1.Controls.Add(Label3)
        Panel1.Controls.Add(Label2)
        Panel1.Controls.Add(Label1)
        Panel1.Controls.Add(btnAddRow)
        Panel1.Controls.Add(BtnMentes)
        Panel1.Controls.Add(flowRows)
        Panel1.Controls.Add(PictureBox1)
        Panel1.Location = New Point(-2, 0)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(873, 509)
        Panel1.TabIndex = 3
        ' 
        ' Label9
        ' 
        Label9.AutoSize = True
        Label9.Font = New Font("Segoe UI", 6.75F)
        Label9.Location = New Point(581, 7)
        Label9.Name = "Label9"
        Label9.Size = New Size(35, 12)
        Label9.TabIndex = 11
        Label9.Text = "Port Nr."
        ' 
        ' Label8
        ' 
        Label8.AutoSize = True
        Label8.Font = New Font("Segoe UI", 6.75F)
        Label8.Location = New Point(747, 7)
        Label8.Name = "Label8"
        Label8.Size = New Size(39, 12)
        Label8.TabIndex = 10
        Label8.Text = "Add App"
        ' 
        ' Label7
        ' 
        Label7.AutoSize = True
        Label7.Font = New Font("Segoe UI", 6.75F)
        Label7.Location = New Point(644, 7)
        Label7.Name = "Label7"
        Label7.Size = New Size(74, 12)
        Label7.TabIndex = 7
        Label7.Text = "Remove From List"
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Font = New Font("Segoe UI", 6.75F)
        Label6.Location = New Point(514, 7)
        Label6.Name = "Label6"
        Label6.Size = New Size(61, 12)
        Label6.TabIndex = 6
        Label6.Text = "Start Delay (S)"
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Font = New Font("Segoe UI", 6.75F)
        Label5.Location = New Point(444, 7)
        Label5.Name = "Label5"
        Label5.Size = New Size(55, 12)
        Label5.TabIndex = 5
        Label5.Text = "Open Config"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Font = New Font("Segoe UI", 6.75F)
        Label4.Location = New Point(389, 7)
        Label4.Name = "Label4"
        Label4.Size = New Size(36, 12)
        Label4.TabIndex = 4
        Label4.Text = "CFG File"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Segoe UI", 6.75F)
        Label3.Location = New Point(303, 7)
        Label3.Name = "Label3"
        Label3.Size = New Size(80, 12)
        Label3.TabIndex = 3
        Label3.Text = "Browse Application"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Segoe UI", 6.75F)
        Label2.Location = New Point(155, 7)
        Label2.Name = "Label2"
        Label2.Size = New Size(79, 12)
        Label2.TabIndex = 2
        Label2.Text = "(Config File Name)"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Segoe UI", 6.75F)
        Label1.Location = New Point(9, 7)
        Label1.Name = "Label1"
        Label1.Size = New Size(76, 12)
        Label1.TabIndex = 1
        Label1.Text = "Application Name"
        ' 
        ' flowRows
        ' 
        flowRows.AutoScroll = True
        flowRows.BackColor = Color.WhiteSmoke
        flowRows.BorderStyle = BorderStyle.FixedSingle
        flowRows.FlowDirection = FlowDirection.TopDown
        flowRows.Location = New Point(-1, 32)
        flowRows.Name = "flowRows"
        flowRows.Size = New Size(732, 471)
        flowRows.TabIndex = 0
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackgroundImage = CType(resources.GetObject("PictureBox1.BackgroundImage"), Image)
        PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
        PictureBox1.Location = New Point(737, 3)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(131, 500)
        PictureBox1.TabIndex = 3
        PictureBox1.TabStop = False
        ' 
        ' Form2
        ' 
        AutoScaleMode = AutoScaleMode.None
        ClientSize = New Size(871, 510)
        Controls.Add(Panel1)
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "Form2"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterScreen
        Text = "SETTINGS"
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)

    End Sub
    Friend WithEvents btnAddRow As Button
    Friend WithEvents BtnMentes As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents flowRows As FlowLayoutPanel
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents Label9 As Label
End Class