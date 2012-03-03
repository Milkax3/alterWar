Imports System.Net
Imports clientEMU.Base.Packets

Namespace Base
    Public Class cClient

        Public Const LoginIP As String = "login.warrock.net" 'Official login.warrock.net
        Public Const LoginPort As Integer = 5330
        Public Const GameIP As String = "173.195.41.199" 'Deutschland_2 173.195.41.199
        Public Const GamePort As Integer = 5340

        Private _SocketLogin As Sockets.Socket
        Public ReadOnly Property SocketLogin() As Sockets.Socket
            Get
                Return _SocketLogin
            End Get
        End Property

        Private _SocketGame As Sockets.Socket
        Public ReadOnly Property SocketGame() As Sockets.Socket
            Get
                Return _SocketGame
            End Get
        End Property

        Public Sub SendLogin(ByVal P As PacketBase)
            Dim Pa As Byte() = P.GetPacketCryptedL()
            _SocketLogin.Send(Pa, Pa.Length, Sockets.SocketFlags.None)
        End Sub
        Public Sub SendGame(ByVal P As PacketBase)
            Dim Pa As Byte() = P.GetPacketCryptedG()
            _SocketGame.Send(Pa, Pa.Length, Sockets.SocketFlags.None)
        End Sub

        Sub New()
            _SocketLogin = New Sockets.Socket(Sockets.AddressFamily.InterNetwork, Sockets.SocketType.Stream, Sockets.ProtocolType.Tcp)
            _SocketGame = New Sockets.Socket(Sockets.AddressFamily.InterNetwork, Sockets.SocketType.Stream, Sockets.ProtocolType.Tcp)
        End Sub

        Public Sub ConnectLogin()
            Try
                _SocketLogin.Connect(LoginIP, LoginPort)
                WriteLine("LClient", "Connected to " & LoginIP & ":" & LoginPort)
                Dim LListener As New Threading.Thread(AddressOf ListenLogin)
                LListener.Start()
            Catch ex As Exception
                WriteLine("LClient", "Error: Couldn't connect to " & LoginIP & ":" & LoginPort)
            End Try
        End Sub
        Public Sub ConnectGame(ByVal IP As String, ByVal Port As Integer)
            Try
                _SocketGame.Connect(IP, Port)
                WriteLine("GClient", "Connected to " & IP & ":" & Port)
                Dim GListener As New Threading.Thread(AddressOf ListenGame)
                GListener.Start()
            Catch ex As Exception
                Console.WriteLine("GCLient", "Error: Couldn't connect to " & IP & ":" & Port)
            End Try
        End Sub

        Private Sub ListenLogin()
            Try
                Dim c As New Crypter()
                While _SocketLogin.Connected
                    Try
                        Dim Input As Byte() = New Byte(1023) {}
                        Dim InputLen As Integer = 0
                        InputLen = _SocketLogin.Receive(Input, 0, Input.Length, Sockets.SocketFlags.None)
                        Dim Buf As Byte() = New Byte(InputLen - 1) {}
                        Array.Copy(Input, 0, Buf, 0, InputLen)
                        Dim Pa As PacketBase = Nothing
                        Pa = c.UnCrypt(c.BytesToString(Buf), True)(0)
                        RaiseEvent ReceivedLogin(Pa)
                    Catch ex As Exception
                    End Try
                End While
            Catch ex As Exception
                WriteLine("LClient", "Fatal Error: Couldn't read data")
            End Try
        End Sub
        Private Sub ListenGame()
            Try
                Dim c As New Crypter()
                While _SocketGame.Connected
                    Try
                        Dim Input As Byte() = New Byte(8192) {}
                        Dim InputLen As Integer = 0
                        InputLen = _SocketGame.Receive(Input, 0, Input.Length, Sockets.SocketFlags.None)
                        Dim Buf As Byte() = New Byte(InputLen - 1) {}
                        Array.Copy(Input, 0, Buf, 0, InputLen)
                        Dim Pa As PacketBase()
                        Pa = c.UnCrypt(c.BytesToString(Buf), False)
                        For Each cP As PacketBase In Pa
                            RaiseEvent ReceivedGame(cP)
                        Next
                    Catch ex As Exception
                    End Try
                End While
            Catch ex As Exception
                WriteLine("GClient", "Fatal Error: Couldn't read data")
            End Try
        End Sub

        Public Event ReceivedLogin(ByVal Packet As PacketBase)
        Public Event ReceivedGame(ByVal packet As PacketBase)

    End Class
End Namespace