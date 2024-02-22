Imports System.Windows.Forms

Imports Microsoft.Data.ConnectionUI

Public Class mdiMain

#Region "Fields"

    Public UtilityConnectionString As String

    Private mruMAX As Integer = 5

#End Region 'Fields

#Region "Methods"

    Private Sub ConnectToDatabaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConnectToDatabaseToolStripMenuItem.Click
        Dim objDataConnectionDialog As DataConnectionDialog
        objDataConnectionDialog = New DataConnectionDialog
        DataSource.AddStandardDataSources(objDataConnectionDialog)

        If DataConnectionDialog.Show(objDataConnectionDialog) = Windows.Forms.DialogResult.OK Then
            UtilityConnectionString = objDataConnectionDialog.ConnectionString
            objDataConnectionDialog.Dispose()
            ' Call sub to add this file as an MRU

            mru_addRecentItem(UtilityConnectionString)

            If mnuMRU.Visible = False Then mnuMRU.Visible = True

            Dim frmDB As New frmGenerate
            frmDB.MdiParent = Me
            frmDB.Show()
        End If

        objDataConnectionDialog.Dispose()
    End Sub

    Private Sub ConnectToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles ConnectToolStripMenuItem.Click
    End Sub

    Private Sub ConnectWithConnectionstringToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConnectWithConnectionstringToolStripMenuItem.Click
        Dim frmConnect As New frmConnection
        'frmConnect.MdiParent = Me
        If frmConnect.ShowDialog(Me) = Windows.Forms.DialogResult.OK AndAlso Not String.IsNullOrEmpty(UtilityConnectionString) Then

            Dim frmDB As New frmGenerate
            frmDB.MdiParent = Me
            frmDB.Show()
        End If
    End Sub

    Private Sub HelpToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles HelpToolStripMenuItem.Click
        Dim frmDB As New frmHelp
        frmDB.MdiParent = Me
        frmDB.Show()
    End Sub

    Private Sub mdiMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        mru_saveRecentItemList()
    End Sub

    Private Sub mdiMain_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        mru_loadRecentItemList()

        GenerateObjectsToolStripMenuItem1.Visible = False
        ExecuteBulkScriptsToolStripMenuItem.Visible = False

    End Sub

    Private Sub mnuMRU_DropDownItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles mnuMRU.DropDownItemClicked
        Dim itm As String() = e.ClickedItem.Text.Split(New Char() {":"}, StringSplitOptions.RemoveEmptyEntries)
        If itm.Length > 1 Then
            If Not String.IsNullOrEmpty(itm(1)) Then
                UtilityConnectionString = itm(1)

                'Dim frmDB As New frmGenerate
                'frmDB.MdiParent = Me
                'frmDB.Show()


                GenerateObjectsToolStripMenuItem1.Visible = True
                ExecuteBulkScriptsToolStripMenuItem.Visible = True

            End If
        Else
            mnuMRU.DropDownItems.Clear()
            mnuMRU.Visible = False
        End If
        
    End Sub

    Public Sub mru_addRecentItem(recentItem As String)
        If Not mnuMRU.DropDownItems.ContainsKey(recentItem) AndAlso mnuMRU.DropDownItems.Count < mruMAX Then
            mnuMRU.DropDownItems.Add(String.Format("{0} : {1}", mnuMRU.DropDownItems.Count + 1, recentItem))
        End If
    End Sub

    Private Sub mru_loadRecentItemList()
        Dim mruCount As Integer = GetSetting(My.Application.Info.ProductName, "MruList", "MRUCount", "0")

        If mruCount > 0 Then
            For i As Integer = 1 To mruCount
                Dim conn As String = GetSetting(My.Application.Info.ProductName, "MruList", "Connection" & i, "").Trim
                If Not String.IsNullOrEmpty(conn.Trim) Then
                    mnuMRU.DropDownItems.Add(String.Format("{0} : {1}", i, conn))
                End If

            Next
            mnuMRU.DropDownItems.Add("Clear List")
        End If

        If mnuMRU.DropDownItems.Count > 0 Then
            mnuMRU.Visible = True
        Else
            mnuMRU.Visible = False
        End If
    End Sub

    Private Sub mru_saveRecentItemList()
        ' Remove previous entries.
        If GetSetting(My.Application.Info.ProductName, "MruList", "Connection1", "").Length > 0 Then
            DeleteSetting(My.Application.Info.ProductName, "MruList")
        End If

        ' Make the new entries.
        For i As Integer = 0 To mnuMRU.DropDownItems.Count - 1
            Dim itm As String() = mnuMRU.DropDownItems.Item(i).Text.Split(New Char() {":"}, StringSplitOptions.RemoveEmptyEntries)
            If itm.Length > 1 Then
                SaveSetting(My.Application.Info.ProductName, "MruList", "Connection" & i + 1, itm(1))
            End If

        Next i
        SaveSetting(My.Application.Info.ProductName, "MruList", "MRUCount", mnuMRU.DropDownItems.Count)
    End Sub

    Private Sub QuitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles QuitToolStripMenuItem.Click
        mru_saveRecentItemList()
        End
    End Sub



    Private Sub RunScriptsToolStripMenuItem_Click(sender As Object, e As EventArgs)

    End Sub

    Public Sub updateStatus(msg As String)
        mainStatus.Text = msg
    End Sub

    Private Sub BulkRenameToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BulkRenameToolStripMenuItem.Click

        Dim f As New frmRename
        f.MdiParent = Me
        f.Show()
    End Sub

    Private Sub GenerateObjectsToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles GenerateObjectsToolStripMenuItem1.Click
        Dim frmDB As New frmGenerate
        frmDB.MdiParent = Me
        frmDB.Show()
    End Sub

    Private Sub ExecuteBulkScriptsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExecuteBulkScriptsToolStripMenuItem.Click
        Dim f As New frmscripts
        f.MdiParent = Me

        If String.IsNullOrEmpty(UtilityConnectionString) Then
            Dim objDataConnectionDialog As DataConnectionDialog
            objDataConnectionDialog = New DataConnectionDialog
            DataSource.AddStandardDataSources(objDataConnectionDialog)

            If DataConnectionDialog.Show(objDataConnectionDialog) = Windows.Forms.DialogResult.OK Then
                UtilityConnectionString = objDataConnectionDialog.ConnectionString
                objDataConnectionDialog.Dispose()
                ' Call sub to add this file as an MRU

                mru_addRecentItem(UtilityConnectionString)

                If mnuMRU.Visible = False Then mnuMRU.Visible = True



                f.Show()
            End If

            objDataConnectionDialog.Dispose()
        Else

            f.Show()
        End If


    End Sub
#End Region 'Methods
End Class