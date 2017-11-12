//Use Microsoft .NET Framework 4 and MultiCad.NET API 7.0
//Class for demonstrating the capabilities of MultiCad.NET
//Assembly for the Nanocad 8.1 is recommended (however, it is possible under 8.5 and may be 8.0)
//Link mapimgd.dll and mapibasetypes.dll from SDK
//The command: draws a face, asks for user input, displays a message box 
//pop-up hints, and calls another command.
//This code in the part of non-infringing rights Nanosoft can be used and distributed in any accessible ways.
//For the consequences of the code application, the developer is not responsible.

//More detailed - https://habrahabr.ru/post/342186/



using System;
using System.Collections.Generic;

using Multicad.AplicationServices;
using Multicad.Runtime;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.DatabaseServices.StandardObjects;

using System.Windows.Forms;


namespace nanoforhabr1
{
    public class startfor81
    {
        [CommandMethod("Dface", CommandFlags.NoCheck | CommandFlags.NoPrefix )]
        public void DrawFace()
        {

            //We draw half of face

            // Draw eye
            DbCircle eye = new DbCircle();
            eye.Radius = 100;
            eye.Center = new Point3d(200, 500,0);
            eye.DbEntity.AddToCurrentDocument();

            //Draw nose
            DbLine nose = new DbLine();
            nose.StartPoint= new Point3d(350, 400, 0);
            nose.EndPoint = new Point3d(350, 200, 0);
            nose.DbEntity.AddToCurrentDocument();

            //Draw mouth 
            DbPolyline mouth = new DbPolyline();
            List<Point3d> mouthPoints = new List<Point3d>() {
                new Point3d(100, 150, 0), new Point3d(200, 100, 0), new Point3d(350, 100, 0) };
            mouth.Polyline=  new Polyline3d(mouthPoints);
            mouth.Polyline.SetClosed(false);
            mouth.DbEntity.Transform(McDocumentsManager.GetActiveDoc().UCS); //change coordinates from UCS to WCS for BD
            mouth.DbEntity.AddToCurrentDocument();

            //draw mirror half the face (2nd half)

            DbCircle eye2 = new DbCircle();
            eye2.Radius = 100;
            eye2.Center = new Point3d(500, 500, 0);
            eye2.DbEntity.AddToCurrentDocument();

            DbPolyline mouth2 = new DbPolyline();
            mouth2.Polyline= mouth.Polyline.Mirror(new Plane3d(new Point3d(350, 100, 0), new Vector3d(200, 0, 0))) as Polyline3d;
            mouth2.DbEntity.AddToCurrentDocument();

            //Get notification in command line
            McContext.ShowNotification("You need to enter data into the console");


            //Get uaser input
            InputJig editorInput = new InputJig();
            string name = editorInput.GetText("Input your name and press Enter");

            //Drawing face's text
            DbText spech = new DbText();
            spech.Text = new TextGeom("Oh Master! Why I'm so ugly? Please remove me!", new Point3d(510, 15, 0), Vector3d.XAxis, "Standard", 15);
            spech.DbEntity.AddToCurrentDocument();


            //Get windows message box
            MessageBox.Show("Congratulation " + name +" you did it! But look, it want, to say something to you...");

            //Get popup help
            McContext.PopupNotification("Delete command has activated");

            //Activate another command (Delete)
            McContext.ExecuteCommand("Delete");


        }
    }
}
