Imports System.Net.Sockets
Imports System.Threading

Namespace alterWar.LoginServer
    Public Class Login
        Private _ServerSocket As Socket
        Public ReadOnly Property ServerSocket() As Socket
            Get
                Return _ServerSocket
            End Get
        End Property

        Private _ConnectionCount As Integer

        Sub New()
            _ConnectionCount = 0
            Try
                _ServerSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                _ServerSocket.Bind(New Net.IPEndPoint(Net.IPAddress.Parse(Config.GetConfig("LOGIN_IP")), Config.GetConfig("LOGIN_PORT")))
                _ServerSocket.Listen(0)

                Dim AcceptT As New Thread(New ThreadStart(AddressOf HandleConnectionsThread))
                AcceptT.Start()

                Log(LogStyle.LoginServer, "Login Server started")
            Catch ex As Exception
                Log(LogStyle.Error, "Couldn't start the Login Server")
                Log(LogStyle.Info, ex.ToString())
            End Try
        End Sub
        Private Sub ClientDisconnected(ByVal sender As PlayerClientLogin)
            _ConnectionCount -= 1
            Log(LogStyle.LoginServer, "Client [" & sender.Connection.RemoteEndPoint.ToString() & "] disconnected")
        End Sub

        Private Sub HandleConnectionsThread()
            While True
                Dim ClientSocket As Socket = _ServerSocket.Accept()
                Dim LoginClient As New PlayerClientLogin(ClientSocket, New LoginHandler())
                AddHandler LoginClient.ClientDisconnected, AddressOf ClientDisconnected
                LoginClient.Listen()
                Log(LogStyle.Info, "New <LoginServer>client [" & ClientSocket.RemoteEndPoint.ToString() & "]")
                _ConnectionCount += 1
                Thread.Sleep(5)
            End While
        End Sub
    End Class
End Namespace