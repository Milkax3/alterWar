Imports clientEMU.Base.Extensions
Imports System.Net.Sockets

Namespace Base
    Public Module Base

        <Runtime.InteropServices.DllImport("winmm.dll")> _
Private Function timeGetTime() As Long
        End Function
        Public Function GetTime() As Long
            Return timeGetTime()
        End Function

        Private _Client As cClient
        Public ReadOnly Property Client() As cClient
            Get
                Return _Client
            End Get
        End Property
        Private _ServerList As List(Of Classes.cServer)
        Public ReadOnly Property ServerList() As List(Of Classes.cServer)
            Get
                Return _ServerList
            End Get
        End Property
        Private _UserData As Classes.cUserData
        Public ReadOnly Property UserData() As Classes.cUserData
            Get
                Return _UserData
            End Get
        End Property
        Private _LoginAccepted As Boolean
        Public ReadOnly Property LoginAccepted() As Boolean
            Get
                Return _LoginAccepted
            End Get
        End Property
        Private _PassCode As String
        Public ReadOnly Property PassCode() As String
            Get
                Return _PassCode
            End Get
        End Property
        Private _GameServerInfo As Classes.cServer
        Public ReadOnly Property GameServerInfo() As Classes.cServer
            Get
                Return _GameServerInfo
            End Get
        End Property
        Private _SpamIndex As Short
        Private _SpamMessages As List(Of String)
        Private _WaitTick As Integer

        Public Sub WriteLine(ByVal Pre As String, ByVal Message As String)
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.Write("[")
            Console.ForegroundColor = ConsoleColor.Green
            Console.Write(Pre)
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.Write("]")
            Dim spacer As String = " "
            If Pre.Length < 8 Then
                For i As Integer = Pre.Length To 7
                    spacer &= " "
                Next
            End If
            Console.Write(spacer)
            Console.ForegroundColor = ConsoleColor.White
            Console.WriteLine(Message)
        End Sub
        Public Function Request(ByVal Pre As String, ByVal Message As String) As String
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.Write("[")
            Console.ForegroundColor = ConsoleColor.Green
            Console.Write(Pre)
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.Write("]")
            Dim spacer As String = " "
            If Pre.Length < 8 Then
                For i As Integer = Pre.Length To 7
                    spacer &= " "
                Next
            End If
            Console.Write(spacer)
            Console.ForegroundColor = ConsoleColor.White
            Console.Write(Message)
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.Write(" ::              ")
            Console.ForegroundColor = ConsoleColor.Green
            Dim Response As String = Console.ReadLine()
            Console.ForegroundColor = ConsoleColor.White
            Return Response
        End Function

        Sub Main()
            Dim T As New Threading.Thread(AddressOf KeepAlive)
            T.Start()
            WriteLine("-", "alterWar clientEMU inofficial Alpha v0.1")
            WriteLine("-", "In the following steps you have to enter username and password")
            WriteLine("-", "of your official account.")
            WriteLine("-", "This is needed for emulating your account!")
            WriteLine("-", "")
            WriteLine("Init", "Now loading data!")
            WriteLine("Init", "Creating objects...")
            _ServerList = New List(Of Classes.cServer)
            WriteLine("Init", "Creating Client...")

            _Client = New cClient()
            AddHandler _Client.ReceivedLogin, AddressOf ReceivedLogin
            AddHandler _Client.ReceivedGame, AddressOf ReceivedGame
            WriteLine("Init", "Loading data finished!")

            _SpamMessages = New List(Of String)
            _SpamMessages.Add(Request("Spam", "Give in Spam Message [1]"))
            _SpamMessages.Add(Request("Spam", "Give in Spam Message [2]"))
            _SpamMessages.Add(Request("Spam", "Give in Spam Message [3]"))
            _SpamMessages.Add(Request("Spam", "Give in Spam Message [4]"))
            _SpamMessages.Add(Request("Spam", "Give in Spam Message [5]"))

TryAgainWaitTick:
            Dim _WaitTickTmp As String = Request("Spam", "Give in the wait time (in ms):")
            If IsNumeric(_WaitTickTmp) AndAlso _WaitTickTmp >= 3000 Then
                _WaitTick = _WaitTickTmp
            Else
                GoTo TryAgainWaitTick
            End If

            Dim Username As String = Request("Account", "Enter your username")
            Dim Password As String = Request("Account", "Enter your password")
            _Client.ConnectLogin()
            Client.SendLogin(New Packets.LoginPackets.LoginPacket(Username, Password))
        End Sub

        Private Sub KeepAlive()
            While True
                Threading.Thread.Sleep(1337)
            End While
        End Sub

        Private Sub ReceivedLogin(ByVal P As Packets.PacketBase)
            If P.OperationCode = 4608 Then 'Welcome packet
                _LoginAccepted = True
            ElseIf P.OperationCode = 4352 Then
                Dim ErrorCode As Integer = P.GetBlock()
                Select Case ErrorCode
                    Case 72010
                        WriteLine("Login", "Unregistered user")
                        Console.Read()
                    Case 72020
                        WriteLine("Login", "Invalid password")
                        Console.Read()
                    Case 72030
                        WriteLine("Login", "Account already logged in")
                        Console.Read()
                    Case 74010
                        WriteLine("Login", "Enter username")
                        Console.Read()
                    Case 74040
                        WriteLine("Login", "Enter password")
                        Console.Read()
                    Case 1
                        Exit Select
                    Case Else
                        Console.WriteLine("Login", "Unknown login errorcode: " & ErrorCode)
                        Console.Read()
                End Select

                If ErrorCode = 1 Then
                    Console.WriteLine("Login", "Logged in. Serverlist:")
                    Dim UserID As Integer = P.GetBlock()
                    P.GetBlock()
                    Dim Username As String = P.GetBlock()
                    Dim Password As String = P.GetBlock()
                    Dim Nickname As String = P.GetBlock()
                    _UserData = New Classes.cUserData(UserID, Username, Nickname)
                    P.GetBlock()
                    P.GetBlock()
                    P.GetBlock()
                    P.GetBlock()
                    _PassCode = P.GetBlock()
                    Dim ServerCount As Integer = P.GetBlock()
                    P.GetBlock()
                    For i As Integer = 0 To ServerCount - 1
                        Dim ServerID As Integer = P.GetBlock()
                        Dim ServerName As String = P.GetBlock()
                        Dim ServerIP As String = P.GetBlock()
                        Dim ServerPort As Integer = P.GetBlock()
                        Dim OnlineUser As Integer = P.GetBlock()
                        P.GetBlock()
                        _ServerList.Add(New Classes.cServer(ServerID, ServerName, ServerIP, ServerPort) With {.OnlineUser = OnlineUser})
                    Next

AskAgain:
                    WriteLine("-", "")
                    For Each S As Classes.cServer In _ServerList
                        WriteLine("Server", "#" & S.ServerID & " :: " & S.ServerName & " - " & S.ServerIP & ":" & S.ServerPort & " (" & S.OnlineUser & " users online)")
                    Next

                    WriteLine("-", "")
                    Dim Sid As Integer = Integer.Parse(Request("Connect", "Please enter the ServerID you want to connect to"))
                    Dim FoundS As Classes.cServer = _ServerList.FindID(Sid)
                    If FoundS.ServerID <> Sid Then GoTo askagain
                    _GameServerInfo = FoundS
                    _Client.ConnectGame(FoundS.ServerIP, FoundS.ServerPort)
                    Threading.Thread.Sleep(500)
                    _Client.SendGame(New Packets.GamePackets.WelcomePacket())
                    WriteLine("Game", "Successfully connected to " & FoundS.ServerName & "!")
                End If
            End If
        End Sub
        Private Sub ReceivedGame(ByVal P As Packets.PacketBase)
            Try
                If P.OperationCode = 24832 Then
                    _Client.SocketLogin.Close()
                    _Client.SendGame(New Packets.GamePackets.CharacterInfo(_UserData, _PassCode))
                ElseIf P.OperationCode = 25088 Then
                    _Client.SendGame(New Packets.GamePackets.SwitchChannel(Packets.GamePackets.SwitchChannel.eChannel.AI))
                ElseIf P.OperationCode = 28673 Then
                    _Client.SendGame(New Packets.GamePackets.RoomList(0))
                    WriteLine("-", "Spammer just began. Waiting " & _WaitTick & "ms for each message")
                    Dim t As New Threading.Thread(AddressOf SendChatLol)
                    t.Start()
                ElseIf P.OperationCode = 29696 Then
                    Dim From As String = P.GetBlockIndex(2)
                    Dim Message As String = P.GetBlockIndex(6)
                    Message = Message.Substring(From.Length + 5, Message.Length - From.Length - 5)
                Else
                    'WriteLine("Game", "Unhandled packet (" & P.OperationCode & ") - " & String.Join(" ", P.Blocks.ToArray))
                End If
            Catch ex As Exception
                WriteLine("ERROR", ex.ToString())
            End Try
        End Sub

        Private Sub SendChatLol()
            While True
                If _SpamIndex >= 5 Then _SpamIndex = 0
                _Client.SendGame(New Packets.GamePackets.Chat(_SpamMessages(_SpamIndex)))
                _SpamIndex += 1
                Threading.Thread.Sleep(3000)
            End While
        End Sub

    End Module
End Namespace