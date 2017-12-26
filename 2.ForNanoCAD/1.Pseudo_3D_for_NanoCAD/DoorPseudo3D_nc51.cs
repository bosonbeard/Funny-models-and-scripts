// Version 1.1
//Use Microsoft .NET Framework 3.5 and old version of MultiCad.NET (for NC 5.1)
//Class for demonstrating the capabilities of MultiCad.NET
//Assembly for the Nanocad 5.1 
//Link mapimgd.dll from Nanocad SDK
//Link System.Windows.Forms and System.Drawing
//upd: for version 1.1 also link .NET API: hostdbmg.dll, hostmgd.dll

//The commands: draws a pseudo 3D door.
//This code in the part of non-infringing rights Nanosoft can be used and distributed in any accessible ways.
//For the consequences of the code application, the developer is not responsible.

// V 1.0. More detailed - https://habrahabr.ru/post/342680/
// V 1.1. More detailed - https://habrahabr.ru/post/343772/


// P.S. A big thanks to Alexander Vologodsky for help in developing a method for pivoting object.



using System;
using System.ComponentModel;
using System.Windows.Forms;
using Multicad.Runtime;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.CustomObjectBase;
using Multicad;

//added in V 1.1. for monitoring
using System.Security.Permissions;
using System.IO;
using Multicad.AplicationServices;
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;

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
            public enum Status {closed , middle, open};
            private Status _dStatus = Status.closed;

        //added in V 1.1. (monitor fileds)
            public enum Mon { off, on};
            private Mon _monitor = Mon.off;
            
            private string _monFilePath = @"E:\test.txt";

        // if it is serialized, you may not be able to copy the object in the CAD editor
            [NonSerialized]
            private FileSystemWatcher _watcher;
            [NonSerialized]
            private FileSystemEventHandler _watchHandler;



        [CommandMethod("DrawDoor", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public void DrawDoor() {
            DoorPseudo3D_nc51 door = new DoorPseudo3D_nc51();
            door.PlaceObject();
            this.TryModify();
           // this.Monitor = false;
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
            Stat = Status.closed;
            
            // Add the object to the database
            DbEntity.AddToCurrentDocument();
            // added in v.1.

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
            if (_dStatus == Status.closed) _vecDirectionClosed = _vecStraightDirection;
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
        public Status Stat
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
                    case Status.closed:
                        _vecStraightDirection = _vecDirectionClosed;
                    break;
                    case Status.middle:
                        _vecStraightDirection = _vecDirectionClosed.Add(_vecDirectionClosed.GetPerpendicularVector().Negate() * 0.575) ;
                    break;
                    case Status.open:
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
     





    //Define the monitoring custom properties , added v. 1.1:

    // added in v. 1.1
    [DisplayName("Monitoring")]
    [Description("Monitoring of file for door")]
    [Category("Monitoring")]
    public Mon Monitor
    {
        get
        {
            return _monitor;
        }
        set
        {
            //Save Undo state and set the object status to "Changed"
            if (!TryModify())
                return;


                _monitor = value;
                if (_monitor==Mon.on)
                {
                    StartMonitoring();
                }
                else StopMonitoring();


                //   if (_monitor)
                //   {
                //       StartMonitoring();
                //  }// Get the command line editor
                //   else StopMonitoring();

            }
    }

    // added in v. 1.1

    [DisplayName("File path for Monitoring")]
    [Description("Monitoring of file for door")]
    [Category("Monitoring")]
    public string MonitoringFilPath
    {
        get
        {
            return _monFilePath;
        }
        set
        {
                // Get the command line editor
                DocumentCollection dm = HostMgd.ApplicationServices.Application.DocumentManager;
                Editor ed = dm.MdiActiveDocument.Editor;
                
                //for hot change filename
                if (Monitor==Mon.on)
            {
                StopMonitoring();
                if (!TryModify())
                    return;

                    _monFilePath = value;
                    StartMonitoring();
                    ed.WriteMessage("Monitored file is changed");

            }
            else
            {
                //Save Undo state and set the object status to "Changed"
                if (!TryModify())
                    return;

                _monFilePath = value;
            }

        }
    }

    //Define the methods, added v. 1.1:
    // added in v. 1.1
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public void StartMonitoring()
    {
            DocumentCollection dm = HostMgd.ApplicationServices.Application.DocumentManager;
            Editor ed = dm.MdiActiveDocument.Editor;
            
            _watcher = new FileSystemWatcher();
            if (File.Exists(_monFilePath))
            {
                _watcher.Path = Path.GetDirectoryName(_monFilePath);
                _watcher.Filter = Path.GetFileName(_monFilePath);
                _watchHandler = new FileSystemEventHandler(OnChanged);
                _watcher.Changed += _watchHandler;
                _watcher.EnableRaisingEvents = true;
            }
            else  ed.WriteMessage("File: " + _monFilePath + " " + "not Exists");
    }

    // added in v. 1.1
    public void StopMonitoring()
    {
            if (_watcher != null & _watchHandler != null)
            {
                _watcher.Changed -= _watchHandler;
                _watcher.EnableRaisingEvents = false;
            }
        }

    // added in v. 1.1
    private void OnChanged(object source, FileSystemEventArgs e)
    {
            DocumentCollection dm = HostMgd.ApplicationServices.Application.DocumentManager;
            Editor ed = dm.MdiActiveDocument.Editor;
            ed.WriteMessage("File: " + e.FullPath + " " + e.ChangeType);

             //read new value from file
        try
        {

            if (File.Exists(_monFilePath))
            {
                int mStatus = -1;
                 ed.WriteMessage("File exists ");

                using (StreamReader sr = new StreamReader(new FileStream(_monFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    if (sr.BaseStream.CanRead)
                    {
                        if (int.TryParse(sr.ReadLine(), out mStatus))
                        {
                            if (Enum.IsDefined(typeof(Status), mStatus))
                            {


                                if (!TryModify()) return;
                                Stat = (Status)mStatus; // преобразование
                                if (!TryModify()) return;
                                DbEntity.Update();
                                McContext.ExecuteCommand("REGENALL");
                                 ed.WriteMessage("Door state is changed");

                            }
                            else  ed.WriteMessage("Incorrect data in the file. Should be in diapason: 0, 1, 2 ");
                        }

                    }
                    else  ed.WriteMessage("Can't read file ");
                }

            }
            else  ed.WriteMessage("File not exists  ");



            _watcher.EnableRaisingEvents = false; // disable tracking
        }
        finally
        {
            _watcher.EnableRaisingEvents = true; // reconnect tracking
        }

    }

    }


    // TODO: There are many shortcomings in this code. 
    // Including failures when working with copying, moving objects and saving files, you can improve it if you want.

}


