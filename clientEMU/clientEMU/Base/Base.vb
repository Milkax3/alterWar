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

        Private _UDPClient1 As UdpClient

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

        Private _GameServerInfo As Classes.cServer
        Public ReadOnly Property GameServerInfo() As Classes.cServer
            Get
                Return _GameServerInfo
            End Get
        End Property

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
            _Client.ConnectLogin()

            WriteLine("Init", "Loading data finished!")

            Dim Username As String = Request("Account", "Enter your username")
            Dim Password As String = Request("Account", "Enter your password")
            Client.SendLogin(New Packets.LoginPackets.LoginPacket(Username, Password))
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
                    Client.SocketLogin.Close()
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
                    P.GetBlock()
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
                    _Client.SendGame(New Packets.GamePackets.WelcomePacket())
                    WriteLine("Game", "Successfully connected to " & FoundS.ServerName & "!")
                End If
            End If
        End Sub
        Private Sub ReceivedGame(ByVal P As Packets.PacketBase)
            'Try
            If P.OperationCode = 24832 Then
                _UDPClient1 = New UdpClient(_GameServerInfo.ServerIP, 5350)
                Dim EP As New Net.IPEndPoint(Net.IPAddress.Parse(_GameServerInfo.ServerIP), 5350)
                WriteLine("UDP", "Connected to GameServer on Port 5351 with UDP (" & _UDPClient1.Client.Connected.ToString() & ")")
                Dim UserID As Integer = UserData.UserID
                Dim bytes() As Byte = BitConverter.GetBytes(UserID)
                Dim b1 As Byte = bytes(3)
                Dim b2 As Byte = bytes(2)
                Dim b3 As Byte = bytes(1)
                Dim b4 As Byte = bytes(0)

                Dim SendBytes As Byte() = New Byte(13) {&H10, &H1, &H1, &H0, &H0, &H63, &H0, &H0, &H0, &H0, b1, b2, b3, b4}

                _UDPClient1.Send(SendBytes, SendBytes.Length)

                Console.WriteLine("EP Is " & EP.ToString())
                Dim RecvBytes As Byte() = _UDPClient1.Receive(EP)
                Console.WriteLine("Received [" & RecvBytes.ArrayToString() & "]")


                SendBytes = New Byte(45) {&H10, &H10, &H0, &H0, &H0, &H63, &HFF, &HFF, &HFF, &HFF, &H0, &H0, &H0, &H0, &H21, &H0, &H0, &H2E, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H55, &H45, &H47, &H45, &H48, &H74, &H4E, &H53, &H64, &H69, &H4D, &H4D, &H4D, &H4D, &H4D, &H4D, &H4D, &H4D}

                _UDPClient1.Send(SendBytes, SendBytes.Length)

                Console.WriteLine("EP Is " & EP.ToString())
                RecvBytes = _UDPClient1.Receive(EP)
                Console.WriteLine("Received [" & RecvBytes.ArrayToString() & "]")


                _Client.SendGame(New Packets.GamePackets.CharacterInfo(_UserData))
                WriteLine("Game", "Startinfo sent")
            ElseIf P.OperationCode = 25088 Then
                WriteLine("Game", "We got character info")
                Console.WriteLine()
                Console.Write(P.GetPacket())

            End If
            'Catch ex As Exception
            '    WriteLine("ERROR", ex.ToString())
            'End Try
        End Sub

    End Module
End Namespace