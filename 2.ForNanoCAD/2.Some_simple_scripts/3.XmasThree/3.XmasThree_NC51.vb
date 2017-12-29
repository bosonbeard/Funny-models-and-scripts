Imports System.Collections.Generic
Imports Multicad.Runtime
Imports Multicad.DatabaseServices
Imports Multicad.Geometry
Imports Multicad.DatabaseServices.StandardObjects
Imports System.Drawing
Imports System.Windows.Forms
Namespace XmasThree
    Class XmasThree
        <CommandMethod("DXThree", CommandFlags.NoCheck Or CommandFlags.NoPrefix)>
        Public Sub DrawXThree()
            Dim _pntBase As Point3d
            Dim _bufPnt As Point3d
            Dim jig As New InputJig()
            'prompts for installation point entry
            Dim res As InputResult = jig.GetPoint("Select first point:")
            If res.Result = InputResult.ResultCode.Normal Then
                'The base point is taken from the entry point (click with mouse)
                _pntBase = res.Point

                'Draw the outline of the left half of the Christmas tree
                'Create base points for the polyline
                Dim leftPatrOfThreePoints As New List(Of Point3d)() From {
                    New Point3d(_pntBase.X, _pntBase.Y, 0),
                    New Point3d(_pntBase.X - 125, _pntBase.Y - 154, 0),
                    New Point3d(_pntBase.X - 31, _pntBase.Y - 137, 0),
                    New Point3d(_pntBase.X - 181, _pntBase.Y - 287, 0),
                    New Point3d(_pntBase.X - 31, _pntBase.Y - 253, 0),
                    New Point3d(_pntBase.X - 242, _pntBase.Y - 400, 0),
                    New Point3d(_pntBase.X - 37, _pntBase.Y - 400, 0),
                    New Point3d(_pntBase.X - 37, _pntBase.Y - 454, 0),
                    New Point3d(_pntBase.X, _pntBase.Y - 454, 0)
                }

                'Create a polyline (geometry)
                Dim leftPatrOfThree As New Polyline3d(leftPatrOfThreePoints)

                'Create a polyline object and place it on the drawing
                Dim XThreeLeft As New DbPolyline()
                XThreeLeft.Polyline = New Polyline3d(leftPatrOfThree)
                XThreeLeft.Polyline.SetClosed(False)
                XThreeLeft.DbEntity.Color = Color.Green
                XThreeLeft.DbEntity.AddToCurrentDocument()

                Dim XThreeRight As New DbPolyline()
                XThreeRight.DbEntity.Color = Color.Green
                XThreeRight.Polyline = DirectCast(XThreeLeft.Polyline.Mirror(New Plane3d(_pntBase, New Vector3d(10, 0, 0))), Polyline3d)
                XThreeRight.DbEntity.AddToCurrentDocument()

                'From the right and left sides we make a single contour for hatching

                Dim XThreeR As New DbPolyline()
                XThreeR.DbEntity.Color = Color.Green
                XThreeR.Polyline = TryCast(XThreeRight.Polyline.GetCopy(), Polyline3d)
                XThreeR.DbEntity.AddToCurrentDocument()

                Dim hatchPoints As New List(Of Point3d)()
                hatchPoints.AddRange(leftPatrOfThreePoints)
                hatchPoints.AddRange(XThreeR.Polyline.Points.Reverse().ToList())

                Dim hatchContur As New Polyline3d(hatchPoints)

                'We will create on the basis of a contour a hatch (geometry) with continuous filling 
                Dim hatch As New Hatch(hatchContur, 0, 10, True)
                hatch.PattType = PatternType.PreDefined
                hatch.PatternName = "SOLID"

                'Based on the geometry of the hatch, we create the document object, set its color properties - green
                Dim dbhatch As New DbGeometry()
                dbhatch.Geometry = New EntityGeometry(hatch)
                dbhatch.DbEntity.Color = Color.Green
                dbhatch.DbEntity.AddToCurrentDocument()

                'Similarly, make a Christmas tree toy (octagon)
                'red
                _bufPnt = _pntBase.Subtract(New Vector3d(30, 95, 0))
                Dim dbOctoRed As DbPolyline = DrawThreeOctogonPl(_bufPnt)
                dbOctoRed.DbEntity.AddToCurrentDocument()
                Dim hatchCirkRed As New Hatch(dbOctoRed.Polyline, 0, 1, False)
                hatchCirkRed.PattType = PatternType.PreDefined
                hatchCirkRed.PatternName = "SOLID"
                Dim dbhatchCirkRed As New DbGeometry()
                dbhatchCirkRed.Geometry = New EntityGeometry(hatchCirkRed)
                dbhatchCirkRed.DbEntity.Color = Color.Red
                dbhatchCirkRed.DbEntity.AddToCurrentDocument()

                'green
                _bufPnt = _pntBase.Subtract(New Vector3d(-40, 200, 0))
                Dim dbOctoGreen As DbPolyline = DrawThreeOctogonPl(_bufPnt)
                dbOctoGreen.DbEntity.AddToCurrentDocument()
                Dim hatchCirkGreen As New Hatch(dbOctoGreen.Polyline, 0, 1, False)
                hatchCirkGreen.PattType = PatternType.PreDefined
                hatchCirkGreen.PatternName = "SOLID"
                Dim dbhatchCirkGreen As New DbGeometry()
                dbhatchCirkGreen.Geometry = New EntityGeometry(hatchCirkGreen)
                dbhatchCirkGreen.DbEntity.Color = Color.LightSeaGreen
                dbhatchCirkGreen.DbEntity.AddToCurrentDocument()

                'blue
                _bufPnt = _pntBase.Subtract(New Vector3d(-12, 350, 0))
                Dim dbOctoBlue As DbPolyline = DrawThreeOctogonPl(_bufPnt)
                dbOctoBlue.DbEntity.AddToCurrentDocument()
                Dim hatchCirkBlue As New Hatch(dbOctoBlue.Polyline, 0, 1, False)
                hatchCirkBlue.PattType = PatternType.PreDefined
                hatchCirkBlue.PatternName = "SOLID"
                Dim dbhatchCirkBlue As New DbGeometry()
                dbhatchCirkBlue.Geometry = New EntityGeometry(hatchCirkBlue)
                dbhatchCirkBlue.DbEntity.Color = Color.Blue
                dbhatchCirkBlue.DbEntity.AddToCurrentDocument()

                MessageBox.Show("I Wish You A Merry Christmas And Happy New Year!!!")
            End If
        End Sub

        Public Function DrawThreeOctogonPl(_pntB As Point3d) As Polyline3d
            'Create points for an octagon
            Dim octoPoints As New List(Of Point3d)() From {
                    New Point3d(_pntB.X, _pntB.Y, 0),
                    New Point3d(_pntB.X - 15, _pntB.Y, 0),
                    New Point3d(_pntB.X - 25, _pntB.Y - 11.3, 0),
                    New Point3d(_pntB.X - 25, _pntB.Y - 26.3, 0),
                    New Point3d(_pntB.X - 15, _pntB.Y - 37.6, 0),
                    New Point3d(_pntB.X, _pntB.Y - 37.6, 0),
                    New Point3d(_pntB.X + 9.7, _pntB.Y - 26.3, 0),
                    New Point3d(_pntB.X + 9.7, _pntB.Y - 11.3, 0),
                    New Point3d(_pntB.X, _pntB.Y, 0)
        }
            Return New Polyline3d(octoPoints)
        End Function



    End Class
End Namespace
