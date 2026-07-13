<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Panel9 = New Panel()
        Panel2 = New Panel()
        MenuStrip1 = New MenuStrip()
        FileToolStripMenuItem = New ToolStripMenuItem()
        ServerStartupToolStripMenuItem = New ToolStripMenuItem()
        ServerShutdownToolStripMenuItem = New ToolStripMenuItem()
        ExitServerShutdownToolStripMenuItem = New ToolStripMenuItem()
        OptionsToolStripMenuItem = New ToolStripMenuItem()
        SettingsToolStripMenuItem = New ToolStripMenuItem()
        AboutToolStripMenuItem = New ToolStripMenuItem()
        BtnShutdownApps = New Button()
        TabControl1 = New TabControl()
        TabPage1 = New TabPage()
        Panel1 = New Panel()
        btnStartUp = New Button()
        btnShutDown = New Button()
        btnSettings = New Button()
        Panel3 = New Panel()
        Panel9.SuspendLayout()
        MenuStrip1.SuspendLayout()
        TabControl1.SuspendLayout()
        TabPage1.SuspendLayout()
        Panel3.SuspendLayout()
        SuspendLayout()
        ' 
        ' Panel9
        ' 
        Panel9.Controls.Add(Panel2)
        Panel9.Controls.Add(MenuStrip1)
        Panel9.Controls.Add(BtnShutdownApps)
        Panel9.Dock = DockStyle.Top
        Panel9.Location = New Point(0, 0)
        Panel9.Name = "Panel9"
        Panel9.Size = New Size(1130, 26)
        Panel9.TabIndex = 0
        ' 
        ' Panel2
        ' 
        Panel2.BackColor = Color.Silver
        Panel2.Location = New Point(0, 27)
        Panel2.Name = "Panel2"
        Panel2.Size = New Size(204, 64)
        Panel2.TabIndex = 5
        ' 
        ' MenuStrip1
        ' 
        MenuStrip1.BackColor = Color.LightGray
        MenuStrip1.Items.AddRange(New ToolStripItem() {FileToolStripMenuItem, OptionsToolStripMenuItem, AboutToolStripMenuItem})
        MenuStrip1.Location = New Point(0, 0)
        MenuStrip1.Name = "MenuStrip1"
        MenuStrip1.Size = New Size(1130, 24)
        MenuStrip1.TabIndex = 0
        MenuStrip1.Text = "MenuStrip1"
        ' 
        ' FileToolStripMenuItem
        ' 
        FileToolStripMenuItem.BackColor = Color.Transparent
        FileToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {ServerStartupToolStripMenuItem, ServerShutdownToolStripMenuItem, ExitServerShutdownToolStripMenuItem})
        FileToolStripMenuItem.ForeColor = Color.FromArgb(CByte(30), CByte(30), CByte(30))
        FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        FileToolStripMenuItem.Size = New Size(37, 20)
        FileToolStripMenuItem.Text = "File"
        ' 
        ' ServerStartupToolStripMenuItem
        ' 
        ServerStartupToolStripMenuItem.Name = "ServerStartupToolStripMenuItem"
        ServerStartupToolStripMenuItem.Size = New Size(192, 22)
        ServerStartupToolStripMenuItem.Text = "Server Startup"
        ' 
        ' ServerShutdownToolStripMenuItem
        ' 
        ServerShutdownToolStripMenuItem.Name = "ServerShutdownToolStripMenuItem"
        ServerShutdownToolStripMenuItem.Size = New Size(192, 22)
        ServerShutdownToolStripMenuItem.Text = "Server Shutdown"
        ' 
        ' ExitServerShutdownToolStripMenuItem
        ' 
        ExitServerShutdownToolStripMenuItem.Name = "ExitServerShutdownToolStripMenuItem"
        ExitServerShutdownToolStripMenuItem.Size = New Size(192, 22)
        ExitServerShutdownToolStripMenuItem.Text = "Exit (Server Shutdown)"
        ' 
        ' OptionsToolStripMenuItem
        ' 
        OptionsToolStripMenuItem.BackColor = Color.Transparent
        OptionsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {SettingsToolStripMenuItem})
        OptionsToolStripMenuItem.ForeColor = Color.FromArgb(CByte(30), CByte(30), CByte(30))
        OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        OptionsToolStripMenuItem.Size = New Size(61, 20)
        OptionsToolStripMenuItem.Text = "Options"
        ' 
        ' SettingsToolStripMenuItem
        ' 
        SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        SettingsToolStripMenuItem.Size = New Size(116, 22)
        SettingsToolStripMenuItem.Text = "Settings"
        ' 
        ' AboutToolStripMenuItem
        ' 
        AboutToolStripMenuItem.BackColor = Color.Transparent
        AboutToolStripMenuItem.ForeColor = Color.FromArgb(CByte(30), CByte(30), CByte(30))
        AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        AboutToolStripMenuItem.Size = New Size(52, 20)
        AboutToolStripMenuItem.Text = "About"
        ' 
        ' BtnShutdownApps
        ' 
        BtnShutdownApps.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        BtnShutdownApps.Location = New Point(1016, 0)
        BtnShutdownApps.Name = "BtnShutdownApps"
        BtnShutdownApps.Size = New Size(110, 24)
        BtnShutdownApps.TabIndex = 2
        BtnShutdownApps.Text = "Shutdown Apps"
        BtnShutdownApps.UseVisualStyleBackColor = True
        ' 
        ' TabControl1
        ' 
        TabControl1.Appearance = TabAppearance.Buttons
        TabControl1.Controls.Add(TabPage1)
        TabControl1.Dock = DockStyle.Top
        TabControl1.DrawMode = TabDrawMode.OwnerDrawFixed
        TabControl1.ImeMode = ImeMode.NoControl
        TabControl1.Location = New Point(0, 26)
        TabControl1.Name = "TabControl1"
        TabControl1.SelectedIndex = 0
        TabControl1.Size = New Size(1130, 615)
        TabControl1.TabIndex = 1
        ' 
        ' TabPage1
        ' 
        TabPage1.BackColor = Color.Transparent
        TabPage1.Controls.Add(Panel1)
        TabPage1.Location = New Point(4, 27)
        TabPage1.Name = "TabPage1"
        TabPage1.Padding = New Padding(3)
        TabPage1.Size = New Size(1122, 584)
        TabPage1.TabIndex = 0
        TabPage1.Text = "NOT DEFINED"
        ' 
        ' Panel1
        ' 
        Panel1.BackColor = Color.Transparent
        Panel1.Dock = DockStyle.Fill
        Panel1.Location = New Point(3, 3)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(1116, 578)
        Panel1.TabIndex = 0
        ' 
        ' btnStartUp
        ' 
        btnStartUp.BackColor = Color.FromArgb(CByte(80), CByte(80), CByte(80))
        btnStartUp.BackgroundImageLayout = ImageLayout.Stretch
        btnStartUp.Font = New Font("Segoe UI Semilight", 8F)
        btnStartUp.ForeColor = Color.WhiteSmoke
        btnStartUp.ImageAlign = ContentAlignment.BottomLeft
        btnStartUp.Location = New Point(425, 15)
        btnStartUp.Name = "btnStartUp"
        btnStartUp.Size = New Size(84, 34)
        btnStartUp.TabIndex = 2
        btnStartUp.Text = "STARTUP"
        btnStartUp.UseVisualStyleBackColor = False
        ' 
        ' btnShutDown
        ' 
        btnShutDown.BackColor = Color.FromArgb(CByte(80), CByte(80), CByte(80))
        btnShutDown.BackgroundImageLayout = ImageLayout.Stretch
        btnShutDown.Font = New Font("Segoe UI Semilight", 8F)
        btnShutDown.ForeColor = Color.WhiteSmoke
        btnShutDown.ImageAlign = ContentAlignment.BottomLeft
        btnShutDown.Location = New Point(515, 15)
        btnShutDown.Name = "btnShutDown"
        btnShutDown.Size = New Size(84, 34)
        btnShutDown.TabIndex = 3
        btnShutDown.Text = "SHUTDOWN"
        btnShutDown.UseVisualStyleBackColor = False
        ' 
        ' btnSettings
        ' 
        btnSettings.BackColor = Color.FromArgb(CByte(80), CByte(80), CByte(80))
        btnSettings.BackgroundImageLayout = ImageLayout.Stretch
        btnSettings.Font = New Font("Segoe UI Semilight", 8F)
        btnSettings.ForeColor = Color.WhiteSmoke
        btnSettings.ImageAlign = ContentAlignment.BottomLeft
        btnSettings.Location = New Point(605, 15)
        btnSettings.Name = "btnSettings"
        btnSettings.Size = New Size(84, 34)
        btnSettings.TabIndex = 4
        btnSettings.Text = "SETTINGS"
        btnSettings.UseVisualStyleBackColor = False
        ' 
        ' Panel3
        ' 
        Panel3.BackColor = Color.LightGray
        Panel3.BorderStyle = BorderStyle.FixedSingle
        Panel3.Controls.Add(btnSettings)
        Panel3.Controls.Add(btnStartUp)
        Panel3.Controls.Add(btnShutDown)
        Panel3.Dock = DockStyle.Bottom
        Panel3.Location = New Point(0, 641)
        Panel3.Name = "Panel3"
        Panel3.Size = New Size(1130, 73)
        Panel3.TabIndex = 2
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        AutoSize = True
        BackColor = Color.DimGray
        ClientSize = New Size(1130, 714)
        Controls.Add(Panel3)
        Controls.Add(TabControl1)
        Controls.Add(Panel9)
        ForeColor = Color.Black
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MainMenuStrip = MenuStrip1
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "MU SERVER CONTROL"
        Panel9.ResumeLayout(False)
        Panel9.PerformLayout()
        MenuStrip1.ResumeLayout(False)
        MenuStrip1.PerformLayout()
        TabControl1.ResumeLayout(False)
        TabPage1.ResumeLayout(False)
        Panel3.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents Panel9 As Panel
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents Panel1 As Panel
    ' Only one tab + panel (dynamic tabs will be created at runtime)
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ServerStartupToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExitServerShutdownToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BtnShutdownApps As Button
    Friend WithEvents OptionsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SettingsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents btnStartUp As Button
    Friend WithEvents btnShutDown As Button
    Friend WithEvents btnSettings As Button
    Friend WithEvents ServerShutdownToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Panel3 As Panel

End Class
