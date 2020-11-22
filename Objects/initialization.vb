Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Public Class initialization
    Implements IExtensionApplication

#Region "Commands"
    <CommandMethod("WriteMessage")> Public Sub cmdWriteMsg()
        Dim ed As Editor
        Dim doc As Document
        doc = Application.DocumentManager.MdiActiveDocument
        ed = doc.Editor
        If IsSavedFile() = True Then
            ed.WriteMessage(vbLf & "An Exisiting file")
        Else
            ed.WriteMessage(vbLf & "New File")
        End If
    End Sub

    <CommandMethod("LIGetversion")> Public Sub cmdAcadVersion()
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor

        Select Case Application.DocumentManager.MdiActiveDocument.Database.LastSavedAsVersion
            Case Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1032
                ed.WriteMessage(vbLf & "AutoCAD2018")
            Case Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC2To21
                ed.WriteMessage(vbLf & "AutoCAD2021")
            Case Else
                ed.WriteMessage(vbLf & "Too old")
        End Select
    End Sub

    <CommandMethod("GetLoginLastName")> Public Sub cmdLogin()
        Dim logname As String = Application.GetSystemVariable("LoginName")
        Dim lastNameLetter As String = logname.Substring(1, 1).ToUpper

        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        ed.WriteMessage(vbLf & lastNameLetter & logname.Substring(2))

    End Sub

    <CommandMethod("GetDocCount")> Public Sub CmdDocCount()
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        ed.WriteMessage(vbLf & "Count of Opened Doc:" & Application.DocumentManager.Count)
    End Sub

    <CommandMethod("GetDocProp")> Public Sub CmdDocProp()
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        acDoc.Editor.WriteMessage(vbLf & acDoc.Name & vbLf & acDoc.FormatForSave & vbLf)
    End Sub

    <CommandMethod("NewDrawing")> Public Sub CmdNewDrawing()
        Dim DocCol As DocumentCollection = Application.DocumentManager
        Dim newdoc As Document = DocCol.Add("C:\Users\haris\AppData\Local\Autodesk\AutoCAD 2021\R24.0\enu\Template\acad.dwt")
        DocCol.MdiActiveDocument = newdoc
    End Sub

    <CommandMethod("DBprops")> Public Sub cmddatabaseprops()
        Dim db As Database = Application.DocumentManager.CurrentDocument.Database
        db.Ltscale = 48
    End Sub

    <CommandMethod("DBobjectID")> Public Sub cmdObjectID()
        Using db As Database = HostApplicationServices.WorkingDatabase 'Cleaner Method to get database
            Dim lyrTableID As ObjectId = db.LayerTableId
            Dim Checkexist As Boolean = lyrTableID.IsErased
        End Using
    End Sub

    <CommandMethod("ACtransactions")> Public Sub cmdTranscations() 'Transcations
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor

        Using db As Database = HostApplicationServices.WorkingDatabase
            Try
                Dim lyrTableID As ObjectId = db.LayerTableId

                Using trans As Transaction = db.TransactionManager.StartTransaction
                    Try
                        Dim lyrtable As LayerTable = trans.GetObject(lyrTableID, OpenMode.ForRead)
                        Dim i As Integer = 0
                        For Each lyrID As ObjectId In lyrtable
                            i += 1
                        Next
                        ed.WriteMessage(vbLf & "layer count:" & i)
                        trans.Commit()
                    Catch ex As Exception
                        Application.ShowAlertDialog("Error with Transacation" & vbLf & ex.Message)
                    End Try
                End Using


            Catch ex As Exception
                Application.ShowAlertDialog("Error with database" & vbLf & ex.Message)
            End Try
        End Using

    End Sub

    <CommandMethod("ACCreatelayer")> Public Sub cmdCreatelayer()
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor

        Using db As Database = HostApplicationServices.WorkingDatabase
            Try
                Dim lyrTableID As ObjectId = db.LayerTableId

                Using trans As Transaction = db.TransactionManager.StartTransaction
                    Try
                        Dim lyrtable As LayerTable = trans.GetObject(lyrTableID, OpenMode.ForWrite)

                        If IsLayerExists(trans, lyrtable, "Solid Lines") = False Then
                            Dim newlyr As New LayerTableRecord
                            newlyr.Name = "Solid Lines"

                            lyrtable.Add(newlyr)
                            trans.AddNewlyCreatedDBObject(newlyr, True)
                        Else
                            Application.ShowAlertDialog(vbLf & "Layer alredy exist")
                        End If

                        trans.Commit()
                    Catch ex As Exception
                        Application.ShowAlertDialog("Error with Transacation" & vbLf & ex.Message)
                    End Try
                End Using


            Catch ex As Exception
                Application.ShowAlertDialog("Error with database" & vbLf & ex.Message)
            End Try
        End Using

    End Sub

    <CommandMethod("ACCreateLine")> Public Sub cmdCreateLine()
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor
        Using db As Database = HostApplicationServices.WorkingDatabase
            Try
                Dim spaceblockID As ObjectId = db.CurrentSpaceId
                Dim Point1 As New Point3d(0, 0, 0)
                Dim Point2 As New Point3d(10, 10, 10)

                Using trans As Transaction = db.TransactionManager.StartTransaction
                    Try
                        Dim CurSpaceblock As BlockTableRecord = trans.GetObject(spaceblockID, OpenMode.ForWrite)

                        Dim lineobj As New Line(Point1, Point2)
                        CurSpaceblock.AppendEntity(lineobj)
                        trans.AddNewlyCreatedDBObject(lineobj, True)

                        trans.Commit()
                    Catch ex As Exception
                        Application.ShowAlertDialog("Error with Transacation" & vbLf & ex.Message)
                    End Try
                End Using

            Catch ex As Exception
                Application.ShowAlertDialog("Error with database" & vbLf & ex.Message)
            End Try

        End Using

    End Sub

    <CommandMethod("ACGetpoint")> Public Sub cmdGetPoint()
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor


        Dim PrmpointOps As New PromptPointOptions(vbLf & "Pick a Point:")
        With PrmpointOps
            .AllowArbitraryInput = False
            .AllowNone = True
        End With

        Dim prpointres1 As PromptPointResult = ed.GetPoint(PrmpointOps)
        If (prpointres1.Status <> PromptStatus.OK) Then Exit Sub
        With PrmpointOps
            .BasePoint = prpointres1.Value
            .UseBasePoint = True
        End With

        Dim prpointres2 As PromptPointResult = ed.GetPoint(PrmpointOps)
        If (prpointres2.Status <> PromptStatus.OK) Then Exit Sub
        With PrmpointOps
            .BasePoint = prpointres2.Value
            .UseBasePoint = True
        End With

        ed.WriteMessage(vbLf & prpointres2.StringResult.ToString())
    End Sub

    <CommandMethod("ACgetDistance")> Public Sub cmdGetDistance()
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor

        Dim PrmpointOps As New PromptDistanceOptions(vbLf & "Pick a Distance:")
        With PrmpointOps
            .AllowArbitraryInput = False
            .AllowNegative = False
            .AllowNone = True
            .AllowZero = True
            .DefaultValue = 1
            .Only2d = True
            .UseDefaultValue = True
        End With

        Dim prDistRes As PromptDoubleResult = ed.GetDistance(PrmpointOps)
        If prDistRes.Status <> PromptStatus.OK Then Exit Sub

        ed.WriteMessage(vbLf & "Distnace is:" & prDistRes.Value.ToString())
        ed.WriteMessage(vbLf & "Distnace is:" & Math.Round(prDistRes.Value, HostApplicationServices.WorkingDatabase.Luprec, MidpointRounding.AwayFromZero))

    End Sub

    <CommandMethod("ACSelectEntity")> Public Sub cmdSelectEntity()
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor

        Dim prmselectops As New PromptEntityOptions(vbLf & "Select a line:")

        With prmselectops
            .AllowNone = True
            .AllowObjectOnLockedLayer = True
            .SetRejectMessage(vbLf & "Select object has to be line")
            .AddAllowedClass(GetType(Line), True)
        End With

        Dim PromptEntityresult As PromptEntityResult = ed.GetEntity(prmselectops)
        If PromptEntityresult.Status <> PromptStatus.OK Then Exit Sub

        Using db As Database = HostApplicationServices.WorkingDatabase
            Try

                Using trans As Transaction = db.TransactionManager.StartTransaction
                    Try
                        Dim ObjID As ObjectId = PromptEntityresult.ObjectId
                        Dim lobj As Line = trans.GetObject(ObjID, OpenMode.ForRead)

                        ed.WriteMessage(vbLf & "Distnace is:" & Math.Round(lobj.Length, HostApplicationServices.WorkingDatabase.Luprec, MidpointRounding.AwayFromZero))

                        trans.Commit()
                    Catch ex As Exception
                        Application.ShowAlertDialog("Error with Transacation" & vbLf & ex.Message)
                    End Try
                End Using

            Catch ex As Exception
                Application.ShowAlertDialog("Error with database" & vbLf & ex.Message)
            End Try
        End Using

    End Sub

    <CommandMethod("ACGetSelectionSet")> Public Sub cmdGetSelectionSet()
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor

        Dim prmselectops As New PromptSelectionOptions

        With prmselectops
            .AllowDuplicates = False
            .MessageForAdding = vbLf & "select lines to count length"
            .MessageForRemoval = vbLf & "select line to remove"
            .RejectObjectsFromNonCurrentSpace = True
            .RejectObjectsOnLockedLayers = False
            .RejectPaperspaceViewport = True
        End With

        Dim tvalue(0) As TypedValue
        tvalue(0) = New TypedValue(DxfCode.Start, "LINE")
        Dim selectionFilter As New SelectionFilter(tvalue)

        Dim PromptSelresult As PromptSelectionResult = ed.GetSelection(prmselectops, selectionFilter)
        If PromptSelresult.Status <> PromptStatus.OK Then Exit Sub


        Using db As Database = HostApplicationServices.WorkingDatabase
            Try

                Using trans As Transaction = db.TransactionManager.StartTransaction
                    Try
                        Dim SelSET As SelectionSet = PromptSelresult.Value
                        Dim lOBJ As Line
                        Dim lentotal As Double = 0

                        For Each selOBJ As SelectedObject In SelSET
                            lOBJ = trans.GetObject(selOBJ.ObjectId, OpenMode.ForRead)
                            lentotal += lOBJ.Length 'total length of line objects 
                        Next

                        ed.WriteMessage(vbLf & "Total length is:" & Math.Round(lOBJ.Length, HostApplicationServices.WorkingDatabase.Luprec, MidpointRounding.AwayFromZero))

                        trans.Commit()
                    Catch ex As Exception
                        Application.ShowAlertDialog("Error with Transacation" & vbLf & ex.Message)
                    End Try
                End Using

            Catch ex As Exception
                Application.ShowAlertDialog("Error with database" & vbLf & ex.Message)
            End Try
        End Using

    End Sub

    <CommandMethod("TestObjects")> Public Sub cmdtestobjects()

        Dim bobj As Objects.BaseObject
        bobj.Name = "LIKE"
        bobj.IsSelected = True

        Dim oID As Object = bobj.BaseObjectID

    End Sub

    <CommandMethod("ACInheritance")> Public Sub cmdInheritance()

        Dim bobj As New Objects.LayerObject
        bobj.Name = "LIKE"
        bobj.IsSelected = True
        bobj.IsFrozen = False

        Dim oID As Object = bobj.BaseObjectID

    End Sub

    <CommandMethod("ACGetLayerlist")> Public Sub cmdGetLayerlist()
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor

        ed.WriteMessage(vbLf & "Current Layer name:" & Objects.LayerObjectCollection.CurrentLayerName())
    End Sub

    <CommandMethod("ACShowLayers")> Public Sub cmdShowlayers()
        Dim lyrobj As New Objects.LayerObjectCollection
        lyrobj.GetCollectionfromDrawing(HostApplicationServices.WorkingDatabase)

        Dim winlyr As New WinLayerList(lyrobj)

        Application.ShowModalWindow(winlyr)

    End Sub

    <CommandMethod("ACChangeLayer")> Public Sub cmdChangeLayers()
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor

        Dim prSelOpts As New PromptSelectionOptions
        With prSelOpts
            .AllowDuplicates = False
            .MessageForAdding = vbLf & "Select objects to change layer: "
            .MessageForRemoval = vbLf & "Remove objects from selection: "
            .RejectObjectsFromNonCurrentSpace = True
            .RejectObjectsOnLockedLayers = True
            .RejectPaperspaceViewport = True
        End With

        Dim prSelRes As PromptSelectionResult = ed.GetSelection(prSelOpts)
        If prSelRes.Status <> PromptStatus.OK Then Exit Sub

        Dim lyrs As New Objects.LayerObjectCollection
        lyrs.GetCollectionfromDrawing(HostApplicationServices.WorkingDatabase)

        Dim winLyr As New WinSelectLayer(lyrs) 'Initiate UserForm
        If Application.ShowModalWindow(winLyr) <> True Then Exit Sub

        Using trans As Transaction = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction
            Try
                Dim ent As Entity

                For Each selObj As SelectedObject In prSelRes.Value
                    ent = trans.GetObject(selObj.ObjectId, OpenMode.ForWrite)
                    ent.LayerId = winLyr.CboLayers.SelectedValue
                Next
                trans.Commit()
            Catch ex As Exception
                Application.ShowAlertDialog("Error in LIChangeLayer->Transcation" & vbLf & ex.Message)
            End Try
        End Using

        ed.WriteMessage(vbLf & prSelRes.Value.Count & " objects moved to " & winLyr.CboLayers.SelectedItem.Name & " layer.")
    End Sub

    <CommandMethod("")> Public Sub cmd()

    End Sub

#End Region

#Region "Functions"
    Private Function IsSavedFile() As Boolean
        If Application.GetSystemVariable("DWGTITLED") <> 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function IsLayerExists(CurrentTransaction As Transaction, Layers As LayerTable, LayerName As String) As Boolean
        Dim lyrObj As LayerTableRecord

        For Each lyrId As ObjectId In Layers
            lyrObj = CurrentTransaction.GetObject(lyrId, OpenMode.ForRead)
            If lyrObj.Name = LayerName Then Return True
        Next

        Return False
    End Function
#End Region

#Region "Extension Routines"
    Public Sub Initialize() Implements IExtensionApplication.Initialize
        Application.MainWindow.Text = "MyApplication" 'Autodesk Name and version 
    End Sub

    Public Sub Terminate() Implements IExtensionApplication.Terminate

    End Sub
#End Region

End Class
