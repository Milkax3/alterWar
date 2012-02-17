Namespace Base.Packets.GamePackets
    Public Class WelcomePacket
        Inherits PacketBase

        Sub New()
            MyBase.New(24832)
            AddBlock("dla#qud$wlr%aks^tp&")
            AddBlock(5300)
            AddBlock("02004c4f4f50")

            '24832 dla#qud$wlr%aks^tp& 5300 02004c4f4f50 
        End Sub
    End Class
End Namespace

