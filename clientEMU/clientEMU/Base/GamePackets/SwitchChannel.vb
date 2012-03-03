Namespace Base.Packets.GamePackets
    Public Class SwitchChannel
        Inherits PacketBase

        Public Enum eChannel
            CQC = 1
            UO = 2
            BG = 3
            AI = 4
        End Enum

        Sub New(ByVal Channel As eChannel)
            MyBase.New(28673)
            AddBlock(Channel)
        End Sub
    End Class
End Namespace

