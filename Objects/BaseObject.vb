Imports System.ComponentModel
Imports Autodesk.AutoCAD.DatabaseServices


Namespace Objects
    Public Class BaseObject
        Implements ComponentModel.INotifyPropertyChanged

        Private _bname As String = ""
        Public Property Name As String
            Get
                Return _bname
            End Get
            Set(value As String)
                _bname = value
                RaisePropertyChanged("Name")
            End Set
        End Property

        Private _bID As ObjectId = Nothing
        Public Property BaseObjectID As ObjectId
            Get
                Return _bID
            End Get
            Set(value As ObjectId)
                _bID = value
                RaisePropertyChanged("BaseObjectID")
            End Set
        End Property

        Private _IsSel As Boolean = Nothing
        Public Property IsSelected As Boolean
            Get
                Return _IsSel
            End Get
            Set(value As Boolean)
                _IsSel = value
                RaisePropertyChanged("IsSelected")
            End Set
        End Property

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Public Sub RaisePropertyChanged(ByVal propertyname As String)
            RaiseEvent PropertyChanged(Me, New ComponentModel.PropertyChangedEventArgs(propertyname))
        End Sub

    End Class

End Namespace

