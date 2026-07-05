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
        MenuStrip1 = New MenuStrip()
        FileToolStripMenuItem = New ToolStripMenuItem()
        ServerStartupToolStripMenuItem = New ToolStripMenuItem()
        SettingsToolStripMenuItem = New ToolStripMenuItem()
        ExitServerShutdownToolStripMenuItem = New ToolStripMenuItem()
        BtnShutdownApps = New Button()
        TabControl1 = New TabControl()
        TabPage1 = New TabPage()
        Panel1 = New Panel()
        Panel9.SuspendLayout()
        MenuStrip1.SuspendLayout()
        TabControl1.SuspendLayout()
        TabPage1.SuspendLayout()
        SuspendLayout()
        ' 
        ' Panel9
        ' 
        Panel9.Controls.Add(MenuStrip1)
        Panel9.Controls.Add(BtnShutdownApps)
        Panel9.Dock = DockStyle.Top
        Panel9.Location = New Point(0, 0)
        Panel9.Name = "Panel9"
        Panel9.Size = New Size(1114, 26)
        Panel9.TabIndex = 0
        ' 
        ' MenuStrip1
        ' 
        MenuStrip1.BackColor = Color.WhiteSmoke
        MenuStrip1.Items.AddRange(New ToolStripItem() {FileToolStripMenuItem})
        MenuStrip1.Location = New Point(0, 0)
        MenuStrip1.Name = "MenuStrip1"
        MenuStrip1.Size = New Size(1114, 24)
        MenuStrip1.TabIndex = 0
        MenuStrip1.Text = "MenuStrip1"
        ' 
        ' FileToolStripMenuItem
        ' 
        FileToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {ServerStartupToolStripMenuItem, SettingsToolStripMenuItem, ExitServerShutdownToolStripMenuItem})
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
        ' SettingsToolStripMenuItem
        ' 
        SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        SettingsToolStripMenuItem.Size = New Size(192, 22)
        SettingsToolStripMenuItem.Text = "Settings"
        ' 
        ' ExitServerShutdownToolStripMenuItem
        ' 
        ExitServerShutdownToolStripMenuItem.Name = "ExitServerShutdownToolStripMenuItem"
        ExitServerShutdownToolStripMenuItem.Size = New Size(192, 22)
        ExitServerShutdownToolStripMenuItem.Text = "Exit (Server Shutdown)"
        ' 
        ' BtnShutdownApps
        ' 
        BtnShutdownApps.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        BtnShutdownApps.Location = New Point(1000, 0)
        BtnShutdownApps.Name = "BtnShutdownApps"
        BtnShutdownApps.Size = New Size(110, 24)
        BtnShutdownApps.TabIndex = 2
        BtnShutdownApps.Text = "Shutdown Apps"
        BtnShutdownApps.UseVisualStyleBackColor = True
        ' 
        ' TabControl1
        ' 
        TabControl1.Controls.Add(TabPage1)
        TabControl1.Dock = DockStyle.Fill
        TabControl1.DrawMode = TabDrawMode.OwnerDrawFixed
        TabControl1.Location = New Point(0, 26)
        TabControl1.Name = "TabControl1"
        TabControl1.SelectedIndex = 0
        TabControl1.Size = New Size(1114, 586)
        TabControl1.TabIndex = 1
        ' 
        ' TabPage1
        ' 
        TabPage1.BackColor = Color.Transparent
        TabPage1.Controls.Add(Panel1)
        TabPage1.Location = New Point(4, 24)
        TabPage1.Name = "TabPage1"
        TabPage1.Padding = New Padding(3)
        TabPage1.Size = New Size(1106, 558)
        TabPage1.TabIndex = 0
        TabPage1.Text = "NOT DEFINED"
        ' 
        ' Panel1
        ' 
        Panel1.Dock = DockStyle.Fill
        Panel1.Location = New Point(3, 3)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(1100, 552)
        Panel1.TabIndex = 0
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.WhiteSmoke
        ClientSize = New Size(1114, 612)
        Controls.Add(TabControl1)
        Controls.Add(Panel9)
        ForeColor = Color.Black
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
    Friend WithEvents SettingsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExitServerShutdownToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BtnShutdownApps As Button

End Class
