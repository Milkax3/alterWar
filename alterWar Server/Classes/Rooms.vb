Public Class RoomClass
    Public Structure sRoomTick
        Public TimePlus As Integer
        Public TimeMinus As Integer
        Public Second As Integer
    End Structure

    Public Enum eRoomStatus
        Playing = 2
        Waiting = 1
    End Enum

    Public Enum eRoomMode
        Explosive = 0
        FreeForAll = 1
        Deathmatch = 2
        Conquest = 3
    End Enum

    Private _Started As Boolean
    Public Property Started() As Boolean
        Get
            Return _Started
        End Get
        Set(ByVal value As Boolean)
            _Started = value
        End Set
    End Property

    Private _RoomID As Integer
    Public ReadOnly Property RoomID() As Integer
        Get
            Return _RoomID
        End Get
    End Property

    Private _RoomName As String
    Public Property RoomName() As String
        Get
            Return _RoomName
        End Get
        Set(ByVal value As String)
            _RoomName = value
        End Set
    End Property

    Private _Passworded As Boolean
    Public Property Passworded() As Boolean
        Get
            Return IIf(_Passworded = True, 1, 0)
        End Get
        Set(ByVal value As Boolean)
            If value = 1 Then _Passworded = True Else _Passworded = False
        End Set
    End Property

    Private _Password As String
    Public Property Password() As String
        Get
            Return _Password
        End Get
        Set(ByVal value As String)
            _Password = value
        End Set
    End Property

    Private _PremiumOnly As Boolean
    Public Property PremiumOnly() As Boolean
        Get
            Return _PremiumOnly
        End Get
        Set(ByVal value As Boolean)
            _PremiumOnly = value
        End Set
    End Property

    Private _MapID As Integer
    Public Property MapID() As Integer
        Get
            Return _MapID
        End Get
        Set(ByVal value As Integer)
            _MapID = value
        End Set
    End Property

    Private _VoteKick As Boolean
    Public Property VoteKick() As Integer
        Get
            Return IIf(_VoteKick = True, 1, 0)
        End Get
        Set(ByVal value As Integer)
            If value = 1 Then _VoteKick = True Else _VoteKick = 0
        End Set
    End Property

    Private _RoomType As Integer
    Public Property RoomType() As Integer
        Get
            Return _RoomType
        End Get
        Set(ByVal value As Integer)
            _RoomType = value
        End Set
    End Property

    Private _MaxPlayer As Integer
    Public Property MaxPlayer() As Integer
        Get
            Return _MaxPlayer
        End Get
        Set(ByVal value As Integer)
            _MaxPlayer = value
            _FixedPlayers = value
            For i As Integer = value To 15 '31 is absolute maxplayer ( its indexbased, usually it is 32 )
                If Not _PlayersNIU(i) Is Nothing Then
                    'Kick player of room
                End If
                If Not _PlayersDerberan(i) Is Nothing Then
                    'Kick player of room
                End If
                _PlayersNIU(i) = Nothing
                _PlayersDerberan(i) = Nothing
            Next
        End Set
    End Property

    Private _LevelLimit As Integer
    Public Property LevelLimit() As Integer
        Get
            Return _LevelLimit
        End Get
        Set(ByVal value As Integer)
            _LevelLimit = value
        End Set
    End Property

    Private _RoomStatus As eRoomStatus
    Public Property RoomStatus() As eRoomStatus
        Get
            Return _RoomStatus
        End Get
        Set(ByVal value As eRoomStatus)
            _RoomStatus = value
        End Set
    End Property

    Private _RoomMode As eRoomMode
    Public Property RoomMode() As eRoomMode
        Get
            Return _RoomMode
        End Get
        Set(ByVal value As eRoomMode)
            _RoomMode = value
            If value = eRoomMode.Explosive Then
                _MapID = 12 'MARIEN
            ElseIf value = eRoomMode.Deathmatch Then
                _MapID = 27 'RED CLOVER
            ElseIf value = eRoomMode.Conquest Then
                _MapID = 40 'SKILL POINTER
            End If
        End Set
    End Property

    Private _Autostart As Integer
    Public Property Autostart() As Boolean
        Get
            Return IIf(_Autostart = 1, True, False)
        End Get
        Set(ByVal value As Boolean)
            If value = True Then _Autostart = 1 Else _Autostart = 0
        End Set
    End Property

    Private _RoomTick As sRoomTick
    Public Property RoomTick() As sRoomTick
        Get
            Return _RoomTick
        End Get
        Set(ByVal value As sRoomTick)
            _RoomTick = value
        End Set
    End Property

    Private _Ping As Integer
    Public Property Ping() As Integer
        Get
            Return _Ping
        End Get
        Set(ByVal value As Integer)
            _Ping = value
        End Set
    End Property

    Private _Rounds As Integer
    Public Property Rounds() As Integer
        Get
            Return _Rounds
        End Get
        Set(ByVal value As Integer)

        End Set
    End Property

    Private _CurrentRound As Integer
    Public Property CurrentRound() As Integer
        Get
            Return _CurrentRound
        End Get
        Set(ByVal value As Integer)
            _CurrentRound = value
        End Set
    End Property

    Private _FixedPlayers As Integer
    Private _FixedSize As Boolean
    Public Property FixedSize() As Boolean
        Get
            Return _FixedSize
        End Get
        Set(ByVal value As Boolean)
            _FixedSize = value
            If value = True Then
                _FixedPlayers = ActivePlayers.Count
            Else
                _FixedPlayers = MaxPlayer
            End If
        End Set
    End Property

    Private _RoomMaster As PlayerClientGame
    Public Property RoomMaster() As PlayerClientGame
        Get
            Return _RoomMaster
        End Get
        Set(ByVal value As PlayerClientGame)
            _RoomMaster = value
        End Set
    End Property
    Public Sub NewMaster(ByVal c As PlayerClientGame)
        _RoomMaster = c
    End Sub

    Private _ChannelID As PlayerClientGame.eChannelID
    Public Property ChannelID() As PlayerClientGame.eChannelID
        Get
            Return _ChannelID
        End Get
        Set(ByVal value As PlayerClientGame.eChannelID)
            _ChannelID = value
        End Set
    End Property

    Private _PlayersDerberan As List(Of PlayerClientGame)
    Public ReadOnly Property PlayersDerberan() As List(Of PlayerClientGame)
        Get
            Return _PlayersDerberan
        End Get
    End Property
    Public ReadOnly Property PlayersDerberanCount() As Integer
        Get
            Dim total As Integer = 0
            For Each P In PlayersDerberan
                If Not P Is Nothing Then total += 1
            Next
            Return total
        End Get
    End Property
    Private _PlayersNIU As List(Of PlayerClientGame)
    Public ReadOnly Property PlayersNIU() As List(Of PlayerClientGame)
        Get
            Return _PlayersNIU
        End Get
    End Property
    Public ReadOnly Property PlayersNIUCount() As Integer
        Get
            Dim total As Integer = 0
            For Each P In PlayersNIU
                If Not P Is Nothing Then total += 1
            Next
            Return total
        End Get
    End Property

    Public ReadOnly Property PlayerByID(ByVal PlayerSlot As Integer) As PlayerClientGame
        Get
            Dim RetPlayer As PlayerClientGame = Nothing
            If PlayerSlot > ((MaxPlayer / 2) - 1) Then
                RetPlayer = _PlayersNIU(PlayerSlot - (MaxPlayer / 2))
            Else
                RetPlayer = _PlayersDerberan(PlayerSlot)
            End If
            Return RetPlayer
        End Get
    End Property

    Public ReadOnly Property IDOfPlayer(ByVal Player As PlayerClientGame) As Integer
        Get
            For i As Integer = 0 To ((MaxPlayer / 2) - 1)
                If _PlayersNIU(i) Is Nothing = False Then
                    If _PlayersNIU(i).UserID = Player.UserID Then
                        Return (i + (MaxPlayer / 2))
                    End If
                End If
                If _PlayersDerberan(i) Is Nothing = False Then
                    If _PlayersDerberan(i).UserID = Player.UserID Then
                        Return i
                    End If
                End If
            Next
            Return -1
        End Get
    End Property

    Public ReadOnly Property ActivePlayers() As List(Of PlayerClientGame)
        Get
            Dim TmpPlayers As New List(Of PlayerClientGame)
            For i As Integer = 0 To ((MaxPlayer / 2) - 1)
                If Not _PlayersDerberan(i) Is Nothing Then
                    TmpPlayers.Add(_PlayersDerberan(i))
                End If
            Next

            For i As Integer = (MaxPlayer / 2) To (MaxPlayer - 1)
                If Not _PlayersNIU(i - (MaxPlayer / 2)) Is Nothing Then
                    TmpPlayers.Add(_PlayersNIU(i - (MaxPlayer / 2)))
                End If
            Next
            Return TmpPlayers
        End Get
    End Property

    Public Sub StartGame()
        _RoomStatus = eRoomStatus.Playing
    End Sub
    Public Sub FinishGame()
        _RoomStatus = eRoomStatus.Waiting
    End Sub

    Public Function AddPlayer(ByVal Player As PlayerClientGame, Optional ByVal SpecialSlot As Integer = -1) As Integer
        Dim PlayerTeam As Integer = 0
        Player.RoomReadyState = 0
        If Player.Room Is Nothing Then
            If ActivePlayers.Count < MaxPlayer Then
                If SpecialSlot = -1 Then
                    If PlayersNIUCount > PlayersDerberanCount Then
                        _PlayersDerberan(PlayersDerberanCount) = Player
                        Player.Room = Me
                        PlayerTeam = 1
                    Else
                        _PlayersNIU(PlayersDerberanCount) = Player
                        Player.Room = Me
                        PlayerTeam = 0
                    End If
                Else
                    If SpecialSlot > ((MaxPlayer / 2) - 1) Then
                        _PlayersNIU(SpecialSlot - (MaxPlayer / 2)) = Player
                        Player.Room = Me
                        PlayerTeam = 1
                    Else
                        _PlayersDerberan(SpecialSlot) = Player
                        Player.Room = Me
                        PlayerTeam = 0
                    End If
                End If
                Player.Room = Me
            Else
                Player.Send(New gSvChat(Player, "System", "Room is Full"))
            End If
        Else
            If SpecialSlot <> -1 Then
                If ActivePlayers.Count < MaxPlayer Then
                    If SpecialSlot = -1 Then
                        If PlayersNIUCount > PlayersDerberanCount Then
                            _PlayersDerberan(PlayersDerberanCount) = Player
                            Player.Room = Me
                            PlayerTeam = 0
                        Else
                            _PlayersNIU(PlayersDerberanCount) = Player
                            Player.Room = Me
                            PlayerTeam = 1
                        End If
                    Else
                        If SpecialSlot > ((MaxPlayer / 2) - 1) Then
                            _PlayersDerberan(SpecialSlot - (MaxPlayer / 2)) = Player
                            Player.Room = Me
                            PlayerTeam = 0
                        Else
                            _PlayersNIU(SpecialSlot) = Player
                            Player.Room = Me
                            PlayerTeam = 1
                        End If
                    End If
                    Player.Room = Me
                Else
                    Player.Send(New gSvChat(Player, "System", "Room is Full"))
                End If
            Else
                Player.Send(New gSvChat(Player, "System", "You're already in a room"))
            End If
        End If
        Return PlayerTeam
    End Function
    Public Sub RemovePlayer(ByVal Player As PlayerClientGame, Optional ByVal ChangeSlot As Boolean = False)
        Dim i As Integer = 0
        For j As Integer = 0 To PlayersNIU.Count - 1
            Dim p As PlayerClientGame = PlayersNIU(j)
            If p Is Nothing Then Continue For
            If p.UserID <> Player.UserID Then Continue For
            _PlayersNIU(i) = Nothing
            p.Room = Nothing
            If ChangeSlot = True Then Exit Sub
            If ActivePlayers.Count <= 0 Then
                Globals.GetGameServer().Rooms.Remove(Me)
                Globals.GetGameServer().RefreshRoomList(Me.ChannelID)
                Exit Sub
            End If
            If p.UserID = RoomMaster.UserID Then
                _RoomMaster = ActivePlayers(ActivePlayers.Count - 1)
            End If
            i += 1
        Next
        i = 0
        For j As Integer = 0 To PlayersDerberan.Count - 1
            Dim p As PlayerClientGame = PlayersDerberan(j)
            If p Is Nothing Then Continue For
            If p.UserID <> Player.UserID Then Continue For
            _PlayersDerberan(i) = Nothing
            p.Room = Nothing
            If ChangeSlot = True Then Exit Sub
            If ActivePlayers.Count <= 0 Then
                Globals.GetGameServer().Rooms.Remove(Me)
                Globals.GetGameServer().RefreshRoomList(Me.ChannelID)
                Exit Sub
            End If
            If p.UserID = RoomMaster.UserID Then
                _RoomMaster = ActivePlayers(ActivePlayers.Count - 1)
            End If
            i += 1
        Next
    End Sub
    Public Sub NextRound()
        _CurrentRound += 1
        For Each P As PlayerClientGame In ActivePlayers

        Next
    End Sub
    Public Sub EndRound()
        _CurrentRound = _Rounds
        For Each P As PlayerClientGame In ActivePlayers
            'P.Send(New gSvRoundEnd(Me)) 'Me = Room, means PlayerData like Kills, Deaths (and experience?)
        Next
    End Sub

    Sub New(ByVal Creator As PlayerClientGame)
        _PlayersDerberan = New List(Of PlayerClientGame)
        _PlayersNIU = New List(Of PlayerClientGame)
        _RoomID = Globals.GetGameServer().Rooms.Count()
        _RoomName = "Default Name"
        _Passworded = False
        _Password = ""
        _PremiumOnly = False
        _MapID = 0
        _VoteKick = True
        _RoomType = 0
        _MaxPlayer = 8
        _LevelLimit = 0
        _RoomStatus = eRoomStatus.Waiting
        _Autostart = 0

        _RoomMode = eRoomMode.Explosive
        _RoomTick = New sRoomTick
        _RoomTick.Second = 0
        _RoomTick.TimePlus = 0 '           0 seconds, will +
        If _RoomMode = eRoomMode.Explosive Then 'Difficulty, will -
            _RoomTick.TimeMinus = 120 * 1000 '2mins
            _MapID = 12 'MARIEN
        ElseIf _RoomMode = eRoomMode.Deathmatch Then
            _RoomTick.TimeMinus = 1200 * 1000 '20mins
            _MapID = 27 'RED CLOVER
        ElseIf _RoomMode = eRoomMode.FreeForAll Then
            _RoomTick.TimeMinus = 900 * 1000 '15mins
            _MapID = 12 'MARIEN
        End If
        _MapID = 0


        _Ping = 0
        _Rounds = 7

        _RoomMaster = Creator

        For i As Integer = 0 To 15
            _PlayersNIU.Add(Nothing)
            _PlayersDerberan.Add(Nothing)
        Next

        AddPlayer(Creator, 0)
        _RoomMaster = Creator
        _ChannelID = PlayerClientGame.eChannelID.CQC
        _FixedSize = False


        'Admin Player
        'Dim AdminPlayer As New PlayerClientGame(Nothing, Nothing, -1)
        'AdminPlayer.Nickname = "~Dedicated"
        'AdminPlayer.Network = New Net.IPEndPoint(Net.IPAddress.Parse(GetProgram.GameServer.ServerIP), 5430)
        'AdminPlayer.AddExp(2147483647)
        'AdminPlayer.Premium = 4
        'AdminPlayer.RoomReadyState = 1
        'AddPlayer(AdminPlayer, 1)

    End Sub
End Class
