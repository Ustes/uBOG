Imports Microsoft.Win32

Public Class frmBrowser
    Public Property WebContent() As String
        Get
            Return WebBrowser1.DocumentText
        End Get
        Set(value As String)
            WebBrowser1.DocumentText = value
        End Set
    End Property


    Private Sub frmBrowser_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

    End Sub

End Class