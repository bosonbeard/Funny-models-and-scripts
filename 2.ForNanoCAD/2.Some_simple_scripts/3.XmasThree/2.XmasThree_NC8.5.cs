//Use Microsoft .NET Framework 4 and old version of MultiCad.NET (for NC 8.X)
//Class for demonstrating the capabilities of MultiCad.NET
//Assembly for the Nanocad 8.X 
//Link mapimgd and mapibasetypes from Nanocad SDK
//Link System.Windows.Forms and System.Drawing
//The commands: draws a christmas three.
//This code in the part of non-infringing rights Nanosoft can be used and distributed in any accessible ways.
//For the consequences of the code application, the developer is not responsible.


using System.Collections.Generic;
using System.Linq;
using Multicad.Runtime;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.DatabaseServices.StandardObjects;
using System.Drawing;
using System.Windows.Forms;

namespace XmasThree
{
    class XmasThree
    {
        [CommandMethod("DXThree", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public void DrawXThree()
        {

            Point3d _pntBase;
            Point3d _bufPnt;

            //prompts for installation point entry
            InputJig jig = new InputJig();

            // Get the first box point from the jig
            InputResult res = jig.GetPoint("Select first point:");

            //It works only if input was successful
            if (res.Result == InputResult.ResultCode.Normal)
            {

                // The base point is taken from the entry point (click with mouse)
                _pntBase = res.Point;

                //Draw the outline of the left half of the Christmas tree
                //Create base points for the polyline
                List<Point3d> leftPatrOfThreePoints = new List<Point3d>()
                {
                    new Point3d(_pntBase.X, _pntBase.Y, 0),
                    new Point3d(_pntBase.X-125, _pntBase.Y-154, 0),
                    new Point3d(_pntBase.X-31, _pntBase.Y-137, 0),
                    new Point3d(_pntBase.X-181, _pntBase.Y-287, 0),
                    new Point3d(_pntBase.X-31, _pntBase.Y-253, 0),
                    new Point3d(_pntBase.X-242, _pntBase.Y-400, 0),
                    new Point3d(_pntBase.X-37, _pntBase.Y-400, 0),
                    new Point3d(_pntBase.X-37, _pntBase.Y-454, 0),
                    new Point3d(_pntBase.X, _pntBase.Y-454, 0)
                };

                //Create a polyline (geometry)
                Polyline3d leftPatrOfThree = new Polyline3d(leftPatrOfThreePoints);

                //Create a polyline object and place it on the drawing
                DbPolyline XThreeLeft = new DbPolyline();
                XThreeLeft.Polyline = new Polyline3d(leftPatrOfThree);
                XThreeLeft.Polyline.SetClosed(false);
                XThreeLeft.DbEntity.Color = Color.Green;
                XThreeLeft.DbEntity.AddToCurrentDocument();


                //The right part of the tree is obtained by mirroring the left
                DbPolyline XThreeRight = new DbPolyline();
                XThreeRight.DbEntity.Color = Color.Green;
                XThreeRight.Polyline = (Polyline3d)XThreeLeft.Polyline.Mirror(new Plane3d(_pntBase, new Vector3d(10, 0, 0)));
                XThreeRight.DbEntity.AddToCurrentDocument();

                //From the right and left sides we make a single contour for hatching

                DbPolyline XThreeR = new DbPolyline();
                XThreeR.DbEntity.Color = Color.Green;
                XThreeR.Polyline = XThreeRight.Polyline.Clone() as Polyline3d; // for NC 8.5
                //XThreeR.Polyline = XThreeRight.Polyline.GetCopy() as Polyline3d; //FOR NC 5.1
                XThreeR.DbEntity.AddToCurrentDocument();

                List<Point3d> hatchPoints = new List<Point3d>();
                hatchPoints.AddRange(leftPatrOfThreePoints);
                hatchPoints.AddRange(XThreeR.Polyline.Points.Reverse().ToList());

                Polyline3d hatchContur = new Polyline3d(hatchPoints);

                //We will create on the basis of a contour a hatch (geometry) with continuous filling 
                Hatch hatch = new Hatch(hatchContur, 0, 10, true);
                hatch.PattType = PatternType.PreDefined;
                hatch.PatternName = "SOLID";

                //Based on the geometry of the hatch, we create the document object, set its color properties - green
                DbGeometry dbhatch = new DbGeometry();
                dbhatch.Geometry = new EntityGeometry(hatch);
                dbhatch.DbEntity.Color = Color.Green;
                dbhatch.DbEntity.AddToCurrentDocument();

                // if you want you can try to draw balls with circles use
                // DrawThreeBalls(_pntBase);

                //Similarly, make a Christmas tree toy (octagon)
                //red
                _bufPnt = _pntBase.Subtract(new Vector3d(30, 95, 0));
                DbPolyline dbOctoRed = DrawThreeOctogonPl(_bufPnt);//implicit
                dbOctoRed.DbEntity.AddToCurrentDocument();
                Hatch hatchCirkRed = new Hatch(dbOctoRed.Polyline, 0, 1, false);
                hatchCirkRed.PattType = PatternType.PreDefined;
                hatchCirkRed.PatternName = "SOLID";
                DbGeometry dbhatchCirkRed = new DbGeometry();
                dbhatchCirkRed.Geometry = new EntityGeometry(hatchCirkRed);
                dbhatchCirkRed.DbEntity.Color = Color.Red;
                dbhatchCirkRed.DbEntity.AddToCurrentDocument();
                //green
                _bufPnt = _pntBase.Subtract(new Vector3d(-40, 200, 0));
                DbPolyline dbOctoGreen = DrawThreeOctogonPl(_bufPnt);//implicit
                dbOctoGreen.DbEntity.AddToCurrentDocument();
                Hatch hatchCirkGreen = new Hatch(dbOctoGreen.Polyline, 0, 1, false);
                hatchCirkGreen.PattType = PatternType.PreDefined;
                hatchCirkGreen.PatternName = "SOLID";
                DbGeometry dbhatchCirkGreen = new DbGeometry();
                dbhatchCirkGreen.Geometry = new EntityGeometry(hatchCirkGreen);
                dbhatchCirkGreen.DbEntity.Color = Color.LightSeaGreen;
                dbhatchCirkGreen.DbEntity.AddToCurrentDocument();
                //blue
                _bufPnt = _pntBase.Subtract(new Vector3d(-12, 350, 0));
                DbPolyline dbOctoBlue = DrawThreeOctogonPl(_bufPnt);//implicit
                dbOctoBlue.DbEntity.AddToCurrentDocument();
                Hatch hatchCirkBlue = new Hatch(dbOctoBlue.Polyline, 0, 1, false);
                hatchCirkBlue.PattType = PatternType.PreDefined;
                hatchCirkBlue.PatternName = "SOLID";
                DbGeometry dbhatchCirkBlue = new DbGeometry();
                dbhatchCirkBlue.Geometry = new EntityGeometry(hatchCirkBlue);
                dbhatchCirkBlue.DbEntity.Color = Color.Blue;
                dbhatchCirkBlue.DbEntity.AddToCurrentDocument();

                //display the text with congratulations
                MessageBox.Show("I Wish You A Merry Christmas And Happy New Year!");

            }

        }


        public Polyline3d DrawThreeOctogonPl(Point3d _pntB)
        {
            //Create points for an octagon
            List<Point3d> octoPoints = new List<Point3d>()
                {
                    new Point3d(_pntB.X, _pntB.Y, 0),
                    new Point3d(_pntB.X-15, _pntB.Y, 0),
                    new Point3d(_pntB.X-25, _pntB.Y-11.3, 0),
                    new Point3d(_pntB.X-25, _pntB.Y-26.3, 0),
                    new Point3d(_pntB.X-15, _pntB.Y-37.6, 0),
                    new Point3d(_pntB.X, _pntB.Y-37.6, 0),
                    new Point3d(_pntB.X+9.7, _pntB.Y-26.3, 0),
                    new Point3d(_pntB.X+9.7, _pntB.Y-11.3, 0),
                    new Point3d(_pntB.X, _pntB.Y, 0)
                };

            return new Polyline3d(octoPoints);
        }


    }
}