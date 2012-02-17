Public Enum LOperationCodes
    SvPatch = 4112
    SvLogin = 4352
    SvConnect = 4608

    ClPatch = 4112
    ClLogin = 4352
End Enum

Public Class lSvConnect
    Inherits PacketBase

    Sub New()
        MyBase.New(LOperationCodes.SvConnect)

        AddBlock(New Random().[Next](11111111, 999999999))
        AddBlock(77)
    End Sub
End Class

Public Class lSvPatch
    Inherits PacketBase

    Sub New()
        MyBase.New(LOperationCodes.SvPatch)
        AddBlock(0)
        'Game Version: Found in version.cfg
        AddBlock(19)
        AddBlock(34)
        AddBlock(53)
        AddBlock(3)
        AddBlock(0)
        AddBlock("http://patch.warrock.net/k2network/warrock/")
    End Sub
End Class

Public Class lSvLogin
    Inherits PacketBase

    Sub New(ByVal ErrCode As Integer)
        MyBase.New(LOperationCodes.SvLogin)
        AddBlock(ErrCode)
    End Sub

    Sub New(ByVal Client As PlayerClientLogin)
        MyBase.New(LOperationCodes.SvLogin)
        AddBlock(1)
        AddBlock(Client.UserID)
        AddBlock(0)
        AddBlock(Client.Username)
        AddBlock(Client.Password)
        AddBlock(Client.Nickname)

        AddBlock(0)
        AddBlock(0)
        AddBlock(81766)
        AddBlock(0)
        AddBlock("1.11025")

        AddBlock(1)

        '1-File-Server: Only 1 Game Server
        Dim GameServer As alterWar.GameServer.Game = Globals.GetGameServer()
        AddBlock(1)
        AddBlock(GameServer.ServerName)
        AddBlock(GameServer.ServerIP)
        AddBlock(GameServer.ServerPort)
        AddBlock(GameServer.ConnectionCount)
        AddBlock(0)

        AddBlock(-1) 'clan id
        AddBlock(-1) 'clan name
        AddBlock(-1) '1= leader; 0= member?
        AddBlock(-1) 'clan symbol
        AddBlock(0) '???
        AddBlock(0) '???
    End Sub
End Class