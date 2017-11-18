//Use Microsoft .NET Framework 3.5 and old version of MultiCad.NET (for NC 5.1)
//Class for demonstrating the capabilities of MultiCad.NET
//Assembly for the Nanocad 5.1 
//Link mapimgd from Nanocad SDK
//Link System.Windows.Forms and System.Drawing
//The commands: draws a pseudo 3D door.
//This code in the part of non-infringing rights Nanosoft can be used and distributed in any accessible ways.
//For the consequences of the code application, the developer is not responsible.

//More detailed - https://habrahabr.ru/post/342680/

// P.S. A big thanks to Alexander Vologodsky for help in developing a method for pivoting object.



using System;
using System.ComponentModel;
using System.Windows.Forms;
using Multicad.Runtime;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.CustomObjectBase;
using Multicad;


namespace nanodoor2
{
    //change "8b0986c0-4163-42a4-b005-187111b499d7" for your Guid from Assembly.
    // Be careful GUID for door and wall classes must be different! 
    // Otherwise there will be problems with saving and moving

    [CustomEntity(typeof(DoorPseudo3D_nc51), "b4edac1f-7978-483f-91b1-10503d20735b", "DoorPseudo3D_nc51", "DoorPseudo3D_nc51 Sample Entity")]
        [Serializable]
        public class DoorPseudo3D_nc51 : McCustomBase
        {
            // First and second vertices of the box
            private Point3d _pnt1 = new Point3d(0, 0, 0);
            private double _scale = 1000;
            private double _h = 2085;
            private Vector3d _vecStraightDirection = new Vector3d(1, 0, 0);
            private Vector3d _vecDirectionClosed =  new Vector3d(1, 0, 0);
            public enum status { closed , middle, open   };
            private status _dStatus = status.closed;





        [CommandMethod("DrawDoor", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public void DrawDoor() {
            DoorPseudo3D_nc51 door = new DoorPseudo3D_nc51();
            door.PlaceObject();

        }

        public override void OnDraw(GeometryBuilder dc)
        {
            dc.Clear();

            // Define the basic points for drawing
            Point3d pnt1 = new Point3d(0, 0, 0);
            Point3d pnt2 = new Point3d(pnt1.X + (984 * _scale), pnt1.Y, 0);
            Point3d pnt3 = new Point3d(pnt2.X + 0, pnt1.Y+(50 * _scale), 0);
            Point3d pnt4 = new Point3d(pnt1.X , pnt3.Y, 0) ;
            // Set the color to ByObject value
            dc.Color = McDbEntity.ByObject;
            Vector3d hvec = new Vector3d(0, 0, _h * _scale) ;

            // Draw the upper and lower sides
            dc.DrawPolyline(new Point3d[] { pnt1, pnt2, pnt3, pnt4, pnt1 });
            dc.DrawPolyline(new Point3d[] { pnt1.Add(hvec),
            pnt2.Add(hvec), pnt3.Add(hvec), pnt4.Add(hvec), pnt1.Add(hvec)});

            // Draw the edges
            dc.DrawLine(pnt1, pnt1.Add(hvec));
            dc.DrawLine(pnt2, pnt2.Add(hvec));
            dc.DrawLine(pnt3, pnt3.Add(hvec));
            dc.DrawLine(pnt4, pnt4.Add(hvec));

            // Drawing a Door Handle
            dc.DrawLine(pnt2.Add(new Vector3d( -190 * _scale, -0, _h*0.45 * _scale)), 
                pnt2.Add(new Vector3d(-100 * _scale, 0, _h * 0.45 * _scale)));

            dc.DrawLine(pnt3.Add(new Vector3d(-190 * _scale, 0, _h * 0.45 * _scale)),
                pnt3.Add(new Vector3d(-100 * _scale, 0, _h * 0.45 * _scale)));


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
            DbEntity.AddToCurrentDocument();

            
            return hresult.s_Ok;
        }

        /// <summary>
        /// Method for changing the object's SC (the graph is built at the origin of coordinates).
        /// </ summary>
        /// <param name = "tfm"> The matrix for changing the position of the object. </ param>
        /// <returns> True - if the matrix is passed, False - if not. </ returns>
        public override bool GetECS(out Matrix3d tfm)
          {
            // Create a matrix that transforms the object.
            // The object is drawn in coordinates(0.0), then it is transformed with the help of this matrix.
            tfm = Matrix3d.Displacement(this._pnt1.GetAsVector()) * Matrix3d.Rotation
                (-this._vecStraightDirection.GetAngleTo(Vector3d.XAxis, Vector3d.ZAxis), Vector3d.ZAxis, Point3d.Origin);
              return true;
             
          }
          

         public override void OnTransform(Matrix3d tfm)
        {
            // To be able to cancel(Undo)
            McUndoPoint undo = new McUndoPoint();
            undo.Start();
            // Get the coordinates of the base point and the rotation vector
            this.TryModify();
            this._pnt1 = this._pnt1.TransformBy(tfm);
            this.TryModify();
            this._vecStraightDirection = this._vecStraightDirection.TransformBy(tfm);
            // We move the door only when it is closed if not - undo
            if (_dStatus == status.closed) _vecDirectionClosed = _vecStraightDirection;
            else
            {
                MessageBox.Show("Please transform only closed door (when its status = 0)");
                undo.Undo();
            }

            

            undo.Stop();
        }

        //Define the custom properties of the object
        [DisplayName("Height")]
        [Description("Height of door")]
        [Category("Door options")]
        public double HDoor
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



        [DisplayName("DScale")]
        [Description("Door Scale")]
        [Category("Door options")]
        public double DScale
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


        [DisplayName("Door status")]
        [Description("0-closed, 1-midle, 2-open")]
        [Category("Door options")]
        public status Stat
        {
            get
            {
                return _dStatus;
            }
            set
            {
                //Save Undo state and set the object status to "Changed"
                if (!TryModify())
                    return;

                // Change the rotation vector for each of the door states
                switch (value)
                    {
                    case status.closed:
                        _vecStraightDirection = _vecDirectionClosed;
                    break;
                    case status.middle:
                        _vecStraightDirection = _vecDirectionClosed.Add(_vecDirectionClosed.GetPerpendicularVector().Negate() * 0.575) ;
                    break;
                    case status.open:
                        _vecStraightDirection = _vecDirectionClosed.GetPerpendicularVector()*-1;
                        break;

                    default:
                        
                    break;
                }

                _dStatus = value;

            }
        }


        // Create a grip for the base point of the object
        public override bool GetGripPoints(GripPointsInfo info)
        {
            info.AppendGrip(new McSmartGrip<DoorPseudo3D_nc51>(_pnt1, (obj, g, offset) => { obj.TryModify(); obj._pnt1 += offset;  }));
            return true;
        }
     

    }


    // TODO: There are many shortcomings in this code. 
    // Including failures when working with copying, moving objects and saving files, you can improve it if you want.

}


