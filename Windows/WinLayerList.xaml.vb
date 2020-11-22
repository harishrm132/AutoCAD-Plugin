Public Class WinLayerList
    Public Sub New(Layers As Objects.LayerObjectCollection)
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

        Me.DataContext = Layers
    End Sub

    Private Sub OK_Button_Click(sender As Object, e As Windows.RoutedEventArgs) Handles OK_Button.Click

        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(sender As Object, e As Windows.RoutedEventArgs) Handles Cancel_Button.Click

        Me.DialogResult = False
        Me.Close()
    End Sub
End Class
