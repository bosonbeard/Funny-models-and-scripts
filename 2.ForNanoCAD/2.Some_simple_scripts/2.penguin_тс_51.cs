using System.Collections.Generic;
using Multicad.Runtime;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.DatabaseServices.StandardObjects;

namespace nanodoor2_51
{
    class penguin
    {

        [CommandMethod("DPing", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public void DrawFace()
        {
            

            DbCircle body = new DbCircle();
            body.Center = new Point3d(200, 200, 0);
            body.Radius = 105;
            body.DbEntity.AddToCurrentDocument();
            DbCircle eye1 = new DbCircle();
            eye1.Center = new Point3d(150, 255, 0);
            eye1.Radius = 8;
            eye1.DbEntity.AddToCurrentDocument();

            DbCircle pupil1 = new DbCircle();
            pupil1.Center = new Point3d(148, 254, 0);
            pupil1.Radius = 2;
            pupil1.DbEntity.AddToCurrentDocument();

            DbCircle eye2 = new DbCircle();
            eye2.Center = new Point3d(250, 260, 0);
            eye2.Radius = 8;
            eye2.DbEntity.AddToCurrentDocument();
            DbCircle pupil2 = new DbCircle();
            pupil2.Center = new Point3d(252, 258, 0);
            pupil2.Radius = 2;
            pupil2.DbEntity.AddToCurrentDocument();

            DbLine hand1 = new DbLine();
            hand1.StartPoint = new Point3d(102, 239, 0);
            hand1.EndPoint = new Point3d(72, 183, 0);
            hand1.DbEntity.AddToCurrentDocument();

            DbLine hand2 = new DbLine();
            hand2.StartPoint = new Point3d(298, 236, 0);
            hand2.EndPoint = new Point3d(325, 192, 0);
            hand2.DbEntity.AddToCurrentDocument();

            DbPolyline nose = new DbPolyline();
            List<Point3d> nosePoints = new List<Point3d>() {
                        new Point3d(171, 222, 0), new Point3d(198, 177, 0), new Point3d(231, 222, 0) };
            nose.Polyline = new Polyline3d(nosePoints);
            nose.Polyline.SetClosed(false);
            nose.DbEntity.Transform(McDocumentsManager.GetActiveDoc().UCS); //change coordinates from UCS to WCS for BD
            nose.DbEntity.AddToCurrentDocument();

            DbText spech = new DbText();
            spech.Text = new TextGeom("Hello Habr!", new Point3d(310, 55, 0), Vector3d.XAxis, "Standard", 25);
            spech.DbEntity.AddToCurrentDocument();


        }
    }
}
