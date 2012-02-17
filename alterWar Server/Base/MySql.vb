Imports MySql.Data.MySqlClient

Namespace alterWar.Base
    Public Class MySqlClass
        Private _SqlConnection As MySqlConnection
        Public ReadOnly Property SqlConnection() As MySqlConnection
            Get
                Return _SqlConnection
            End Get
        End Property

        Private _Executes As List(Of String)
        Public ReadOnly Property Executes() As List(Of String)
            Get
                Return _Executes
            End Get
        End Property

        Public Sub New(ByVal ServerIP As String, ByVal Username As String, ByVal Port As Integer, ByVal Password As String, ByVal Database As String)
            _Executes = New List(Of String)
            Try
                _SqlConnection = New MySqlConnection("Server=" & ServerIP & ";Uid=" & Username & ";Pwd=" & Password & ";Database=" & Database)
                _SqlConnection.Open()

                If _SqlConnection.State <> ConnectionState.Open Then
                    If _SqlConnection.State = ConnectionState.Broken Then
                        Log(LogStyle.Info, "The MySql Connection is broken. (Unknown)")
                        Console.ReadKey()
                    ElseIf _SqlConnection.State = ConnectionState.Closed Then
                        Log(LogStyle.Error, "The MySql Connection is broken. (Closed)")
                        Console.ReadKey()
                    ElseIf _SqlConnection.State = ConnectionState.Open Then
                        Log(LogStyle.Info, "Successfully connected to the MySql Server (at " & ServerIP & " onto Database " & Database & ")")
                    End If
                End If
            Catch ex As MySqlException
                Log(LogStyle.Error, "Can't connect to the MySql Server (" & ex.ErrorCode & ")")
                Log(LogStyle.Info, ex.Message)
                Console.ReadKey()
            End Try
        End Sub
        Public Function Execute(ByVal Command As String) As Integer
            Try
                Dim Result As Integer = 0
                Result = New MySqlCommand(Command, SqlConnection).ExecuteNonQuery()
                _Executes.Add(Command)
                Return Result
            Catch ex As MySqlException
                Log(LogStyle.Error, "MYSQL: Execution (""" & Command & """) failed.")
                Log(LogStyle.Info, ex.ToString)
                Console.ReadKey()
            End Try
            Return Nothing
        End Function
        Public Function ExecuteReader(ByVal Command As String) As MySqlDataReader
            Try
                Dim Reader As MySqlDataReader
                Reader = New MySqlCommand(Command, SqlConnection).ExecuteReader()
                _Executes.Add("[ExecuteReader]" & Command)
                Return Reader
            Catch ex As MySqlException
                Log(LogStyle.Error, "MYSQL: Reader Execution (""" & Command & """) failed.")
                Log(LogStyle.Info, ex.ToString)
                Console.ReadKey()
            End Try
            Return Nothing
        End Function
    End Class
End Namespace
