Imports System.Net.Sockets
Imports System.Threading
Imports System.Net

Namespace alterWar.GameServer
    Public Class Game

        Public ReadOnly Property ServerName() As String
            Get
                Return Config.GetConfig("GAME_NAME")
            End Get
        End Property

        Public ReadOnly Property ServerIP() As String
            Get
                Return Config.GetConfig("GAME_IP")
            End Get
        End Property

        Public ReadOnly Property ServerPort() As Integer
            Get
                Return Config.GetConfig("GAME_PORT")
            End Get
        End Property

        Private _Rooms As List(Of RoomClass)
        Public ReadOnly Property Rooms() As List(Of RoomClass)
            Get
                Return _Rooms
            End Get
        End Property

        Private _ConnectionCount As Integer
        Public ReadOnly Property ConnectionCount() As Integer
            Get
                Return _ConnectionCount
            End Get
        End Property

        Public ReadOnly Property PlayersInChannel(ByVal ChannelId As PlayerClientGame.eChannelID) As PlayerClientGame()
            Get
                Dim loP As New List(Of PlayerClientGame)
                For Each P As PlayerClientGame In Connections
                    If P.ChannelID = ChannelId Then loP.Add(P)
                Next
                Return loP.ToArray()
            End Get
        End Property

        Private _Connections As List(Of PlayerClientGame)
        Public ReadOnly Property Connections() As List(Of PlayerClientGame)
            Get
                Return _Connections
            End Get
        End Property

        Private _TcpServer As Socket
        Private _UdpClient1 As UdpClient
        Private _UdpClient2 As UdpClient
        Private _RoomTick As Threading.Thread

        Private Sub RunUdp1()
            Dim btReceiveData As Byte()
            Dim GroupEP As IPEndPoint
            Dim GroupIP As IPAddress
            Dim RemoteEP As IPEndPoint = Nothing
            GroupIP = IPAddress.Parse("0.0.0.0")
            GroupEP = New IPEndPoint(GroupIP, 5350)
            _UdpClient1 = New UdpClient(5350)

            Log(LogStyle.GameServer, "UDP Server #1 [5350] started")

            While True
                btReceiveData = _UdpClient1.Receive(RemoteEP)
                Dim Response As Byte() = AnalyzePacket(btReceiveData, RemoteEP)
                _UdpClient1.Send(Response, Response.Length, RemoteEP)
                RemoteEP = New IPEndPoint(GroupIP, 5350)
            End While
        End Sub
        Private Sub RunUdp2()
            Dim btReceiveData As Byte()
            Dim GroupEP As IPEndPoint
            Dim GroupIP As IPAddress
            Dim RemoteEP As IPEndPoint = Nothing
            GroupIP = IPAddress.Parse("0.0.0.0")
            GroupEP = New IPEndPoint(GroupIP, 5351)
            _UdpClient2 = New UdpClient(5351)

            Log(LogStyle.GameServer, "UDP Server #2 [5351] started")

            While True
                btReceiveData = _UdpClient2.Receive(RemoteEP)
                Dim Response As Byte() = AnalyzePacket(btReceiveData, RemoteEP)
                _UdpClient2.Send(Response, Response.Length, RemoteEP)
                RemoteEP = New IPEndPoint(GroupIP, 5350)
            End While
        End Sub
        Private Function AnalyzePacket(ByVal RecvPacket As Byte(), ByVal IPeo As Net.IPEndPoint) As Byte()
            Dim Response As Byte() = New [Byte](-1) {}

            If RecvPacket(0) = &H10 AndAlso RecvPacket(1) = &H10 AndAlso RecvPacket(2) = &H0 Then
                ' 2 Bytes changed every time [ 0x7a, 0x8f ]
                ' 2 Bytes changed every time [ 0x7a, 0x8f ]

                '10 10 00 00 00 0D FF FF FF FF 00 00 00 00 21 00 00 2E 00 00 00 00 00 00 00 00 00 00 55 45 47 45 43 98 85 ED 47 35 4D 4D 4D 4D 4D 4D 4D 4D
                '10 10 00 00 00 0D FF FF FF FF 00 00 00 00 21 00 00 41 00 00 00 00 00 00 00 00 00 00 01 11 13 11 17 CC C8 F9 27 2B 11 11 11 11 11 11 11 11 01 11 13 11 17 CC D1 B9 13 61 19 19 19 19 19 19 19 19 11 

                'Changes server packet: 1D 5E -> 17 CC

                Response = New [Byte](64) {&H10, &H10, &H0, &H0, &H2, &H3B, _
                 &HFF, &HFF, &HFF, _
                 &HFF, &H0, &H0, &H0, &H0, &H21, _
                 &H0, &H0, &H41, &H0, &H0, &H0, _
                 &H0, &H0, &H0, &H0, &H0, &H0, _
                 &H0, &H1, &H11, &H13, &H11, &H1F, _
                 &H19, &HC8, &HF9, &H53, &H96, &H11, _
                 &H11, &H11, &H11, &H11, &H11, &H11, _
                 &H11, &H1, &H11, &H13, &H11, &H1F, &H19, &H1A, _
                 &H7, &H30, &H3D, &H19, &H19, &H19, _
                 &H19, &H19, &H19, &H19, &H19, &H11}
            ElseIf RecvPacket(0) = &H10 AndAlso RecvPacket(1) = &H1 AndAlso RecvPacket(2) = &H1 Then
                Response = New [Byte](13) {&H10, &H1, &H1, &H0, &H14, &HE6, _
                 &H0, &H0, &H0, &H0, RecvPacket(RecvPacket.Length - 4), RecvPacket(RecvPacket.Length - 3), _
                 RecvPacket(RecvPacket.Length - 2), RecvPacket(RecvPacket.Length - 1)}

                '0x00 0x14 0xE6 0x00 0x00 0x00 0x00 0x00 0xD7 0x39 0x28


                'here get client shitt(id, ip, etc)
                Dim tID As Integer = (RecvPacket(RecvPacket.Length - 4) << 24) Or (RecvPacket(RecvPacket.Length - 3) << 16) Or (RecvPacket(RecvPacket.Length - 2) << 8) Or RecvPacket(RecvPacket.Length - 1)

                For Each _Client As PlayerClientGame In Connections
                    If _Client.UserID = tID Then
                        Log(LogStyle.Network, "Setting network for player '" & _Client.Nickname & "' ->  " & IPeo.Address.ToString() & ":" & IPeo.Port)

                        _Client.Network = IPeo
                        ' Set UDP EndPoint @Client, because it needs this Information(Network IP, Network Port, Local IP, Local Port)
                        'Log(LogStyle.GameServer, "IP: " & _Client.Network.Address.ToString().Split(":")(0))
                        'Log(LogStyle.GameServer, "IPA: " & _Client.Network.Address.ToString())
                        Exit For
                    End If
                Next
            Else
                Response = New [Byte](0) {&H0}
            End If

            'Log(LogStyle.Info, "sending response (" & Response.ToString("X2") & ") to " & IPeo.ToString())

            Return Response
        End Function

        Sub New()
            Try
                _Connections = New List(Of PlayerClientGame)
                _Rooms = New List(Of RoomClass)

                Dim StartUDP1 As New Thread(AddressOf RunUdp1)
                StartUDP1.Start()
                Threading.Thread.Sleep(200)
                Dim StartUDP2 As New Thread(AddressOf RunUdp2)
                StartUDP2.Start()
                Threading.Thread.Sleep(200)

                _TcpServer = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                _TcpServer.Bind(New IPEndPoint(IPAddress.Parse("0.0.0.0"), ServerPort))
                _TcpServer.Listen(64)

                Dim AcceptT As New Thread(AddressOf HandleConnectionThread)
                AcceptT.Start()

                _RoomTick = New Threading.Thread(AddressOf RoomTick)
                _RoomTick.Start()

                Log(LogStyle.GameServer, "Game Server started")
                Thread.Sleep(25)
            Catch ex As Exception
                Log(LogStyle.Error, "Couldn't start the Game Server")
                Log(LogStyle.Info, ex.ToString())
            End Try
        End Sub

        Private Sub RoomTick()
            While True
                For i As Integer = 0 To (Rooms.Count - 1)
                    Try
                        If Rooms(i).RoomTick.Second = Date.Now.Second Then Continue For
                        For Each P As PlayerClientGame In Rooms(i).ActivePlayers
                            P.Send(New gSvSpawnCount(Rooms(i)))
                            Rooms(i).RoomTick = New RoomClass.sRoomTick() With { _
                                .Second = Date.Now.Second, _
                                .TimeMinus = Rooms(i).RoomTick.TimeMinus - 1000, _
                                .TimePlus = Rooms(i).RoomTick.TimePlus + 1000}

                            If Rooms(i).RoomMode = RoomClass.eRoomMode.Explosive Then
                                RoomTick_Explosive(Rooms(i))
                            ElseIf Rooms(i).RoomMode = RoomClass.eRoomMode.Deathmatch Then
                                RoomTick_Deathmatch(Rooms(i))
                            ElseIf Rooms(i).RoomMode = RoomClass.eRoomMode.FreeForAll Then
                                RoomTick_FreeForAll(Rooms(i))
                            End If

                        Next
                    Catch ex As Exception
                    End Try
                Next
            End While
        End Sub
        Private Sub RoomTick_Explosive(ByVal Room As RoomClass)
            If Room.RoomTick.TimeMinus <= 0 Then
                Room.NextRound()
                If Room.RoomTick.Second Mod 10 = 0 Then
                    For Each p As PlayerClientGame In Room.ActivePlayers
                        For Each p1 As PlayerClientGame In Room.ActivePlayers
                            p.Send(New gSvSpawn(p1))
                        Next
                    Next
                End If
            End If

        End Sub
        Private Sub RoomTick_Deathmatch(ByVal Room As RoomClass)
            If Room.RoomTick.TimeMinus <= 0 Then
                Room.EndRound()
            End If
        End Sub
        Private Sub RoomTick_FreeForAll(ByVal Room As RoomClass)
            If Room.RoomTick.TimeMinus <= 0 Then
                Room.EndRound()
            End If
        End Sub
        Private Sub HandleConnectionThread()
            While True
                If _ConnectionCount < Config.GetConfig("GAME_LIMIT") Then
                    Try
                        Dim Connection As Socket = _TcpServer.Accept()
                        Dim GameClient As New PlayerClientGame(Connection, New GameHandler(), _ConnectionCount + 1)
                        AddHandler GameClient.ClientDisconnected, AddressOf ClientDisconnected
                        GameClient.Listen()
                        _ConnectionCount += 1
                        _Connections.Add(GameClient)
                        Log(LogStyle.GameServer, "New (GameServer)Client [REP " & Connection.RemoteEndPoint.ToString() & "]")
                    Catch ex As Exception
                        Log(LogStyle.GameServer, "Something failed...")
                        Log(LogStyle.Info, ex.ToString())
                    End Try
                Else
                    Log(LogStyle.GameServer, "Client limit exceeded.")
                End If
            End While
        End Sub
        Private Sub ClientDisconnected(ByVal Sender As PlayerClientGame)
            _ConnectionCount -= 1
            _Connections.Remove(Sender)
            Log(LogStyle.GameServer, "Client [" & Sender.Connection.RemoteEndPoint.ToString() & "] disconnected")
        End Sub

        Public Sub RefreshRoomList(Optional ByVal Channel As PlayerClientGame.eChannelID = -1)
            If Channel = -1 Then
                For Each P As PlayerClientGame In Connections
                    P.Send(New gSvRoomList(P))
                Next
            Else
                For Each P As PlayerClientGame In PlayersInChannel(Channel)
                    P.Send(New gSvRoomList(P))
                Next
            End If
        End Sub

    End Class
End Namespace