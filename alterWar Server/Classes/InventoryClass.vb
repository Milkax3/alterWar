'TODO:
'  FIX THIS CHANGING AND SO...
'  AND MAKE IT IN GamePackets.vb CORRECTLY
Namespace alterWar.Classes
    Public Class InventoryClass

        Private _HasPX As Boolean
        Public Property HasPX() As Boolean
            Get
                Return _HasPX
            End Get
            Set(ByVal value As Boolean)
                _HasPX = value
            End Set
        End Property

        Private _defaultItems As List(Of String)
        Public ReadOnly Property DefaultItems() As List(Of String)
            Get
                Return _defaultItems
            End Get
        End Property

        Private _Engineer As List(Of Item)
        Public ReadOnly Property Engineer() As List(Of Item)
            Get
                Return _Engineer
            End Get
        End Property
        Private _Medic As List(Of Item)
        Public ReadOnly Property Medic() As List(Of Item)
            Get
                Return _Medic
            End Get
        End Property
        Private _Sniper As List(Of Item)
        Public ReadOnly Property Sniper() As List(Of Item)
            Get
                Return _Sniper
            End Get
        End Property
        Private _Assault As List(Of Item)
        Public ReadOnly Property Assault() As List(Of Item)
            Get
                Return _Assault
            End Get
        End Property
        Private _Heavy As List(Of Item)
        Public ReadOnly Property Heavy() As List(Of Item)
            Get
                Return _Heavy
            End Get
        End Property

        Private _PxItems As List(Of Item)
        Public ReadOnly Property PxItems() As List(Of Item)
            Get
                Return _PxItems
            End Get
        End Property

        Private _NotUsed As List(Of Item)
        Public ReadOnly Property NotUsed() As List(Of Item)
            Get
                Return _NotUsed
            End Get
        End Property

        Sub New()
            _defaultItems = New List(Of String)
            _defaultItems.Add("DA02")
            _defaultItems.Add("DB01")
            _defaultItems.Add("DF01")
            _defaultItems.Add("DG05")
            _defaultItems.Add("DC02")
            _defaultItems.Add("DJ01")
            _defaultItems.Add("DR01")
            _defaultItems.Add("DQ02")
            _defaultItems.Add("DN03")
            _defaultItems.Add("DL01")

            _Engineer = New List(Of Item)
            _Medic = New List(Of Item)
            _Sniper = New List(Of Item)
            _Assault = New List(Of Item)
            _Heavy = New List(Of Item)
            _NotUsed = New List(Of Item)
            _PxItems = New List(Of Item)

            For i As Integer = 0 To 7 'clear all slots
                _Engineer.Add(New Item() With {.ItemCode = "^", .ExpireDate = 0})
                _Medic.Add(New Item() With {.ItemCode = "^", .ExpireDate = 0})
                _Sniper.Add(New Item() With {.ItemCode = "^", .ExpireDate = 0})
                _Assault.Add(New Item() With {.ItemCode = "^", .ExpireDate = 0})
                _Heavy.Add(New Item() With {.ItemCode = "^", .ExpireDate = 0})
            Next

            'default weapons

            _Engineer(0) = New Item() With {.ItemCode = "DA02", .ExpireDate = "2147483647"}
            _Medic(0) = New Item() With {.ItemCode = "DA02", .ExpireDate = "2147483647"}
            _Sniper(0) = New Item() With {.ItemCode = "DA02", .ExpireDate = "2147483647"}
            _Assault(0) = New Item() With {.ItemCode = "DA02", .ExpireDate = "2147483647"}
            _Heavy(0) = New Item() With {.ItemCode = "DA02", .ExpireDate = "2147483647"}

            _Engineer(1) = New Item() With {.ItemCode = "DB01", .ExpireDate = "2147483647"}
            _Medic(1) = New Item() With {.ItemCode = "DB01", .ExpireDate = "2147483647"}
            _Sniper(1) = New Item() With {.ItemCode = "DB01", .ExpireDate = "2147483647"}
            _Assault(1) = New Item() With {.ItemCode = "DB01", .ExpireDate = "2147483647"}
            _Heavy(1) = New Item() With {.ItemCode = "DB01", .ExpireDate = "2147483647"}

            _Engineer(2) = New Item() With {.ItemCode = "DF01", .ExpireDate = "2147483647"}
            _Medic(2) = New Item() With {.ItemCode = "DF01", .ExpireDate = "2147483647"}
            _Sniper(2) = New Item() With {.ItemCode = "DG05", .ExpireDate = "2147483647"}
            _Assault(2) = New Item() With {.ItemCode = "DC02", .ExpireDate = "2147483647"}
            _Heavy(2) = New Item() With {.ItemCode = "DJ01", .ExpireDate = "2147483647"}

            _Engineer(3) = New Item() With {.ItemCode = "DR01", .ExpireDate = "2147483647"}
            _Medic(3) = New Item() With {.ItemCode = "DQ02", .ExpireDate = "2147483647"}
            _Sniper(3) = New Item() With {.ItemCode = "DN03", .ExpireDate = "2147483647"}
            _Assault(3) = New Item() With {.ItemCode = "DN03", .ExpireDate = "2147483647"}
            _Heavy(3) = New Item() With {.ItemCode = "DL01", .ExpireDate = "2147483647"}
        End Sub

        Public Function AllItems() As List(Of Item)
            Dim L As New List(Of Item)
            For Each cE As Item In _Engineer
                If cE.ItemCode <> "^" AndAlso L.Contains(cE.ItemCode) = False AndAlso _defaultItems.Contains(cE.ItemCode) = False Then L.Add(cE)
            Next
            For Each cM As Item In _Medic
                If cM.ItemCode <> "^" AndAlso L.Contains(cM.ItemCode) = False AndAlso _defaultItems.Contains(cM.ItemCode) = False Then L.Add(cM)
            Next
            For Each cS As Item In _Sniper
                If cS.ItemCode <> "^" AndAlso L.Contains(cS.ItemCode) = False AndAlso _defaultItems.Contains(cS.ItemCode) = False Then L.Add(cS)
            Next
            For Each cA As Item In _Assault
                If cA.ItemCode <> "^" AndAlso L.Contains(cA.ItemCode) = False AndAlso _defaultItems.Contains(cA.ItemCode) = False Then L.Add(cA)
            Next
            For Each cH As Item In _Heavy
                If cH.ItemCode <> "^" AndAlso L.Contains(cH.ItemCode) = False AndAlso _defaultItems.Contains(cH.ItemCode) = False Then L.Add(cH)
            Next
            For Each cPx As Item In _PxItems
                If cPx.ItemCode <> "^" AndAlso L.Contains(cPx.ItemCode) = False AndAlso _defaultItems.Contains(cPx.ItemCode) = False Then L.Add(cPx)
            Next
            For Each cNu As Item In _NotUsed
                If cNu.ItemCode <> "^" AndAlso L.Contains(cNu.ItemCode) = False AndAlso _defaultItems.Contains(cNu.ItemCode) = False Then L.Add(cNu)
            Next
            L = RemoveDoubleItems(L)
            Return L
        End Function

        Private Function HaveItemCodeInList(ByRef Lst As List(Of Item), ByVal ItemCode As String) As Boolean
            Dim Have As Boolean = False
            For Each I As Item In Lst
                If I.ItemCode = ItemCode Then
                    Have = True
                    Return True
                    Exit For
                End If
            Next
            Return Have
        End Function
        Private Function RemoveDoubleItems(ByVal List As List(Of Item)) As List(Of Item)
            Dim KeyList As New Generic.Dictionary(Of Item, String)
            Dim NewList As New List(Of Item)

            For Each Item As Item In List
                If KeyList.ContainsKey(Item) = False Then
                    KeyList.Add(Item, String.Empty)
                    NewList.Add(Item)
                End If
            Next
            Return NewList
        End Function

        Public Enum eDays As Integer
            Days3 = 0
            Days7 = 1
            Days15 = 2
            Days30 = 3
        End Enum
        Public Enum eErrorCodes As Integer
            NoPremium = 98010
            CantBuy = 97020
            LowMoney = 97040

            Success = 1
        End Enum
    End Class

    Public Class Item
        Private _ItemCode As String
        Public Property ItemCode() As String
            Get
                Return _ItemCode
            End Get
            Set(ByVal value As String)
                _ItemCode = value
            End Set
        End Property

        Private _ExpireDate As Integer
        Public Property ExpireDate() As Integer
            Get
                Return _ExpireDate
            End Get
            Set(ByVal value As Integer)
                _ExpireDate = value
            End Set
        End Property
    End Class

    Public Class InventoryFunctions
        Public Shared Function TimeAdd(ByVal Add As Integer) As String
            Dim [Date] As String = ""
            Dim Day As String = ""
            Dim Month As String = ""
            If DateTime.Now.AddDays(Add).Day < 10 Then
                Day = "0" & DateTime.Now.AddDays(Add).Day.ToString()
            Else

                Day = DateTime.Now.AddDays(Add).Day.ToString()
            End If


            If DateTime.Now.AddDays(Add).Month < 10 Then
                Month = "0" & (DateTime.Now.AddDays(Add).Month + 1).ToString()
            Else

                Month = (DateTime.Now.AddDays(Add).Month + 1).ToString()
            End If


            [Date] = 11 & Month & Day & (DateTime.Now.Hour + 1).ToString()

            Return [Date]
        End Function
    End Class

    Public Module Extensions
        <Runtime.CompilerServices.Extension()> _
        Public Function Contains(ByVal L As List(Of Item), ByVal itemCode As String) As Boolean
            Dim ContainsItem As Boolean = False
            For Each tI As Item In L
                If tI.ItemCode = itemCode Then
                    ContainsItem = True
                End If
            Next
            Return ContainsItem
        End Function
    End Module
End Namespace