Public Class PacketBase

    Private _OperationCode As Integer
    Public ReadOnly Property OperationCode() As Integer
        Get
            Return _OperationCode
        End Get
    End Property

    Private _Blocks As List(Of String)
    Public ReadOnly Property Blocks() As List(Of String)
        Get
            Return _Blocks
        End Get
    End Property

    Private _BlockIndex As Integer
    Public Property BlockIndex() As Integer
        Get
            Return _BlockIndex
        End Get
        Set(ByVal value As Integer)
            _BlockIndex = value
        End Set
    End Property

    Public Function GetBlock() As String
        Dim RetBlock As String
        If Blocks.Count >= BlockIndex Then
            RetBlock = Blocks(BlockIndex)
        Else
            RetBlock = Blocks(Blocks.Count)
        End If
        _BlockIndex += 1
        Return RetBlock
    End Function

    Public ReadOnly Property GetBlockIndex(ByVal I As Integer) As String
        Get
            Return _Blocks(I)
        End Get
    End Property

    Public Function GetPacket() As String
        Dim sPacket As String = String.Empty
        sPacket = Convert.ToString(GetTime()) & Convert.ToChar(&H20) & Convert.ToString(OperationCode) & Convert.ToChar(&H20)

        For Each B As String In Blocks
            sPacket += B.Replace(Convert.ToChar(&H20), Convert.ToChar(&H1D)) & Convert.ToChar(&H20)
        Next

        Return sPacket & Convert.ToChar(&HA)
    End Function
    Public Function GetPacketCrypted() As Byte()
        If _OperationCode = LOperationCodes.ClLogin OrElse _OperationCode = LOperationCodes.ClPatch OrElse _OperationCode = LOperationCodes.SvConnect OrElse _OperationCode = LOperationCodes.SvLogin OrElse _OperationCode = LOperationCodes.SvPatch Then
            Return GetCrypter().StringToBytes(GetCrypter().Crypt(Me, True))
        End If
        Return GetCrypter().StringToBytes(GetCrypter().Crypt(Me, False))
    End Function

    Public Sub AddBlock(ByVal Block As String)
        If Block = Nothing Then Block = "NULL"
        Blocks.Add(Block)
    End Sub
    Public Sub AddRoomInfo(ByVal Room As RoomClass)
        AddBlock(Room.RoomID)
        AddBlock(1)
        AddBlock(CInt(Room.RoomStatus))
        AddBlock(Room.IDOfPlayer(Room.RoomMaster))
        AddBlock(Room.RoomName.Replace(" ", Chr(&H1D)))
        AddBlock(CInt(Room.Passworded))
        If Room.MaxPlayer = 0 Then Room.MaxPlayer = 8
        AddBlock(Room.MaxPlayer)
        AddBlock(Room.ActivePlayers.Count)
        AddBlock(Room.MapID)
        AddBlock(3)
        AddBlock(2)
        AddBlock(0)
        AddBlock(Room.RoomMode)
        AddBlock(4)
        If Room.RoomStatus = RoomClass.eRoomStatus.Waiting AndAlso Room.ActivePlayers.Count < Room.MaxPlayer Then
            AddBlock(1) 'Joinable
        Else
            AddBlock(0) 'Unjoinable
        End If
        AddBlock(0)
        AddBlock(0) '1 = Room has supermaster; 0 = Normal room
        AddBlock(Room.RoomType)
        AddBlock(Room.LevelLimit)
        AddBlock(CInt(Room.PremiumOnly))
        AddBlock(CInt(Room.VoteKick))
        AddBlock(0)
        AddBlock(93) 'changes everytime.
        AddBlock(Room.Ping)
        AddBlock(-1)
    End Sub

    Sub New(ByVal Opc As Integer)
        _Blocks = New List(Of String)
        _BlockIndex = 0
        _OperationCode = 0

        _OperationCode = Opc
    End Sub
End Class
