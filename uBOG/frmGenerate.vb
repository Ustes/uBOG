Imports System.Collections.Generic
Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.Security
Imports System.Security.Principal.WindowsIdentity
Imports System.Text
Imports System.Text.RegularExpressions
Imports UseThisBOG.bogHelper
Public Class frmGenerate
    Inherits System.Windows.Forms.Form

#Region "Fields"
    Friend WithEvents btnEntity As System.Windows.Forms.Button
    Friend WithEvents btnGenFunction As System.Windows.Forms.Button
    Friend WithEvents btnHTML As System.Windows.Forms.Button
    Friend WithEvents btnLoadSQL As Button
    Friend WithEvents cboClassLang As System.Windows.Forms.ComboBox
    Friend WithEvents cboJSFramework As ComboBox
    Friend WithEvents cboPageType As System.Windows.Forms.ComboBox
    Friend WithEvents cboTypes As System.Windows.Forms.ComboBox
    Friend WithEvents cboUIFramework As System.Windows.Forms.ComboBox
    Friend WithEvents chkButtons As System.Windows.Forms.CheckBox
    Friend WithEvents chkColumn As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents chkDelete As System.Windows.Forms.CheckBox
    Friend WithEvents chkEditable As CheckBox
    Friend WithEvents chkInsert As System.Windows.Forms.CheckBox
    Friend WithEvents chkJson As System.Windows.Forms.CheckBox
    Friend WithEvents chkSelect As System.Windows.Forms.CheckBox
    Friend WithEvents chkUseTable As System.Windows.Forms.CheckBox
    Friend WithEvents cmbDatabase As System.Windows.Forms.ComboBox
    Friend WithEvents dgvDetails As System.Windows.Forms.DataGridView
    Friend WithEvents gbSQL As GroupBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox6 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox7 As GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents lstDetails As System.Windows.Forms.ListBox
    Friend WithEvents scMain As System.Windows.Forms.SplitContainer
    Friend WithEvents scMain2 As System.Windows.Forms.SplitContainer
    Friend WithEvents tcMain As TabControl
    Friend WithEvents tpClass As TabPage
    Friend WithEvents tpHTML As TabPage
    Friend WithEvents tpSQL As TabPage
    Friend WithEvents txtSQL As TextBox

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer
    Dim dsColumns As DataSet
    Dim dsDatabases As DataSet
    Dim dsDetails As DataSet
    Dim intGridRowCount As Integer
    Dim MyProcedureStyle As New DataGridTableStyle

    'Table styles
    Dim MyTableStyle As New DataGridTableStyle
    Dim SQLCommand As SqlClient.SqlCommand
    Dim SQLConnection As SqlClient.SqlConnection
    Dim SQLDataAdapter As SqlClient.SqlDataAdapter
    Dim SqlParam As SqlClient.SqlParameter
    Friend WithEvents chkUseAutoProp As CheckBox
    Friend WithEvents grpItems As GroupBox
    Dim strFunctionText As StringBuilder

#End Region 'Fields

#Region "Constructors"

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
    End Sub

#End Region 'Constructors

#Region "Methods"

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    Private Sub btnEntity_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEntity.Click
        strFunctionText = New StringBuilder

        If Not IsNothing(lstDetails.SelectedItem) OrElse cboTypes.SelectedItem.ToString.ToLower = "sql" Then
            Select Case cboTypes.SelectedItem.ToString.ToLower
                Case "tables", "views"
                    strFunctionText.Append(GenerateEntity(lstDetails.SelectedItem.ToString(), dgvDetails, cboClassLang.SelectedItem.ToString, chkUseAutoProp.Checked))


                Case "procedures"
                    'Generates Vb function to call selected Stored procedure
                    GenerateFunction()
                Case "sql"
                    'GenerateSQL()
                    strFunctionText.Append(GenerateEntity("test", dgvDetails, cboClassLang.SelectedItem.ToString, chkUseAutoProp.Checked))
            End Select
            Dim r As New frmResults
            r.FormResults.Add(New frmResults.resultObject() With {.ResultText = strFunctionText.ToString, .ResultName = "Class Object"})
            r.Show()
        Else
            MsgBox("Nothing selected")
        End If
    End Sub

    Private Sub btnGenFunction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGenFunction.Click
        Dim bInsert As Boolean = False
        Dim bUpdate As Boolean = False
        Dim bDelete As Boolean = False
        Dim bSelect As Boolean = False
        Dim r As New frmResults

        strFunctionText = New StringBuilder
        If chkInsert.Checked Then
            r.FormResults.Add(New frmResults.resultObject() With {.ResultText = GenerateInsert(lstDetails.SelectedItem.ToString(), dgvDetails, dsColumns), .ResultName = "SQL INSERT"})
        End If
        If chkDelete.Checked Then
            r.FormResults.Add(New frmResults.resultObject() With {.ResultText = GenerateDelete(lstDetails.SelectedItem.ToString(), dgvDetails), .ResultName = "SQL DELETE"})
        End If

        If chkSelect.Checked Then
            r.FormResults.Add(New frmResults.resultObject() With {.ResultText = GenerateSelect(lstDetails.SelectedItem.ToString()), .ResultName = "SQL SELECT"})
        End If

        r.Show()
        'writeStatus("Stored procedure generated successfully.")
    End Sub

    Private Sub btnHTML_Click(sender As System.Object, e As System.EventArgs) Handles btnHTML.Click
        strFunctionText = New StringBuilder

        If Not IsNothing(lstDetails.SelectedItem) Then

            GenerateHTMLForm()

        Else
            MsgBox("Nothing selected")
        End If
    End Sub

    Private Sub cboTypes_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboTypes.SelectedIndexChanged
        CreateDataset()
        loadObjectTypes()
    End Sub

    Private Sub cbType_SelectedIndexChanged(sender As System.Object, e As System.EventArgs)
        If Not IsNothing(cmbDatabase.SelectedItem) Then
            If chkInsert.Checked Then
                lstDetails.DisplayMember = "TABLE_NAME"
                lstDetails.ValueMember = "TABLE_NAME"
                If Not IsNothing(dsDetails.Tables("Tables")) AndAlso dsDetails.Tables("Tables").Rows.Count > 0 Then
                    lstDetails.DataSource = dsDetails.Tables("Tables")
                    btnEntity.Text = "Generate Entity"
                    PopulateColumns(1)
                Else
                    lstDetails.DataSource = Nothing
                    'lstDetails.Refresh()
                    MsgBox("There are no Tables in the selected database")
                End If
            End If
        End If
    End Sub



    Private Sub cmbDatabase_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbDatabase.SelectionChangeCommitted
        Try
            CreateDataset()
            lstDetails.DataSource = Nothing
            cboTypes.SelectedIndex = 0
            loadObjectTypes()
            cboTypes.Visible = True
            lstDetails.Visible = True

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub cmbDetails_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstDetails.SelectedIndexChanged
        Select Case cboTypes.SelectedItem.ToString.ToLower
            Case "tables"
                PopulateColumns(1)
            Case "views"
                PopulateColumns(1)
            Case "procedures"
                PopulateColumns(2)
        End Select
    End Sub
    ''' <summary>
    ''' Populate a Listbox with a databases table names
    ''' </summary>
    ''' <param name="ConnectionString">database connection string</param>
    ''' <param name="TablesListBox">Listbox to populate with table names</param>
    ''' <remarks></remarks>
    Public Sub GetAllTableNames(ByVal ConnectionString As String, ByVal TablesListBox As ListBox)
        TablesListBox.DataSource = Nothing
        Using cn As New OleDbConnection(ConnectionString)
            cn.Open()
            Dim DatabaseSchema As New DataTable
            DatabaseSchema = cn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                                                    New Object() {Nothing, Nothing, Nothing, "TABLE"})
            TablesListBox.DisplayMember = "TABLE_NAME"
            TablesListBox.DataSource = DatabaseSchema
        End Using
    End Sub
    Private Sub CreateDataset()
        Dim j, i As Integer
        dsDetails = New DataSet
        Dim dsTables As New DataSet
        Dim arrDataRows() As Data.DataRow ' Used in filtering system tables
        Dim dtTables As Data.DataTable
        If Not IsNothing(cmbDatabase.SelectedItem) Then
            If IsNothing(cboTypes.SelectedItem) Then
                cboTypes.SelectedIndex = 0
            End If

            Dim newConn As String = ""
            'change command text to system stored procedure
            If mdiMain.UtilityConnectionString.IndexOf("Catalog") > 0 Then
                'replace database
                Dim pattern As String = "Initial Catalog=[a-zA-Z]+\;"
                Dim replacement As String = String.Format("Initial Catalog={0};", cmbDatabase.SelectedItem.ToString())
                Dim Input As String = mdiMain.UtilityConnectionString
                newConn = Regex.Replace(Input, pattern, replacement)

            ElseIf mdiMain.UtilityConnectionString.IndexOf("Database") > 0 Then
                'replace database
                Dim pattern As String = "Database=[a-zA-Z]+\;"
                Dim replacement As String = String.Format("Database={0};", cmbDatabase.SelectedItem.ToString())
                Dim Input As String = mdiMain.UtilityConnectionString
                newConn = Regex.Replace(Input, pattern, replacement)
            Else
                newConn = mdiMain.UtilityConnectionString & ";Database=" & cmbDatabase.SelectedItem.ToString()
            End If

            mdiMain.UtilityConnectionString = newConn
            SQLDataAdapter.SelectCommand.Connection.ConnectionString = mdiMain.UtilityConnectionString

            Select Case cboTypes.SelectedItem.ToString.ToLower
                Case "tables"

                    Try
                        Dim dt As DataTable = DataHelper.GetDataTable("sp_tables", CommandType.StoredProcedure)

                        'SQLDataAdapter.SelectCommand.Connection.ConnectionString = mdiMain.UtilityConnectionString
                        'SQLDataAdapter.SelectCommand.CommandText = "sp_tables"

                        'Get tables from selected datebase
                        'SQLDataAdapter.Fill(dsTables, "Tables")

                        'Code to filter System tables and views by selecting only user tables
                        'arrDataRows = dsTables.Tables("Tables").Select("TABLE_TYPE in ('TABLE')")
                        arrDataRows = dt.Select("TABLE_TYPE in ('TABLE')")

                        dtTables = dt.Clone ' dsTables.Tables("Tables").Clone()
                        dsTables = Nothing
                        dtTables.TableName = "Tables"
                        If arrDataRows.GetUpperBound(0) <= 0 Then

                            lstDetails.DataSource = Nothing
                            dgvDetails.DataSource = Nothing
                            Throw New ApplicationException("There are no tables in the selected database")

                        End If

                        For i = 0 To arrDataRows.GetUpperBound(0)
                            dtTables.Rows.Add(arrDataRows(i).ItemArray())
                        Next

                        If dsDetails.Tables.Contains("Tables") Then dsDetails.Tables.Remove("Tables")
                        dsDetails.Tables.Add(dtTables)
                        dsTables = Nothing
                        arrDataRows = Nothing
                        'Code to filter System tables- ENDS
                    Catch ex As SqlException
                        Throw
                    Catch ex As Exception
                        Throw
                    End Try
                Case "views"
                    Try

                        'change command text to system stored procedure
                        'SQLDataAdapter.SelectCommand.Connection.ConnectionString = mdiMain.UtilityConnectionString & ";Database=" & cmbDatabase.SelectedItem.ToString()
                        SQLDataAdapter.SelectCommand.CommandText = "sp_tables"

                        'Get tables from selected datebase
                        SQLDataAdapter.Fill(dsTables, "Tables")

                        'Code to filter System tables and views by selecting only user tables
                        arrDataRows = dsTables.Tables("Tables").Select("TABLE_TYPE in ('VIEW')")
                        dtTables = dsTables.Tables("Tables").Clone()
                        dsTables = Nothing
                        dtTables.TableName = "Tables"
                        If arrDataRows.GetUpperBound(0) <= 0 Then

                            lstDetails.DataSource = Nothing
                            dgvDetails.DataSource = Nothing
                            Throw New ApplicationException("There are no views in the selected database")

                        End If

                        For i = 0 To arrDataRows.GetUpperBound(0)
                            dtTables.Rows.Add(arrDataRows(i).ItemArray())
                        Next

                        If dsDetails.Tables.Contains("Tables") Then dsDetails.Tables.Remove("Tables")
                        dsDetails.Tables.Add(dtTables)
                        dsTables = Nothing
                        arrDataRows = Nothing

                    Catch ex As SqlException
                        Throw
                    Catch ex As Exception
                        Throw
                    End Try
                Case "procedures"

                    Try
                        'change command text to system stored procedure
                        SQLDataAdapter.SelectCommand.CommandText = "sp_stored_procedures"

                        If dsDetails.Tables.Contains("StoredProcs") Then dsDetails.Tables.Remove("StoredProcs")
                        'Get tables from selected datebase
                        SQLDataAdapter.Fill(dsDetails, "StoredProcs")
                        If dsDetails.Tables("StoredProcs").Rows.Count = 0 Then
                            lstDetails.DataSource = Nothing
                            Throw New ApplicationException("There are no stored procedures in the selected database")
                        End If
                        'Remove leading ";1" from stored procedure names
                        For i = 0 To dsDetails.Tables("StoredProcs").Rows.Count - 1
                            j = InStr(dsDetails.Tables("StoredProcs").Rows(i).Item("PROCEDURE_NAME"), ";")
                            dsDetails.Tables("StoredProcs").Rows(i).Item("PROCEDURE_NAME") = Mid(dsDetails.Tables("StoredProcs").Rows(i).Item("PROCEDURE_NAME"), 1, j - 1)
                        Next
                    Catch ex As SqlClient.SqlException
                        Throw
                    Catch ex As Exception
                        Throw
                    End Try
            End Select

        End If
    End Sub

    'To select a row on which user clicks
    Private Sub dgDetails_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        'Have columns which has "Allow Nulls=NO" selected all the time when inserting
        'When updating or deleting, primary key columns are selected
        'Any other rows selected in the grid will be used as parameters
        'when Functions are generated.
        'Row selection is not required when entity class is generated.
        Dim i As Integer
        Dim htInfo As DataGrid.HitTestInfo

        For i = 0 To intGridRowCount - 1
            Try
                If Trim(dgvDetails.Item(i, 4).Value) = "NO" Then
                    'select this row
                    dgvDetails.Rows(i).Cells(0).Value = True
                End If
            Catch ex As Exception
                Continue For
            End Try

        Next

        'htInfo = dgDetails.HitTest(e.X, e.Y)

        'If htInfo.Row <> -1 Then
        '    dgDetails.Select(htInfo.Row)
        'End If
    End Sub

    Private Sub dgvDetails_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgvDetails.ColumnHeaderMouseClick
        If e.ColumnIndex = 0 Then
            For Each row As DataGridViewRow In dgvDetails.Rows
                row.Cells(0).Value = If(row.Cells(0).Value = True, False, True)
            Next

        End If
    End Sub

    Private Sub frmUtilities_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            cboClassLang.SelectedIndex = 0

            Dim arrServers() As String = Nothing
            Dim IntI As Integer = 0

            'Prepare SQL Connection
            SQLConnection = New SqlClient.SqlConnection(mdiMain.UtilityConnectionString)
            SQLCommand = New SqlClient.SqlCommand
            SQLCommand.CommandText = "SELECT * FROM sys.databases" '"sp_databases"
            SQLCommand.CommandType = CommandType.Text

            'SQl Data Adapter
            SQLDataAdapter = New SqlClient.SqlDataAdapter
            SQLDataAdapter.SelectCommand = SQLCommand
            SQLDataAdapter.SelectCommand.Connection = SQLConnection
            dsDatabases = New DataSet
            ' Get Database information from database
            SQLDataAdapter.Fill(dsDatabases, "Database")

            cmbDatabase.DisplayMember = "Text"

            Using dt As DataTable = dsDatabases.Tables("Database")
                For Each dr As DataRow In dt.Rows
                    cmbDatabase.Items.Add(dr("Name").ToString)
                Next
            End Using
            Dim builder As New SqlConnectionStringBuilder(mdiMain.UtilityConnectionString)
            If Not String.IsNullOrEmpty(builder.InitialCatalog) Then
                For i As Integer = 0 To cmbDatabase.Items.Count - 1
                    If cmbDatabase.Items(i).ToString = builder.InitialCatalog Then
                        cmbDatabase.SelectedIndex = i
                        Exit For

                    End If
                Next
            End If

            CreateDataset()
            PrepareTableStyles()
            mdiMain.mainStatus.Text = "Connection Success!!..." & mdiMain.UtilityConnectionString

            cboTypes.SelectedIndex = 0

            cboPageType.SelectedIndex = 0
            cboUIFramework.SelectedIndex = 1

        Catch ex As Exception
            MsgBox(ex.Message)

        End Try
    End Sub


    Private Sub GenerateFunction()
        Dim strBuilder As New StringBuilder
        Dim strFunctionName As String
        Dim i As Integer

        strBuilder.Append("Public function ")
        strFunctionName = InputBox("Enter Function Name", "Function Name")

        'Return type assumed to be Dataset
        strBuilder.Append(strFunctionName & "() as System.Data.Dataset") 'Modify this statement to add return type
        strBuilder.Append(vbCrLf)
        'Declaring Dataset varaibale
        strBuilder.Append("Dim dsResults as System.Data.DataSet" & vbCrLf)
        'Delacring Parameter length based on the number of parameters required
        strBuilder.Append("Dim SQLParam(") ' Creates SQLParameter object array
        i = dsColumns.Tables("Procedure").Rows.Count - 1
        If i > 1 Then
            strBuilder.Append(i - 1 & ") as SQLParameter")
        Else
            strBuilder.Append(0 & ") as SQLParameter")
        End If
        strBuilder.Append(vbCrLf)

        strBuilder.Append("Try") ' Try block starts
        strBuilder.Append(vbCrLf & vbCrLf)
        For i = 1 To dsColumns.Tables("Procedure").Rows.Count - 1
            'strBuilder.Append("ErrMsg =" & Chr(34) & " Preparing Parameter " & i & Chr(34))
            strBuilder.Append(vbCrLf)
            strBuilder.Append("SQLParam(" & (i - 1).ToString & ")=new System.Data.SqlClient.SqlParameter" & vbCrLf)
            strBuilder.Append("with SQLParam(" & (i - 1).ToString & ")" & vbCrLf) 'Begin of WITH block
            strBuilder.Append(".ParameterName=" & Chr(34) & dsColumns.Tables("Procedure").Rows(i).Item("COLUMN_NAME") & Chr(34) & vbCrLf)
            strBuilder.Append(".DbType=" & GetSQLDataType(dsColumns.Tables("Procedure").Rows(i).Item("TYPE_NAME")) & vbCrLf)
            If dsColumns.Tables("Procedure").Rows(i).Item("COLUMN_TYPE") = 1 Then 'Input parameter
                strBuilder.Append(".Direction = ParameterDirection.Input" & vbCrLf)
            ElseIf dsColumns.Tables("Procedure").Rows(i).Item("COLUMN_TYPE") = 2 Then ' Output parameter
                strBuilder.Append(".Direction = ParameterDirection.Output" & vbCrLf)
            End If

            If Not IsDBNull(dsColumns.Tables("Procedure").Rows(i).Item("CHAR_OCTET_LENGTH")) Then
                'This will be null for all numeric data types
                strBuilder.Append(".Size = " & dsColumns.Tables("Procedure").Rows(i).Item("PRECISION") & vbCrLf)
            End If

            strBuilder.Append("'.Value = " & "'Uncomment this after providing value for this parameter" & vbCrLf)

            strBuilder.Append("End with" & vbCrLf) ' End of With block
            strBuilder.Append(vbCrLf & vbCrLf)

        Next
        'Write ExecuteDataset statment
        'Replace this statement if required with any other
        'For example you may not be using MS Data Access Blocks, or ExecuteScalar etc...
        strBuilder.Append("'calling ExecuteDataset Method " & vbCrLf)
        strBuilder.Append("SqlHelper.ExecuteDataset(m_ConnectionString, CommandType.StoredProcedure," & Chr(34))
        strBuilder.Append(lstDetails.SelectedItem.ToString() & Chr(34) & ",")
        strBuilder.Append("SQLParam)")
        strBuilder.Append(vbCrLf & vbCrLf)
        'Catch block
        strBuilder.Append("Catch ex As Exception")
        strBuilder.Append(vbCrLf & vbCrLf)
        '
        'Implement you own method of exception handling
        strBuilder.Append("'Your own method of exception handling" & vbCrLf)
        'like sending an email to the concerned
        '
        strBuilder.Append(vbCrLf & vbCrLf)
        strBuilder.Append("Finally")
        strBuilder.Append(vbCrLf & vbCrLf)
        'Destroying SQL Parameter Objects
        strBuilder.Append("Dim i As Integer" & vbCrLf)
        strBuilder.Append("For i = 0 To SQLParam.Length - 1" & vbCrLf)
        strBuilder.Append("SQLParam(i) = Nothing" & vbCrLf)
        strBuilder.Append("Next" & vbCrLf)
        strBuilder.Append(vbCrLf)
        strBuilder.Append("end try" & vbCrLf) ' End of Try block
        strBuilder.Append(vbCrLf)

        'Return Statement
        strBuilder.Append("Return dsResults") ' Normally a dataset, change this statement if required
        strBuilder.Append(vbCrLf & vbCrLf)

        strBuilder.Append("End function") 'End of function

        strFunctionText.Append(strBuilder.ToString)

        strBuilder = Nothing
    End Sub

    Private Sub GenerateHTMLForm()
        Dim strBuilder As New StringBuilder
        Dim strCodeBehindBuilder As New StringBuilder
        Dim strJSBuilder As New StringBuilder
        Dim strUpdate As New StringBuilder
        Dim strEntityName As String
        Dim i As Integer

        strEntityName = InputBox("Enter Entity Name", "Entity", lstDetails.SelectedItem)

        'validate entity name
        If strEntityName = "" Then
            MsgBox("Please enter valid name for Entity")
            Exit Sub
        End If

        'get selected columns
        Dim selectedCols As New List(Of String)
        For IntJ As Integer = 0 To intGridRowCount - 1
            'Check if selected column is primary key
            If dgvDetails.Rows(IntJ).Cells(0).Value = True Then
                selectedCols.Add(dgvDetails.Item("Column_Name", IntJ).Value)
            End If
        Next

        'Build Private variables list based on the table column names and data type
        If chkJson.Checked Then
            strJSBuilder.AppendFormat("var newObject = new Object;" & vbCrLf)
        End If

        strCodeBehindBuilder.AppendLine("Private Sub loadData()")
        strUpdate.AppendLine("Private Sub updateData()")
        strUpdate.AppendLine("  Dim ssql as string = ""SQL NEEDED"" ")
        strUpdate.AppendLine("  Dim params As New List(Of SqlParameter)")
        strCodeBehindBuilder.AppendLine("   using dt as datatable = SqlHelper.GetDataTable(ssql, CommandType.Text)")
        strCodeBehindBuilder.AppendLine("       for each dr as datarow in dt.rows")

        Dim jsFramework As String = cboJSFramework.SelectedItem.ToString
        Dim uiFramework As String = cboUIFramework.SelectedItem.ToString

        Dim tblclass As String = ""
        Dim divclass As String = ""
        Dim tdHdrClass As String = ""
        Dim tdCntClass As String = ""
        Dim btnClass As New List(Of String)
        Select Case uiFramework.ToLower
            Case "jquery ui"
                tdHdrClass = "class=""ui=widget-header"" "
                tdCntClass = "class=""ui=widget-content"" "

                btnClass.Add("class=""jqSaveButton"" ")
                btnClass.Add("class=""jqDeleteButton"" ")
                btnClass.Add("class=""jqCancelButton"" ")
            Case "bootstrap"
                tblclass = "class=""table table-striped"" "

                btnClass.Add("class=""btn btn-primary"" ")
                btnClass.Add("class=""btn btn-danger"" ")
                btnClass.Add("class=""btn btn-default"" ")

            Case "semantic-ui"
                tblclass = "class=""ui table"" "
                divclass = "class=""ui form grid"" "
                btnClass.Add("class=""ui button primary"" ")
                btnClass.Add("class=""ui button error"" ")
                btnClass.Add("class=""ui button"" ")

            Case Else
                tdHdrClass = ""
                tdCntClass = ""

                btnClass.Add("")
                btnClass.Add("")
                btnClass.Add("")

        End Select

        If chkUseTable.Checked Then
            strBuilder.AppendFormat("<table style=""width:100%;"" {0}>", tblclass)
            For i = 0 To dsColumns.Tables("Tables").Rows.Count - 1
                Dim vbType As String = GetVBDataType(dsColumns.Tables("Tables").Rows(i).Item("TYPE_NAME"))
                Dim colName As String = dsColumns.Tables("Tables").Rows(i).Item("COLUMN_NAME")

                If selectedCols.Contains(colName) Then
                    colName = colName.Replace(" ", "").Replace("'", "_").Replace("-", "_").Replace("(", "_").Replace(")", "_")

                    Dim ctlID As String = ""
                    Select Case vbType.ToLower
                        Case "boolean", "byte"
                            ctlID = String.Format("chk{0}", colName)
                            strBuilder.AppendFormat("   <tr><td  style=""font-weight:bold;"" {1}>{0}</td><td {2}><asp:CheckBox id=""{3}"" runat=""server"" /></td></tr>" & vbCrLf, colName, tdHdrClass, tdCntClass, ctlID)
                            strCodeBehindBuilder.AppendFormat("       {1}.checked = dr(""{0}"")" & vbCrLf, colName, ctlID)
                            strUpdate.AppendFormat("    params.Add(New SqlParameter(""{0}"", {1}.checked.tostring))" & vbCrLf, colName, ctlID)
                        Case Else
                            ctlID = String.Format("txt{0}", colName)
                            strBuilder.AppendFormat("   <tr><td style=""font-weight:bold;"" {1}>{0}</td><td {2}><asp:TextBox style=""width:100%;"" id=""{3}"" runat=""server"" /></td></tr>" & vbCrLf, colName, tdHdrClass, tdCntClass, ctlID)
                            strCodeBehindBuilder.AppendFormat("       {1}.text = dr(""{0}"").tostring" & vbCrLf, colName, ctlID)
                            strUpdate.AppendFormat("    params.Add(New SqlParameter(""{0}"", {1}.text.tostring))" & vbCrLf, colName, ctlID)
                    End Select
                    If chkJson.Checked Then
                        strJSBuilder.AppendFormat("newObject.{1} = $(""#{0}"").val();" & vbCrLf, ctlID, colName)
                    End If

                End If

            Next
            If chkButtons.Checked Then
                strBuilder.AppendLine("<tr><td colspan=""2"">")
                strBuilder.AppendFormat("<asp:linkbutton id=""lnkSave"" runat=""server"" text=""Save"" />", btnClass(0))
                strBuilder.AppendFormat("<asp:linkbutton id=""lnkArchve"" runat=""server"" text=""Archive"" />", btnClass(1))
                strBuilder.AppendFormat("<asp:linkbutton id=""lnkCancel"" runat=""server"" text=""Cancel"" />", btnClass(2))
                strBuilder.AppendLine("</td></tr>")
            End If

            strBuilder.Append("</table>")
        Else
            Dim pageType As String = cboPageType.SelectedItem.ToString
            Select Case pageType.ToLower
                Case "detail"
                    strBuilder.AppendFormat("<div {0}>" & Environment.NewLine, divclass)
                    For i = 0 To dsColumns.Tables("Tables").Rows.Count - 1
                        Dim vbType As String = GetVBDataType(dsColumns.Tables("Tables").Rows(i).Item("TYPE_NAME"))
                        Dim colName As String = dsColumns.Tables("Tables").Rows(i).Item("COLUMN_NAME")
                        If selectedCols.Contains(colName) Then
                            colName = colName.Replace(" ", "").Replace("'", "_").Replace("-", "_").Replace("(", "_").Replace(")", "_")
                            Dim ctlID As String = ""

                            strBuilder.Append("<div class=""field"">" & Environment.NewLine)
                            strBuilder.AppendFormat("<label>{0}</label>" & Environment.NewLine, colName)
                            Select Case vbType.ToLower
                                Case "boolean", "byte"
                                    ctlID = String.Format("chk{0}", colName)
                                    If chkEditable.Checked Then
                                        strBuilder.AppendFormat("<div ng-bind=""{1}.{0}"" ng-show=""DisplayMode""></div>" & Environment.NewLine, colName, strEntityName)
                                        strBuilder.AppendFormat("<div ng-show=""EditMode""><input ng-checked=""{1}.{0}"" type=""checkbox"" /></div>" & Environment.NewLine, colName, strEntityName)
                                    Else
                                        strBuilder.AppendFormat("<input type=""checkbox"" name=""input"" ng-checked=""{1}.{0}"" />" & Environment.NewLine, colName, strEntityName)
                                    End If
                                Case Else
                                    ctlID = String.Format("txt{0}", colName)
                                    If chkEditable.Checked Then
                                        strBuilder.AppendFormat("<div ng-bind=""{1}.{0}"" ng-show=""DisplayMode""></div>" & Environment.NewLine, colName, strEntityName)
                                        strBuilder.AppendFormat("<div ng-show=""EditMode""><input ng-model=""{1}.{0}"" type=""text"" /></div>" & Environment.NewLine, colName, strEntityName)
                                    Else
                                        strBuilder.AppendFormat("<input type=""text"" name=""input"" ng-model=""{1}.{0}"" />" & Environment.NewLine, colName, strEntityName)
                                    End If
                            End Select
                            strBuilder.Append("</div>" & Environment.NewLine)
                        End If
                    Next

                    strBuilder.Append("</div>")
                Case "edit"
                    strBuilder.AppendFormat("<div {0}>" & Environment.NewLine, divclass)
                    For i = 0 To dsColumns.Tables("Tables").Rows.Count - 1
                        Dim vbType As String = GetVBDataType(dsColumns.Tables("Tables").Rows(i).Item("TYPE_NAME"))
                        Dim colName As String = dsColumns.Tables("Tables").Rows(i).Item("COLUMN_NAME")
                        If selectedCols.Contains(colName) Then
                            colName = colName.Replace(" ", "").Replace("'", "_").Replace("-", "_").Replace("(", "_").Replace(")", "_")
                            Dim ctlID As String = ""
                            Select Case jsFramework.ToLower
                                Case "angularjs"
                                    Select Case vbType.ToLower
                                        Case "boolean", "byte"
                                            ctlID = String.Format("chk{0}", colName)
                                            strBuilder.Append("<div class=""field"">" & Environment.NewLine)
                                            strBuilder.AppendFormat("<label>{0}</label>" & Environment.NewLine, colName)
                                            If chkEditable.Checked Then
                                                strBuilder.AppendFormat("<div ng-bind=""{1}.{0}"" ng-show=""DisplayMode""></div>" & Environment.NewLine, colName, strEntityName)
                                                strBuilder.AppendFormat("<div ng-show=""EditMode""><input ng-checked=""{1}.{0}"" type=""checkbox"" /></div>" & Environment.NewLine, colName, strEntityName)
                                            Else
                                                strBuilder.AppendFormat("<input type=""checkbox"" name=""input"" ng-checked=""{1}.{0}"" />" & Environment.NewLine, colName, strEntityName)
                                            End If

                                            strBuilder.Append("</div>" & Environment.NewLine)
                                        Case Else
                                            ctlID = String.Format("txt{0}", colName)
                                            strBuilder.Append("<div class=""field"">" & Environment.NewLine)
                                            strBuilder.AppendFormat("<label>{0}</label>" & Environment.NewLine, colName)
                                            If chkEditable.Checked Then
                                                strBuilder.AppendFormat("<div ng-bind=""{1}.{0}"" ng-show=""DisplayMode""></div>" & Environment.NewLine, colName, strEntityName)
                                                strBuilder.AppendFormat("<div ng-show=""EditMode""><input ng-model=""{1}.{0}"" type=""text"" /></div>" & Environment.NewLine, colName, strEntityName)
                                            Else
                                                strBuilder.AppendFormat("<input type=""text"" name=""input"" ng-model=""{1}.{0}"" />" & Environment.NewLine, colName, strEntityName)
                                            End If

                                            strBuilder.Append("</div>" & Environment.NewLine)
                                    End Select
                                Case Else
                                    ctlID = String.Format("txt{0}", colName)
                                    strBuilder.Append("<div Class=""field"">" & Environment.NewLine)
                                    strBuilder.AppendFormat("<label>{0}</label>" & Environment.NewLine, colName)
                                    strBuilder.AppendFormat("<input type=""text"" name=""input"" id=""{1}_{0}"" />" & Environment.NewLine, colName, strEntityName)
                                    strBuilder.Append("</div>" & Environment.NewLine)

                            End Select

                        End If
                    Next

                    strBuilder.Append("</div>")
                Case Else
                    Dim cols As New List(Of String)
                    Dim ths As New List(Of String)
                    Dim tds As New List(Of String)
                    For i = 0 To dsColumns.Tables("Tables").Rows.Count - 1
                        Dim colName As String = dsColumns.Tables("Tables").Rows(i).Item("COLUMN_NAME")
                        If selectedCols.Contains(colName) Then
                            colName = colName.Replace(" ", "").Replace("'", "_").Replace("-", "_").Replace("(", "_").Replace(")", "_")
                            cols.Add(colName)
                            ths.Add(String.Format("<th>{0}</th>" & Environment.NewLine, colName))
                            tds.Add("<td>{{" & String.Format("{1}.{0}", colName, strEntityName.Substring(0, 1)) & "}}</td>" & Environment.NewLine)
                        End If
                    Next
                    strBuilder.AppendFormat("<table {0}>" & Environment.NewLine, tblclass)
                    strBuilder.AppendFormat("<thead><tr>{0}</tr></thead>", Join(ths.ToArray, ""))
                    strBuilder.AppendFormat("<tbody><tr ng-repeat=""{1} in {1}"">{0}</tr></tbody>", Join(tds.ToArray, ""), strEntityName, strEntityName.Substring(0, 1))
                    strBuilder.Append("</table>")
            End Select

        End If

        strCodeBehindBuilder.AppendLine("           exit for")
        strCodeBehindBuilder.AppendLine("       next")
        strCodeBehindBuilder.AppendLine("   end using")
        strCodeBehindBuilder.AppendLine("End Sub")
        strUpdate.AppendLine("End Sub")

        strFunctionText.Append(strBuilder.ToString)
        Dim fb As New frmBrowser
        fb.WebContent = strBuilder.ToString.Replace("asp:CheckBox", "input type=""checkbox"" ").Replace("asp:TextBox", "input type=""text"" ").Replace("asp:linkbutton", "input type=""button"" ")
        fb.Show()

        Dim r As New frmResults
        r.FormResults.Add(New frmResults.resultObject() With {.ResultText = strBuilder.ToString, .ResultName = "HTML Form"})
        r.FormResults.Add(New frmResults.resultObject() With {.ResultText = strJSBuilder.ToString, .ResultName = "Javascript"})
        r.FormResults.Add(New frmResults.resultObject() With {.ResultText = strCodeBehindBuilder.ToString & strUpdate.ToString, .ResultName = "Code Behind"})
        r.Show()

        strCodeBehindBuilder = Nothing
        strUpdate = Nothing
        strJSBuilder = Nothing
        strBuilder = Nothing
    End Sub


    Private Sub GenerateSQL()
        Dim strBuilder As New StringBuilder
        Dim strClassName As String = String.Empty
        Dim i As Integer = 0

        strClassName = InputBox("Enter Function Name", "Function Name")

        Select Case cboClassLang.SelectedItem.ToString
            Case "C#"
                strBuilder.AppendFormat("public class {0}" & vbCrLf, strClassName)
                'Build Properties
                Dim sbPrivs As New StringBuilder
                Dim sbProps As New StringBuilder
                strBuilder.Append("{" & vbCrLf)
                For Each row As DataGridViewRow In dgvDetails.Rows
                    If row.Cells(0).Value = True Then
                        Dim dbType As String = GetCSharpDataType(row.Cells("DATA_TYPE").Value)
                        Dim strColumnName As String = row.Cells("Column_Name").Value
                        sbPrivs.AppendFormat("private {1} _{0};" & vbCrLf, strColumnName, dbType)
                        sbProps.AppendFormat("public {1} {0}" & vbCrLf, strColumnName, dbType)
                        sbProps.Append("{" & vbCrLf)
                        sbProps.Append("get {")
                        sbProps.AppendFormat("return _{0};", strColumnName)
                        sbProps.Append("}" & vbCrLf)

                        sbProps.Append("set {")
                        sbProps.AppendFormat("_{0} = value;", strColumnName)
                        sbProps.Append("} " & vbCrLf)
                        sbProps.Append("}")
                        sbProps.Append(vbCrLf)
                        sbProps.Append(vbCrLf)
                    End If
                Next

                strBuilder.Append("#region Properties" & vbCrLf)
                strBuilder.Append(sbPrivs.ToString)
                strBuilder.Append(vbCrLf)
                strBuilder.Append(sbProps.ToString)
                strBuilder.Append(vbCrLf)
                strBuilder.Append("#endregion" & vbCrLf) 'Properties region ends


                strBuilder.Append("}")

            Case Else
                strBuilder.AppendFormat("Public Class {0}", strClassName)
                'Build Properties
                Dim sbPrivs As New StringBuilder
                Dim sbProps As New StringBuilder
                For Each row As DataGridViewRow In dgvDetails.Rows
                    If row.Cells(0).Value = True Then
                        Dim dbType As String = GetVBDataType(row.Cells("DATA_TYPE").Value)
                        Dim strColumnName As String = row.Cells("Column_Name").Value
                        sbPrivs.AppendFormat("Private _{0} as {1}" & vbCrLf, strColumnName, dbType)
                        sbProps.AppendFormat("Public Property {0} () as {1}" & vbCrLf, strColumnName, dbType)
                        sbProps.Append("Get" & vbCrLf)
                        sbProps.Append("return _" & strColumnName & vbCrLf)
                        sbProps.Append("End Get" & vbCrLf)
                        sbProps.Append("Set(ByVal Value As " & dbType & ")" & vbCrLf)
                        sbProps.Append("_" & strColumnName & " = Value" & vbCrLf)
                        sbProps.Append("end set" & vbCrLf)
                        sbProps.Append("end Property ")
                        sbProps.Append(vbCrLf)
                        sbProps.Append(vbCrLf)
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



        strFunctionText.Append(strBuilder.ToString)
        Dim r As New frmResults
        r.FormResults.Add(New frmResults.resultObject() With {.ResultText = strFunctionText.ToString, .ResultName = "Class Object"})
        r.Show()
        strBuilder = Nothing


    End Sub

    'Private Function GenerateUpdate() As String
    '    Dim strBld As New StringBuilder
    '    Dim IntJ As Integer
    '    Dim intI As Integer
    '    Dim blnNotPrimary As Boolean
    '    Dim strColumnName As String

    '    'Start of comments
    '    strBld.AppendFormat("--Created on {0}" & vbCrLf, Now.ToString)
    '    strBld.Append("--Autogenerated by UseThisBOG " & vbCrLf & vbCrLf)
    '    'End of comments

    '    strBld.AppendFormat("CREATE PROCEDURE [stp_{0}_Update]" & vbCrLf, lstDetails.SelectedItem.ToString())
    '    strBld.Append("(" & vbCrLf)
    '    'Variable definitions
    '    For Each row As DataGridViewRow In dgvDetails.Rows
    '        If row.Cells(0).Value = True Then
    '            Dim dbType As String = GetVBDataType(row.Cells("Type_Name").Value)
    '            Dim cname As String = row.Cells("Column_Name").Value

    '            strBld.Append("@" & cname & " ")
    '            strBld.Append(Space(35 - Len(cname)))
    '            Select Case dgvDetails.Item("Type_Name", IntJ).Value.ToString
    '                Case "int", "ntext", "text", "datetime", "date", "smallint", "decimal"
    '                    strBld.Append(cname & "," & vbCrLf)
    '                Case Else
    '                    strBld.Append(dgvDetails.Item("Type_Name", IntJ).Value & "(")
    '                    strBld.Append(dgvDetails.Item("Precision", IntJ).Value & ")," & vbCrLf)
    '            End Select

    '        End If
    '    Next

    '    'Remove last ","
    '    strBld.Remove(strBld.Length - 3, 2)
    '    strBld.Append(vbCrLf & ")")
    '    strBld.Append(vbCrLf)
    '    strBld.Append(vbCrLf)
    '    strBld.Append("AS")
    '    strBld.Append(vbCrLf)
    '    strBld.Append("BEGIN")
    '    strBld.Append(vbCrLf)

    '    strBld.Append(Space(7) & "UPDATE " & lstDetails.SelectedItem.ToString & vbCrLf)
    '    strBld.Append(Space(7) & "SET" & vbCrLf)

    '    Dim lstParams As New List(Of String)
    '    For IntJ = 0 To intGridRowCount - 1

    '        'Check if selected column is primary key
    '        If dgvDetails.Rows(IntJ).Cells(0).Value Then
    '            blnNotPrimary = True
    '            strColumnName = dgvDetails.Item("Column_Name", IntJ).Value
    '            For intI = 0 To dsColumns.Tables("Pkeys").Rows.Count - 1
    '                If strColumnName = dsColumns.Tables("Pkeys").Rows(intI).Item("COLUMN_NAME") Then
    '                    blnNotPrimary = False
    '                End If
    '            Next
    '            If blnNotPrimary Then
    '                'If IntJ > 0 Then strBld.Append("," & vbCrLf)
    '                'strBld.Append(Space(10) & strColumnName & " = @" & strColumnName)
    '                lstParams.Add(Space(10) & strColumnName & " = @" & strColumnName & vbCrLf)
    '            End If
    '        End If

    '    Next

    '    'Remove last ","
    '    'strBld.Remove(strBld.Length - 3, 2)
    '    strBld.Append(Join(lstParams.ToArray, ","))
    '    strBld.Append(vbCrLf)
    '    'Adding where condition using Primary keys
    '    'by default all the primary keys are used in where condition
    '    If dsColumns.Tables("Pkeys").Rows.Count > 0 Then

    '        strBld.Append(Space(7) & "WHERE" & vbCrLf)

    '        For IntJ = 0 To dsColumns.Tables("Pkeys").Rows.Count - 2
    '            strColumnName = dsColumns.Tables("Pkeys").Rows(IntJ).Item("COLUMN_NAME")
    '            strBld.Append(Space(10) & strColumnName & " = " & "@" & strColumnName & " and " & vbCrLf)
    '        Next
    '        strColumnName = dsColumns.Tables("Pkeys").Rows(IntJ).Item("COLUMN_NAME")
    '        strBld.Append(Space(10) & strColumnName & " = " & "@" & strColumnName & vbCrLf)

    '    End If
    '    strBld.Append("END" & vbCrLf)

    '    'Body ends
    '    strBld.Append("Go")
    '    strFunctionText.Append(strBld.ToString)
    '    strBld = Nothing
    '    Return strFunctionText.ToString
    'End Function



    <System.Diagnostics.DebuggerStepThrough>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGenerate))
        Me.scMain = New System.Windows.Forms.SplitContainer()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.gbSQL = New System.Windows.Forms.GroupBox()
        Me.txtSQL = New System.Windows.Forms.TextBox()
        Me.btnLoadSQL = New System.Windows.Forms.Button()
        Me.lstDetails = New System.Windows.Forms.ListBox()
        Me.cboTypes = New System.Windows.Forms.ComboBox()
        Me.lblUser = New System.Windows.Forms.Label()
        Me.cmbDatabase = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.scMain2 = New System.Windows.Forms.SplitContainer()
        Me.dgvDetails = New System.Windows.Forms.DataGridView()
        Me.chkColumn = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.tcMain = New System.Windows.Forms.TabControl()
        Me.tpSQL = New System.Windows.Forms.TabPage()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.chkDelete = New System.Windows.Forms.CheckBox()
        Me.btnGenFunction = New System.Windows.Forms.Button()
        Me.chkSelect = New System.Windows.Forms.CheckBox()
        Me.chkInsert = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tpHTML = New System.Windows.Forms.TabPage()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.chkEditable = New System.Windows.Forms.CheckBox()
        Me.btnHTML = New System.Windows.Forms.Button()
        Me.chkJson = New System.Windows.Forms.CheckBox()
        Me.chkButtons = New System.Windows.Forms.CheckBox()
        Me.chkUseTable = New System.Windows.Forms.CheckBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.cboUIFramework = New System.Windows.Forms.ComboBox()
        Me.GroupBox7 = New System.Windows.Forms.GroupBox()
        Me.cboJSFramework = New System.Windows.Forms.ComboBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.cboPageType = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.tpClass = New System.Windows.Forms.TabPage()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.chkUseAutoProp = New System.Windows.Forms.CheckBox()
        Me.btnEntity = New System.Windows.Forms.Button()
        Me.cboClassLang = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.grpItems = New System.Windows.Forms.GroupBox()
        CType(Me.scMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.scMain.Panel1.SuspendLayout()
        Me.scMain.Panel2.SuspendLayout()
        Me.scMain.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.gbSQL.SuspendLayout()
        CType(Me.scMain2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.scMain2.Panel1.SuspendLayout()
        Me.scMain2.Panel2.SuspendLayout()
        Me.scMain2.SuspendLayout()
        CType(Me.dgvDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tcMain.SuspendLayout()
        Me.tpSQL.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.tpHTML.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox7.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.tpClass.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.grpItems.SuspendLayout()
        Me.SuspendLayout()
        '
        'scMain
        '
        Me.scMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.scMain.Location = New System.Drawing.Point(0, 0)
        Me.scMain.Name = "scMain"
        '
        'scMain.Panel1
        '
        Me.scMain.Panel1.Controls.Add(Me.GroupBox2)
        '
        'scMain.Panel2
        '
        Me.scMain.Panel2.Controls.Add(Me.scMain2)
        Me.scMain.Size = New System.Drawing.Size(1256, 715)
        Me.scMain.SplitterDistance = 213
        Me.scMain.SplitterWidth = 10
        Me.scMain.TabIndex = 4
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.grpItems)
        Me.GroupBox2.Controls.Add(Me.gbSQL)
        Me.GroupBox2.Controls.Add(Me.cboTypes)
        Me.GroupBox2.Controls.Add(Me.lblUser)
        Me.GroupBox2.Controls.Add(Me.cmbDatabase)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox2.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.GroupBox2.Size = New System.Drawing.Size(213, 715)
        Me.GroupBox2.TabIndex = 15
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Server Info"
        '
        'gbSQL
        '
        Me.gbSQL.Controls.Add(Me.txtSQL)
        Me.gbSQL.Controls.Add(Me.btnLoadSQL)
        Me.gbSQL.Location = New System.Drawing.Point(8, 118)
        Me.gbSQL.Name = "gbSQL"
        Me.gbSQL.Size = New System.Drawing.Size(200, 133)
        Me.gbSQL.TabIndex = 18
        Me.gbSQL.TabStop = False
        Me.gbSQL.Text = "SQL Statement"
        '
        'txtSQL
        '
        Me.txtSQL.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtSQL.Location = New System.Drawing.Point(3, 16)
        Me.txtSQL.Multiline = True
        Me.txtSQL.Name = "txtSQL"
        Me.txtSQL.Size = New System.Drawing.Size(194, 91)
        Me.txtSQL.TabIndex = 17
        '
        'btnLoadSQL
        '
        Me.btnLoadSQL.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.btnLoadSQL.Location = New System.Drawing.Point(3, 107)
        Me.btnLoadSQL.Name = "btnLoadSQL"
        Me.btnLoadSQL.Size = New System.Drawing.Size(194, 23)
        Me.btnLoadSQL.TabIndex = 18
        Me.btnLoadSQL.Text = "Load SQL"
        Me.btnLoadSQL.UseVisualStyleBackColor = True
        '
        'lstDetails
        '
        Me.lstDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstDetails.FormattingEnabled = True
        Me.lstDetails.Location = New System.Drawing.Point(3, 16)
        Me.lstDetails.Name = "lstDetails"
        Me.lstDetails.Size = New System.Drawing.Size(194, 298)
        Me.lstDetails.TabIndex = 15
        '
        'cboTypes
        '
        Me.cboTypes.Dock = System.Windows.Forms.DockStyle.Top
        Me.cboTypes.FormattingEnabled = True
        Me.cboTypes.Items.AddRange(New Object() {"Tables", "Views", "SQL"})
        Me.cboTypes.Location = New System.Drawing.Point(5, 69)
        Me.cboTypes.Name = "cboTypes"
        Me.cboTypes.Size = New System.Drawing.Size(203, 21)
        Me.cboTypes.TabIndex = 16
        '
        'lblUser
        '
        Me.lblUser.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblUser.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUser.Location = New System.Drawing.Point(5, 53)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(203, 16)
        Me.lblUser.TabIndex = 11
        Me.lblUser.Text = "Object Type"
        '
        'cmbDatabase
        '
        Me.cmbDatabase.Dock = System.Windows.Forms.DockStyle.Top
        Me.cmbDatabase.Location = New System.Drawing.Point(5, 32)
        Me.cmbDatabase.Name = "cmbDatabase"
        Me.cmbDatabase.Size = New System.Drawing.Size(203, 21)
        Me.cmbDatabase.TabIndex = 10
        '
        'Label2
        '
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label2.Location = New System.Drawing.Point(5, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(203, 16)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Select Database"
        '
        'scMain2
        '
        Me.scMain2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.scMain2.Location = New System.Drawing.Point(0, 0)
        Me.scMain2.Name = "scMain2"
        '
        'scMain2.Panel1
        '
        Me.scMain2.Panel1.Controls.Add(Me.dgvDetails)
        Me.scMain2.Panel1.Padding = New System.Windows.Forms.Padding(10)
        '
        'scMain2.Panel2
        '
        Me.scMain2.Panel2.BackColor = System.Drawing.Color.SlateGray
        Me.scMain2.Panel2.Controls.Add(Me.tcMain)
        Me.scMain2.Panel2.Padding = New System.Windows.Forms.Padding(5)
        Me.scMain2.Size = New System.Drawing.Size(1033, 715)
        Me.scMain2.SplitterDistance = 706
        Me.scMain2.TabIndex = 17
        '
        'dgvDetails
        '
        Me.dgvDetails.AllowUserToAddRows = False
        Me.dgvDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvDetails.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.chkColumn})
        Me.dgvDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvDetails.Location = New System.Drawing.Point(10, 10)
        Me.dgvDetails.Name = "dgvDetails"
        Me.dgvDetails.RowHeadersVisible = False
        Me.dgvDetails.Size = New System.Drawing.Size(686, 695)
        Me.dgvDetails.TabIndex = 5
        '
        'chkColumn
        '
        Me.chkColumn.Frozen = True
        Me.chkColumn.HeaderText = ""
        Me.chkColumn.Name = "chkColumn"
        Me.chkColumn.Width = 20
        '
        'tcMain
        '
        Me.tcMain.Controls.Add(Me.tpSQL)
        Me.tcMain.Controls.Add(Me.tpHTML)
        Me.tcMain.Controls.Add(Me.tpClass)
        Me.tcMain.Dock = System.Windows.Forms.DockStyle.Top
        Me.tcMain.Location = New System.Drawing.Point(5, 5)
        Me.tcMain.Name = "tcMain"
        Me.tcMain.SelectedIndex = 0
        Me.tcMain.Size = New System.Drawing.Size(313, 401)
        Me.tcMain.TabIndex = 25
        '
        'tpSQL
        '
        Me.tpSQL.Controls.Add(Me.GroupBox5)
        Me.tpSQL.Controls.Add(Me.Label1)
        Me.tpSQL.Location = New System.Drawing.Point(4, 22)
        Me.tpSQL.Name = "tpSQL"
        Me.tpSQL.Padding = New System.Windows.Forms.Padding(3)
        Me.tpSQL.Size = New System.Drawing.Size(305, 375)
        Me.tpSQL.TabIndex = 0
        Me.tpSQL.Text = "SQL"
        Me.tpSQL.UseVisualStyleBackColor = True
        '
        'GroupBox5
        '
        Me.GroupBox5.AutoSize = True
        Me.GroupBox5.Controls.Add(Me.chkDelete)
        Me.GroupBox5.Controls.Add(Me.btnGenFunction)
        Me.GroupBox5.Controls.Add(Me.chkSelect)
        Me.GroupBox5.Controls.Add(Me.chkInsert)
        Me.GroupBox5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox5.Location = New System.Drawing.Point(3, 3)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(299, 318)
        Me.GroupBox5.TabIndex = 23
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "SQL"
        '
        'chkDelete
        '
        Me.chkDelete.AutoSize = True
        Me.chkDelete.Dock = System.Windows.Forms.DockStyle.Top
        Me.chkDelete.Enabled = False
        Me.chkDelete.Location = New System.Drawing.Point(3, 50)
        Me.chkDelete.Name = "chkDelete"
        Me.chkDelete.Padding = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.chkDelete.Size = New System.Drawing.Size(293, 17)
        Me.chkDelete.TabIndex = 21
        Me.chkDelete.Text = "Delete"
        Me.chkDelete.UseVisualStyleBackColor = True
        '
        'btnGenFunction
        '
        Me.btnGenFunction.BackColor = System.Drawing.Color.LightSalmon
        Me.btnGenFunction.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.btnGenFunction.Location = New System.Drawing.Point(3, 291)
        Me.btnGenFunction.Name = "btnGenFunction"
        Me.btnGenFunction.Size = New System.Drawing.Size(293, 24)
        Me.btnGenFunction.TabIndex = 10
        Me.btnGenFunction.Text = "Generate SQL"
        Me.btnGenFunction.UseVisualStyleBackColor = False
        '
        'chkSelect
        '
        Me.chkSelect.AutoSize = True
        Me.chkSelect.Dock = System.Windows.Forms.DockStyle.Top
        Me.chkSelect.Location = New System.Drawing.Point(3, 33)
        Me.chkSelect.Name = "chkSelect"
        Me.chkSelect.Padding = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.chkSelect.Size = New System.Drawing.Size(293, 17)
        Me.chkSelect.TabIndex = 18
        Me.chkSelect.Text = "Select"
        Me.chkSelect.UseVisualStyleBackColor = True
        '
        'chkInsert
        '
        Me.chkInsert.AutoSize = True
        Me.chkInsert.Dock = System.Windows.Forms.DockStyle.Top
        Me.chkInsert.Location = New System.Drawing.Point(3, 16)
        Me.chkInsert.Name = "chkInsert"
        Me.chkInsert.Padding = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.chkInsert.Size = New System.Drawing.Size(293, 17)
        Me.chkInsert.TabIndex = 19
        Me.chkInsert.Text = "Insert/Update"
        Me.chkInsert.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(3, 321)
        Me.Label1.Name = "Label1"
        Me.Label1.Padding = New System.Windows.Forms.Padding(2)
        Me.Label1.Size = New System.Drawing.Size(299, 51)
        Me.Label1.TabIndex = 22
        Me.Label1.Text = "Use these checkboxes to determine what sql script to generate."
        '
        'tpHTML
        '
        Me.tpHTML.Controls.Add(Me.GroupBox4)
        Me.tpHTML.Controls.Add(Me.Label3)
        Me.tpHTML.Location = New System.Drawing.Point(4, 22)
        Me.tpHTML.Name = "tpHTML"
        Me.tpHTML.Padding = New System.Windows.Forms.Padding(3)
        Me.tpHTML.Size = New System.Drawing.Size(305, 375)
        Me.tpHTML.TabIndex = 1
        Me.tpHTML.Text = "HTML"
        Me.tpHTML.UseVisualStyleBackColor = True
        '
        'GroupBox4
        '
        Me.GroupBox4.AutoSize = True
        Me.GroupBox4.Controls.Add(Me.chkEditable)
        Me.GroupBox4.Controls.Add(Me.btnHTML)
        Me.GroupBox4.Controls.Add(Me.chkJson)
        Me.GroupBox4.Controls.Add(Me.chkButtons)
        Me.GroupBox4.Controls.Add(Me.chkUseTable)
        Me.GroupBox4.Controls.Add(Me.GroupBox1)
        Me.GroupBox4.Controls.Add(Me.GroupBox7)
        Me.GroupBox4.Controls.Add(Me.GroupBox3)
        Me.GroupBox4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox4.Location = New System.Drawing.Point(3, 3)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Padding = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.GroupBox4.Size = New System.Drawing.Size(299, 329)
        Me.GroupBox4.TabIndex = 18
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "HTML Options"
        '
        'chkEditable
        '
        Me.chkEditable.AutoSize = True
        Me.chkEditable.Dock = System.Windows.Forms.DockStyle.Top
        Me.chkEditable.Location = New System.Drawing.Point(5, 195)
        Me.chkEditable.Name = "chkEditable"
        Me.chkEditable.Size = New System.Drawing.Size(289, 17)
        Me.chkEditable.TabIndex = 21
        Me.chkEditable.Text = "Is Editable"
        Me.chkEditable.UseVisualStyleBackColor = True
        '
        'btnHTML
        '
        Me.btnHTML.BackColor = System.Drawing.Color.SpringGreen
        Me.btnHTML.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.btnHTML.Location = New System.Drawing.Point(5, 298)
        Me.btnHTML.Name = "btnHTML"
        Me.btnHTML.Size = New System.Drawing.Size(289, 28)
        Me.btnHTML.TabIndex = 15
        Me.btnHTML.Text = "Generate HTML"
        Me.btnHTML.UseVisualStyleBackColor = False
        '
        'chkJson
        '
        Me.chkJson.AutoSize = True
        Me.chkJson.Dock = System.Windows.Forms.DockStyle.Top
        Me.chkJson.Location = New System.Drawing.Point(5, 178)
        Me.chkJson.Name = "chkJson"
        Me.chkJson.Size = New System.Drawing.Size(289, 17)
        Me.chkJson.TabIndex = 2
        Me.chkJson.Text = "Add JSON Object"
        Me.chkJson.UseVisualStyleBackColor = True
        '
        'chkButtons
        '
        Me.chkButtons.AutoSize = True
        Me.chkButtons.Dock = System.Windows.Forms.DockStyle.Top
        Me.chkButtons.Location = New System.Drawing.Point(5, 161)
        Me.chkButtons.Name = "chkButtons"
        Me.chkButtons.Size = New System.Drawing.Size(289, 17)
        Me.chkButtons.TabIndex = 1
        Me.chkButtons.Text = "Add Buttons"
        Me.chkButtons.UseVisualStyleBackColor = True
        '
        'chkUseTable
        '
        Me.chkUseTable.AutoSize = True
        Me.chkUseTable.Dock = System.Windows.Forms.DockStyle.Top
        Me.chkUseTable.Location = New System.Drawing.Point(5, 144)
        Me.chkUseTable.Name = "chkUseTable"
        Me.chkUseTable.Size = New System.Drawing.Size(289, 17)
        Me.chkUseTable.TabIndex = 17
        Me.chkUseTable.Text = "Use HTML Table"
        Me.chkUseTable.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.cboUIFramework)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox1.Location = New System.Drawing.Point(5, 104)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(289, 40)
        Me.GroupBox1.TabIndex = 19
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "UI Framework"
        '
        'cboUIFramework
        '
        Me.cboUIFramework.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cboUIFramework.FormattingEnabled = True
        Me.cboUIFramework.Items.AddRange(New Object() {"JQuery UI", "Semantic-UI", "Bootstrap"})
        Me.cboUIFramework.Location = New System.Drawing.Point(3, 16)
        Me.cboUIFramework.Name = "cboUIFramework"
        Me.cboUIFramework.Size = New System.Drawing.Size(283, 21)
        Me.cboUIFramework.TabIndex = 16
        '
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.cboJSFramework)
        Me.GroupBox7.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox7.Location = New System.Drawing.Point(5, 64)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Size = New System.Drawing.Size(289, 40)
        Me.GroupBox7.TabIndex = 22
        Me.GroupBox7.TabStop = False
        Me.GroupBox7.Text = "JS Framework"
        '
        'cboJSFramework
        '
        Me.cboJSFramework.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cboJSFramework.FormattingEnabled = True
        Me.cboJSFramework.Items.AddRange(New Object() {"None", "AngularJS"})
        Me.cboJSFramework.Location = New System.Drawing.Point(3, 16)
        Me.cboJSFramework.Name = "cboJSFramework"
        Me.cboJSFramework.Size = New System.Drawing.Size(283, 21)
        Me.cboJSFramework.TabIndex = 16
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.cboPageType)
        Me.GroupBox3.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox3.Location = New System.Drawing.Point(5, 16)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(289, 48)
        Me.GroupBox3.TabIndex = 20
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Page Type"
        '
        'cboPageType
        '
        Me.cboPageType.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cboPageType.FormattingEnabled = True
        Me.cboPageType.Items.AddRange(New Object() {"List", "Edit", "Detail"})
        Me.cboPageType.Location = New System.Drawing.Point(3, 16)
        Me.cboPageType.Name = "cboPageType"
        Me.cboPageType.Size = New System.Drawing.Size(283, 21)
        Me.cboPageType.TabIndex = 18
        '
        'Label3
        '
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(3, 332)
        Me.Label3.Name = "Label3"
        Me.Label3.Padding = New System.Windows.Forms.Padding(2)
        Me.Label3.Size = New System.Drawing.Size(299, 40)
        Me.Label3.TabIndex = 23
        Me.Label3.Text = "Use these checkboxes to add CRUD Buttons,Create JSON Onject, or Jquery UI css."
        '
        'tpClass
        '
        Me.tpClass.Controls.Add(Me.GroupBox6)
        Me.tpClass.Controls.Add(Me.Label4)
        Me.tpClass.Location = New System.Drawing.Point(4, 22)
        Me.tpClass.Name = "tpClass"
        Me.tpClass.Padding = New System.Windows.Forms.Padding(3)
        Me.tpClass.Size = New System.Drawing.Size(305, 375)
        Me.tpClass.TabIndex = 2
        Me.tpClass.Text = "Class"
        Me.tpClass.UseVisualStyleBackColor = True
        '
        'GroupBox6
        '
        Me.GroupBox6.AutoSize = True
        Me.GroupBox6.Controls.Add(Me.chkUseAutoProp)
        Me.GroupBox6.Controls.Add(Me.btnEntity)
        Me.GroupBox6.Controls.Add(Me.cboClassLang)
        Me.GroupBox6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox6.Location = New System.Drawing.Point(3, 3)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Padding = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.GroupBox6.Size = New System.Drawing.Size(299, 320)
        Me.GroupBox6.TabIndex = 24
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Class Language"
        '
        'chkUseAutoProp
        '
        Me.chkUseAutoProp.AutoSize = True
        Me.chkUseAutoProp.Dock = System.Windows.Forms.DockStyle.Top
        Me.chkUseAutoProp.Location = New System.Drawing.Point(5, 37)
        Me.chkUseAutoProp.Name = "chkUseAutoProp"
        Me.chkUseAutoProp.Size = New System.Drawing.Size(289, 17)
        Me.chkUseAutoProp.TabIndex = 13
        Me.chkUseAutoProp.Text = "Use Auto Property"
        Me.chkUseAutoProp.UseVisualStyleBackColor = True
        '
        'btnEntity
        '
        Me.btnEntity.BackColor = System.Drawing.Color.LightGoldenrodYellow
        Me.btnEntity.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.btnEntity.Location = New System.Drawing.Point(5, 289)
        Me.btnEntity.Name = "btnEntity"
        Me.btnEntity.Size = New System.Drawing.Size(289, 28)
        Me.btnEntity.TabIndex = 11
        Me.btnEntity.Text = "Generate Class"
        Me.btnEntity.UseVisualStyleBackColor = False
        '
        'cboClassLang
        '
        Me.cboClassLang.Dock = System.Windows.Forms.DockStyle.Top
        Me.cboClassLang.FormattingEnabled = True
        Me.cboClassLang.Items.AddRange(New Object() {"VB", "C#"})
        Me.cboClassLang.Location = New System.Drawing.Point(5, 16)
        Me.cboClassLang.Name = "cboClassLang"
        Me.cboClassLang.Size = New System.Drawing.Size(289, 21)
        Me.cboClassLang.TabIndex = 12
        '
        'Label4
        '
        Me.Label4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(3, 323)
        Me.Label4.Name = "Label4"
        Me.Label4.Padding = New System.Windows.Forms.Padding(2)
        Me.Label4.Size = New System.Drawing.Size(299, 49)
        Me.Label4.TabIndex = 23
        Me.Label4.Text = "Generate a class object to use with sql or html."
        '
        'grpItems
        '
        Me.grpItems.Controls.Add(Me.lstDetails)
        Me.grpItems.Location = New System.Drawing.Point(10, 257)
        Me.grpItems.Name = "grpItems"
        Me.grpItems.Size = New System.Drawing.Size(200, 317)
        Me.grpItems.TabIndex = 19
        Me.grpItems.TabStop = False
        Me.grpItems.Text = "Objects"
        Me.grpItems.Visible = False
        '
        'frmGenerate
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(1256, 715)
        Me.Controls.Add(Me.scMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmGenerate"
        Me.Text = "SQL Code Generator"
        Me.scMain.Panel1.ResumeLayout(False)
        Me.scMain.Panel2.ResumeLayout(False)
        CType(Me.scMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.scMain.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.gbSQL.ResumeLayout(False)
        Me.gbSQL.PerformLayout()
        Me.scMain2.Panel1.ResumeLayout(False)
        Me.scMain2.Panel2.ResumeLayout(False)
        CType(Me.scMain2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.scMain2.ResumeLayout(False)
        CType(Me.dgvDetails, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tcMain.ResumeLayout(False)
        Me.tpSQL.ResumeLayout(False)
        Me.tpSQL.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.tpHTML.ResumeLayout(False)
        Me.tpHTML.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox7.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.tpClass.ResumeLayout(False)
        Me.tpClass.PerformLayout()
        Me.GroupBox6.ResumeLayout(False)
        Me.GroupBox6.PerformLayout()
        Me.grpItems.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Private Sub loadObjectTypes()
        lstDetails.Items.Clear()

        Select Case cboTypes.SelectedItem.ToString.ToLower
            Case "tables", "views"
                If Not IsNothing(dsDetails.Tables("Tables")) Then

                    chkInsert.Enabled = True
                    chkDelete.Enabled = True

                    For Each dr As DataRow In dsDetails.Tables("Tables").Rows
                        lstDetails.Items.Add(dr("TABLE_NAME").ToString)
                    Next
                End If

                grpItems.Visible = True
                grpItems.Dock = DockStyle.Fill


                gbSQL.Dock = DockStyle.None
                gbSQL.Visible = False

            Case "procedures"
                If Not IsNothing(dsDetails.Tables("StoredProcs")) Then
                    chkInsert.Enabled = False
                    chkDelete.Enabled = False

                    For Each dr As DataRow In dsDetails.Tables("StoredProcs").Rows
                        Dim procName As String = dr("PROCEDURE_NAME").ToString
                        Dim procOwner As String = dr("PROCEDURE_OWNER").ToString
                        If Not procOwner.ToLower = "sys" Then
                            lstDetails.Items.Add(dr("PROCEDURE_NAME").ToString)
                        End If
                    Next

                End If

                grpItems.Visible = True
                grpItems.Dock = DockStyle.Fill


                gbSQL.Dock = DockStyle.None
                gbSQL.Visible = False

            Case "sql"
                gbSQL.Dock = DockStyle.Fill
                gbSQL.Visible = True

                grpItems.Visible = False
                grpItems.Dock = DockStyle.None

                tcMain.SelectTab(2)
                tpSQL.Enabled = False
        End Select
    End Sub

    Private Sub lstDetails_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Select Case cboTypes.SelectedItem.ToString.ToLower
            Case "tables"
                PopulateColumns(1)
            Case "procedures"
                PopulateColumns(2)
        End Select
    End Sub

    Private Sub PopulateColumns(ByVal flag As Integer)
        Dim i, j As Integer
        Dim blnNotPrimary As Boolean

        dsColumns = New DataSet

        Select Case flag
            Case 1 ' Table columns
                Try
                    Dim dtCol As DataTable = DataHelper.GetDataTable(String.Format("sp_columns @table_name='{0}'", lstDetails.SelectedItem), CommandType.Text)
                    dtCol.TableName = "Tables"
                    dsColumns.Tables.Add(dtCol)


                    Dim dtKeys As DataTable = DataHelper.GetDataTable(String.Format("sp_pkeys @table_name='{0}'", lstDetails.SelectedItem), CommandType.Text)
                    dtKeys.TableName = "Pkeys"
                    dsColumns.Tables.Add(dtKeys)


                    dsColumns.Tables("Tables").Columns.Add(New DataColumn("Primary_key"))
                    dsColumns.Tables("Tables").Columns("Primary_key").DefaultValue = "No"
                    intGridRowCount = dsColumns.Tables("Tables").Rows.Count

                    If dsColumns.Tables("Pkeys").Rows.Count <= 0 Then
                        For i = 0 To dsColumns.Tables("Tables").Rows.Count - 1
                            dsColumns.Tables("Tables").Rows(i).Item("Primary_key") = "NO"
                        Next
                    Else
                        For i = 0 To dsColumns.Tables("Tables").Rows.Count - 1
                            blnNotPrimary = True
                            For j = 0 To dsColumns.Tables("Pkeys").Rows.Count - 1
                                If dsColumns.Tables("Tables").Rows(i).Item("COLUMN_NAME") = dsColumns.Tables("Pkeys").Rows(j).Item("COLUMN_NAME") Then
                                    dsColumns.Tables("Tables").Rows(i).Item("Primary_key") = "YES"
                                    blnNotPrimary = False
                                    Exit For
                                End If
                                If blnNotPrimary Then
                                    dsColumns.Tables("Tables").Rows(i).Item("Primary_key") = "NO"
                                End If
                            Next
                        Next
                    End If
                    'If Not dgDetails.TableStyles.Contains(MyTableStyle) Then dgDetails.TableStyles.Add(MyTableStyle)

                    dgvDetails.DataSource = dsColumns.Tables("Tables")

                    Dim validColumns As New List(Of String) From {"COLUMN_NAME", "TYPE_NAME", "PRECISION", "IS_NULLABLE", "Primary_key"}

                    For icol As Integer = 1 To dgvDetails.Columns.Count - 1
                        Dim c As DataGridViewColumn = dgvDetails.Columns(icol)
                        Debug.WriteLine(c.Name)
                        If Not validColumns.Contains(c.Name) Then
                            c.Visible = False
                        End If
                    Next

                    dgvDetails.Columns("Column_Name").DisplayIndex = 1
                    dgvDetails.Columns("Column_Name").Frozen = True
                    dgvDetails.Columns("TYPE_NAME").DisplayIndex = 2
                    dgvDetails.Columns("PRECISION").DisplayIndex = 3
                    dgvDetails.Columns("Is_Nullable").DisplayIndex = 4
                    dgvDetails.Columns("Primary_Key").DisplayIndex = 5

                    'Iterate through the datagrid rows and select rows that can't be null
                    If chkInsert.Checked Then
                        For i = 0 To intGridRowCount - 1
                            If Trim(dgvDetails.Item("Is_Nullable", i).Value) = "NO" Then
                                'select this row
                                dgvDetails.Rows(i).Cells(0).Value = True
                            End If
                        Next
                    Else
                        For i = 0 To intGridRowCount - 1
                            If Trim(dgvDetails.Item("Is_Nullable", i).Value) = "NO" Then
                                'select this row
                                dgvDetails.Rows(i).Cells(0).Value = True
                            End If
                        Next
                    End If

                Catch ex As SqlException
                    Throw
                Catch ex As Exception
                    Throw
                Finally

                    SqlParam = Nothing
                End Try

            Case 2
                ' Stored Proc parameters
                Try

                    Dim dtCol As DataTable = DataHelper.GetDataTable(String.Format("sp_sproc_columns @procedure_name='{0}'", lstDetails.SelectedItem), CommandType.Text)
                    dtCol.TableName = "Procedure"
                    dsColumns.Tables.Add(dtCol)

                    'SqlParam = New SqlClient.SqlParameter
                    'SqlParam.Direction = ParameterDirection.Input
                    'SqlParam.DbType = DbType.String
                    'SqlParam.Value = lstDetails.SelectedItem
                    'SqlParam.ParameterName = "@procedure_name"
                    'SqlParam.Size = 390

                    'SQLDataAdapter.SelectCommand.Parameters.Add(SqlParam)
                    'SQLDataAdapter.SelectCommand.CommandText = "sp_sproc_columns"
                    'SQLDataAdapter.Fill(dsColumns, "Procedure")
                    'SQLDataAdapter.SelectCommand.Parameters.RemoveAt(0)
                    intGridRowCount = dsColumns.Tables("Procedure").Rows.Count
                    'If Not dgDetails.TableStyles.Contains(MyProcedureStyle) Then dgDetails.TableStyles.Add(MyProcedureStyle)
                    dgvDetails.DataSource = dsColumns.Tables("Procedure")
                Catch ex As SqlClient.SqlException
                    Throw
                Catch ex As Exception
                    Throw
                Finally
                    SqlParam = Nothing
                End Try

        End Select
    End Sub

    'Created tables styles for Datagrid control.
    Private Sub PrepareTableStyles()
        MyTableStyle.GridColumnStyles.Clear()
        MyProcedureStyle.GridColumnStyles.Clear()

        'Prepare Table Columns style
        Dim colPosition As New DataGridTextBoxColumn
        colPosition.MappingName = "ORDINAL_POSITION"
        colPosition.HeaderText = "S.No"
        colPosition.Width = 50
        MyTableStyle.GridColumnStyles.Add(colPosition)

        Dim colColumnName As New DataGridTextBoxColumn
        colColumnName.MappingName = "COLUMN_NAME"
        colColumnName.HeaderText = "Column Name"
        colColumnName.Width = 200
        MyTableStyle.GridColumnStyles.Add(colColumnName)

        Dim colTypeName As New DataGridTextBoxColumn
        colTypeName.MappingName = "TYPE_NAME"
        colTypeName.HeaderText = "Data Type"
        colTypeName.Width = 100
        MyTableStyle.GridColumnStyles.Add(colTypeName)

        Dim colLength As New DataGridTextBoxColumn
        colLength.MappingName = "PRECISION"
        colLength.HeaderText = "Length"
        colLength.Width = 100
        MyTableStyle.GridColumnStyles.Add(colLength)

        Dim colNullable As New DataGridTextBoxColumn
        colNullable.MappingName = "IS_NULLABLE"
        colNullable.HeaderText = " Allow Nulls"

        MyTableStyle.GridColumnStyles.Add(colNullable)

        Dim colPrimaryKey As New DataGridTextBoxColumn
        colPrimaryKey.MappingName = "Primary_key"
        colPrimaryKey.HeaderText = " Primary Key"
        MyTableStyle.GridColumnStyles.Add(colPrimaryKey)

        MyTableStyle.MappingName = "Tables"

        'Table style for stored procedure parameters
        Dim paramPosition As New DataGridTextBoxColumn
        paramPosition.MappingName = "ORDINAL_POSITION"
        paramPosition.HeaderText = "S.No"
        paramPosition.Width = 50
        MyProcedureStyle.GridColumnStyles.Add(paramPosition)

        Dim pramColumnName As New DataGridTextBoxColumn
        pramColumnName.MappingName = "COLUMN_NAME"
        pramColumnName.HeaderText = "Column Name"
        pramColumnName.Width = 200
        MyProcedureStyle.GridColumnStyles.Add(pramColumnName)

        Dim ParamTypeName As New DataGridTextBoxColumn
        ParamTypeName.MappingName = "TYPE_NAME"
        ParamTypeName.HeaderText = "Data Type"
        ParamTypeName.Width = 100
        MyProcedureStyle.GridColumnStyles.Add(ParamTypeName)

        Dim paramLength As New DataGridTextBoxColumn
        paramLength.MappingName = "PRECISION"
        paramLength.HeaderText = "Length"
        paramLength.Width = 100
        MyProcedureStyle.GridColumnStyles.Add(paramLength)

        MyProcedureStyle.MappingName = "Procedure"
    End Sub

    Private Sub rgStpType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        If chkSelect.Checked Then
            For i As Integer = 0 To intGridRowCount - 1
                Try

                    'select this row
                    dgvDetails.Rows(i).Cells(0).Value = True

                Catch ex As Exception
                    Continue For
                End Try

            Next

        ElseIf chkInsert.Checked Then
            For i As Integer = 0 To intGridRowCount - 1
                Try
                    If Trim(dgvDetails.Item(i, 4).Value) = "NO" Then 'Not null columns
                        'select this row
                        dgvDetails.Rows(i).Cells(0).Value = True
                    Else
                        dgvDetails.Rows(i).Cells(0).Value = False
                    End If
                Catch ex As Exception
                    Continue For
                End Try
            Next
        Else
            For i As Integer = 0 To intGridRowCount - 1
                Try
                    If Trim(dgvDetails.Item(i, 4).Value) = "YES" Then 'Primary key columns
                        'select this row
                        dgvDetails.Rows(i).Cells(0).Value = True
                    Else
                        dgvDetails.Rows(i).Cells(0).Value = False
                    End If
                Catch ex As Exception
                    Continue For
                End Try

            Next
        End If
    End Sub

    Private Sub btnLoadSQL_Click(sender As Object, e As EventArgs) Handles btnLoadSQL.Click
        Try
            'change command text to system stored procedure
            SQLDataAdapter.SelectCommand.CommandType = CommandType.Text
            SQLDataAdapter.SelectCommand.CommandText = txtSQL.Text

            'Dim dt As New DataTable
            'SQLDataAdapter.Fill(dt)

            If dsDetails.Tables.Contains("SQLScript") Then dsDetails.Tables.Remove("SQLScript")


            'Get tables from selected datebase
            SQLDataAdapter.Fill(dsDetails, "SQLScript")

            If dsDetails.Tables("SQLScript").Rows.Count = 0 Then
                lstDetails.DataSource = Nothing
                Throw New ApplicationException("There are no stored procedures in the selected database")
            Else
                Dim dt As New DataTable
                dt.Columns.Add("COLUMN_NAME")
                dt.Columns.Add("TYPE_NAME")
                dt.Columns.Add("PRECISION")
                dt.Columns.Add("IS_NULLABLE")
                dt.Columns.Add("Primary_key")

                For Each dc As DataColumn In dsDetails.Tables("SQLScript").Columns
                    Dim dr As DataRow = dt.NewRow
                    dr("COLUMN_Name") = dc.ColumnName
                    dr("TYPE_NAME") = GetVBDataType(dc.DataType.ToString)
                    dt.Rows.Add(dr)
                Next
                dgvDetails.DataSource = dt


                For i As Integer = 0 To dgvDetails.Rows.Count - 1
                    'select this row
                    dgvDetails.Rows(i).Cells(0).Value = True
                Next


            End If

        Catch ex As SqlClient.SqlException
            Throw
        Catch ex As Exception
            Throw
        End Try
    End Sub

#End Region 'Methods

End Class