Public Class ServerInfo

    Public Enum eServerType As Integer
        LoginServer = 1
        GameServer = 2
    End Enum

    Private _ServerID As Integer
    Public Property ServerID() As Integer
        Get
            Return _ServerID
        End Get
        Set(ByVal value As Integer)
            _ServerID = value
        End Set
    End Property

    Private _ServerName As String
    Public Property ServerName() As String
        Get
            Return _ServerName
        End Get
        Set(ByVal value As String)
            _ServerName = value
        End Set
    End Property

    Private _ServerIP As String
    Public Property ServerIP() As String
        Get
            Return _ServerIP
        End Get
        Set(ByVal value As String)
            _ServerIP = value
        End Set
    End Property

    Private _ServerPort As Integer
    Public Property ServerPort() As Integer
        Get
            Return _ServerPort
        End Get
        Set(ByVal value As Integer)
            _ServerPort = value
        End Set
    End Property

    Private _ServerType As eServerType
    Public ReadOnly Property ServerType() As eServerType
        Get
            Return _ServerType
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
End Class
