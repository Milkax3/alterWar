Namespace Base.Packets
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
        Public Function GetPacketCryptedL() As Byte()
            Dim C As New Crypter()
            Return C.StringToBytes(C.Crypt(Me, True))
        End Function
        Public Function GetPacketCryptedG() As Byte()
            Dim C As New Crypter()
            Return C.StringToBytes(C.Crypt(Me, False))
        End Function
        Public Sub AddBlock(ByVal Block As String)
            If Block = Nothing Then Block = "NULL"
            Blocks.Add(Block)
        End Sub

        Sub New(ByVal Opc As Integer)
            _Blocks = New List(Of String)
            _BlockIndex = 0
            _OperationCode = 0

            _OperationCode = Opc
        End Sub
    End Class

End Namespace
