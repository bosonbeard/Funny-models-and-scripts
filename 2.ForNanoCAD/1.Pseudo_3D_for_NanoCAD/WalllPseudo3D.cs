// Version 1.0
//Use Microsoft .NET Framework 4 and MultiCad.NET API 7.0
//Class for demonstrating the capabilities of MultiCad.NET
//Assembly for the Nanocad 8.5 SDK is recommended (however, it is may be possible in the all 8.Х family)
//Link imapimgd, mapimgd.dll and mapibasetypes.dll from SDK
//Link System.Windows.Forms and System.Drawing
//The commands: draws a pseudo 3D wall.
//This code in the part of non-infringing rights Nanosoft can be used and distributed in any accessible ways.
//For the consequences of the code application, the developer is not responsible.

//More detailed - https://habrahabr.ru/post/342680/



using System;
using System.Collections.Generic;
using System.ComponentModel;
using Multicad.Runtime;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.CustomObjectBase;
using Multicad;

namespace nanowall2
{
    //change "8b0986c0-4163-42a4-b005-187111b499d7" for your Guid from Assembly.
    // Be careful GUID for door and wall classes must be different! 
    // Otherwise there will be problems with saving and moving
    [CustomEntity(typeof(WalllPseudo3D), "8b0986c0-4163-42a4-b005-187111b499d7", "WalllPseudo3D", "WalllPseudo3D Sample Entity")]
        [Serializable]
        public class WalllPseudo3D : McCustomBase
        {
            // First and second vertices of the box
            private Point3d _pnt1 = new Point3d(100, 100, 0);
            private Point3d _pnt2 = new Point3d(500, 100, 0);
            private double _h = 2085;




        [CommandMethod("DrawWall", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public void DrawWall() {

            WalllPseudo3D wall = new WalllPseudo3D();
            wall.PlaceObject();
      
        }

        public override void OnDraw(GeometryBuilder dc)
        {
            dc.Clear();
            
            // Define the basic points for drawing
            Point3d pnt1 = _pnt1;
            Point3d pnt2 = new Point3d(_pnt2.X, pnt1.Y, 0);
            Point3d pnt3 = new Point3d(pnt2.X, pnt1.Y+150, 0);
            Point3d pnt4 = new Point3d(pnt1.X , pnt3.Y, 0);
            // Set the color to ByObject value
            dc.Color = McDbEntity.ByObject;
            Vector3d hvec = new Vector3d(0, 0, _h);

            // Draw the upper and lower sides
            dc.DrawPolyline(new Point3d[] { pnt1, pnt2, pnt3, pnt4, _pnt1 });
            dc.DrawPolyline(new Point3d[] { _pnt1.Add(hvec),
            pnt2.Add(hvec), pnt3.Add(hvec), pnt4.Add(hvec), pnt1.Add(hvec)});

            // Draw the edges
            dc.DrawLine(pnt1, pnt1.Add(hvec));
            dc.DrawLine(pnt2, pnt2.Add(hvec));
            dc.DrawLine(pnt3, pnt3.Add(hvec));
            dc.DrawLine(pnt4, pnt4.Add(hvec));

            // Create contours for the front and rear sides and hatch them
            // In this demo, we hatch only two sides, you can tailor the others yourself
            List<Polyline3d> c1 = new List<Polyline3d>();
            c1.Add(new Polyline3d(
               new List<Point3d>() { pnt1, pnt1.Add(hvec), pnt2.Add(hvec), pnt2, pnt1, }));         
            dc.DrawGeometry(new Hatch(c1, "BRICK", 0, 20, false, HatchStyle.Normal, PatternType.PreDefined, 30), 1);

            List<Polyline3d> c2 = new List<Polyline3d>();
            c2.Add(new Polyline3d(
              new List<Point3d>() { pnt4, pnt4.Add(hvec), pnt3.Add(hvec), pnt3, pnt4, }));
            dc.DrawGeometry(new Hatch(c2, "BRICK", 0, 20, false, HatchStyle.Normal, PatternType.PreDefined, 30), 1);

        }

        //Define the custom properties of the object
        [DisplayName("Height")]
        [Description("Height of wall")]
        [Category("Wall options")]
        public double HWall
        {
            get
            {
                return _h;
            }
            set
            {
                //Save Undo state and set the object status to "Changed"
                if (!TryModify())
                    return;

                _h = value;

            }
        }

        public override hresult PlaceObject(PlaceFlags lInsertType)
        {
            InputJig jig = new InputJig();

            // Get the first box point from the jig
            InputResult res = jig.GetPoint("Select first point:");
            if (res.Result != InputResult.ResultCode.Normal)
                return hresult.e_Fail;
            _pnt1 = res.Point;

            // Add the object to the database
            this.DbEntity.AddToCurrentDocument();
            
            //Exclude the object from snap points
            jig.ExcludeObject(ID);

            // Monitoring mouse moving and interactive entity redrawing 
            jig.MouseMove = (s, a) => { TryModify(); _pnt2 = a.Point; this.DbEntity.Update(); };

            // Get the second box point from the jig
            res = jig.GetPoint("Select second point:");
            if (res.Result != InputResult.ResultCode.Normal)
            {
                this.DbEntity.Erase();
                return hresult.e_Fail;
            }
            _pnt2 = res.Point;
            
            return hresult.s_Ok;
        }

        // Create a grip for the base point of the object
        public override bool GetGripPoints(GripPointsInfo info)
        {
            info.AppendGrip(new McSmartGrip<WalllPseudo3D>(_pnt1, (obj, g, offset) => { obj.TryModify(); obj._pnt1 += offset; }));
            info.AppendGrip(new McSmartGrip<WalllPseudo3D>(_pnt2, (obj, g, offset) => { obj.TryModify(); obj._pnt2 += offset; }));
            return true;
        }


    }

    // TODO: There are many shortcomings in this code. 
    // Including failures when working with copying, moving objects and saving files, you can improve it if you want.


}


