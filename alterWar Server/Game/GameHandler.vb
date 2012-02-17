Imports MySql.Data.MySqlClient

Public Class GameHandler

    Public Enum LoginErrorCodes As Integer
        Success = 1

        Unregistered = 72010
        InvalidPassword = 72020
        AlreadyLoggedIn = 72030

        NormalLogin = 73030
        Banned = 73050

        EnterName = 74010
        EnterPassword = 74020
    End Enum
    Private _Second As Integer

    Public Sub HandlePacket(ByVal D As Byte(), ByVal Client As PlayerClientGame)
        'Try
        Dim Packet As PacketBase = Nothing
        Packet = GetCrypter().UnCrypt(GetCrypter().BytesToString(D), False)
        HandlePacketSub(Packet, Client)
        'Catch ex As Exception
        '    Log(LogStyle.Error, "(GameServer) Client packet went wrong -> Disconnecting it")
        '    Client.Connection.Disconnect(True)
        'End Try
    End Sub

    Private Sub HandlePacket_Welcome(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        Client.Send(New gSvWelcome())
    End Sub
    Private Sub HandlePacket_CharacterInfo(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        Dim UserId As Integer = CInt(P.GetBlockIndex(0))
        Client.UserID = UserId
        Client.LoadData()
        Client.Send(New gSvCharacterInfo(Client))
        Client.Send(New gSvSecurity())
        Client.Send(New gSvPremiumLeft(Client))
    End Sub
    Private Sub HandlePacket_PremiumLeft(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        Client.Send(New gSvPremiumLeft(Client))
        _Second = Date.Now.Second
    End Sub
    Private Sub HandlePacket_ChangeChannel(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        Client.ChannelID = CInt(P.GetBlock())
        Client.Send(New gSvChangeChannel(Client))
        Dim ChannelName As String = "Undefined"
        If Client.ChannelID = PlayerClientGame.eChannelID.CQC Then ChannelName = "Close Quarters Combats"
        If Client.ChannelID = PlayerClientGame.eChannelID.UrbanOps Then ChannelName = "Urban Ops"
        If Client.ChannelID = PlayerClientGame.eChannelID.BattleGroup Then ChannelName = "Battle Group"
        Client.Send(New gSvChat(Client, "System", "You changed the channel to " & ChannelName))
    End Sub
    Private Sub HandlePacket_RoomList(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        Dim Page As Integer = CInt(P.GetBlock())
        Client.RoomPage = Page
        Client.Send(New gSvRoomList(Client))
        Client.Send(New gSvChat(Client, "System", "You switched to page " & Page & "."))
    End Sub
    Private Sub HandlePacket_Chat(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        Dim Type As Integer = P.GetBlock()
        Dim TargetID As Integer = P.GetBlock()
        Dim TargetName As String = P.GetBlock()
        Dim Message As String = P.GetBlock()

        If Type = gSvChat.ChatType.Room_ToAll Then
            If Not Client.Room Is Nothing Then
                For Each pl As PlayerClientGame In Client.Room.ActivePlayers
                    pl.Send(New gSvChat(Client, gSvChat.ChatType.Room_ToAll, Message, 1, Client.Nickname))
                Next
            End If
        ElseIf Type = gSvChat.ChatType.Room_ToTeam Then
            If Not Client.Room Is Nothing Then
                If Client.Team = 0 Then
                    For Each pl As PlayerClientGame In Client.Room.PlayersDerberan
                        pl.Send(New gSvChat(Client, gSvChat.ChatType.Room_ToTeam, Message, 1, Client.Nickname))
                    Next
                ElseIf Client.Team = 1 Then
                    For Each pl As PlayerClientGame In Client.Room.PlayersNIU
                        pl.Send(New gSvChat(Client, gSvChat.ChatType.Room_ToTeam, Message, 1, Client.Nickname))
                    Next
                End If
            End If
        ElseIf Type = gSvChat.ChatType.Lobby_ToAll Then
            For Each Pl As PlayerClientGame In Globals.GetGameServer().Connections
                If Pl.Room Is Nothing Then Pl.Send(New gSvChat(Client, Type, Message, 1000, Client.Nickname))
            Next
        ElseIf Type = gSvChat.ChatType.Lobby_ToChannel Then
            For Each Pl As PlayerClientGame In Globals.GetGameServer().PlayersInChannel(Client.ChannelID)
                If Pl.Room Is Nothing Then Pl.Send(New gSvChat(Client, Type, Message, 1000, Client.Nickname))
            Next
        Else
            Client.Send(New gSvChat(Client, "System", "No such Message Type"))
        End If
    End Sub
    Private Sub HandlePacket_Buy(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        P.GetBlock()
        Dim WeaponCode As String = P.GetBlock()
        Dim WeaponID As Integer = CInt(P.GetBlock())
        P.GetBlock()
        Dim Days As alterWar.Classes.InventoryClass.eDays = CInt(P.GetBlock())
        Dim TrueDays As Integer = 0
        Dim Price As Integer = CInt(P.GetBlock())
        Dim ErrCode As alterWar.Classes.InventoryClass.eErrorCodes = alterWar.Classes.InventoryClass.eErrorCodes.Success
        Dim IsPx As Boolean = False
        If WeaponCode.Substring(0, 1) = "C" Then IsPx = True 'Itemcodes of PX Items begins with C
        Select Case Days
            Case alterWar.Classes.InventoryClass.eDays.Days3
                TrueDays = 3
                Exit Select
            Case alterWar.Classes.InventoryClass.eDays.Days7
                TrueDays = 7
                Exit Select
            Case alterWar.Classes.InventoryClass.eDays.Days15
                TrueDays = 15
                Exit Select
            Case alterWar.Classes.InventoryClass.eDays.Days30
                TrueDays = 30
                Exit Select
            Case Else
                TrueDays = 1
        End Select
        For Each Wep As alterWar.Classes.Item In Client.Inventory.AllItems
            If Wep.ItemCode = WeaponCode Then
                ErrCode = alterWar.Classes.InventoryClass.eErrorCodes.CantBuy
            End If
        Next
        If ErrCode = alterWar.Classes.InventoryClass.eErrorCodes.Success Then
            Dim Items As String = WeaponCode & "-1-1-" & alterWar.Classes.InventoryFunctions.TimeAdd(TrueDays) & "-0-0-0-0-0,"
            For i As Integer = 0 To (31 - (Client.Inventory.AllItems().Count))
                Items &= "^,"
            Next
            Items = Items.Substring(0, Items.Length - 1)
            Dim Weaponstring As String = WeaponCode & "-1-1-" & alterWar.Classes.InventoryFunctions.TimeAdd(TrueDays) & "-0-0-0-0-0," & Items
            Client.Send(New gSvBuyWeapon(Client, ErrCode, Items))
            GetMySql().Execute("INSERT INTO `wremu`.`inventory` (`id`, `userId`, `itemCode`, `expireDate`, `isPX`, `engineer`, `medic`, `sniper`, `assault`, `heavy`) VALUES (NULL, '" & Client.UserID & "', '" & WeaponCode & "', '" & alterWar.Classes.InventoryFunctions.TimeAdd(TrueDays) & "', '" & CStr(IsPx) & "', '10,20,30,40,50,60,70,80', '10,20,30,40,50,60,70,80', '10,20,30,40,50,60,70,80', '10,20,30,40,50,60,70,80', '10,20,30,40,50,60,70,80');")
            Client.Dinar -= Price
            Client.SaveData()
        Else
            Client.Send(New gSvBuyWeapon(Client, ErrCode, ""))
        End If
    End Sub
    Private Sub HandlePacket_ChangeWeapon(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        Dim Unequip As Integer = CInt(P.GetBlock())
        Dim WeaponClass As Integer = CInt(P.GetBlock())
        'WeaponClass -= 1 'Make it indexbased lol
        P.GetBlock()
        P.GetBlock()
        Dim WeaponCode As String = P.GetBlock()
        Dim Slot As Integer = CInt(P.GetBlock())
        Slot += 1 'that was indexbased lol

        If Slot = 0 Then Slot += 1

        Select Case WeaponClass
            Case 0
                Dim NameWeaponClass As String = "engineer"
                Dim WeaponString As String = String.Empty
                For i As Integer = 1 To 8
                    If Slot = i Then
                        WeaponString &= i & "1,"
                    Else
                        WeaponString &= i & "0,"
                    End If
                Next
                WeaponString = WeaponString.Substring(0, WeaponString.Length - 1)
                Client.Inventory.Engineer(Slot - 1) = New alterWar.Classes.Item() With {.ItemCode = WeaponCode}
                GetMySql().Execute("UPDATE `wremu`.`inventory` SET `" & NameWeaponClass & "` = '" & WeaponString & "' WHERE `inventory`.`userId` = " & Client.UserID & " AND `inventory`.`itemCode` = '" & WeaponCode & "' LIMIT 1;")
                Log(LogStyle.Info, "User switched weapon: " & WeaponClass & ": " & WeaponString)
                Exit Select
            Case 1
                Dim NameWeaponClass As String = "medic"
                Dim WeaponString As String = String.Empty
                For i As Integer = 1 To 8
                    If Slot = i Then
                        WeaponString &= i & "1,"
                    Else
                        WeaponString &= i & "0,"
                    End If
                Next
                WeaponString = WeaponString.Substring(0, WeaponString.Length - 1)
                Client.Inventory.Medic(Slot - 1) = New alterWar.Classes.Item() With {.ItemCode = WeaponCode}
                GetMySql().Execute("UPDATE `wremu`.`inventory` SET `" & NameWeaponClass & "` = '" & WeaponString & "' WHERE `inventory`.`userId` = " & Client.UserID & " AND `inventory`.`itemCode` = '" & WeaponCode & "' LIMIT 1;")
                Log(LogStyle.Info, "User switched weapon: " & WeaponClass & ": " & WeaponString)
                Exit Select
            Case 2
                Dim NameWeaponClass As String = "sniper"
                Dim WeaponString As String = String.Empty
                For i As Integer = 1 To 8
                    If Slot = i Then
                        WeaponString &= i & "1,"
                    Else
                        WeaponString &= i & "0,"
                    End If
                Next
                WeaponString = WeaponString.Substring(0, WeaponString.Length - 1)
                Client.Inventory.Sniper(Slot - 1) = New alterWar.Classes.Item() With {.ItemCode = WeaponCode}
                GetMySql().Execute("UPDATE `wremu`.`inventory` SET `" & NameWeaponClass & "` = '" & WeaponString & "' WHERE `inventory`.`userId` = " & Client.UserID & " AND `inventory`.`itemCode` = '" & WeaponCode & "' LIMIT 1;")
                Log(LogStyle.Info, "User switched weapon: " & WeaponClass & ": " & WeaponString)
                Exit Select
            Case 3
                Dim NameWeaponClass As String = "assault"
                Dim WeaponString As String = String.Empty
                For i As Integer = 1 To 8
                    If Slot = i Then
                        WeaponString &= i & "1,"
                    Else
                        WeaponString &= i & "0,"
                    End If
                Next
                WeaponString = WeaponString.Substring(0, WeaponString.Length - 1)
                Client.Inventory.Assault(Slot - 1) = New alterWar.Classes.Item() With {.ItemCode = WeaponCode}
                GetMySql().Execute("UPDATE `wremu`.`inventory` SET `" & NameWeaponClass & "` = '" & WeaponString & "' WHERE `inventory`.`userId` = " & Client.UserID & " AND `inventory`.`itemCode` = '" & WeaponCode & "' LIMIT 1;")
                Log(LogStyle.Info, "User switched weapon: " & WeaponClass & ": " & WeaponString)
                Exit Select
            Case 4
                Dim WeaponStringH As String = "10,20,30,40,50,60,70,80"
                If Slot = 1 AndAlso Unequip = False Then
                    WeaponStringH = "11,20,30,40,50,60,70,80"
                ElseIf Slot = 2 And Unequip = False Then
                    WeaponStringH = "10,21,30,40,50,60,70,80"
                ElseIf Slot = 3 And Unequip = False Then
                    WeaponStringH = "10,20,31,40,50,60,70,80"
                ElseIf Slot = 4 And Unequip = False Then
                    WeaponStringH = "10,20,30,41,50,60,70,80"
                ElseIf Slot = 5 And Unequip = False Then
                    WeaponStringH = "10,20,30,40,51,60,70,80"
                ElseIf Slot = 6 And Unequip = False Then
                    WeaponStringH = "10,20,30,40,50,61,70,80"
                ElseIf Slot = 7 And Unequip = False Then
                    WeaponStringH = "10,20,30,40,50,60,71,80"
                ElseIf Slot = 8 And Unequip = False Then
                    WeaponStringH = "10,20,30,40,50,60,70,81"
                End If
                Client.Inventory.Heavy(Slot - 1) = New alterWar.Classes.Item() With {.ItemCode = WeaponCode}
                GetMySql().Execute("UPDATE `wremu`.`inventory` SET `heavy` = '" & WeaponStringH & "' WHERE `inventory`.`userId` = " & Client.UserID & " AND `inventory`.`itemCode` = '" & WeaponCode & "' LIMIT 1;")
                Log(LogStyle.Info, "User switched weapon: " & WeaponClass & ": " & WeaponStringH)
                Exit Select
        End Select
        Log(LogStyle.Info, "Player Class: " & WeaponClass + 1 & " - " & WeaponCode & " Slot: " & Slot & " Unequip: " & Unequip)
        Client.Send(New gSvChangeWeapon(Client, WeaponClass))
    End Sub
    Private Sub HandlePacket_RoomJoin(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        Dim RoomID As Integer = P.GetBlock()
        Dim RoomPW As String = P.GetBlock()
        If Globals.GetGameServer().Rooms(RoomID) Is Nothing Then
            Client.Send(New gSvJoinRoom(gSvJoinRoom.eErrorCode.CantJoin))
            Client.Send(New gSvChat(Client, "System", "Can't join room ( No ID )"))
        End If
        If Globals.GetGameServer().Rooms(RoomID).Passworded = True Then
            If Globals.GetGameServer().Rooms(RoomID).Password = RoomPW Then
                If Globals.GetGameServer().Rooms(RoomID).ActivePlayers.Count < Globals.GetGameServer().Rooms(RoomID).MaxPlayer Then
                    Client.Team = Globals.GetGameServer().Rooms(RoomID).AddPlayer(Client)
                    Client.Room = Globals.GetGameServer().Rooms(RoomID)
                    Client.Send(New gSvJoinRoom(RoomID))
                    Dim PlayerList As New List(Of PlayerClientGame)
                    For Each Player As PlayerClientGame In Globals.GetGameServer().Rooms(RoomID).ActivePlayers
                        PlayerList.Add(Player)
                    Next
                    PlayerList.Remove(Client)
                    Client.Send(New gSvPlayerInRoom(PlayerList.ToArray()))
                    PlayerList.Clear()
                    PlayerList.Add(Client)
                    Client.Send(New gSvPlayerInRoom(PlayerList.ToArray()))

                    For Each Pl As PlayerClientGame In Globals.GetGameServer().Rooms(RoomID).ActivePlayers
                        If Pl.UserID = Client.UserID Then Continue For
                        Pl.Send(New gSvPlayerInRoom(PlayerList.ToArray()))
                    Next
                Else
                    Client.Send(New gSvJoinRoom(gSvJoinRoom.eErrorCode.CantJoin))
                    Client.Send(New gSvChat(Client, "System", "Can't join room ( Room full )"))
                End If
            Else
                Client.Send(New gSvJoinRoom(gSvJoinRoom.eErrorCode.WrongPassword))
            End If
        Else
            If Globals.GetGameServer().Rooms(RoomID).ActivePlayers.Count < Globals.GetGameServer().Rooms(RoomID).MaxPlayer Then
                Client.Team = Globals.GetGameServer().Rooms(RoomID).AddPlayer(Client)
                Client.Room = Globals.GetGameServer().Rooms(RoomID)
                Client.Send(New gSvJoinRoom(RoomID))
                Dim PlayerList As New List(Of PlayerClientGame)
                For Each Player As PlayerClientGame In Globals.GetGameServer().Rooms(RoomID).ActivePlayers
                    PlayerList.Add(Player)
                Next
                PlayerList.Remove(Client)
                Client.Send(New gSvPlayerInRoom(PlayerList.ToArray()))
                PlayerList.Clear()
                PlayerList.Add(Client)
                Client.Send(New gSvPlayerInRoom(PlayerList.ToArray()))

                For Each Pl As PlayerClientGame In Globals.GetGameServer().Rooms(RoomID).ActivePlayers
                    If Pl.UserID = Client.UserID Then Continue For
                    Pl.Send(New gSvPlayerInRoom(PlayerList.ToArray()))
                Next
            Else
                Client.Send(New gSvJoinRoom(gSvJoinRoom.eErrorCode.CantJoin))
                Client.Send(New gSvChat(Client, "System", "Can't join room ( Room full )"))
            End If
        End If
    End Sub
    Private Sub HandlePacket_Security(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        Client.Send(New gSvSecurity())
    End Sub
    Private Sub HandlePacket_DeleteWeapon(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        'TODO
    End Sub
    Private Sub HandlePacket_RoomData(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        Dim Room As RoomClass = Client.Room()
        Dim tType As Integer = Integer.Parse(P.GetBlockIndex(3))
        Dim tValue As Integer = Integer.Parse(P.GetBlockIndex(6))
        Dim Value As Integer = Integer.Parse(P.GetBlockIndex(9))
        Dim tPlace1 As Integer = Integer.Parse(P.GetBlockIndex(6))
        Dim tPlace2 As Integer = Integer.Parse(P.GetBlockIndex(7))

        Dim Changes As Boolean = False

        Select Case tType
            Case 1
                Room.Started = True
                Room.RoomStatus = RoomClass.eRoomStatus.Playing
                tType += 3
                tValue = Room.MapID
                Changes = True

                Exit Select

            Case 402
                tType += 1
                tValue = 3
                tPlace1 = 882
                Value = 1

                Exit Select

            Case 50 'Readystate
                If Client.RoomReadyState = 1 Then
                    Client.RoomReadyState = 0
                    tValue = 0
                Else
                    Client.RoomReadyState = 1
                    tValue = 1
                End If

                Exit Select
            Case 51 'Map change
                Room.MapID = tValue
                Changes = True
                Exit Select

            Case 52 'Change room mode
                Room.RoomMode = tValue
                Changes = True
                Exit Select

            Case 53 'Rounds / Kills
                Room.Rounds = tValue
                Changes = True
                Exit Select

            Case 59 'Ping
                Room.Ping = tValue
                Changes = True
                Exit Select

            Case 62 'Autostart
                Log(LogStyle.GameServer, "Autostart switch: " & String.Join(" ", P.Blocks.ToArray()))

                Exit Select

            Case 56 'Change team
                Client.RoomChangeSide()
                tPlace1 = Room.IDOfPlayer(Client)
                tPlace2 = Room.IDOfPlayer(Room.RoomMaster)

                Exit Select

            Case 150 'Spawn place
                tPlace1 = Integer.Parse(P.GetBlockIndex(7))
                tPlace2 = Integer.Parse(P.GetBlockIndex(8))

                Exit Select

            Case 58 'Fixed size

                If tValue = 1 Then
                    Room.FixedSize = False
                Else
                    Room.FixedSize = True
                End If
                Changes = True

                Exit Select
            Case 500 'Fall Damage
                Log(LogStyle.Network, "Fall damage : " & P.GetPacket())
                Exit Select

            Case Else 'Other subtype
                Log(LogStyle.GameServer, "30000 : " & tType & " :: " & String.Join(" ", P.Blocks.ToArray()))

                Exit Select
        End Select

        Dim ArrSorted() As String = {P.GetBlockIndex(0), P.GetBlockIndex(1), P.GetBlockIndex(2), P.GetBlockIndex(4), P.GetBlockIndex(5), tValue, tPlace1, tPlace2, Value, P.GetBlockIndex(10), P.GetBlockIndex(11), P.GetBlockIndex(12), P.GetBlockIndex(12)}
        If tType = 403 Then
            Client.Send(New gSvRoomData(tType, ArrSorted))
        ElseIf tType = 500 Then 'Fall Damage
            Dim Damage As Integer = Value
            Dim NewHP As Integer = Client.Player.Health - Damage
            'TODO:
            'If NewHP <= 0 Then Client.Player.Suicide()
            If NewHP <= 0 Then NewHP += 700
            ArrSorted = New String() {P.GetBlockIndex(0), P.GetBlockIndex(1), P.GetBlockIndex(2), P.GetBlockIndex(4), P.GetBlockIndex(5), P.GetBlockIndex(6), P.GetBlockIndex(7), P.GetBlockIndex(8), P.GetBlockIndex(9), P.GetBlockIndex(10), Damage, NewHP, P.GetBlockIndex(12)}
            Client.Send(New gSvRoomData(tType, ArrSorted))
            Client.Player.SetHealth(NewHP)
        ElseIf tType = 200 Then 'Car Join
            Dim CurrentHP As Integer = 2700
            Dim MaxHP As Integer = 2700
            ArrSorted = New String() {P.GetBlockIndex(0), P.GetBlockIndex(1), P.GetBlockIndex(2), P.GetBlockIndex(4), P.GetBlockIndex(5), P.GetBlockIndex(6), P.GetBlockIndex(7), CurrentHP, MaxHP, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "$"}
            Log(LogStyle.Network, "Outgoing seat 200 : " & String.Join(" ", ArrSorted))
            Client.Send(New gSvRoomData(tType, ArrSorted))
        ElseIf tType = 202 Then 'Car Leave
            ArrSorted = New String() {P.GetBlockIndex(0), P.GetBlockIndex(1), P.GetBlockIndex(2), P.GetBlockIndex(4), P.GetBlockIndex(5), P.GetBlockIndex(6), P.GetBlockIndex(7), 0, 0, 0, 1, 0, 0}
            Log(LogStyle.Network, "Outgoing seat 202 : " & String.Join(" ", ArrSorted))
            Client.Send(New gSvRoomData(tType, ArrSorted))
        Else
            For Each cClient As PlayerClientGame In Room.ActivePlayers
                cClient.Send(New gSvRoomData(tType, ArrSorted))
            Next
        End If
        If Changes Then
            Globals.GetGameServer.RefreshRoomList(Client.ChannelID)
        End If
    End Sub
    Private Sub HandlePacket_CreateRoom(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        Dim Room As New RoomClass(Client)
        'If Client.ChannelID <> PlayerClientGame.eChannelID.CQC Then
        '   Client.Send(New gSvChat(Client, "Actually you can only create Rooms in CQC"))
        '   Client.Send(New gSvChat(Client, "UO and BGB had been disabled because of"))
        '   Client.Send(New gSvChat(Client, "unfinished Data (flag, vehicle, etc)."))
        '   Exit Sub
        'End If
        Dim RoomID As Integer = Globals.GetGameServer().Rooms.Count()
        Dim RoomName As String = P.GetBlock()
        Dim Passworded As String = P.GetBlock()
        Dim Password As String = String.Empty
        If Passworded = 1 Then
            Password = P.GetBlock()
        End If
        Dim MaxPlayerS As String = P.GetBlock()
        Dim MaxPlayer As Integer
        If MaxPlayerS = "NULL" Then
            MaxPlayer = 8 'default number for players
        Else
            MaxPlayer = (8 * (MaxPlayerS + 1))
        End If
        Dim MapID As Integer = CInt(P.GetBlock())
        P.GetBlock()
        P.GetBlock()
        Dim RoomType As Integer = CInt(P.GetBlock())
        Dim LevelLimit As Integer = CInt(P.GetBlock())
        Dim PremiumOnly As Integer = P.GetBlock()
        Dim VoteKick As Integer = P.GetBlock()
        Dim RoomMode As RoomClass.eRoomMode = RoomClass.eRoomMode.Explosive

        If Client.ChannelID = 1 Then
            Room.RoomMode = RoomClass.eRoomMode.Explosive
        ElseIf Client.ChannelID = PlayerClientGame.eChannelID.UrbanOps Then
            Room.RoomMode = RoomClass.eRoomMode.Conquest
        Else
            Room.RoomMode = RoomClass.eRoomMode.Deathmatch
        End If

        Room.RoomName = RoomName
        If Password <> "" Then
            Room.Passworded = True
            Room.Password = Password
        End If
        Room.MaxPlayer = MaxPlayer
        Room.RoomType = RoomType
        Room.PremiumOnly = CBool(PremiumOnly)
        Room.VoteKick = VoteKick
        Room.NewMaster(Client)
        Client.Room = Room
        Globals.GetGameServer().Rooms.Add(Room)
        Globals.GetGameServer.RefreshRoomList(Client.ChannelID)
        Client.Send(New gSvCreateRoom(Room))
    End Sub
    Private Sub HandlePacket_LeaveRoom(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        Dim oldPlace As Integer = Client.Room.IDOfPlayer(Client)
        Dim newMaster As Integer = -1
        If Client.Room.ActivePlayers.Count = 1 Then
            Globals.GetGameServer().Rooms.Remove(Client.Room)
        End If
        For Each cP As PlayerClientGame In Client.Room.ActivePlayers
            cP.Send(New gSvLeaveRoom(Client, oldPlace, newMaster))
        Next
        Client.Room.RemovePlayer(Client)
        Client.Send(New gSvLeaveRoom(Client, oldPlace, newMaster))

        For Each cp As PlayerClientGame In Globals.GetGameServer().PlayersInChannel(Client.ChannelID)
            cp.Send(New gSvRoomList(cp))
        Next
    End Sub
    Private Sub HandlePacket_LuckyShotOpen(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        Client.Send(New gSvLuckyShotLoad(New alterWar.Classes.LuckyShotItem() {New alterWar.Classes.LuckyShotItem(True, "DC64", 100, 1000), New alterWar.Classes.LuckyShotItem(False, "DF36", 10, 100)}))
    End Sub
    Private Sub HandlePacket_ItemShop(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        Dim SubType As Int16 = P.GetBlock()
        If SubType = 1110 Then 'bought a g1 weapon, handling it now. :)
            Client.Send(New gSvBuyWeaponG1(alterWar.Classes.InventoryClass.eErrorCodes.LowMoney))
        ElseIf SubType = 1113 Then 'Opened Itemshop, sending the g1 credits amount
            Client.Send(New gSvCredits(Client))
        End If
    End Sub
    Private Sub HandleUnknownPacket(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        Log(LogStyle.Network, "~ NEW [GAME] CLIENT PACKET ~")
        Log(LogStyle.Info, "Operation Code: " & P.OperationCode & " - Block Count & " & P.Blocks.Count)
        Log(LogStyle.Info, "Packet Content: ")
        Log(LogStyle.Info, P.OperationCode & " " & String.Join(" ", P.Blocks.ToArray))
    End Sub

    Private Sub HandlePacketSub(ByVal P As PacketBase, ByVal Client As PlayerClientGame)
        Select Case P.OperationCode
            Case GOperationCodes.ClWelcome
                HandlePacket_Welcome(P, Client)
                Exit Select
            Case GOperationCodes.ClCharacterInfo
                HandlePacket_CharacterInfo(P, Client)
                Exit Select
            Case GOperationCodes.ClPremimLeft
                HandlePacket_PremiumLeft(P, Client)
                Exit Select
            Case GOperationCodes.ClChangeChannel
                HandlePacket_ChangeChannel(P, Client)
                Exit Select
            Case GOperationCodes.ClRoomList
                HandlePacket_RoomList(P, Client)
                Exit Select
            Case GOperationCodes.ClChat
                HandlePacket_Chat(P, Client)
                Exit Select
            Case GOperationCodes.ClBuy ' TODO : MAKE THIS UNHACKABLE (price is clientsided actually, let this run thru a database!)
                HandlePacket_Buy(P, Client)
                Exit Select
            Case GOperationCodes.ClDeleteWeapon
                HandlePacket_DeleteWeapon(P, Client) 'TODO: Make the server packet (and look at the client packet again!)
                Exit Select
            Case GOperationCodes.ClChangeWeapon
                HandlePacket_ChangeWeapon(P, Client)
                Exit Select
            Case GOperationCodes.ClCreateRoom
                HandlePacket_CreateRoom(P, Client)
                Exit Select
            Case GOperationCodes.ClJoinRoom
                HandlePacket_RoomJoin(P, Client)
                Exit Select
            Case GOperationCodes.ClSecurity
                HandlePacket_Security(P, Client)
                Exit Select
            Case GOperationCodes.ClRoomData
                HandlePacket_RoomData(P, Client)
                Exit Select
            Case GOperationCodes.ClLeaveRoom
                HandlePacket_LeaveRoom(P, Client)
                Exit Select
            Case GOperationCodes.ClItemShop
                HandlePacket_ItemShop(P, Client)
                Exit Select
            Case GOperationCodes.ClLuckyShotOpen
                HandlePacket_LuckyShotOpen(P, Client)
                Exit Sub
            Case Else
                HandleUnknownPacket(P, Client)
        End Select
    End Sub
End Class