Imports System.Data
Imports System.Data.SqlClient

Imports Microsoft.VisualBasic
Imports System.Text
Imports System
Imports System.Configuration
Imports System.Collections.Generic

''' <summary>
''' Summary description for DataHelper
''' </summary>
Public Class DataHelper


#Region "Methods"
    Public Shared Sub ExecuteNonQuery(cmdText As String, cmdType As CommandType, params As SqlParameter(), connectionString As String)
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

                cmd.Connection = New SqlConnection(connectionString)
                If cmd.Connection.State <> ConnectionState.Open Then
                    cmd.Connection.Open()
                End If
                cmd.ExecuteNonQuery()

                cmd.Parameters.Clear()
                cmd.Connection.Close()
            End Using
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Public Shared Function ExecuteScalar(cmdText As String, cmdType As CommandType, params As SqlParameter(), connectionString As String) As Object
        Dim obj As Object
        Using cmd As New SqlCommand()
            Try
                cmd.CommandType = cmdType
                cmd.CommandText = cmdText
                If Not IsNothing(params) Then
                    Dim tempparams As SqlParameter() = TryCast(params.Clone(), SqlParameter())
                    cmd.Parameters.AddRange(params)
                End If

                cmd.Connection = New SqlConnection(connectionString)
                If cmd.Connection.State <> ConnectionState.Open Then
                    cmd.Connection.Open()
                End If
                obj = cmd.ExecuteScalar()
            Catch ex As Exception
                Throw ex
            Finally
                cmd.Connection.Close()
            End Try
        End Using

        Return obj
    End Function

    Public Shared Function GetDataSet(cmdText As String, cmdType As CommandType, params As SqlParameter(), connectionString As String, Optional dsName As String = Nothing) As DataSet
        Dim ds As New DataSet()
        Try
            Using cmd As New SqlCommand()
                cmd.Connection = New SqlConnection(connectionString)
                cmd.CommandType = cmdType
                cmd.CommandText = cmdText
                If Not IsNothing(params) Then
                    Dim tempparams As SqlParameter() = TryCast(params.Clone(), SqlParameter())
                    cmd.Parameters.AddRange(params)
                End If
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(ds, dsName)
                    cmd.Dispose()
                End Using
                cmd.Connection.Close()
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return ds
    End Function

    Public Shared Function GetDataTable(cmdText As String, cmdType As CommandType, params As SqlParameter(), connectionString As String) As DataTable
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
                cmd.Connection = New SqlConnection(connectionString)

                If cmd.Connection.State <> ConnectionState.Open Then
                    cmd.Connection.Open()
                End If

                dtTemp.Load(cmd.ExecuteReader())
                cmd.Parameters.Clear()
                cmd.Connection.Close()
            End Using

            Return dtTemp
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Shared Sub HydrateObject(ByRef destinationObject As Object, rowOfData As DataRow)
        For Each col As DataColumn In rowOfData.Table.Columns
            For Each p As System.Reflection.PropertyInfo In destinationObject.GetType().GetProperties(Reflection.BindingFlags.Public + Reflection.BindingFlags.Instance)
                If Not IsNothing(p) Then
                    Try
                        If p.CanRead And p.CanWrite Then
                            'p.Name, p.GetValue(bi, Nothing)
                            If p.Name.ToLower = col.ColumnName.ToLower Then
                                Try
                                    If IsDBNull(rowOfData(col.ColumnName)) Then
                                        p.SetValue(destinationObject, Convert.ChangeType("", p.PropertyType), Nothing)
                                    Else
                                        p.SetValue(destinationObject, Convert.ChangeType(rowOfData(col.ColumnName), p.PropertyType), Nothing)
                                    End If
                                Catch ic As InvalidCastException
                                    Dim val As Object = [Enum].Parse(p.PropertyType, rowOfData(col.ColumnName).ToString)
                                    p.SetValue(destinationObject, val, Nothing)
                                Catch ex As Exception

                                End Try


                            End If
                        End If
                    Catch ex As Exception
                        Continue For
                    End Try

                End If
            Next
        Next
    End Sub

    

#End Region 'Methods

End Class