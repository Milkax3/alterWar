Namespace Base.Packets.GamePackets
    Public Class Chat
        Inherits PacketBase

        Sub New(ByVal _Message As String)
            MyBase.New(29696)
            AddBlock(8)
            AddBlock(0)
            AddBlock("NULL")
            AddBlock("G00DM00D3>>" & _Message.Replace(" ", Chr(&H1D)))
        End Sub
    End Class
End Namespace

