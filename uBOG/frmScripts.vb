Imports System.IO
Imports System.Data.SqlClient
Imports Microsoft.SqlServer.Server
Imports Microsoft.SqlServer.Management.Smo
Imports Microsoft.SqlServer.Management.Common

Public Class frmscripts
    Public folderPath As String
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        If Not String.IsNullOrEmpty(txtpath.Text) Then
            FolderBrowserDialog1.SelectedPath = txtpath.Text
        End If

        If FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            loadFiles(FolderBrowserDialog1.SelectedPath)
        End If


    End Sub
    Private Sub loadFiles(selectedFolder As String)
        folderPath = selectedFolder
        txtpath.Text = folderPath
        Dim files As String() = IO.Directory.GetFiles(folderPath)
        Dim lst As New System.Collections.Generic.List(Of CheckBoxItem)
        For Each s As String In files
            lst.Add(New CheckBoxItem() With {.Name = IO.Path.GetFileName(s), .Value = s})
        Next
        cblFiles.DataSource = lst
        cblFiles.DisplayMember = "Name"
        cblFiles.ValueMember = "Value"

        For i As Integer = 0 To cblFiles.Items.Count - 1
            cblFiles.SetItemChecked(i, True)
        Next

        writeResult(String.Format("Found {0} file(s) in {1}...", files.Length, folderPath))
    End Sub
    Public Sub writeResult(msg As String)
        txtResults.Text &= String.Format("{0} : {1}", Now, msg) & vbCrLf
    End Sub

    Private Sub btnExecute_Click(sender As Object, e As EventArgs) Handles btnExecute.Click

        Using conn As New SqlConnection(mdiMain.UtilityConnectionString)
            writeResult(String.Format("Prcessing {0} file(s).", cblFiles.CheckedItems.Count))
            For Each s As CheckBoxItem In cblFiles.CheckedItems
                Dim file As New FileInfo(s.Value)
                Dim script As String = file.OpenText().ReadToEnd()
                Dim server As New Server(New ServerConnection(conn))
                Try
                    writeResult(String.Format("Executing {0}...", s.Name))
                    server.ConnectionContext.ExecuteNonQuery(script)
                    writeResult(String.Format("Executed {0}...", s.Name))
                Catch ex As Exception
                    Dim errMsg As String = ex.Message & vbCrLf
                    If Not IsNothing(ex.InnerException) Then
                        errMsg = vbCrLf & ex.InnerException.Message.ToString & vbCrLf
                    End If
                    writeResult(String.Format("ERROR: {0} - {1}", s.Name, errMsg))
                End Try
            Next
            writeResult("Finished!!!")

        End Using

    End Sub

    Private Sub frmscripts_Load(sender As Object, e As EventArgs) Handles Me.Load
        writeResult(mdiMain.UtilityConnectionString)

    End Sub

    Public Class CheckBoxItem
        Public Property Name As String
        Public Property Value As String
        Public Property IsChecked As Boolean
        Sub New()
            IsChecked = True
        End Sub
    End Class
End Class