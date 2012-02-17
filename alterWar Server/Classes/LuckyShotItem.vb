Namespace alterWar.Classes
    Public Class LuckyShotItem

        Private _DinarItem As Boolean
        Public Property DinarItem() As Boolean
            Get
                Return _DinarItem
            End Get
            Set(ByVal value As Boolean)
                _DinarItem = value
            End Set
        End Property

        Private _WeaponCode As String
        Public Property WeaponCode() As String
            Get
                Return _WeaponCode
            End Get
            Set(ByVal value As String)
                _WeaponCode = value
            End Set
        End Property

        Private _Bet As Integer
        Public Property Bet() As Integer
            Get
                Return _Bet
            End Get
            Set(ByVal value As Integer)
                _Bet = value
            End Set
        End Property

        Private _Stock As Integer
        Public Property Stock() As Integer
            Get
                Return _Stock
            End Get
            Set(ByVal value As Integer)
                _Stock = value
            End Set
        End Property

        Sub New(ByVal __DinarItem As Boolean, ByVal __WeaponCode As String, ByVal __Bet As Integer, ByVal __Stock As Integer)
            _DinarItem = __DinarItem
            _WeaponCode = __WeaponCode
            _Bet = __Bet
            _Stock = __Stock
        End Sub
    End Class
End Namespace
