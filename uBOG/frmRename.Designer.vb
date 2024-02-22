<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmRename
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
        Me.grpReplaceText = New System.Windows.Forms.GroupBox()
        Me.txtTo = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtFrom = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.chkReplaceText = New System.Windows.Forms.CheckBox()
        Me.chkChangeExt = New System.Windows.Forms.CheckBox()
        Me.tsProgress = New System.Windows.Forms.ToolStripProgressBar()
        Me.tsStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.txtNewExt = New System.Windows.Forms.TextBox()
        Me.grpChangeExt = New System.Windows.Forms.GroupBox()
        Me.btnRename = New System.Windows.Forms.Button()
        Me.chkChangeFile = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.txtFolder = New System.Windows.Forms.TextBox()
        Me.btnSHowFolders = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.grpReplaceText.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.grpChangeExt.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpReplaceText
        '
        Me.grpReplaceText.Controls.Add(Me.txtTo)
        Me.grpReplaceText.Controls.Add(Me.Label2)
        Me.grpReplaceText.Controls.Add(Me.txtFrom)
        Me.grpReplaceText.Controls.Add(Me.Label1)
        Me.grpReplaceText.Dock = System.Windows.Forms.DockStyle.Top
        Me.grpReplaceText.Enabled = False
        Me.grpReplaceText.Location = New System.Drawing.Point(3, 87)
        Me.grpReplaceText.Name = "grpReplaceText"
        Me.grpReplaceText.Size = New System.Drawing.Size(264, 100)
        Me.grpReplaceText.TabIndex = 5
        Me.grpReplaceText.TabStop = False
        '
        'txtTo
        '
        Me.txtTo.Dock = System.Windows.Forms.DockStyle.Top
        Me.txtTo.Location = New System.Drawing.Point(3, 67)
        Me.txtTo.Name = "txtTo"
        Me.txtTo.Size = New System.Drawing.Size(258, 20)
        Me.txtTo.TabIndex = 2
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label2.Location = New System.Drawing.Point(3, 49)
        Me.Label2.Name = "Label2"
        Me.Label2.Padding = New System.Windows.Forms.Padding(0, 5, 0, 0)
        Me.Label2.Size = New System.Drawing.Size(58, 18)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "With This :"
        '
        'txtFrom
        '
        Me.txtFrom.Dock = System.Windows.Forms.DockStyle.Top
        Me.txtFrom.Location = New System.Drawing.Point(3, 29)
        Me.txtFrom.Name = "txtFrom"
        Me.txtFrom.Size = New System.Drawing.Size(258, 20)
        Me.txtFrom.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label1.Location = New System.Drawing.Point(3, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(76, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Replace This :"
        '
        'chkReplaceText
        '
        Me.chkReplaceText.AutoSize = True
        Me.chkReplaceText.Dock = System.Windows.Forms.DockStyle.Top
        Me.chkReplaceText.Location = New System.Drawing.Point(3, 70)
        Me.chkReplaceText.Name = "chkReplaceText"
        Me.chkReplaceText.Size = New System.Drawing.Size(264, 17)
        Me.chkReplaceText.TabIndex = 3
        Me.chkReplaceText.Text = "Replace Text"
        Me.chkReplaceText.UseVisualStyleBackColor = True
        '
        'chkChangeExt
        '
        Me.chkChangeExt.AutoSize = True
        Me.chkChangeExt.Dock = System.Windows.Forms.DockStyle.Top
        Me.chkChangeExt.Location = New System.Drawing.Point(3, 3)
        Me.chkChangeExt.Name = "chkChangeExt"
        Me.chkChangeExt.Size = New System.Drawing.Size(264, 17)
        Me.chkChangeExt.TabIndex = 2
        Me.chkChangeExt.Text = "Change Extension"
        Me.chkChangeExt.UseVisualStyleBackColor = True
        '
        'tsProgress
        '
        Me.tsProgress.Name = "tsProgress"
        Me.tsProgress.Size = New System.Drawing.Size(100, 16)
        '
        'tsStatus
        '
        Me.tsStatus.Name = "tsStatus"
        Me.tsStatus.Size = New System.Drawing.Size(734, 17)
        Me.tsStatus.Spring = True
        Me.tsStatus.Text = "ToolStripStatusLabel1"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsStatus, Me.tsProgress})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 525)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(851, 22)
        Me.StatusStrip1.TabIndex = 3
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'txtNewExt
        '
        Me.txtNewExt.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtNewExt.Location = New System.Drawing.Point(3, 16)
        Me.txtNewExt.MaxLength = 3
        Me.txtNewExt.Name = "txtNewExt"
        Me.txtNewExt.Size = New System.Drawing.Size(258, 20)
        Me.txtNewExt.TabIndex = 1
        '
        'grpChangeExt
        '
        Me.grpChangeExt.Controls.Add(Me.txtNewExt)
        Me.grpChangeExt.Dock = System.Windows.Forms.DockStyle.Top
        Me.grpChangeExt.Enabled = False
        Me.grpChangeExt.Location = New System.Drawing.Point(3, 20)
        Me.grpChangeExt.Name = "grpChangeExt"
        Me.grpChangeExt.Size = New System.Drawing.Size(264, 50)
        Me.grpChangeExt.TabIndex = 4
        Me.grpChangeExt.TabStop = False
        '
        'btnRename
        '
        Me.btnRename.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.btnRename.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRename.Location = New System.Drawing.Point(3, 447)
        Me.btnRename.Name = "btnRename"
        Me.btnRename.Size = New System.Drawing.Size(264, 37)
        Me.btnRename.TabIndex = 0
        Me.btnRename.Text = "Rename File(s)"
        Me.btnRename.UseVisualStyleBackColor = True
        '
        'chkChangeFile
        '
        Me.chkChangeFile.Frozen = True
        Me.chkChangeFile.HeaderText = ""
        Me.chkChangeFile.Name = "chkChangeFile"
        Me.chkChangeFile.Width = 50
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.chkChangeFile})
        Me.DataGridView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridView1.Location = New System.Drawing.Point(3, 3)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.RowHeadersVisible = False
        Me.DataGridView1.Size = New System.Drawing.Size(571, 481)
        Me.DataGridView1.TabIndex = 0
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 38)
        Me.SplitContainer3.Name = "SplitContainer3"
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.DataGridView1)
        Me.SplitContainer3.Panel1.Padding = New System.Windows.Forms.Padding(3)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.grpReplaceText)
        Me.SplitContainer3.Panel2.Controls.Add(Me.chkReplaceText)
        Me.SplitContainer3.Panel2.Controls.Add(Me.grpChangeExt)
        Me.SplitContainer3.Panel2.Controls.Add(Me.chkChangeExt)
        Me.SplitContainer3.Panel2.Controls.Add(Me.btnRename)
        Me.SplitContainer3.Panel2.Padding = New System.Windows.Forms.Padding(3)
        Me.SplitContainer3.Size = New System.Drawing.Size(851, 487)
        Me.SplitContainer3.SplitterDistance = 577
        Me.SplitContainer3.TabIndex = 4
        '
        'txtFolder
        '
        Me.txtFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFolder.Location = New System.Drawing.Point(3, 14)
        Me.txtFolder.Name = "txtFolder"
        Me.txtFolder.Size = New System.Drawing.Size(804, 20)
        Me.txtFolder.TabIndex = 0
        '
        'btnSHowFolders
        '
        Me.btnSHowFolders.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.btnSHowFolders.Location = New System.Drawing.Point(813, 10)
        Me.btnSHowFolders.Name = "btnSHowFolders"
        Me.btnSHowFolders.Size = New System.Drawing.Size(32, 26)
        Me.btnSHowFolders.TabIndex = 1
        Me.btnSHowFolders.Text = "..."
        Me.btnSHowFolders.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.txtFolder)
        Me.GroupBox1.Controls.Add(Me.btnSHowFolders)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(851, 38)
        Me.GroupBox1.TabIndex = 5
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Select Folder"
        '
        'frmRename
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(851, 547)
        Me.Controls.Add(Me.SplitContainer3)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "frmRename"
        Me.Text = "frmRename"
        Me.grpReplaceText.ResumeLayout(False)
        Me.grpReplaceText.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.grpChangeExt.ResumeLayout(False)
        Me.grpChangeExt.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        Me.SplitContainer3.Panel2.PerformLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents grpReplaceText As GroupBox
    Friend WithEvents txtTo As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents txtFrom As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents chkReplaceText As CheckBox
    Friend WithEvents chkChangeExt As CheckBox
    Friend WithEvents tsProgress As ToolStripProgressBar
    Friend WithEvents tsStatus As ToolStripStatusLabel
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents FolderBrowserDialog1 As FolderBrowserDialog
    Friend WithEvents txtNewExt As TextBox
    Friend WithEvents grpChangeExt As GroupBox
    Friend WithEvents btnRename As Button
    Friend WithEvents chkChangeFile As DataGridViewCheckBoxColumn
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents SplitContainer3 As SplitContainer
    Friend WithEvents txtFolder As TextBox
    Friend WithEvents btnSHowFolders As Button
    Friend WithEvents GroupBox1 As GroupBox
End Class
