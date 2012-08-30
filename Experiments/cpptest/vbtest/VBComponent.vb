Imports Kinectitude.Core.Base
Imports Kinectitude.Core.Data

Public Class VBComponent
    Inherits Component

    Private matcher As ITypeMatcher

    Public Property Type As ITypeMatcher
        Get
            Return matcher
        End Get
        Set(ByVal value As ITypeMatcher)
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
