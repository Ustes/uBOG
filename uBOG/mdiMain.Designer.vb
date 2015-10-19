<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class mdiMain
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(mdiMain))
        Me.StatusStrip = New System.Windows.Forms.StatusStrip()
        Me.mainStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.ConnectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConnectToDatabaseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConnectWithConnectionstringToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuMRU = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.QuitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RunScriptsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RunScriptsToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mainStatus})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 500)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(874, 22)
        Me.StatusStrip.TabIndex = 7
        Me.StatusStrip.Text = "StatusStrip"
        '
        'mainStatus
        '
        Me.mainStatus.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.mainStatus.Name = "mainStatus"
        Me.mainStatus.Size = New System.Drawing.Size(859, 17)
        Me.mainStatus.Spring = True
        Me.mainStatus.Text = "Status"
        Me.mainStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ConnectToolStripMenuItem, Me.HelpToolStripMenuItem, Me.RunScriptsToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(874, 24)
        Me.MenuStrip1.TabIndex = 9
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'ConnectToolStripMenuItem
        '
        Me.ConnectToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ConnectToDatabaseToolStripMenuItem, Me.ConnectWithConnectionstringToolStripMenuItem, Me.mnuMRU, Me.ToolStripSeparator1, Me.QuitToolStripMenuItem})
        Me.ConnectToolStripMenuItem.Name = "ConnectToolStripMenuItem"
        Me.ConnectToolStripMenuItem.Size = New System.Drawing.Size(64, 20)
        Me.ConnectToolStripMenuItem.Text = "Connect"
        '
        'ConnectToDatabaseToolStripMenuItem
        '
        Me.ConnectToDatabaseToolStripMenuItem.Name = "ConnectToDatabaseToolStripMenuItem"
        Me.ConnectToDatabaseToolStripMenuItem.Size = New System.Drawing.Size(244, 22)
        Me.ConnectToDatabaseToolStripMenuItem.Text = "Connect to Database"
        '
        'ConnectWithConnectionstringToolStripMenuItem
        '
        Me.ConnectWithConnectionstringToolStripMenuItem.Name = "ConnectWithConnectionstringToolStripMenuItem"
        Me.ConnectWithConnectionstringToolStripMenuItem.Size = New System.Drawing.Size(244, 22)
        Me.ConnectWithConnectionstringToolStripMenuItem.Text = "Connect with Connection String"
        '
        'mnuMRU
        '
        Me.mnuMRU.Name = "mnuMRU"
        Me.mnuMRU.Size = New System.Drawing.Size(244, 22)
        Me.mnuMRU.Text = "Recent Connections"
        Me.mnuMRU.Visible = False
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(241, 6)
        '
        'QuitToolStripMenuItem
        '
        Me.QuitToolStripMenuItem.Name = "QuitToolStripMenuItem"
        Me.QuitToolStripMenuItem.Size = New System.Drawing.Size(244, 22)
        Me.QuitToolStripMenuItem.Text = "Quit"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.HelpToolStripMenuItem.Text = "Help"
        '
        'RunScriptsToolStripMenuItem
        '
        Me.RunScriptsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RunScriptsToolStripMenuItem1})
        Me.RunScriptsToolStripMenuItem.Name = "RunScriptsToolStripMenuItem"
        Me.RunScriptsToolStripMenuItem.Size = New System.Drawing.Size(54, 20)
        Me.RunScriptsToolStripMenuItem.Text = "Scripts"
        '
        'RunScriptsToolStripMenuItem1
        '
        Me.RunScriptsToolStripMenuItem1.Name = "RunScriptsToolStripMenuItem1"
        Me.RunScriptsToolStripMenuItem1.Size = New System.Drawing.Size(152, 22)
        Me.RunScriptsToolStripMenuItem1.Text = "Run Script(s)"
        '
        'mdiMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(874, 522)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.IsMdiContainer = True
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "mdiMain"
        Me.Text = "u3 SQL Business Object Generator"
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents mainStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents StatusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents ConnectToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ConnectToDatabaseToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ConnectWithConnectionstringToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnuMRU As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents QuitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RunScriptsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RunScriptsToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem

End Class
