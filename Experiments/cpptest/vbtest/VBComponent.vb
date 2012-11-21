Imports Kinectitude.Core.Base
Imports Kinectitude.Core.Data

Public Class VBComponent
    Inherits Component

    Private matcher As TypeMatcher
    Public Property Type As TypeMatcher
        Get
            Return matcher
        End Get
        Set(ByVal value As TypeMatcher)
            If (Not matcher Is value) Then
                matcher = value
                Change("Type")
            End If
        End Set
    End Property

    Public Overrides Sub OnUpdate(ByVal frameDelta As Single)

    End Sub

    Public Overrides Sub Destroy()

    End Sub
End Class
