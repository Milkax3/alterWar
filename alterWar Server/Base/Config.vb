Imports System.Runtime.InteropServices
Imports System.Text


Namespace alterWar.Base

    Public Class ConfigClass

        Private Structure ConfigEntry
            Public Category As String
            Public Name As String
            Public [Default] As String
            Sub New(ByVal _Category As String, ByVal _Name As String, ByVal _Default As String)
                Category = _Category
                Name = _Name
                [Default] = _Default
            End Sub
        End Structure

        <DllImport("kernel32.dll", SetLastError:=True)> _
        Private Shared Function GetPrivateProfileString(ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As StringBuilder, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
        End Function

        Private _Configs As Dictionary(Of String, String)
        Private _ConfigsToRead As List(Of ConfigEntry)
        Public ReadOnly Property GetConfig(ByVal Name As String) As Object
            Get
                If _Configs.ContainsKey(Name) = False AndAlso Name <> "SERVER_DEBUGLEVEL" Then
                    Console.WriteLine("CONFIG NOT FOUND: """ & Name & """")
                    Console.Read()
                End If
                Dim RetValue As String = String.Empty
                _Configs.TryGetValue(Name, RetValue)
                Return RetValue
            End Get
        End Property

        Private _IniFile As String
        Public Property IniFile() As String
            Get
                Return _IniFile
            End Get
            Set(ByVal value As String)
                _IniFile = value
            End Set
        End Property

        Sub New(ByVal IniName As String)
            _IniFile = IO.Path.Combine(IO.Path.GetFullPath(Environment.CurrentDirectory), "Data\" & IniName & ".ini")
            _ConfigsToRead = New List(Of ConfigEntry)
            _Configs = New Dictionary(Of String, String)
        End Sub

        Private Sub AddConfigEntry(ByVal Category As String, ByVal Name As String, Optional ByVal [Default] As String = "")
            _ConfigsToRead.Add(New ConfigEntry(Category, Name, [Default]))
        End Sub

        Public Sub ReadIni()
            Log(LogStyle.Info, "Reading Configuration File [" & _IniFile & "]")
            Try

                AddConfigEntry("SQL", "IP", "127.0.0.1")
                AddConfigEntry("SQL", "PORT", 3306)
                AddConfigEntry("SQL", "USERNAME", "root")
                AddConfigEntry("SQL", "PASSWORD")
                AddConfigEntry("SQL", "DATABASE", "wremu")

                AddConfigEntry("LOGIN", "IP", "0.0.0.0")
                AddConfigEntry("LOGIN", "PORT", 5330)

                'users try to connect to that ip, thats not only for listening
                AddConfigEntry("GAME", "IP", "127.0.0.1")
                AddConfigEntry("GAME", "PORT", 5340)
                AddConfigEntry("GAME", "LIMIT", 32)
                AddConfigEntry("GAME", "NAME", "alterWar")
                AddConfigEntry("GAME", "AIENABLED", "FALSE")

                AddConfigEntry("SERVER", "DEBUGLEVEL", 0)

                Dim tmpString As New StringBuilder(Byte.MaxValue)

                For Each Entry As ConfigEntry In _ConfigsToRead
                    GetPrivateProfileString(Entry.Category, Entry.Name, Entry.[Default], tmpString, tmpString.Capacity, IniFile)
                    _Configs.Add(Entry.Category & "_" & Entry.Name, tmpString.ToString())
                    tmpString.Remove(0, tmpString.Length)
                Next

            Catch ex As Exception
                If IO.File.Exists(IniFile) = False Then
                    Log(LogStyle.Error, "The Config File [" & IniFile & "] does not exist.")
                    Log(LogStyle.Debug, "Create a folder called ""Data"" in the Server Folder and put in a File called ""Server.ini"".")
                Else
                    Log(LogStyle.Error, "Error while reading Config File [" & IniFile & "]. Exception:")
                    Log(LogStyle.Debug, ex.Message)
                End If
            End Try
        End Sub

    End Class
End Namespace