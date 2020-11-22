Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices

Namespace Objects
    Public Class LayerObject
        Inherits BaseObject

        Private _isFrz As Boolean = False
        Public Property IsFrozen As Boolean
            Get
                Return _isFrz
            End Get
            Set(value As Boolean)
                _isFrz = value
            End Set
        End Property

    End Class

    Public Class LayerObjectCollection
        Inherits ObjectModel.ObservableCollection(Of LayerObject)

        Public Sub GetCollectionfromDrawing(DwgDatabase As Database)

            Using trans As Transaction = DwgDatabase.TransactionManager.StartTransaction
                Try
                    Dim lyrTbl As LayerTable = trans.GetObject(DwgDatabase.LayerTableId, OpenMode.ForRead)
                    Dim lyrTblRec As LayerTableRecord
                    Dim lyrObj As LayerObject

                    For Each lyrId As ObjectId In lyrTbl
                        lyrTblRec = trans.GetObject(lyrId, OpenMode.ForRead)
                        lyrObj = New LayerObject
                        With lyrObj
                            .BaseObjectID = lyrId
                            .IsFrozen = lyrTblRec.IsFrozen
                            .Name = lyrTblRec.Name
                        End With
                        Me.Add(lyrObj)
                    Next
                    trans.Commit()
                Catch ex As Exception
                    Application.ShowAlertDialog("Error in LayerObjectCollection->GetCollectionFromDrawing->Transaction" &
                                                vbLf & ex.Message)
                End Try
            End Using

        End Sub

        Public Shared Function CurrentLayerName() As String
            Dim resstring As String = ""

            Using db As Database = HostApplicationServices.WorkingDatabase
                Try

                    Using trans As Transaction = db.TransactionManager.StartTransaction
                        Try
                            Dim layertablerec As LayerTableRecord = trans.GetObject(db.Clayer, OpenMode.ForRead)
                            resstring = layertablerec.Name
                            trans.Commit()
                        Catch ex As Exception

                        End Try
                    End Using

                Catch ex As Exception

                End Try
            End Using

        End Function





    End Class



End Namespace

