Public Enum GOperationCodes As Integer
    ClWelcome = 24832
    ClCharacterInfo = 25088
    ClPremimLeft = 25600
    ClChangeChannel = 28673
    ClRoomList = 29184
    ClChat = 29696
    ClBuy = 30208
    ClChangeWeapon = 29970
    ClRoomData = 30000
    ClCreateRoom = 29440
    ClJoinRoom = 29456
    ClSecurity = 31264
    ClDeleteWeapon = 30224
    ClLeaveRoom = 29504
    ClLuckyShotOpen = 30257
    ClItemShop = 30720
    ClLuckyShotClose = 30259

    SvWelcome = 24832
    SvCharacterInfo = 25088
    SvPremiumLeft = 25600
    SvChat = 29696
    SvChangeChannel = 28673
    SvRoomList = 29184
    SvBuy = 30208
    SvChangeWeapon = 29970
    SvRoomData = 30000
    SvRoomDataLobby = 29200
    SvCreateRoom = 29440
    SvJoinRoom = 29456
    SvPlayerInRoom = 29952
    Sv6thSlot = 30976
    SvUserlist = 28960
    SvSecurity = 31264
    SvDeleteWeapon = 30224
    SvSpawnCount = 30016
    SvPlayerNumber = 29968
    SvLeaveRoom = 29504
    SvLuckyShotLoad = 30257
    SvCredits = 30720
    SvItemShop = 30720
End Enum

Public Class gSvWelcome
    Inherits PacketBase

    Sub New()
        MyBase.New(GOperationCodes.SvWelcome)

        AddBlock(1)
        AddBlock(DateTime.Now.ToString("s\/m\/H\/d\/M\/11") & "12/2/79/0")
        '1/      5    /    349    /0
        '?/Day of Week/Day of Year/?
    End Sub
End Class

Public Class gSvCharacterInfo
    Inherits PacketBase

    Sub New(ByVal Client As PlayerClientGame)
        MyBase.New(GOperationCodes.SvCharacterInfo)
        'Global player data
        AddBlock(1)
        AddBlock("Gameserver1")
        AddBlock(2)
        AddBlock(Client.UserID)
        AddBlock(4)
        AddBlock(Client.Nickname)

        'Clan
        AddBlock(-1) 'Clan ID                      -> 1 Have Clan | 2 Have NO Clan
        AddBlock("NULL") ' Clan Name
        AddBlock(-1) '1 = Leader, 0 = Member
        AddBlock(-1) 'Clan Image

        'Special player data
        AddBlock(Client.Premium) 'Premium
        AddBlock(0) 'WTF?
        AddBlock(9)
        AddBlock(-1)
        AddBlock(0)
        AddBlock(Client.Level)
        AddBlock(Client.EXP)
        AddBlock(52722) 'Mini idea: Minutes of played warrock
        AddBlock(0)
        AddBlock(Client.Dinar)
        AddBlock(Client.Kills)
        AddBlock(Client.Deaths)

        AddBlock(0)
        AddBlock(0)
        AddBlock(0)
        AddBlock(0)
        AddBlock(0)

        'Inventory
        Dim Slots As String = ""
        If Client.Inventory.AllItems.Contains(New alterWar.Classes.Item() With {.ItemCode = "CA01"}) Then
            Slots &= "T,"
        Else
            Slots &= "F,"
        End If
        If Client.Inventory.HasPX() Then
            Slots &= "T,"
        Else
            Slots &= "F,"
        End If
        Slots &= "F,F"
        AddBlock(Slots)

        Dim WeaponE = Client.Inventory.Engineer(0).ItemCode & "," & Client.Inventory.Engineer(1).ItemCode & "," & Client.Inventory.Engineer(2).ItemCode & "," & Client.Inventory.Engineer(3).ItemCode & "," & Client.Inventory.Engineer(4).ItemCode & "," & Client.Inventory.Engineer(5).ItemCode & "," & Client.Inventory.Engineer(6).ItemCode & "," & Client.Inventory.Engineer(7).ItemCode
        Dim WeaponM = Client.Inventory.Medic(0).ItemCode & "," & Client.Inventory.Medic(1).ItemCode & "," & Client.Inventory.Medic(2).ItemCode & "," & Client.Inventory.Medic(3).ItemCode & "," & Client.Inventory.Medic(4).ItemCode & "," & Client.Inventory.Medic(5).ItemCode & "," & Client.Inventory.Medic(6).ItemCode & "," & Client.Inventory.Medic(7).ItemCode
        Dim WeaponS = Client.Inventory.Sniper(0).ItemCode & "," & Client.Inventory.Sniper(1).ItemCode & "," & Client.Inventory.Sniper(2).ItemCode & "," & Client.Inventory.Sniper(3).ItemCode & "," & Client.Inventory.Sniper(4).ItemCode & "," & Client.Inventory.Sniper(5).ItemCode & "," & Client.Inventory.Sniper(6).ItemCode & "," & Client.Inventory.Sniper(7).ItemCode
        Dim WeaponA = Client.Inventory.Assault(0).ItemCode & "," & Client.Inventory.Assault(1).ItemCode & "," & Client.Inventory.Assault(2).ItemCode & "," & Client.Inventory.Assault(3).ItemCode & "," & Client.Inventory.Assault(4).ItemCode & "," & Client.Inventory.Assault(5).ItemCode & "," & Client.Inventory.Assault(6).ItemCode & "," & Client.Inventory.Assault(7).ItemCode
        Dim WeaponH = Client.Inventory.Heavy(0).ItemCode & "," & Client.Inventory.Heavy(1).ItemCode & "," & Client.Inventory.Heavy(2).ItemCode & "," & Client.Inventory.Heavy(3).ItemCode & "," & Client.Inventory.Heavy(4).ItemCode & "," & Client.Inventory.Heavy(5).ItemCode & "," & Client.Inventory.Heavy(6).ItemCode & "," & Client.Inventory.Heavy(7).ItemCode

        AddBlock(WeaponE)
        AddBlock(WeaponM)
        AddBlock(WeaponS)
        AddBlock(WeaponA)
        AddBlock(WeaponH)

        Dim Items As String = ""

        For Each pItem As alterWar.Classes.Item In Client.Inventory.AllItems
            Items &= pItem.ItemCode & "-1-0-" & pItem.ExpireDate.ToString() & "-0-0-0-0-0,"
        Next
        For i As Integer = 1 To (31 - Client.Inventory.AllItems.Count)
            Items &= "^,"
        Next
        Items = Items.Substring(0, Items.Length - 1)

        Dim CostumeE As String = "BA06,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^"
        Dim CostumeM As String = "BA10,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^"
        Dim CostumeS As String = "BA16,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^"
        Dim CostumeA As String = "BA14,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^"
        Dim CostumeH As String = "BA01,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^"

        Dim CostumeInventory As String = "^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^,^" 'This is the "inventory" for costumes

        AddBlock(Items) 'Thats for the inventory list (when click inventory), there are PX items shown aswell

        AddBlock(CostumeE)
        AddBlock(CostumeM)
        AddBlock(CostumeS)
        AddBlock(CostumeA)
        AddBlock(CostumeH)

        AddBlock(CostumeInventory)

        AddBlock(0)
        AddBlock(CInt(CBool(Config.GetConfig("GAME_AIENABLED")))) '1 AI Channel | 0 NO AI Channel

        AddBlock("you_are@a_damned.sniffer#16e6")
    End Sub
End Class

Public Class gSvSecurity
    Inherits PacketBase

    Sub New()
        MyBase.New(GOperationCodes.SvSecurity)

        AddBlock(64)
        AddBlock("6B5382578520947A526EC3691F1EED4C59CB70DB4F6ED8A75E75ED72AB5FDD0A692442F4C99E25DD16ACB6C012D9F8CCEFFFDD77919321437F7701BFF26ACAE8")
    End Sub
End Class

Public Class gSv6thSlot
    Inherits PacketBase

    Sub New(ByVal Client As PlayerClientGame)
        MyBase.New(GOperationCodes.Sv6thSlot)

        AddBlock(1)
        AddBlock("T,T,F,F")

        Dim WeaponE As String = ""
        WeaponE = Client.Inventory.Engineer(0).ItemCode & "," & Client.Inventory.Engineer(1).ItemCode & "," & Client.Inventory.Engineer(2).ItemCode & "," & Client.Inventory.Engineer(3).ItemCode & "," & Client.Inventory.Engineer(4).ItemCode & "," & Client.Inventory.Engineer(5).ItemCode & "," & Client.Inventory.Engineer(6).ItemCode & "," & Client.Inventory.Engineer(7).ItemCode
        Dim WeaponM As String = String.Empty
        WeaponM = Client.Inventory.Medic(0).ItemCode & "," & Client.Inventory.Medic(1).ItemCode & "," & Client.Inventory.Medic(2).ItemCode & "," & Client.Inventory.Medic(3).ItemCode & "," & Client.Inventory.Medic(4).ItemCode & "," & Client.Inventory.Medic(5).ItemCode & "," & Client.Inventory.Medic(6).ItemCode & "," & Client.Inventory.Medic(7).ItemCode
        Dim WeaponS As String = String.Empty
        WeaponS = Client.Inventory.Sniper(0).ItemCode & "," & Client.Inventory.Sniper(1).ItemCode & "," & Client.Inventory.Sniper(2).ItemCode & "," & Client.Inventory.Sniper(3).ItemCode & "," & Client.Inventory.Sniper(4).ItemCode & "," & Client.Inventory.Sniper(5).ItemCode & "," & Client.Inventory.Sniper(6).ItemCode & "," & Client.Inventory.Sniper(7).ItemCode
        Dim WeaponA As String = String.Empty
        WeaponA = Client.Inventory.Assault(0).ItemCode & "," & Client.Inventory.Assault(1).ItemCode & "," & Client.Inventory.Assault(2).ItemCode & "," & Client.Inventory.Assault(3).ItemCode & "," & Client.Inventory.Assault(4).ItemCode & "," & Client.Inventory.Assault(5).ItemCode & "," & Client.Inventory.Assault(6).ItemCode & "," & Client.Inventory.Assault(7).ItemCode
        Dim WeaponH As String = String.Empty
        WeaponH = Client.Inventory.Heavy(0).ItemCode & "," & Client.Inventory.Heavy(1).ItemCode & "," & Client.Inventory.Heavy(2).ItemCode & "," & Client.Inventory.Heavy(3).ItemCode & "," & Client.Inventory.Heavy(4).ItemCode & "," & Client.Inventory.Heavy(5).ItemCode & "," & Client.Inventory.Heavy(6).ItemCode & "," & Client.Inventory.Heavy(7).ItemCode

        AddBlock(WeaponE)
        AddBlock(WeaponM)
        AddBlock(WeaponS)
        AddBlock(WeaponA)
        AddBlock(WeaponH)

        AddBlock(1)
        AddBlock("DV01")
    End Sub
End Class

Public Class gSvRoomList
    Inherits PacketBase

    Sub New(ByVal Client As PlayerClientGame)
        MyBase.New(GOperationCodes.SvRoomList)

        Dim Rooms As List(Of RoomClass) = Globals.GetGameServer().Rooms
        AddBlock(Rooms.Count)
        AddBlock(Client.RoomPage)
        AddBlock(0)

        For Each Room As RoomClass In Rooms
            AddRoomInfo(Room)
        Next
    End Sub
End Class

Public Class gSvPremiumLeft
    Inherits PacketBase

    Sub New(ByVal Client As PlayerClientGame)
        MyBase.New(GOperationCodes.SvPremiumLeft)

        AddBlock(5000)
        AddBlock(0)
        AddBlock(0)
        AddBlock(-1)
        AddBlock(4)
        AddBlock(0)
        AddBlock(0)

        AddBlock(Client.PremiumLeft * 60) 'Premiumtime left in seconds!
    End Sub
End Class

Public Class gSvChat
    Inherits PacketBase

    Public Enum ChatType As Integer
        Notice1 = 1
        Notice2
        Lobby_ToChannel
        Room_ToAll
        Room_ToTeam
        Whisper
        Lobby_ToAll = 8
        Clan
    End Enum

    Sub New(ByVal Client As PlayerClientGame, ByVal Name As String, ByVal Message As String)
        MyBase.New(GOperationCodes.SvChat)

        AddBlock(1)
        AddBlock(-1)
        AddBlock(Name.Replace(" ", Chr(&H1D)))
        AddBlock(ChatType.Room_ToAll)
        AddBlock(999)
        AddBlock("NULL")
        AddBlock(Name.Replace(" ", Chr(&H1D)) & Chr(&H1D) & Chr(&H1D) & Chr(&H1D) & Chr(&H1D) & ">>" & Chr(&H1D) & Chr(&H1D) & Message.Replace(" ", Chr(&H1D)))
    End Sub

    Sub New(ByVal Client As PlayerClientGame, ByVal Type As ChatType, ByVal Message As String, ByVal TargetID As Long, ByVal TargetName As String)
        MyBase.New(GOperationCodes.SvChat)

        AddBlock(1)
        AddBlock(Client.SessionID)
        AddBlock(Client.Nickname)
        AddBlock(CInt(Type))
        AddBlock(TargetID)
        AddBlock(TargetName)
        AddBlock(Message)
    End Sub
End Class

Public Class gSvChangeChannel
    Inherits PacketBase

    Sub New(ByVal Client As PlayerClientGame)
        MyBase.New(GOperationCodes.SvChangeChannel)

        AddBlock(1)
        AddBlock(Client.ChannelID)
    End Sub
End Class

Public Class gSvBuyWeapon
    Inherits PacketBase

    Sub New(ByVal Client As PlayerClientGame, ByVal ErrCode As alterWar.Classes.InventoryClass.eErrorCodes, ByVal WeaponString As String)
        MyBase.New(GOperationCodes.SvBuy)
        AddBlock(ErrCode)
        AddBlock(1110)
        If ErrCode = 1 Then
            AddBlock(-1)
            AddBlock(3)
            AddBlock(4)
            AddBlock(WeaponString)
            AddBlock(Client.Dinar)

            Dim Slots As String = ""
            If Client.Inventory.AllItems.Contains(New alterWar.Classes.Item() With {.ItemCode = "CA01"}) Then
                Slots &= "T,"
            Else
                Slots &= "F,"
            End If
            If Client.Inventory.HasPX() Then
                Slots &= "T,"
            Else
                Slots &= "F,"
            End If
            Slots &= "F,F"
            AddBlock(Slots)
        End If
    End Sub
End Class

Public Class gSvBuyWeaponG1
    Inherits PacketBase

    Sub New(ByVal ErrorCode As Integer)
        MyBase.New(GOperationCodes.SvItemShop)
        AddBlock(1110)
        AddBlock(ErrorCode)
    End Sub


    'TODO

    Sub New(ByVal Client As PlayerClientGame, ByVal WeaponString As String) 'unknown...
        MyBase.New(GOperationCodes.SvItemShop)
        AddBlock(1110)
        AddBlock(1)
        AddBlock(-1)
        AddBlock(3)
        AddBlock(4)
        AddBlock(WeaponString)
        AddBlock(Client.awC)
        Dim Slots As String = ""
        If Client.Inventory.AllItems.Contains(New alterWar.Classes.Item() With {.ItemCode = "CA01"}) Then
            Slots &= "T,"
        Else
            Slots &= "F,"
        End If
        If Client.Inventory.HasPX() Then
            Slots &= "T,"
        Else
            Slots &= "F,"
        End If
        Slots &= "F,F"
        AddBlock(Slots)
    End Sub
End Class

Public Class gSvChangeWeapon
    Inherits PacketBase

    Sub New(ByVal Client As PlayerClientGame, ByVal PlayerClass As Integer) 'PlayerClass: 0e,1m,2s,3a,4h
        MyBase.New(GOperationCodes.SvChangeWeapon)
        AddBlock(1)
        AddBlock(PlayerClass)
        Dim WeaponString As String = String.Empty
        Select Case PlayerClass
            Case 0
                For Each Wep As alterWar.Classes.Item In Client.Inventory.Engineer
                    WeaponString &= Wep.ItemCode & ","
                Next
                WeaponString = WeaponString.Substring(0, WeaponString.Length - 1)
                Exit Select
            Case 1
                For Each Wep As alterWar.Classes.Item In Client.Inventory.Medic
                    WeaponString &= Wep.ItemCode & ","
                Next
                WeaponString = WeaponString.Substring(0, WeaponString.Length - 1)
                Exit Select
            Case 2
                For Each Wep As alterWar.Classes.Item In Client.Inventory.Sniper
                    WeaponString &= Wep.ItemCode & ","
                Next
                WeaponString = WeaponString.Substring(0, WeaponString.Length - 1)
                Exit Select
            Case 3
                For Each Wep As alterWar.Classes.Item In Client.Inventory.Assault
                    WeaponString &= Wep.ItemCode & ","
                Next
                WeaponString = WeaponString.Substring(0, WeaponString.Length - 1)
                Exit Select
            Case 4
                For Each Wep As alterWar.Classes.Item In Client.Inventory.Heavy
                    WeaponString &= Wep.ItemCode & ","
                Next
                WeaponString = WeaponString.Substring(0, WeaponString.Length - 1)
                Exit Select
            Case Else
                For Each Wep As alterWar.Classes.Item In Client.Inventory.Engineer
                    WeaponString &= Wep.ItemCode & ","
                Next
                WeaponString = WeaponString.Substring(0, WeaponString.Length - 1)
        End Select
        AddBlock(WeaponString)
    End Sub
End Class

Public Class gSvRoomData
    Inherits PacketBase

    Sub New(ByVal tType As Integer, ByVal Parameters As String())
        MyBase.New(GOperationCodes.SvRoomData)

        AddBlock(1) 'Default added by Server
        AddBlock(Parameters(0))
        AddBlock(Parameters(1))
        AddBlock(Parameters(2))
        AddBlock(tType)
        AddBlock(Parameters(3))
        AddBlock(Parameters(4))
        AddBlock(Parameters(5))
        AddBlock(Parameters(6))
        AddBlock(Parameters(7))
        AddBlock(Parameters(8))
        AddBlock(Parameters(9))
        AddBlock(Parameters(10))
        AddBlock(Parameters(11))
        AddBlock(Parameters(12))
    End Sub
End Class

Public Class gSvCreateRoom
    Inherits PacketBase

    Sub New(ByVal Room As RoomClass)
        MyBase.New(GOperationCodes.SvCreateRoom)

        AddBlock(1)
        AddBlock(0)
        AddRoomInfo(Room)
    End Sub
End Class

Public Class gSvJoinRoom
    Inherits PacketBase

    Public Enum eErrorCode As Integer
        WrongLevel = 94300
        CantJoin = 94201
        WrongPassword = 94030
    End Enum

    Sub New(ByVal ErrorCode As eErrorCode)
        MyBase.New(GOperationCodes.SvJoinRoom)
        AddBlock(ErrorCode)
    End Sub

    Sub New(ByVal RoomID As Integer)
        MyBase.New(GOperationCodes.SvJoinRoom)

        AddBlock(1)
        AddBlock(4)
        AddRoomInfo(Globals.GetGameServer().Rooms(RoomID))
    End Sub
End Class

Public Class gSvPlayerInRoom
    Inherits PacketBase

    Private Function GetTruePort(ByVal input As UShort) As String
        Dim HexCode As String = input.ToString("X")
        Dim HexPartA As String = HexCode.Substring(0, 2)
        Dim HexPartB As String = HexCode.Substring(2, HexCode.Length - 2)
        Dim TrueHex As String = "&H" & HexPartB & HexPartA
        Dim TruePort As UShort = CUShort(TrueHex)
        Return TruePort
    End Function

    Sub New(ByVal PlayerList As PlayerClientGame())
        MyBase.New(GOperationCodes.SvPlayerInRoom)
        AddBlock(PlayerList.Length)
        For Each P As PlayerClientGame In PlayerList
            AddBlock(P.UserID)
            AddBlock(P.SessionID)
            AddBlock(P.Room.IDOfPlayer(P))
            AddBlock(P.RoomReadyState)
            AddBlock(1)
            AddBlock(0)
            AddBlock(0)
            AddBlock(0)
            AddBlock(1000)
            AddBlock(P.Nickname)
            AddBlock(-1) 'clan ID
            AddBlock(-1) 'clan [Leader]?
            AddBlock(-1) 'clan [Image]?
            AddBlock(1)
            AddBlock(30)
            AddBlock(910)
            AddBlock(P.Premium)
            AddBlock(9)
            AddBlock(-1)
            AddBlock(P.Kills)
            AddBlock(P.Deaths)
            AddBlock(21236) 'How minutes of played warrock?
            AddBlock(P.EXP)
            AddBlock(-1)
            AddBlock(-1)
            AddBlock("ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ")
            AddBlock(P.Network.Address().GetAddressInt())
            AddBlock(GetTruePort(P.Network.Port))
            AddBlock(P.Network.Address().GetAddressInt()) '(LOCAL) Network
            AddBlock(GetTruePort(P.Network.Port))
            AddBlock(0)
        Next

        Log(LogStyle.Info, MyBase.GetPacket())
    End Sub
End Class

Public Class gSvSpawn
    Inherits PacketBase

    Sub New(ByVal P As PlayerClientGame)
        MyBase.New(GOperationCodes.SvRoomData)

        AddBlock(P.Room.IDOfPlayer(P))
        AddBlock(P.Room.IDOfPlayer(P))
        AddBlock(32)
        AddBlock(P.Room.IDOfPlayer(P))

        AddBlock(150)

        AddBlock(0)
        AddBlock(1)
        AddBlock(9)
        AddBlock(0)
        AddBlock(1)
        AddBlock(0)
        AddBlock(4)
        AddBlock(1)
        AddBlock(0)
        AddBlock(0)
        AddBlock(0)
        AddBlock(0)
        AddBlock(0)
        AddBlock(0)
        AddBlock(0)
        AddBlock(0)
        AddBlock(0)
        AddBlock(0)
        AddBlock(0)
        AddBlock(0)
        AddBlock(0)
        AddBlock(0)
        AddBlock("ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ")
    End Sub
End Class

Public Class gSvDeleteWeapon
    Inherits PacketBase

    Sub New()
        MyBase.New(GOperationCodes.SvDeleteWeapon)
        'Todo!
    End Sub
End Class

Public Class gSvSpawnCount
    Inherits PacketBase

    Sub New(ByVal R As RoomClass)
        MyBase.New(GOperationCodes.SvSpawnCount)
        AddBlock(R.RoomTick.TimePlus) 'Spawn tick (10, 9, 8, ...)
        AddBlock(R.RoomTick.TimeMinus) ' Spawn tick (eg. 100 seconds left)
        AddBlock(5) 'Round
        AddBlock(0)
        AddBlock(2)
        AddBlock(2)
        AddBlock(0)
        AddBlock(30)
    End Sub
End Class

Public Class gSvLeaveRoom
    Inherits PacketBase

    Sub New(ByVal Client As PlayerClientGame, ByVal oldPlace As Integer, ByVal newMaster As Integer)
        MyBase.New(GOperationCodes.SvLeaveRoom)
        AddBlock(1)
        AddBlock(Client.SessionID)
        AddBlock(oldPlace)
        AddBlock(0)
        AddBlock(newMaster)
        AddBlock(Client.EXP)
        AddBlock(Client.Dinar)
    End Sub
End Class

Public Class gSvIngameData
    Inherits PacketBase

    Sub New(ByVal SubArg1 As Object, ByVal SubArg2 As Object, ByVal SubArg3 As Object, ByVal SubArg4 As Object, ByVal SubID As Integer, ByVal ParamArray Args As String())
        MyBase.New(GOperationCodes.SvRoomData)
        AddBlock(SubArg1)
        AddBlock(SubArg2)
        AddBlock(SubArg3)
        AddBlock(SubArg4)
        AddBlock(SubID)
    End Sub
End Class

Public Class gSvZombieWave
    Inherits PacketBase

    Sub New()
        MyBase.New(29968)
        AddBlock(1)
        AddBlock(2)
        AddBlock(0)
        AddBlock(1)
        AddBlock(467)
        AddBlock(1)
        AddBlock(0)
        AddBlock(-1)
        AddBlock(0)
        AddBlock(0)
        AddBlock(0)
        AddBlock(0)
        AddBlock(1000)
        AddBlock(-1)
        AddBlock(-1)
        AddBlock(0)
    End Sub
End Class

Public Class gSvSpawnZombie
    Inherits PacketBase

    Sub New()
        MyBase.new(13432)
        AddBlock(4)
        AddBlock(0)
        AddBlock(0)
        AddBlock(20)
        AddBlock(150)
    End Sub
End Class

Public Class gSvLuckyShotLoad
    Inherits PacketBase

    Sub New(ByVal Items As alterWar.Classes.LuckyShotItem())
        MyBase.New(GOperationCodes.SvLuckyShotLoad)

        AddBlock(Items.Length)
        For i As Integer = 0 To (Items.Count - 1)
            AddBlock(IIf(Items(i).DinarItem = True, 1, 0))
            AddBlock(Items(i).WeaponCode)
            AddBlock(Items(i).Stock)
            AddBlock(Items(i).Bet)
        Next
    End Sub
End Class

Public Class gSvCredits
    Inherits PacketBase

    Sub New(ByVal P As PlayerClientGame)
        MyBase.New(GOperationCodes.SvCredits)
        AddBlock(1113)
        AddBlock(1)
        AddBlock(P.awC)
    End Sub
End Class