Imports System.Runtime.CompilerServices
Imports System.Net

Public Module Extensions

    ''' <summary>
    ''' Get the HexCode-Array of the Byte Array
    ''' </summary>
    ''' <param name="i">The Byte Array</param>
    ''' <returns>A HexCode-Array</returns>
    ''' <remarks></remarks>
    <Extension()> _
    Public Function ToHexArray(ByVal i As Byte()) As String()
        Dim hexArray As New List(Of String)
        For Each e As Byte In i
            hexArray.Add(e.ToString("X2"))
        Next
        Return hexArray.ToArray()
    End Function

    ''' <summary>
    ''' Reverses an IP Address (eg. 127.0.0.1 to 1.0.0.127)
    ''' </summary>
    ''' <param name="tString">The IP Address as String</param>
    ''' <returns>The reversed IP</returns>
    ''' <remarks></remarks>
    Private Function ReverseIP(ByVal tString As String) As String
        Dim bString As String() = tString.Split(New Char() {"."c})
        Dim tNew As String = ""
        For i As Integer = (bString.Length - 1) To -1 + 1 Step -1
            tNew += bString(i) & "."
        Next
        Return tNew.Substring(0, tNew.Length - 1)
    End Function

    ''' <summary>
    ''' Converts the IPAddress to a Long (Ready for the Peer2Peer Connection)
    ''' </summary>
    ''' <param name="addr">The Source IP Address</param>
    ''' <returns>The Long from the IP Address</returns>
    ''' <remarks></remarks>
    <Extension()> _
    Public Function GetAddressInt(ByVal addr As IPAddress) As Long
        Dim IpAddr As IPAddress = addr
        Dim IpAddrInt As Long = Long.Parse(IpAddr.Address)
        Return IpAddrInt
    End Function

    ''' <summary>
    ''' Converts the Long to the IP Address
    ''' </summary>
    ''' <param name="address">The IP Address (from Peer2Peer)</param>
    ''' <returns>The IP Address from the Long</returns>
    ''' <remarks></remarks>
    <Extension()> _
    Public Function ToIP(ByVal address As Long) As IPAddress
        Return IPAddress.Parse(ReverseIP(IPAddress.Parse(address).ToString()))
    End Function
End Module
