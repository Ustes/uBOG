Imports System.Data.SqlClient
Imports System.Security.Principal.WindowsIdentity
Public Class frmConnection

    Private Sub frmConnection_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

       
    End Sub

  
  
    Private Sub btnConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConnect.Click
        Cursor.Current = Cursors.WaitCursor

        Try
    
                    mdiMain.UtilityConnectionString = txtConnString.Text
           
           
            If Not String.IsNullOrEmpty(mdiMain.UtilityConnectionString) Then
                DialogResult = Windows.Forms.DialogResult.OK
                Me.Close()
            Else
                DialogResult = Windows.Forms.DialogResult.Cancel
            End If

        Catch ex As SqlException
            MsgBox(ex.Message)
        Catch ex As Exception
            'Error in Connecting to the databse
            MsgBox(ex.ToString)
        End Try
        Cursor.Current = Cursors.Default
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub
End Class