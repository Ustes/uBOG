﻿Imports System.Collections.Generic
Imports System.Text

Public Class bogHelper

    Public Shared Function GenerateUpdate(tableName As String, tableGrid As DataGridView, columnData As DataSet) As String
        Dim strBld As New StringBuilder
        Dim IntJ As Integer
        Dim intI As Integer
        Dim blnNotPrimary As Boolean
        Dim strColumnName As String

        'Start of comments
        strBld.AppendFormat("--Created on {0}" & vbCrLf, Now.ToString)
        strBld.Append("--Autogenerated by UseThisBOG " & vbCrLf & vbCrLf)
        'End of comments

        strBld.AppendFormat("CREATE PROCEDURE [stp_{0}_Update]" & vbCrLf, tableName)
        strBld.Append("(" & vbCrLf)
        'Variable definitions
        For Each row As DataGridViewRow In tableGrid.Rows
            If row.Cells(0).Value = True Then
                Dim dbType As String = GetVBDataType(row.Cells("Type_Name").Value)
                Dim cname As String = row.Cells("Column_Name").Value

                strBld.Append("@" & cname & " ")
                strBld.Append(Space(35 - Len(cname)))
                Select Case tableGrid.Item("Type_Name", IntJ).Value.ToString
                    Case "int", "ntext", "text", "datetime", "date", "smallint", "decimal"
                        strBld.Append(cname & "," & vbCrLf)
                    Case Else
                        strBld.Append(tableGrid.Item("Type_Name", IntJ).Value & "(")
                        strBld.Append(tableGrid.Item("Precision", IntJ).Value & ")," & vbCrLf)
                End Select

            End If
        Next

        'Remove last ","
        strBld.Remove(strBld.Length - 3, 2)
        strBld.Append(vbCrLf & ")")
        strBld.Append(vbCrLf)
        strBld.Append(vbCrLf)
        strBld.Append("AS")
        strBld.Append(vbCrLf)
        strBld.Append("BEGIN")
        strBld.Append(vbCrLf)

        strBld.Append(Space(7) & "UPDATE " & tableName & vbCrLf)
        strBld.Append(Space(7) & "SET" & vbCrLf)

        Dim lstParams As New List(Of String)
        For IntJ = 0 To tableGrid.Rows.Count - 1

            'Check if selected column is primary key
            If tableGrid.Rows(IntJ).Cells(0).Value Then
                blnNotPrimary = True
                strColumnName = tableGrid.Item("Column_Name", IntJ).Value
                For intI = 0 To columnData.Tables("Pkeys").Rows.Count - 1
                    If strColumnName = columnData.Tables("Pkeys").Rows(intI).Item("COLUMN_NAME") Then
                        blnNotPrimary = False
                    End If
                Next
                If blnNotPrimary Then
                    'If IntJ > 0 Then strBld.Append("," & vbCrLf)
                    'strBld.Append(Space(10) & strColumnName & " = @" & strColumnName)
                    lstParams.Add(Space(10) & strColumnName & " = @" & strColumnName & vbCrLf)
                End If
            End If

        Next

        'Remove last ","
        'strBld.Remove(strBld.Length - 3, 2)
        strBld.Append(Join(lstParams.ToArray, ","))
        strBld.Append(vbCrLf)
        'Adding where condition using Primary keys
        'by default all the primary keys are used in where condition
        If columnData.Tables("Pkeys").Rows.Count > 0 Then

            strBld.Append(Space(7) & "WHERE" & vbCrLf)

            For IntJ = 0 To columnData.Tables("Pkeys").Rows.Count - 2
                strColumnName = columnData.Tables("Pkeys").Rows(IntJ).Item("COLUMN_NAME")
                strBld.Append(Space(10) & strColumnName & " = " & "@" & strColumnName & " and " & vbCrLf)
            Next
            strColumnName = columnData.Tables("Pkeys").Rows(IntJ).Item("COLUMN_NAME")
            strBld.Append(Space(10) & strColumnName & " = " & "@" & strColumnName & vbCrLf)

        End If
        strBld.Append("END" & vbCrLf)

        'Body ends
        strBld.Append("Go")

        Return strBld.ToString
    End Function
    Public Shared Function GenerateEntity(tableName As String, tableGrid As DataGridView, Optional language As String = "VB", Optional bUseAutoProperty As Boolean = False, Optional bAddNotifyChangeEvent As Boolean = False, Optional bAddSerializable As Boolean = False) As String
        Dim strBuilder As New StringBuilder
        Dim strEntityName As String = tableName


        'validate entity name
        If strEntityName = "" Then
            MsgBox("Please enter valid name for Entity")
            Return String.Empty
        End If

        Select Case language
            Case "C#"
                If bAddSerializable Then
                    strBuilder.AppendLine("[Serializable]")
                End If
                strBuilder.Append("public class " & strEntityName)
                If bAddNotifyChangeEvent Then
                    strBuilder.AppendLine(" : INotifyPropertyChanged")
                End If
                strBuilder.AppendLine()
                strBuilder.Append("{ " & vbCrLf)

                If bAddNotifyChangeEvent Then
                    strBuilder.AppendLine("public event PropertyChangedEventHandler PropertyChanged;")
                    strBuilder.AppendLine("private void NotifyPropertyChanged([CallerMemberName] string propertyName = """"){")
                    strBuilder.AppendLine(" if (PropertyChanged != null){")
                    strBuilder.AppendLine("     PropertyChanged(this, new PropertyChangedEventArgs(propertyName));")
                    strBuilder.AppendLine(" }")
                    strBuilder.AppendLine("}")
                End If

                'Build Properties
                Dim sbPrivs As New StringBuilder
                Dim sbProps As New StringBuilder
                For Each row As DataGridViewRow In tableGrid.Rows
                    If row.Cells(0).Value = True Then
                        Dim dbType As String = GetCSharpDataType(row.Cells("Type_Name").Value)
                        Dim strColumnName As String = row.Cells("Column_Name").Value

                        If bUseAutoProperty = False Then

                            sbPrivs.AppendFormat("private {1} _{0};" & vbCrLf, strColumnName, dbType)
                            sbProps.AppendFormat("public {1} {0}", strColumnName, dbType)
                            sbProps.Append("{" & vbCrLf)

                            sbProps.Append("get {")
                            sbProps.AppendFormat("return _{0};", strColumnName)
                            sbProps.Append("}" & vbCrLf)

                            sbProps.Append("set {")

                            If bAddNotifyChangeEvent = False Then
                                sbProps.AppendFormat("_{0} = value;", strColumnName)
                            Else
                                sbProps.AppendFormat("if (value != _{0})" & vbCrLf, strColumnName)
                                sbProps.AppendLine("{")
                                sbProps.AppendFormat("  _{0} = value;", strColumnName)
                                sbProps.AppendLine("    NotifyPropertyChanged();")
                                sbProps.AppendLine("}")
                            End If

                            sbProps.Append("} " & vbCrLf)

                            sbProps.Append("}")
                        Else
                            sbProps.AppendFormat("public {1} {0} ", strColumnName, dbType)
                            sbProps.Append("{ get; set; }")
                        End If

                        sbProps.Append(vbCrLf)
                        sbProps.Append(vbCrLf)
                    End If
                Next

                'strBuilder.Append("#region Properties" & vbCrLf)
                strBuilder.Append(sbPrivs.ToString)
                strBuilder.Append(vbCrLf)
                strBuilder.Append(sbProps.ToString)
                strBuilder.Append(vbCrLf)
                'strBuilder.Append("#endregion" & vbCrLf) 'Properties region ends
                strBuilder.Append("}" & vbCrLf)
            Case Else
                If bAddSerializable Then
                    strBuilder.AppendLine("<Serializable>")
                End If
                strBuilder.Append("Public class " & strEntityName & vbCrLf)
                If bAddNotifyChangeEvent Then
                    strBuilder.AppendLine("Implements INotifyPropertyChanged")

                    strBuilder.AppendLine("Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged")

                    strBuilder.AppendLine("Private Sub NotifyPropertyChanged(<CallerMemberName()> Optional ByVal propertyName As String = Nothing)")
                    strBuilder.AppendLine(" RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))")
                    strBuilder.AppendLine("End Sub")

                End If
                strBuilder.AppendLine()
                'Build Properties
                Dim sbPrivs As New StringBuilder
                Dim sbProps As New StringBuilder
                For Each row As DataGridViewRow In tableGrid.Rows
                    If row.Cells(0).Value = True Then
                        Dim dbType As String = GetVBDataType(row.Cells("Type_Name").Value)
                        Dim strColumnName As String = row.Cells("Column_Name").Value
                        If bUseAutoProperty = False Then
                            sbPrivs.AppendFormat("Private _{0} as {1}" & vbCrLf, strColumnName, dbType)
                            sbProps.AppendFormat("Public Property {0} () as {1}" & vbCrLf, strColumnName, dbType)
                            sbProps.Append("Get" & vbCrLf)
                            sbProps.Append("return _" & strColumnName & vbCrLf)
                            sbProps.Append("End Get" & vbCrLf)
                            sbProps.Append("Set(ByVal Value As " & dbType & ")" & vbCrLf)
                            If bAddNotifyChangeEvent = False Then
                                sbProps.Append("_" & strColumnName & " = Value" & vbCrLf)
                            Else
                                sbProps.AppendFormat("If Not (value = _{0}) Then" & vbCrLf, strColumnName)
                                sbProps.Append("_" & strColumnName & " = Value" & vbCrLf)
                                sbProps.Append("NotifyPropertyChanged" & vbCrLf)
                                sbProps.AppendLine("End If")
                            End If


                            sbProps.Append("end set" & vbCrLf)
                            sbProps.Append("end Property ")
                            sbProps.Append(vbCrLf)
                            sbProps.Append(vbCrLf)
                        Else
                            sbProps.AppendFormat("Public Property {0} As {1}" & vbCrLf, strColumnName, dbType)
                        End If

                    End If
                Next

                strBuilder.Append("#Region "" Properties """ & vbCrLf)
                strBuilder.Append(sbPrivs.ToString)
                strBuilder.Append(vbCrLf)
                strBuilder.Append(sbProps.ToString)
                strBuilder.Append(vbCrLf)
                strBuilder.Append("# end Region " & vbCrLf) 'Properties region ends

                strBuilder.Append(vbCrLf & "end class") ' Class building Ends
        End Select


        Return strBuilder.ToString


    End Function
    Public Shared Function GenerateInsert(tableName As String, tableGrid As DataGridView, columnData As DataSet) As String
        Dim strBld As New StringBuilder
        Dim IntJ As Integer
        Dim intI As Integer
        Dim blnNotPrimary As Boolean
        Dim strColumnName As String

        Dim procName As String = String.Format("stp_{0}_InsertUpdate", tableName)

        'Start of comments
        strBld.AppendFormat("--{1} Created on {0}" & vbCrLf, Now.ToString, procName)
        strBld.Append("--Autogenerated by UseThisBOG " & vbCrLf & vbCrLf)
        'End of comments


        strBld.Append("-- Drop stored procedure if it already exists" & vbCrLf)
        strBld.AppendFormat("If EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE SPECIFIC_SCHEMA = N'dbo' AND SPECIFIC_NAME = N'{0}' )" & vbCrLf, procName)
        strBld.AppendFormat(vbTab & "DROP PROCEDURE dbo.{0}" & vbCrLf, procName)
        strBld.Append("GO" & vbCrLf)



        strBld.AppendFormat("CREATE PROCEDURE [{0}]" & vbCrLf, procName) 'Stored procedure name
        strBld.Append("(" & vbCrLf)
        'Building Parameter list
        For IntJ = 0 To tableGrid.Rows.Count - 1
            If tableGrid.Rows(IntJ).Cells(0).Value Then
                If IntJ <> 0 Then
                    strBld.Append("," & vbCrLf)
                End If

                Dim colName As String = tableGrid.Item("Column_Name", IntJ).Value.ToString
                Dim colType As String = tableGrid.Item("Type_Name", IntJ).Value.ToString
                Dim colLen As String = tableGrid.Item("Precision", IntJ).Value.ToString

                strBld.Append("@" & colName & " ")
                strBld.Append(Space(35 - Len(colName)))
                Select Case colType
                    Case "int", "ntext", "text", "datetime", "date", "smallint", "decimal", "money", "int identity"
                        'Data types that do not require length
                        strBld.Append(colType.Replace("identity", "").Trim)
                    Case Else
                        'Others like varchar, char, nvarchar etc.
                        strBld.AppendFormat("{0} ({1})", colType, colLen)
                End Select
            End If

        Next
        strBld.Append(vbCrLf & ")")
        strBld.Append(vbCrLf & "AS" & vbCrLf)
        strBld.Append("BEGIN" & vbCrLf & vbCrLf)
        strBld.Append("DECLARE @newid int" & vbCrLf)

        If columnData.Tables("Pkeys").Rows.Count > 0 Then
            'Start writing if condition to avoid primary key violation errors
            strBld.Append("IF EXISTS(select * from " & tableName & vbCrLf)
            strBld.Append(Space(10) & "where ")
            For IntJ = 0 To columnData.Tables("Pkeys").Rows.Count - 2
                strColumnName = columnData.Tables("Pkeys").Rows(IntJ).Item("COLUMN_NAME")
                strBld.Append(Space(10) & strColumnName & " = " & "@" & strColumnName & " and ")
            Next
            'itemChecked = dsPkeys.Tables(0).Rows.Count - 1
            strColumnName = columnData.Tables("Pkeys").Rows(IntJ).Item("COLUMN_NAME")
            strBld.Append(Space(10) & strColumnName & " = " & "@" & strColumnName & ")" & vbCrLf)

            strBld.Append(Space(5) & "BEGIN" & vbCrLf)
            strBld.Append(Space(7) & "--Update existing row" & vbCrLf)
            strBld.Append(Space(7) & "UPDATE " & tableName & vbCrLf)
            strBld.Append(Space(7) & "SET" & vbCrLf)

            For IntJ = 0 To tableGrid.Rows.Count - 1

                'Check if selected column is primary key
                If tableGrid.Rows(IntJ).Cells(0).Value Then
                    blnNotPrimary = True
                    strColumnName = tableGrid.Item("Column_Name", IntJ).Value
                    For intI = 0 To columnData.Tables("Pkeys").Rows.Count - 1
                        If strColumnName = columnData.Tables("Pkeys").Rows(intI).Item("COLUMN_NAME") Then
                            blnNotPrimary = False
                        End If
                    Next
                    If blnNotPrimary Then
                        strBld.Append(Space(10) & strColumnName & " = @" & strColumnName)
                        strBld.Append("," & vbCrLf)
                    End If
                End If

            Next

            strBld.Remove(strBld.Length - 3, 2)

            strBld.Append(Space(7) & "WHERE" & vbCrLf)
            For IntJ = 0 To columnData.Tables("Pkeys").Rows.Count - 2
                strColumnName = columnData.Tables("Pkeys").Rows(IntJ).Item("COLUMN_NAME")
                strBld.Append(Space(10) & strColumnName & " = " & "@" & strColumnName & " and " & vbCrLf)
            Next
            'itemChecked = dsPkeys.Tables(0).Rows.Count - 1
            strColumnName = columnData.Tables("Pkeys").Rows(IntJ).Item("COLUMN_NAME")
            strBld.Append(Space(10) & strColumnName & " = " & "@" & strColumnName & vbCrLf)
            strBld.Append(vbCrLf)
            strBld.Append(Space(10) & "set @newid = @" & strColumnName & vbCrLf)

            strBld.Append(Space(5) & "END" & vbCrLf)

            strBld.Append("ELSE" & vbCrLf) ' Row Does not exists

            strBld.Append(Space(5) & "BEGIN" & vbCrLf)
            strBld.Append(Space(7) & "--Insert new row" & vbCrLf)
            'insert into "tableName"(column_names) values(columns_values)
            strBld.Append(Space(7) & "INSERT INTO " & tableName & "(" & vbCrLf)

            For IntJ = 0 To tableGrid.Rows.Count - 1

                If tableGrid.Rows(IntJ).Cells(0).Value Then
                    strColumnName = tableGrid.Item("Column_Name", IntJ).Value() & ","
                    strBld.Append(Space(10) & strColumnName & vbCrLf)
                End If

            Next

            'Remove last ","
            strBld.Remove(strBld.Length - 3, 2)

            strBld.Append(")" & vbCrLf)

            strBld.Append(Space(7) & "VALUES(" & vbCrLf)

            For IntJ = 0 To tableGrid.Rows.Count - 1 - 1

                If tableGrid.Rows(IntJ).Cells(0).Value Then
                    strColumnName = "@" & tableGrid.Item("Column_Name", IntJ).Value() & ","
                    strBld.Append(Space(10) & strColumnName & vbCrLf)
                End If

            Next
            'Remove last ","
            strBld.Remove(strBld.Length - 3, 2)

            strBld.Append(")" & vbCrLf)

            strBld.Append(Space(10) & "set @newid = SCOPE_IDENTITY()" & vbCrLf)

            strBld.Append(Space(5) & "END" & vbCrLf)
        Else
            strBld.Append(Space(7) & "INSERT INTO " & tableName & "(" & vbCrLf)
            For IntJ = 0 To tableGrid.Rows.Count - 1

                If tableGrid.Rows(IntJ).Cells(0).Value Then
                    strColumnName = tableGrid.Item("Column_Name", IntJ).Value() & ","
                    strBld.Append(Space(10) & strColumnName & vbCrLf)
                End If

            Next

            'Remove last ","
            strBld.Remove(strBld.Length - 3, 2)

            strBld.Append(")" & vbCrLf)

            strBld.Append(Space(7) & "VALUES(" & vbCrLf)

            For IntJ = 0 To tableGrid.Rows.Count - 1

                If tableGrid.Rows(IntJ).Cells(0).Value Then
                    strColumnName = "@" & tableGrid.Item("Column_Name", IntJ).Value() & ","
                    strBld.Append(Space(10) & strColumnName & vbCrLf)
                End If

            Next
            'Remove last ","
            strBld.Remove(strBld.Length - 3, 2)

            strBld.Append(")" & vbCrLf)

            strBld.Append(Space(10) & "set @newid = SCOPE_IDENTITY()" & vbCrLf)

        End If

        strBld.Append(vbCrLf)
        strBld.AppendFormat(Space(10) & "SELECT * FROM {0} where id = @newid", tableName)
        strBld.Append(vbCrLf)

        strBld.Append("END")

        Return strBld.ToString
    End Function
    Public Shared Function GenerateDelete(tableName As String, tableGrid As DataGridView) As String
        Dim strBld As New StringBuilder
        Dim itemChecked As Integer = 0
        Dim intI As Integer
        Dim blnNotPrimary As Boolean = False
        Dim strColumnName As String
        'Start of comments
        Dim procName As String = String.Format("stp_{0}_Delete", tableName)

        'Start of comments
        strBld.AppendFormat("--{1} Created on {0}" & vbCrLf, Now.ToString, procName)
        strBld.Append("--Autogenerated by UseThisBOG " & vbCrLf & vbCrLf)
        'End of comments


        strBld.Append("-- Drop stored procedure if it already exists" & vbCrLf)
        strBld.AppendFormat("If EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE SPECIFIC_SCHEMA = N'dbo' AND SPECIFIC_NAME = N'{0}' )" & vbCrLf, procName)
        strBld.AppendFormat(vbTab & "DROP PROCEDURE dbo.{0}" & vbCrLf, procName)
        strBld.Append("GO" & vbCrLf)
        'End of comments

        strBld.AppendFormat("CREATE PROCEDURE [stp_{0}_Delete]" & vbCrLf, tableName)
        strBld.Append("(" & vbCrLf)
        'Variable definitions
        intI = 0
        For intI = 0 To tableGrid.Rows.Count - 1

            If tableGrid.Rows(intI).Cells(0).Value = True Then
                strBld.Append("@" & tableGrid.Item("Column_Name", intI).Value)
                strBld.Append(Space(35 - Len(tableGrid.Item("Column_Name", intI).Value)))
                Select Case tableGrid.Item("Type_Name", intI).ToString
                    Case "int", "ntext", "text", "datetime", "date", "smallint", "decimal"
                        strBld.Append(tableGrid.Item("Type_Name", intI).Value() & "," & vbCrLf)

                    Case Else
                        strBld.Append(tableGrid.Item("Type_Name", intI).Value & "(")
                        strBld.Append(tableGrid.Item("Precision", intI).Value & ")," & vbCrLf)
                End Select
            End If

        Next

        'Remova last ","
        strBld.Remove(strBld.Length - 3, 2)

        strBld.Append(vbCrLf)
        strBld.Append(")")
        strBld.Append(vbCrLf)
        strBld.Append("AS")
        strBld.Append(vbCrLf)
        strBld.Append("BEGIN")
        strBld.Append(vbCrLf)
        'Body of the procedure
        strBld.Append(Space(7) & "DELETE FROM " & tableName & vbCrLf)
        strBld.Append(Space(7) & "WHERE" & vbCrLf)
        intI = 0
        For intI = 0 To tableGrid.Rows.Count - 1
            If tableGrid.Rows(intI).Cells(0).Value = True Then
                If intI <> 0 Then strBld.Append(" and " & vbCrLf)
                strColumnName = tableGrid.Item("Column_Name", intI).Value 'columnData.Tables("Pkeys").Rows(intI).Item("COLUMN_NAME").ToString()
                strBld.Append(Space(10) & strColumnName & " = " & "@" & strColumnName)
            End If
        Next

        strBld.Append(vbCrLf)
        strBld.Append("END")
        strBld.Append(vbCrLf)

        'Body ends
        strBld.Append("Go")

        Return strBld.ToString

    End Function
    Public Shared Function GenerateSelect(tableName As String) As String
        Dim strBld As New StringBuilder
        Dim strDec As New List(Of String)
        Dim strSet As New List(Of String)

        Dim procName As String = String.Format("stp_{0}_Select", tableName)

        'Start of comments
        strBld.AppendFormat("--{1} Created on {0}" & vbCrLf, Now.ToString, procName)
        strBld.Append("--Autogenerated by UseThisBOG " & vbCrLf & vbCrLf)
        'End of comments


        strBld.Append("-- Drop stored procedure if it already exists" & vbCrLf)
        strBld.AppendFormat("If EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE SPECIFIC_SCHEMA = N'dbo' AND SPECIFIC_NAME = N'{0}' )" & vbCrLf, procName)
        strBld.AppendFormat(vbTab & "DROP PROCEDURE dbo.{0}" & vbCrLf, procName)
        strBld.Append("GO" & vbCrLf)
        'End of comments

        strBld.AppendFormat("CREATE PROCEDURE [stp_{0}_Select]" & vbCrLf, tableName)
        strBld.Append("as" & vbCrLf)
        strBld.AppendLine("Begin")
        strBld.AppendFormat("Select * From {0}" & vbCrLf, tableName)
        strBld.AppendLine("End")

        Return strBld.ToString
    End Function


    'Converts SQL data type to DbType for creating stored procedure
    Public Shared Function GetSQLDataType(ByVal typeName As String) As String
        'Add more data types if required
        Dim dbType As String = String.Empty
        Select Case typeName
            Case "int"
                dbType = "DbType.Int32"
            Case "varchar", "nvarchar", "text", "char", "ntext"
                dbType = "DbType.String"
            Case "smallint"
                dbType = "DbType.Int16"
            Case "money", "decimal"
                dbType = "DbType.Single"
            Case "datetime"
                dbType = "DbType.Date"
            Case "float"
                dbType = "DbType.Double"
            Case Else
                'Add other missing data types
        End Select

        Return dbType
    End Function

    'Performs data type conversions from SQL server type to VB type.
    Public Shared Function GetVBDataType(ByVal typeName As String) As String
        Select Case typeName.ToLower
            Case "numeric".ToLower, "int".ToLower, "System.Int32".ToLower
                Return "Integer"
            Case "nvarchar", "varchar", "ntext", "text", "System.String".ToLower, "System.DateTime".ToLower
                Return "String"
            Case "money", "decimal"
                Return "Single"
            Case "smallint"
                Return "Int16"
            Case "datetime"
                Return "Date"
            Case "byte"
                Return "Byte"
            Case "bit", "System.Boolean".ToLower
                Return "Boolean"
            Case "char"
                Return "Char"
            Case "float", "System.Decimal".ToLower
                Return "Double"
            Case Else
                'Add other missing data types
                Return "String"
        End Select
    End Function
    Public Shared Function GetCSharpDataType(ByVal typeName As String) As String
        Select Case typeName.ToLower
            Case "numeric", "int", "System.Int32".ToLower, "int identity", "integer"
                Return "int"
            Case "nvarchar", "varchar", "ntext", "text", "System.String".ToLower, "System.DateTime".ToLower, "string"
                Return "string?"
            Case "money", "decimal"
                Return "decimal"
            Case "smallint"
                Return "short"
            Case "datetime"
                Return "DateTime"
            Case "byte"
                Return "byte"
            Case "bit", "System.Boolean".ToLower, "boolean"
                Return "bool"
            Case "char"
                Return "char"
            Case "float", "System.Decimal".ToLower
                Return "double"
            Case Else
                'Add other missing data types
                Return "string?"
        End Select
    End Function
End Class
