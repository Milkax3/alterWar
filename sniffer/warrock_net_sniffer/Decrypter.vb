Public Class Decrypter
    Public Function UnCrypt(ByVal b As Byte(), ByVal IsLoginPacket As Boolean) As String
        Dim tBytes As Byte() = b
        Dim tBytesUncrypted As New List(Of Byte)
        For i As Integer = 9 To tBytes.Length - 2
            If IsLoginPacket Then
                tBytesUncrypted.Add(Convert.ToByte(tBytes(i) Xor &HC3))
            Else
                tBytesUncrypted.Add(Convert.ToByte(Convert.ToByte(Convert.ToByte(tBytes(i) Xor &HC3) Xor 230) Xor &H60))
            End If
        Next
        Return System.Text.Encoding.Default.GetString(tBytesUncrypted.ToArray())
    End Function
    Public Function UnCryptS(ByVal b As Byte(), ByVal IsLoginPacket As Boolean) As String
        Dim tBytes As Byte() = b
        Dim tBytesUncrypted As New List(Of Byte)
        For i As Integer = 9 To tBytes.Length - 2
            If IsLoginPacket Then
                tBytesUncrypted.Add(Convert.ToByte(tBytes(i) Xor &H96))
            Else
                tBytesUncrypted.Add(Convert.ToByte(Convert.ToByte(Convert.ToByte(tBytes(i) Xor &H96) Xor &HCF) Xor &H48))
            End If
        Next
        Return System.Text.Encoding.Default.GetString(tBytesUncrypted.ToArray())
    End Function

    Public Function StringToBytes(ByVal Input As String) As Byte()
        Return Text.Encoding.[Default].GetBytes(Input)
    End Function
    Public Function BytesToString(ByVal Input As Byte()) As String
        Return Text.Encoding.[Default].GetString(Input)
    End Function
End Class
