Namespace Base.Classes
    Public Class cServer
        Private _ServerID As Integer
        Public ReadOnly Property ServerID() As Integer
            Get
                Return _ServerID
            End Get
        End Property

        Private _ServerName As String
        Public ReadOnly Property ServerName() As String
            Get
                Return _ServerName
            End Get
        End Property

        Private _ServerIP As String
        Public ReadOnly Property ServerIP() As String
            Get
                Return _ServerIP
            End Get
        End Property

        Private _ServerPort As Integer
        Public ReadOnly Property ServerPort() As Integer
            Get
                Return _ServerPort
            End Get
        End Property

        Private _OnlineUser As Integer
        Public Property OnlineUser() As Integer
            Get
                Return _OnlineUser
            End Get
            Set(ByVal value As Integer)
                _OnlineUser = value
            End Set
        End Property

        Sub New(ByVal __ServerID As Integer, ByVal __ServerName As String, ByVal __ServerIP As String, ByVal __ServerPort As Integer)
            _ServerID = __ServerID
            _ServerName = __ServerName
            _ServerIP = __ServerIP
            _ServerPort = __ServerPort
            _OnlineUser = 0
        End Sub

        Public Sub Connect(ByVal _S As Net.Sockets.Socket)
            _S.Connect(_ServerIP, _ServerPort)
        End Sub
    End Class
End Namespace
