'Prgrammer : Raj Settipalli
'Date      : 01.02.2005
'Purpose   : To list SQL Servers available on the network using NetServerEnum API
Imports System.Runtime.InteropServices
Imports System.Text
Public Class clsSQLServers
    Implements IDisposable

    Private alstServers As ArrayList

    Private Const SV_TYPE_SQLSERVER As Integer = 4 ' This constant is to denote SQL Servers.
    'Other servers are domain controllers, Windows NT workstations, Unix etc available on the network
    Private Const PLATFORM_ID_NT As Integer = 500 'Denotes NT platform, other possibilities are OS2, DOS etc

    'Private structure which will be returned by NetServerEnum API function
    'stored in ServerInfo Structure
    Private Structure SERVER_INFO_101

        Dim PlatformID As Integer
        Dim Name As IntPtr     'Pointer to string
        Dim VersionMajor As Integer
        Dim VersionMinor As Integer
        Dim Type As Integer
        Dim Comment As IntPtr   'Pointer to string

    End Structure

    Public Class ServerInfo
        Public _servername As String
        Public PlatformId As Int32
        Public ReadOnly Property ServerName() As String
            Get
                Return _servername
            End Get
        End Property
        Public VersionMajor As Integer
        Public VersionMinor As Integer
        Public type As Integer
        Public Comment As String

    End Class

    Private strSQLServers() As String ' String array to store SQL Servers
    Private intAvailableEntries As Integer ' Number of entries read

    'API call to deallicate buffer
    Private Declare Auto Function NetApiBufferFree Lib "netapi32.dll" (ByVal bufptr As IntPtr) As Integer

    'NetServerEnum PInvoke function declaration
    Private Declare Auto Function NetServerEnum Lib "netapi32.dll" _
                                            (ByVal nullptr As Integer, _
                                            ByVal Level As Integer, _
                                            ByRef BufPtr As IntPtr, _
                                            ByVal BufMaxLen As Integer, _
                                            ByRef EntriesRead As Integer, _
                                            ByRef TotalEntries As Integer, _
                                            ByVal ServerType As Integer, _
                                            ByVal Domain As IntPtr, _
                                            ByVal ResumeHandle As Integer) As Integer

    Private Declare Function lstrlenW Lib "kernel32" (ByVal lpString As Int32) As Int32

    'To Read error messages
    Private strLastError As String

    Public ReadOnly Property LastError() As String
        Get
            Return strLastError
        End Get
    End Property

    Public ReadOnly Property NumMachines() As Integer
        Get
            Return intAvailableEntries
        End Get
    End Property

    Public ReadOnly Property SQLServers() As ArrayList
        Get
            Return alstServers
        End Get
    End Property

    Public Function GetSQLServers(Optional ByVal ServerType As Integer = SV_TYPE_SQLSERVER, Optional ByVal strDomainName As String = "") As Boolean

        Dim intResult As Integer
        Dim lptrBuffer As IntPtr ' long pointer to buffer
        Const intLevel As Integer = 101
        Dim intCount As Integer
        Dim intIterator As Integer
        Dim PtrDomain As IntPtr 'Pointer to Domain String
        Dim intSize As Integer
        Dim iPtrNext As IntPtr
        Dim stuctServerInfo As SERVER_INFO_101
        Dim objServer As ServerInfo

        strLastError = String.Empty

        If strDomainName = "" Or IsNothing(strDomainName) Then
            'Pointer to null
            PtrDomain = New IntPtr(0)
        Else
            'Converting string domain name into pointer pointing to null terminated string
            PtrDomain = Marshal.StringToBSTR(strDomainName & Chr(0))
        End If

        'Get servers
        intResult = NetServerEnum(0&, intLevel, lptrBuffer, -1, intAvailableEntries, intCount, SV_TYPE_SQLSERVER, PtrDomain, 0)

        'Release string pointer
        Marshal.FreeBSTR(PtrDomain)
        If intResult = 0& Then ' Function call is successful

            intSize = Marshal.SizeOf(stuctServerInfo)
            alstServers = New ArrayList
            Try
                For intIterator = 0 To intAvailableEntries - 1
                    'Get Pointer to next structure
                    iPtrNext = New IntPtr(lptrBuffer.ToInt32 + (intSize * intIterator))
                    'Convert structure pointer to structure
                    stuctServerInfo = Marshal.PtrToStructure(iPtrNext, GetType(SERVER_INFO_101))
                    objServer = New ServerInfo
                    objServer._servername = Marshal.PtrToStringUni(stuctServerInfo.Name)
                    objServer.Comment = Marshal.PtrToStringUni(stuctServerInfo.Comment)
                    objServer.PlatformId = stuctServerInfo.PlatformID
                    objServer.type = stuctServerInfo.Type
                    objServer.VersionMajor = stuctServerInfo.VersionMajor
                    objServer.VersionMinor = stuctServerInfo.VersionMinor
                    alstServers.Add(objServer)
                Next
            Catch ex As Exception

                strLastError = ex.ToString + vbCrLf + ex.Message
            Finally
                NetApiBufferFree(lptrBuffer)
            End Try

        Else

            strLastError = "PInvoke call failed"
            Return False

        End If

        Return True

    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If
            alstServers = Nothing
            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
