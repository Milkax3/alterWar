Namespace Base.Classes
    Public Class cUserData

        Private _UserID As Integer
        Public ReadOnly Property UserID() As Integer
            Get
                Return _UserID
            End Get
        End Property

        Private _Username As String
        Public ReadOnly Property Username() As String
            Get
                Return _Username
            End Get
        End Property

        Private _Nickname As String
        Public ReadOnly Property Nickname() As String
            Get
                Return _Nickname
            End Get
        End Property

        Sub New(ByVal __UserID As Integer, ByVal __Username As String, ByVal __Nickname As String)
            _UserID = __UserID
            _Username = __Username
            _Nickname = __Nickname
        End Sub

    End Class
End Namespace
