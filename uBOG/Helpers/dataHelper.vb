Imports System.Data
Imports System.Data.SqlClient

Imports Microsoft.VisualBasic
Imports System.Configuration
Imports System.Text
Imports System.Collections.Generic

''' <summary>
''' Summary description for DataHelper
''' </summary>
Public Class DataHelper

#Region "Enumerations"

#End Region 'Enumerations

#Region "Methods"
    Public Shared Sub ExecuteNonQuery(cmdText As String, cmdType As CommandType, Optional params As SqlParameter() = Nothing)
        Try
            Using cmd As New SqlCommand()
                cmd.CommandType = cmdType
                cmd.CommandText = cmdText
                cmd.CommandTimeout = 360
                '6min
                If Not IsNothing(params) Then
                    Dim tempparams As SqlParameter() = TryCast(params.Clone(), SqlParameter())
                    cmd.Parameters.AddRange(params)
                End If
                Dim connectionString As String = getConnectionString()
                cmd.Connection = New SqlConnection(connectionString)
                If cmd.Connection.State <> ConnectionState.Open Then
                    cmd.Connection.Open()
                End If
                cmd.ExecuteNonQuery()

                cmd.Parameters.Clear()
            End Using
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Public Shared Function ExecuteScalar(cmdText As String, cmdType As CommandType, Optional params As SqlParameter() = Nothing) As Object
        Dim obj As Object
        Using cmd As New SqlCommand()
            Try
                cmd.CommandType = cmdType
                cmd.CommandText = cmdText
                If Not IsNothing(params) Then
                    Dim tempparams As SqlParameter() = TryCast(params.Clone(), SqlParameter())
                    cmd.Parameters.AddRange(params)
                End If
                Dim connectionString As String = getConnectionString()
                cmd.Connection = New SqlConnection(connectionString)
                If cmd.Connection.State <> ConnectionState.Open Then
                    cmd.Connection.Open()
                End If
                obj = cmd.ExecuteScalar()
            Catch ex As Exception
                Throw
            End Try
        End Using

        Return obj
    End Function

    Public Shared Function GetDataSet(cmdText As String, cmdType As CommandType, Optional params As SqlParameter() = Nothing) As DataSet
        Dim ds As New DataSet()
        Try
            Using cmd As New SqlCommand()
                Dim connectionString As String = getConnectionString()
                cmd.Connection = New SqlConnection(connectionString)
                cmd.CommandType = cmdType
                cmd.CommandText = cmdText
                If Not IsNothing(params) Then
                    Dim tempparams As SqlParameter() = TryCast(params.Clone(), SqlParameter())
                    cmd.Parameters.AddRange(params)
                End If
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(ds)
                    cmd.Dispose()
                End Using
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function

    Public Shared Function GetDataTable(cmdText As String, cmdType As CommandType, Optional params As SqlParameter() = Nothing) As DataTable
        Dim dtTemp As New DataTable()
        Try
            Using cmd As New SqlCommand()
                cmd.CommandType = cmdType
                cmd.CommandText = cmdText
                cmd.CommandTimeout = 360
                '6min
                If Not IsNothing(params) Then
                    Dim tempparams As SqlParameter() = TryCast(params.Clone(), SqlParameter())

                    cmd.Parameters.AddRange(params)
                End If
                Dim connectionString As String = getConnectionString()
                cmd.Connection = New SqlConnection(connectionString)

                If cmd.Connection.State <> ConnectionState.Open Then
                    cmd.Connection.Open()
                End If

                dtTemp.Load(cmd.ExecuteReader())
                cmd.Parameters.Clear()
            End Using

            Return dtTemp
        Catch ex As Exception
            Throw
        End Try
    End Function

    Public Shared Sub HydrateObject(ByRef destinationObject As Object, rowOfData As DataRow)
        For Each col As DataColumn In rowOfData.Table.Columns
            For Each p As System.Reflection.PropertyInfo In destinationObject.GetType().GetProperties()
                If p.CanRead Then
                    'p.Name, p.GetValue(bi, Nothing)
                    If p.Name.ToLower = col.ColumnName.ToLower Then
                        p.SetValue(destinationObject, Convert.ChangeType(rowOfData(col.ColumnName), p.PropertyType), Nothing)
                    End If
                End If
            Next

        Next
    End Sub

    Private Shared Function getConnectionString() As String

        Return mdiMain.UtilityConnectionString
    End Function

#End Region 'Methods

End Class