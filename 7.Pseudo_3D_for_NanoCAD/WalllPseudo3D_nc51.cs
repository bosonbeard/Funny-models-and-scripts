//Use Microsoft .NET Framework 3.5 and old version of MultiCad.NET (for NC 5.1)
//Class for demonstrating the capabilities of MultiCad.NET
//Assembly for the Nanocad 5.1 
//Link mapimgd from Nanocad SDK
//Link System.Windows.Forms and System.Drawing
//The commands: draws a pseudo 3D wall.
//This code in the part of non-infringing rights Nanosoft can be used and distributed in any accessible ways.
//For the consequences of the code application, the developer is not responsible.

//More detailed - https://habrahabr.ru/post/342680/



using System;
using System.ComponentModel;
using Multicad.Runtime;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.CustomObjectBase;
using Multicad;


namespace nanowall2
{
    //change "b4edac1f-7978-483f-91b1-10503d20735a" for your Guid from AssemblyInfo.cs for your Project
    [CustomEntity(typeof(WalllPseudo3D_nc51), "b4edac1f-7978-483f-91b1-10503d20735a", "WalllPseudo3D_nc51", "WalllPseudo3D_nc51 Sample Entity")]
        [Serializable]
        public class WalllPseudo3D_nc51 : McCustomBase
        {
            // First and second vertices of the box
            private Point3d _pnt1 = new Point3d(100, 100, 0);
            private Point3d _pnt2 = new Point3d(500, 100, 0);
            private double _h = 2485;
            public enum status { closed , middle, open   };
            public  status _dStatus;
            private double _scale = 1000;

        [CommandMethod("DrawWall", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public void DrawWall() {



            
            WalllPseudo3D_nc51 wall = new WalllPseudo3D_nc51();
            wall.PlaceObject();
      
        }

        public override void OnDraw(GeometryBuilder dc)
        {

            dc.Clear();
            // Define the basic points for drawing
            Point3d pnt1 = _pnt1;
            Point3d pnt2 = new Point3d(_pnt2.X + (984 * _scale), pnt1.Y, 0);
            Point3d pnt3 = new Point3d(pnt2.X + 0, pnt1.Y+(150 * _scale), 0);
            Point3d pnt4 = new Point3d(pnt1.X , pnt3.Y, 0);
            // Set the color to ByObject value
            dc.Color = McDbEntity.ByObject;
            Vector3d hvec = new Vector3d(0, 0, _h * _scale);

            // Draw the upper and lower sidestes

            dc.DrawPolyline(new Point3d[] { pnt1, pnt2, pnt3, pnt4, pnt1 });
            dc.DrawPolyline(new Point3d[] { _pnt1.Add(hvec),
            pnt2.Add(hvec), pnt3.Add(hvec), pnt4.Add(hvec), pnt1.Add(hvec)});

            // Draw the edges
            dc.DrawLine(pnt1, pnt1.Add(hvec));
            dc.DrawLine(pnt2, pnt2.Add(hvec));
            dc.DrawLine(pnt3, pnt3.Add(hvec));
            dc.DrawLine(pnt4, pnt4.Add(hvec));


        }

        //Define the custom properties of the object
        [DisplayName("WScale")]
        [Description("Wall Scale")]
        [Category("Wall options")]
        public double WScale
        {
            get
            {
                return _scale;
            }
            set
            {
                if (!TryModify())
                    return;
                _scale = value;
            }
        }



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
            info.AppendGrip(new McSmartGrip<WalllPseudo3D_nc51>(_pnt1, (obj, g, offset) => { obj.TryModify(); obj._pnt1 += offset; }));
            return true;
        }


    }

    // TODO: There are many shortcomings in this code. 
    // Including failures when working with copying and moving objects, you can improve it if you want.


}


