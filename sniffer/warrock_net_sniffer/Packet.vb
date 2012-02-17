Imports System
Imports System.Text
Imports System.Net
Imports System.Net.Sockets
Imports System.Collections.Generic

    Public Enum Precedence
        Routine = 0
        Priority = 1
        Immediate = 2
        Flash = 3
        FlashOverride = 4
        CRITICECP = 5
        InternetworkControl = 6
        NetworkControl = 7
    End Enum
    Public Enum Delay
        NormalDelay = 0
        LowDelay = 1
    End Enum
    Public Enum Throughput
        NormalThroughput = 0
        HighThroughput = 1
    End Enum
    Public Enum Reliability
        NormalReliability = 0
        HighReliability = 1
    End Enum
    Public Enum Protocol
        Ggp = 3
        Icmp = 1
        Idp = 22
        Igmp = 2
        IP = 4
        ND = 77
        Pup = 12
        Tcp = 6
        Udp = 17
        Other = -1
    End Enum
    Public Class Packet
        Private m_Raw() As Byte
        Private m_Time As DateTime
        Private m_Version As Integer
        Private m_HeaderLength As Integer
        Private m_Precedence As Precedence
        Private m_Delay As Delay
        Private m_Throughput As Throughput
        Private m_Reliability As Reliability
        Private m_TotalLength As Integer
        Private m_Identification As Integer
        Private m_TimeToLive As Integer
        Private m_Protocol As Protocol
        Private m_Checksum() As Byte
        Private m_SourceAddress As String
        Private m_DestinationAddress As String
        Private m_SourcePort As Integer
        Private m_DestinationPort As Integer


        Public Sub New(ByVal raw() As Byte)
            Me.New(raw, DateTime.Now)
        End Sub
        Public Sub New(ByVal raw() As Byte, ByVal time As DateTime)

            If raw Is Nothing Then Throw New ArgumentNullException
            If raw.Length < 20 Then Throw New ArgumentException
            m_Raw = raw
            m_Time = time
            'm_Version = (raw(0) & &HF0) > 4
            m_HeaderLength = (raw(0) And &HF) * 4
            If (raw(0) And &HF) < 5 Then Throw New ArgumentException
            m_Precedence = ((raw(1) And &HE0) > 5)
            m_Delay = ((raw(1) And &H10) > 4)
            m_Throughput = ((raw(1) And &H8) > 3)
            m_Reliability = ((raw(1) And &H4) > 2)
            m_TotalLength = raw(2) * 256 + raw(3)
            If Not m_TotalLength = raw.Length Then Throw New ArgumentException ' invalid size of packet
            m_Identification = raw(4) * 256 + raw(5)
            m_TimeToLive = raw(8)

            If ([Enum].IsDefined(GetType(Protocol), CType(raw(9), Protocol))) Then
                m_Protocol = raw(9)
            Else
                m_Protocol = Protocol.Other
            End If

            ReDim m_Checksum(2)
            m_Checksum(0) = raw(11)
            m_Checksum(1) = raw(10)



            Try
                m_SourceAddress = GetIPAddress(raw, 12)
                m_DestinationAddress = GetIPAddress(raw, 16)
            Catch ex As Exception
                'MsgBox(ex.Message)
            End Try

            If m_Protocol = Protocol.Tcp Or m_Protocol = Protocol.Udp Then
                m_SourcePort = raw(m_HeaderLength) * 256 + raw(m_HeaderLength + 1)
                m_DestinationPort = raw(m_HeaderLength + 2) * 256 + raw(m_HeaderLength + 3)
            Else
                m_SourcePort = -1
                m_DestinationPort = -1
            End If

        End Sub

        Protected ReadOnly Property Raw() As Byte()
            Get
                Return m_Raw
            End Get
        End Property
        Public ReadOnly Property Time() As DateTime
            Get
                Return m_Time
            End Get
        End Property
        Public ReadOnly Property Version() As Integer
            Get
                Return m_Version
            End Get
        End Property
        Public ReadOnly Property HeaderLength() As Integer
            Get
                Return m_HeaderLength
            End Get
        End Property
        Public ReadOnly Property Precedence() As Precedence
            Get
                Return m_Precedence
            End Get
        End Property
        Public ReadOnly Property Delay() As Delay
            Get
                Return m_Delay
            End Get
        End Property
        Public ReadOnly Property Throughput() As Throughput
            Get
                Return m_Throughput
            End Get
        End Property
        Public ReadOnly Property Reliability() As Reliability
            Get
                Return m_Reliability
            End Get
        End Property
        Public ReadOnly Property TotalLength() As Integer
            Get
                Return m_TotalLength
            End Get
        End Property
        Public ReadOnly Property Identification() As Integer
            Get
                Return m_Identification
            End Get
        End Property
        Public ReadOnly Property TimeToLive() As Integer
            Get
                Return m_TimeToLive
            End Get
        End Property
        Public ReadOnly Property Protocol() As Protocol
            Get
                Return m_Protocol
            End Get
        End Property
        Public ReadOnly Property Checksum() As Byte()
            Get
                Return m_Checksum
            End Get
        End Property
        Public ReadOnly Property SourceAddress() As String
            Get
                Return m_SourceAddress
            End Get
        End Property
        Public ReadOnly Property DestinationAddress() As String
            Get
                Return m_DestinationAddress
            End Get
        End Property
        Public ReadOnly Property SourcePort() As Integer
            Get
                Return m_SourcePort
            End Get
        End Property
        Public ReadOnly Property DestinationPort() As Integer
            Get
                Return m_DestinationPort
            End Get
        End Property
        Public ReadOnly Property Source() As String
            Get
                If Not m_SourcePort = -1 Then
                    Return SourceAddress.ToString() + ":" + m_SourcePort.ToString()
                Else
                    Return SourceAddress.ToString()
                End If
            End Get
        End Property
        Public ReadOnly Property Destination() As String
            Get
                If Not m_DestinationPort = -1 Then
                    Return DestinationAddress.ToString() + ":" + m_DestinationPort.ToString()
                Else
                    Return DestinationAddress.ToString()
                End If
            End Get
        End Property
        Public Shadows Function ToString() As String
            Return Me.ToString(False)
        End Function
        Public Shadows Function GetBytes() As Byte()
            Return Raw
        End Function

        Public Function GetDataBytes() As Byte()
            Dim tmpLB As New List(Of Byte)
            For Each b As Byte In Raw
                tmpLB.Add(b)
            Next
            tmpLB.RemoveRange(0, 31)

            Return tmpLB.ToArray()
    End Function

    Public Function GetDataBytesUDP() As Byte()
        Dim tmpLB As New List(Of Byte)
        For Each b As Byte In Raw
            tmpLB.Add(b)
        Next
        tmpLB.RemoveRange(0, 28)

        Return tmpLB.ToArray()
    End Function

        Public Shadows Function ToString(ByVal RawData As Boolean) As String

            Dim sb As StringBuilder = New StringBuilder(Raw.Length)
            Dim i As Integer, j As Integer

            If RawData Then

            Else
                Dim rawString As String = Me.ToString(True)
                For i = 0 To Raw.Length - 1 Step 16
                    For j = i To Raw.Length - 1 And j < i + 16
                        sb.Append(Raw(j).ToString("X2") + " ")
                    Next
                Next
                If rawString.Length < i + 16 Then
                    sb.Append(" ", ((16 - (rawString.Length Mod 16)) Mod 16) * 3)
                    sb.Append(" " + rawString.Substring(i) + vbCrLf)
                Else
                    sb.Append(" " + rawString.Substring(i, 16) + "\r\n")
                End If

            End If
            Return sb.ToString()


        End Function
        Public Function GetIPAddress(ByVal bArray() As Byte, ByVal nStart As Integer) As String
            Dim tmp(3) As Byte
            If bArray.Length > nStart + 2 Then
                tmp(0) = bArray(nStart)
                tmp(1) = bArray(nStart + 1)
                tmp(2) = bArray(nStart + 2)
                tmp(3) = bArray(nStart + 3)
            End If

            Return tmp(0) & "." & tmp(1) & "." & tmp(2) & "." & tmp(3)
        End Function

    End Class