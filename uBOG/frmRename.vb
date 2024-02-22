Imports System.IO
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq

Public Class frmRename

#Region "Fields"

    Private _dt As DataTable = Nothing

#End Region 'Fields

#Region "Methods"

    Public Sub InitForm()
        mdiMain.updateStatus("Waiting on Rename Operation...")
        _dt = New DataTable
        _dt.Columns.Add("File_Name")
        _dt.Columns.Add("File_Path")
        _dt.Columns.Add("File_Type")
        '_dt.Columns.Add("File_Size")
        _dt.Columns.Add("Create_Date")
        tsStatus.Text = "Ready!!!"
        tsProgress.Value = 0

        'Get a list of drives

    End Sub

    Private Sub btnRename_Click(sender As Object, e As EventArgs) Handles btnRename.Click
        Dim iCnt As Integer = 0
        For Each row As Windows.Forms.DataGridViewRow In DataGridView1.Rows
            Dim cellChecked As Boolean = Convert.ToBoolean(row.Cells(0).EditedFormattedValue.ToString)
            If cellChecked = True Then
                iCnt += 1
            End If
        Next
        tsProgress.Maximum = iCnt
        tsStatus.Text = String.Format("Renaming {0} File(s)", iCnt)

        Dim dicNames As New Dictionary(Of String, String)
        For Each row As Windows.Forms.DataGridViewRow In DataGridView1.Rows
            Dim cellChecked As Boolean = Convert.ToBoolean(row.Cells(0).EditedFormattedValue.ToString)
            If cellChecked = True Then
                Dim fileName As String = row.Cells(1).EditedFormattedValue.ToString
                Dim filePath As String = row.Cells(2).EditedFormattedValue.ToString
                Dim newFileName As String = fileName
                If Not String.IsNullOrEmpty(fileName) Then
                    Dim bGotRenameAction As Boolean = False

                    If chkChangeExt.Checked Then
                        newFileName = String.Format("{0}.{1}", Path.GetFileNameWithoutExtension(fileName), txtNewExt.Text.ToLower)
                        bGotRenameAction = True
                    End If
                    If chkReplaceText.Checked Then
                        newFileName = newFileName.ToLower.Replace(txtFrom.Text.ToLower, txtTo.Text)
                        bGotRenameAction = True
                    End If
                    Try
                        If bGotRenameAction = True Then
                            dicNames.Add(fileName, newFileName)
                            My.Computer.FileSystem.RenameFile(filePath, newFileName)
                        End If
                        If iCnt = 1 Then
                            MsgBox(String.Format("File name changed from {0} to {1}", filePath, newFileName))
                        End If

                    Catch ex As Exception
                        MsgBox(ex.Message.ToString)
                    End Try

                    If tsProgress.Value + 1 <= tsProgress.Maximum Then
                        tsProgress.Value += 1
                    End If
                End If
            End If
        Next
        If dicNames.Count > 1 Then
            MsgBox(String.Format("{0} successful name change(s)!!", dicNames.Count))
        End If
        getFileData(txtFolder.Text)
        tsStatus.Text = "Done!!!"
    End Sub

    Private Sub btnSHowFolders_Click(sender As Object, e As EventArgs) Handles btnSHowFolders.Click
        If FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then

            _dt.Rows.Clear()

            Dim foldPath As String = FolderBrowserDialog1.SelectedPath
            txtFolder.Text = foldPath

            getFileData(foldPath)

        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        InitForm()
    End Sub

    Private Sub getFileData(ByVal foldPath As String)
        If Not String.IsNullOrEmpty(foldPath) Then
            _dt.Rows.Clear()

            Try
                Dim sFiles As String() = Directory.GetFiles(foldPath)

                For i As Integer = 0 To sFiles.Count - 1
                    Dim f As FileInfo = New FileInfo(sFiles(i))
                    Dim f1 As New FileInfo(sFiles(i))
                    Dim dr As DataRow = _dt.NewRow()
                    'Get File name of each file name
                    dr("File_Name") = f1.Name
                    dr("File_Path") = sFiles(i)
                    'Get File Type/Extension of each file
                    dr("File_Type") = f1.Extension
                    'Get File Size of each file in KB format
                    'dr("File_Size") = (f.Length / 1024).ToString()
                    'Get file Create Date and Time
                    dr("Create_Date") = f1.CreationTime.Date.ToString("MM/dd/yyyy")
                    'Insert collected file details in Datatable
                    _dt.Rows.Add(dr)
                Next
                If _dt.Rows.Count > 0 Then
                    'Finally Add DataTable into DataGridView
                    DataGridView1.DataSource = _dt
                    For Each dr As DataGridViewRow In DataGridView1.Rows
                        dr.Cells(0).Value = True
                    Next
                End If

            Catch aex As UnauthorizedAccessException

            End Try

        End If
    End Sub

    Private Sub ChkReplaceText_CheckedChanged(sender As Object, e As EventArgs) Handles chkReplaceText.CheckedChanged
        If chkReplaceText.Checked Then
            chkChangeExt.Checked = False
            grpReplaceText.Enabled = True
            grpChangeExt.Enabled = False
        End If
    End Sub

    Private Sub ChkChangeExt_CheckedChanged(sender As Object, e As EventArgs) Handles chkChangeExt.CheckedChanged
        If chkChangeExt.Checked Then
            chkReplaceText.Checked = False
            grpReplaceText.Enabled = False
            grpChangeExt.Enabled = True
        End If
    End Sub


#End Region 'Methods

End Class