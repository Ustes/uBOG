Imports System.Collections.Generic

Public Class frmResults
    Public Structure resultObject
        Public ResultName As String
        Public ResultText As String
    End Structure
    Private _FormResults As List(Of resultObject)

    Public Property FormResults() As List(Of resultObject)
        Get
           
            Return _FormResults
        End Get
        Set(ByVal value As List(Of resultObject))
            _FormResults = value
        End Set
    End Property
    

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub frmResults_Load(sender As Object, e As EventArgs) Handles Me.Load
        tcResults.TabPages.Clear()

        If Not IsNothing(FormResults) Then
            For Each fr As resultObject In FormResults
                Dim tp As New TabPage(fr.ResultName)

                Dim txt As New TextBox
                txt.Multiline = True
                txt.Dock = DockStyle.Fill
                txt.ScrollBars = ScrollBars.Vertical
                txt.Text = fr.ResultText
                tp.Controls.Add(txt)
                tcResults.TabPages.Add(tp)

            Next
        End If

     

    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _FormResults = New List(Of resultObject)
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        'To save the generated class/function/procedure to a file.

        'Show file save dialog

        If dlgFileSave.ShowDialog = Windows.Forms.DialogResult.OK Then
            Try

                Using fsMyStream As New System.IO.StreamWriter(dlgFileSave.FileName)
                    For Each tp As TabPage In tcResults.TabPages
                        Dim txt As TextBox = TryCast(tp.Controls(0), TextBox)
                        fsMyStream.WriteLine(tp.Text)
                        fsMyStream.Write(txt.Text & Environment.NewLine)

                    Next

                    MsgBox("File saved successfully")
                End Using

            Catch ex As IO.PathTooLongException
                MsgBox(ex.Message)
            Catch ex As IO.FileNotFoundException
                MsgBox(ex.Message)
            Catch ex As Exception
                MsgBox("Could not save the generated function or procedure" & vbCrLf & vbCrLf & ex.Message)
            End Try
        End If

      

    End Sub
End Class