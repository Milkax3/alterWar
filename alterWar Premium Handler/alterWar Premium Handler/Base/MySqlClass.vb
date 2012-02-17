Imports MySql.Data.MySqlClient

Namespace Base
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
                _SqlConnection = New MySqlConnection("Server=" & ServerIP & ";Port=" & Port & ";Uid=" & Username & ";Pwd=" & Password & ";Database=" & Database)
                _SqlConnection.Open()

                If _SqlConnection.State <> ConnectionState.Open Then
                    If _SqlConnection.State = ConnectionState.Broken Then
                        Output("The MySqlClass1 Connection is broken. (Unknown)", ConsoleStyle.Error)
                        Console.ReadKey()
                    ElseIf _SqlConnection.State = ConnectionState.Closed Then
                        Output("The MySqlClass1 Connection is broken. (Closed)", ConsoleStyle.Error)
                        Console.ReadKey()
                    ElseIf _SqlConnection.State = ConnectionState.Open Then
                        Output("Successfully connected to the MySqlClass1 Server (at " & ServerIP & " onto Database " & Database & ")", ConsoleStyle.Good)
                    End If
                End If
            Catch ex As MySqlException
                Output("Can't connect to the MySqlClass1 Server", ConsoleStyle.Error)
                Output(ex.ToString, ConsoleStyle.Info)
                Console.ReadKey()
            End Try
        End Sub
        Public Function Execute(ByVal Command As String) As Integer
            Try
                Dim Result As Integer = 0
                Result = New MySqlCommand(Command, SqlConnection).ExecuteNonQuery()
                _Executes.Add(Command)
                RaiseEvent Executed(Command, eExecuteType.NormalExecute)
                Return Result
            Catch ex As MySqlException
                Output("MYSQL: Execution (""" & Command & """) failed.", ConsoleStyle.Error)
                Output(ex.ToString, ConsoleStyle.Info)
                Console.ReadKey()
            End Try
            Return Nothing
        End Function
        Public Function ExecuteReader(ByVal Command As String) As MySqlDataReader
            Try
                Dim Reader As MySqlDataReader
                Reader = New MySqlCommand(Command, SqlConnection).ExecuteReader()
                RaiseEvent Executed(Command, eExecuteType.NormalExecute)
                _Executes.Add("[ExecuteReader]" & Command)
                Return Reader
            Catch ex As MySqlException
                Output("MYSQL: Reader Execution (""" & Command & """) failed.", ConsoleStyle.Error)
                Output(ex.ToString, ConsoleStyle.Info)
                Console.ReadKey()
            End Try
            Return Nothing
        End Function

        Public Event Executed(ByVal ExecuteString As String, ByVal ExecuteType As eExecuteType)
        Public Enum eExecuteType As Integer
            NormalExecute = 1
            ReaderExecute = 2
        End Enum


        Public Enum ConsoleStyle As Integer
            Normal = 0
            Highlighted = 1
            FlowerPower = 2

            Info = 3
            [Error] = 4
            Good = 5
        End Enum

        Public Sub Output(ByVal Entry As String, ByVal Style As ConsoleStyle)
            Dim ConsoleColorBefore As ConsoleColor = Console.ForegroundColor
            Select Case Style
                Case ConsoleStyle.Normal
                    Console.ForegroundColor = ConsoleColor.White
                    Console.WriteLine(Entry)
                Case ConsoleStyle.Highlighted
                    Console.ForegroundColor = ConsoleColor.Cyan
                    Console.WriteLine(Entry)
                Case ConsoleStyle.FlowerPower
                    Dim CrCol As Integer = 0
                    For i As Integer = 0 To Entry.Length - 1
                        If CrCol = 0 Then Console.ForegroundColor = ConsoleColor.Yellow
                        If CrCol = 1 Then Console.ForegroundColor = ConsoleColor.White
                        If CrCol = 2 Then Console.ForegroundColor = ConsoleColor.Red
                        If CrCol = 3 Then Console.ForegroundColor = ConsoleColor.Magenta
                        If CrCol = 4 Then Console.ForegroundColor = ConsoleColor.Green
                        If CrCol = 5 Then Console.ForegroundColor = ConsoleColor.Cyan
                        If CrCol = 6 Then Console.ForegroundColor = ConsoleColor.Blue
                        Console.Write(Entry.ToCharArray()(i).ToString)
                        CrCol += 1
                        If CrCol >= 7 Then CrCol = 0
                    Next
                    Console.WriteLine()
                Case ConsoleStyle.Info
                    Console.ForegroundColor = ConsoleColor.Yellow
                    Console.WriteLine(Entry)
                Case ConsoleStyle.Error
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine(Entry)
                Case ConsoleStyle.Good
                    Console.ForegroundColor = ConsoleColor.Green
                    Console.WriteLine(Entry)
                Case Else
                    Console.ForegroundColor = ConsoleColor.White
                    Console.WriteLine(Entry)
            End Select
            Console.ForegroundColor = ConsoleColorBefore
        End Sub
    End Class
End Namespace