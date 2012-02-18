Imports System.Net.Sockets
Imports System.Threading
Imports MySql.Data.MySqlClient

Public Class PlayerClientLogin

    Private _UserID As Integer
    Private _Username As String
    Private _Nickname As String
    Private _Password As String
    Private _ClientSocket As Socket
    Private _Handler As LoginHandler
    Public ReadOnly Property Handler() As LoginHandler
        Get
            Return _Handler
        End Get
    End Property

    Public Property UserID() As Integer
        Get
            Return _UserID
        End Get
        Set(ByVal value As Integer)
            _UserID = value
        End Set
    End Property

    Public Property Username() As String
        Get
            Return _Username
        End Get
        Set(ByVal value As String)
            _Username = value
        End Set
    End Property

    Public Property Nickname() As String
        Get
            Return _Nickname
        End Get
        Set(ByVal value As String)
            _Nickname = value
        End Set
    End Property

    Public Property Password() As String
        Get
            Return _Password
        End Get
        Set(ByVal value As String)
            _Password = value
        End Set
    End Property

    Public ReadOnly Property Connection() As Socket
        Get
            Return _ClientSocket
        End Get
    End Property

    Public Sub Send(ByVal P As PacketBase)
        _ClientSocket.Send(P.GetPacketCrypted(), P.GetPacketCrypted().Length, SocketFlags.None)
    End Sub

    Public Sub Listen()
        Dim ListenT As New Thread(AddressOf Listening)
        ListenT.Start()
    End Sub

    Private Sub Listening()
        Send(New lSvConnect)
        While Connection.Connected
            Dim Data As Byte() = New Byte(1023) {} '1kb [1024 bytes] is maximum for packet
            Dim rBuf As Byte()
            Dim DataLength As Integer = 0
            Try
                DataLength = _ClientSocket.Receive(Data, 0, Data.Length, SocketFlags.None)
            Catch
            End Try

            If _ClientSocket.Connected AndAlso DataLength > 0 Then
                rBuf = New Byte(DataLength - 1) {}
                Array.Copy(Data, 0, rBuf, 0, DataLength)
                _Handler.HandlePacket(rBuf, Me)
            End If
            Thread.Sleep(20)
        End While
        RaiseEvent ClientDisconnected(Me)
    End Sub

    Sub New(ByVal S As Socket, ByVal Lh As LoginHandler)
        _ClientSocket = S
        _Handler = Lh
    End Sub

    Public Event ClientDisconnected(ByVal Sender As PlayerClientLogin)
End Class



Public Class PlayerClientGame

    Public Enum eChannelID
        CQC = 1
        UrbanOps = 2
        BattleGroup = 3
    End Enum

    Public Structure cPlayer
        Public Health As Integer
        Public Sub SetHealth(ByVal _Health As Integer)
            Health = _Health
        End Sub
        Public WeaponIndex As Integer
        Public Sub SetWeaponIndex(ByVal _WeaponIndex As Integer)
            WeaponIndex = _WeaponIndex
        End Sub

        'TODO!
        Public Sub Suicide()
            'For Each P As PlayerClientGame In Me.Room
            '   P.Send(New gSvSuicide(Me))
            'Next P
        End Sub
    End Structure

    Private _UserID As Integer
    Private _SessionID As Integer
    Private _Username As String
    Private _Nickname As String
    Private _Password As String
    Private _Accesslevel As Integer
    Private _awC As Integer
    Private _Team As Integer

    Private _Premium As Integer

    Private _Level As Integer
    Private _EXP As Integer
    Private _Kills As Integer
    Private _Deaths As Integer
    Private _Dinar As Integer
    Private _Inventory As alterWar.Classes.InventoryClass
    Private _ChannelID As eChannelID
    Private _RoomPage As Integer
    Private _RoomReadyState As Integer

    Private _Network As Net.IPEndPoint
    Private _ClientSocket As Socket
    Private _Handler As GameHandler
    Private _Minute As Integer

    Private _Player As cPlayer
    Private _Room As RoomClass

    Public ReadOnly Property Handler() As GameHandler
        Get
            Return _Handler
        End Get
    End Property

    Public Property RoomReadyState() As Integer
        Get
            Return _RoomReadyState
        End Get
        Set(ByVal value As Integer)
            _RoomReadyState = value
        End Set
    End Property
    Public Property UserID() As Integer
        Get
            Return _UserID
        End Get
        Set(ByVal value As Integer)
            _UserID = value
        End Set
    End Property
    Public Property Username() As String
        Get
            Return _Username
        End Get
        Set(ByVal value As String)
            _Username = value
        End Set
    End Property
    Public Property Nickname() As String
        Get
            Return _Nickname
        End Get
        Set(ByVal value As String)
            _Nickname = value
        End Set
    End Property
    Public Property Password() As String
        Get
            Return _Password
        End Get
        Set(ByVal value As String)
            _Password = value
        End Set
    End Property
    Public Property Network() As Net.IPEndPoint
        Get
            Return _Network
        End Get
        Set(ByVal value As Net.IPEndPoint)
            _Network = value
        End Set
    End Property
    Public ReadOnly Property Accesslevel() As Integer
        Get
            Return _Accesslevel
        End Get
    End Property

    Public Property Premium() As Integer
        Get
            Return _Premium
        End Get
        Set(ByVal value As Integer)
            _Premium = value
        End Set
    End Property
    Public ReadOnly Property PremiumLeft() As Integer
        Get
            Dim Q As MySqlDataReader = GetMySql.ExecuteReader("SELECT `PremiumLeft` FROM `account_detail` WHERE `id`='" & UserID & "';")
            Try
                Dim RetVal As Integer = 0
                While Q.Read()
                    RetVal = Q.GetInt32("PremiumLeft")
                End While
                Q.Close()
                Return RetVal
            Catch ex As Exception
                Q.Close()
                Return -1
            End Try
        End Get
    End Property
    Public ReadOnly Property Level() As Integer
        Get
            Return _Level
        End Get
    End Property
    Public ReadOnly Property EXP() As Integer
        Get
            Return _EXP
        End Get
    End Property
    Public Property Kills() As Integer
        Get
            Return _Kills
        End Get
        Set(ByVal value As Integer)
            _Kills = value
        End Set
    End Property
    Public Property Deaths() As Integer
        Get
            Return _Deaths
        End Get
        Set(ByVal value As Integer)
            _Deaths = value
        End Set
    End Property
    Public Property Dinar() As Integer
        Get
            Return _Dinar
        End Get
        Set(ByVal value As Integer)
            _Dinar = value
        End Set
    End Property
    Public Property ChannelID() As eChannelID
        Get
            Return _ChannelID
        End Get
        Set(ByVal value As eChannelID)
            _ChannelID = value
        End Set
    End Property
    Public Property RoomPage() As Integer
        Get
            Return _RoomPage
        End Get
        Set(ByVal value As Integer)
            _RoomPage = value
        End Set
    End Property
    Public Property awC() As Integer
        Get
            Return _awC
        End Get
        Set(ByVal value As Integer)
            _awC = value
        End Set
    End Property
    Public Property Player() As cPlayer
        Get
            Return _Player
        End Get
        Set(ByVal value As cPlayer)
            _Player = value
        End Set
    End Property
    Public Property Team() As Integer
        Get
            Return _Team
        End Get
        Set(ByVal value As Integer)
            _Team = value
        End Set
    End Property

    Public Property Room() As RoomClass
        Get
            Return _Room
        End Get
        Set(ByVal value As RoomClass)
            _Room = value
            _Player = New cPlayer() With {.Health = 1000, .WeaponIndex = 0}
        End Set
    End Property
    Public ReadOnly Property SessionID() As Integer
        Get
            Return _SessionID
        End Get
    End Property
    Public ReadOnly Property Inventory() As alterWar.Classes.InventoryClass
        Get
            Return _Inventory
        End Get
    End Property

    Public Sub AddExp(ByVal MoreEXp As Integer)
        _EXP += MoreEXp
    End Sub
    Public ReadOnly Property Connection() As Socket
        Get
            Return _ClientSocket
        End Get
    End Property

    Public Sub Chat(Optional ByVal From As String = "System", Optional ByVal Message As String = "Not defined Message")
        Send(New gSvChat(Me, From, Message))
    End Sub
    Public Sub Send(ByVal P As PacketBase)
        Try
            _ClientSocket.Send(P.GetPacketCrypted(), P.GetPacketCrypted().Length, SocketFlags.None)
        Catch ex As Exception
        End Try
    End Sub
    Public Sub Listen()
        Dim ListenT As New Thread(AddressOf Listening)
        ListenT.Start()
    End Sub
    Public Sub LoadData()
        Dim Query As MySqlDataReader = GetMySql().ExecuteReader("SELECT * FROM `account` WHERE `id`='" & UserID & "'")
        While Query.Read()
            _Username = Query.GetString("name")
            _Password = Query.GetString("passwd")
            _Nickname = Query.GetString("nickname")
            _Accesslevel = Query.GetInt32("accesslevel")
            _awC = Query.GetInt32("awc")
        End While
        Query.Close()

        Query = GetMySql().ExecuteReader("SELECT * FROM `account_detail` WHERE `id`='" & UserID & "'")
        While Query.Read()
            _EXP = Query.GetInt64("exp")
            _Level = Query.GetInt32("level")
            _Dinar = Query.GetInt64("dinar")
            _Kills = Query.GetInt64("kills")
            _Deaths = Query.GetInt64("death")

            _Premium = Query.GetInt32("Premium")
        End While
        Query.Close()

        Query = GetMySql().ExecuteReader("SELECT * FROM `inventory` WHERE `id`='" & UserID & "'")
        While Query.Read()
            Dim itemCode As String = Query.GetString("itemCode")
            Dim expireDate As Integer = Query.GetInt64("expireDate")

            Dim isPx As Boolean = Boolean.Parse(Query.GetString("isPX"))
            If isPx Then
                Inventory.PxItems.Add(New alterWar.Classes.Item() With {.ItemCode = itemCode, .ExpireDate = expireDate})
                Inventory.HasPX = True
            End If

            Dim classE As String() = Query.GetString("engineer").Split(",")
            Dim classM As String() = Query.GetString("medic").Split(",")
            Dim classS As String() = Query.GetString("sniper").Split(",")
            Dim classA As String() = Query.GetString("assault").Split(",")
            Dim classH As String() = Query.GetString("heavy").Split(",")
            Dim UsedInClass As Boolean = False

            For Each cE As String In classE
                Dim slot As Integer = cE(0).ToString
                slot -= 1
                Dim enabled As Integer = cE(1).ToString
                If enabled = 1 Then
                    Inventory.Engineer(slot).ExpireDate = expireDate
                    Inventory.Engineer(slot).ItemCode = itemCode
                    UsedInClass = True
                End If
            Next
            For Each cM As String In classM
                Dim slot As Integer = cM(0).ToString
                slot -= 1
                Dim enabled As Integer = cM(1).ToString
                If enabled = 1 Then
                    Inventory.Medic(slot).ExpireDate = expireDate
                    Inventory.Medic(slot).ItemCode = itemCode
                    UsedInClass = True
                End If
            Next
            For Each cS As String In classS
                Dim slot As Integer = cS(0).ToString
                slot -= 1
                Dim enabled As Integer = cS(1).ToString
                If enabled = 1 Then
                    Inventory.Sniper(slot).ExpireDate = expireDate
                    Inventory.Sniper(slot).ItemCode = itemCode
                    UsedInClass = True
                End If
            Next
            For Each cA As String In classA
                Dim slot As Integer = cA(0).ToString
                slot -= 1
                Dim enabled As Integer = cA(1).ToString
                If enabled = 1 Then
                    Inventory.Assault(slot).ExpireDate = expireDate
                    Inventory.Assault(slot).ItemCode = itemCode
                    UsedInClass = True
                End If
            Next
            For Each cH As String In classH
                Dim slot As Integer = cH(0).ToString
                slot -= 1
                Dim enabled As Integer = cH(1).ToString
                If enabled = 1 Then
                    Inventory.Heavy(slot).ExpireDate = expireDate
                    Inventory.Heavy(slot).ItemCode = itemCode
                    UsedInClass = True
                End If
            Next
            If UsedInClass = False Then
                Inventory.NotUsed.Add(New alterWar.Classes.Item() With {.ItemCode = itemCode, .ExpireDate = expireDate})
            End If
        End While
        Query.Close()
    End Sub
    Private Sub Listening()
        While True
            If Connection.Connected = False Then
                RaiseEvent ClientDisconnected(Me)
                Me._ClientSocket = Nothing 'free memory
                Exit Sub 'and exit the listening...
            End If
            Dim Data As Byte() = New Byte(1023) {} '1kb [1024 bytes] is maximum for packet
            Dim rBuf As Byte()
            Dim DataLength As Integer = 0
            Try
                DataLength = _ClientSocket.Receive(Data, 0, Data.Length, SocketFlags.None)
            Catch
            End Try

            If _ClientSocket.Connected AndAlso DataLength > 0 Then
                rBuf = New Byte(DataLength - 1) {}
                Array.Copy(Data, 0, rBuf, 0, DataLength)
                _Handler.HandlePacket(rBuf, Me)
            End If
            Thread.Sleep(20)
        End While
    End Sub
    Sub New(ByVal S As Socket, ByVal Lh As GameHandler, ByVal Sid As Integer)
        _Inventory = New alterWar.Classes.InventoryClass()
        _ClientSocket = S
        _Handler = Lh
        _SessionID = Sid
    End Sub
    Public Function RoomChangeSide() As Integer
        Dim RmBackup As RoomClass = Room
        If Team = 0 Then
            If Room.PlayersNIUCount < (Room.MaxPlayer / 2) Then
                Room.RemovePlayer(Me, True)
                Team = RmBackup.AddPlayer(Me, (RmBackup.MaxPlayer / 2) + RmBackup.PlayersNIUCount)
            Else
                Send(New gSvChat(Me, "System", "You can't switch the Team"))
            End If
        Else
            If Room.PlayersDerberanCount < (Room.MaxPlayer / 2) Then
                Room.RemovePlayer(Me, True)
                Team = RmBackup.AddPlayer(Me, RmBackup.PlayersDerberanCount)
            Else
                Send(New gSvChat(Me, "System", "You can't switch the Team"))
            End If
        End If
        Return Team
    End Function
    Public Sub SaveData()
        GetMySql.Execute("UPDATE `wremu`.`account` SET `awc` = '" & awC & "' WHERE `account`.`id` = " & UserID & ";")
        GetMySql.Execute("UPDATE `wremu`.`account_detail` SET `exp` = '" & EXP & "', `level` = '" & LevelManager.GetLevel(EXP) & "', `dinar` = '" & Dinar & "', `kills` = '" & Kills & "', `death` = '" & Deaths & "' WHERE `account_detail`.`id` = " & UserID & ";")
    End Sub

    Public Event ClientDisconnected(ByVal Sender As PlayerClientGame)
End Class