// Version 0.1
//Use Microsoft .NET Framework 4 and MultiCad.NET API 7.0
//Class for demonstrating the capabilities of MultiCad.NET
//Assembly for the Nanocad 8.5 SDK is recommended (however, it is may be possible in the all 8.Х family)
//Link imapimgd, mapimgd.dll and mapibasetypes.dll from SDK
//Link System.Windows.Forms and System.Drawing
//The commands: draws a fortune-teller ball 
//This code in the part of non-infringing rights Nanosoft can be used and distributed in any accessible ways.
//For the consequences of the code application, the developer is not responsible.

//More detailed - https://habrahabr.ru/post/347720/


using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using Multicad.Runtime;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.CustomObjectBase;
using Multicad;
using Multicad.AplicationServices;

namespace Fortuneteller
{
    [CustomEntity(typeof(Ball), "2e814ea6-f1f0-469d-9767-269fedb32226", "Ball", "Fortuneteller Ball for NC85 Entity")]
    [Serializable]
    public class Ball : McCustomBase
    {

        private Point3d _basePnt = new Point3d(0, 0, 0);
        double _radius=300;
        string _predText = "...";

        public List<String> predictions =
             new List<String>()
             {"Act now!",
              "Do not do this!",
              "Maybe",
              "I dont know",
              "Everything is unclear",
              "Yes!",
              "No!",
              "Take rest"
             };



        public override void OnDraw(GeometryBuilder dc)
        {
            dc.Clear();
            dc.Color = McDbEntity.ByObject;
            dc.DrawCircle(_basePnt, _radius);
            dc.DrawCircle(_basePnt, _radius/2.0);
            dc.TextHeight = 31;
            dc.DrawMText(_basePnt, Vector3d.XAxis, _predText, HorizTextAlign.Center, VertTextAlign.Center, _radius / 2.05);
         
        }


        public override void OnTransform(Matrix3d tfm)
        {
            // To be able to cancel(Undo)
            McUndoPoint undo = new McUndoPoint();
            undo.Start();

            // Get the coordinates of the base point and the rotation vector
            this.TryModify();
            this._basePnt = this._basePnt.TransformBy(tfm);
            undo.Stop();
        }

        public override hresult OnEdit(Point3d pnt, EditFlags lInsertType)
        {
            CallForm();
            return hresult.s_Ok;
        }


        private void CallForm()
        {
            ListEditorForm frm = new ListEditorForm(this);
            frm.Lpredictions.Items.AddRange(predictions.ToArray());
            frm.ShowDialog();
        }

        [CommandMethod("DFTBall", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public void DrawDoor()
        {
            Ball ball = new Ball();
            ball.PlaceObject();
            McContext.ShowNotification("Use green grip or shake (move) ball to get prediction");
        }

        public override bool GetGripPoints(GripPointsInfo info)
        {

            //frist grip to move
            info.AppendGrip(new McSmartGrip<Ball>(_basePnt+new Vector3d(0, _radius,0), (obj, g, offset) => {
                obj.TryModify();
                obj._basePnt += offset;
                obj.TryModify();
                obj.ShakePredict();
            }));
          
            //command grip
            var ctxGrip = new McSmartGrip<Ball>(McBaseGrip.GripType.PopupMenu, 2, _basePnt - 1.0 * new Vector3d(_radius, 0, 0),
                                            McBaseGrip.GripAppearance.PopupMenu, 0, "Select menu", Color.Lime);
            ctxGrip.GetContextMenu = (obj, items) =>
            {
                items.Add(new ContextMenuItem("Get prediction", "none", 1));
                items.Add(new ContextMenuItem("Edit predictions", "none", 2));
            };
            ctxGrip.OnCommand = (obj, commandId, grip) =>
            {
                if (grip.Id == 2)
                {
                    switch (commandId)
                    {

                        case 1:
                            {
                                ShakePredict();
                                break;
                            }
                        case 2:
                            {

                                CallForm();
                                break;

                            }
                    }
                }
            };
            info.AppendGrip(ctxGrip);
            return true;
        }

                
        public override hresult PlaceObject(PlaceFlags lInsertType)
        {
            InputJig jig = new InputJig();

            // Get the first box point from the jig
            InputResult res = jig.GetPoint("Select center point:");
            if (res.Result != InputResult.ResultCode.Normal)
                return hresult.e_Fail;

            _basePnt = res.Point;

            // Add the object to the database
            DbEntity.AddToCurrentDocument();

            return hresult.s_Ok;
        }

        private void ShakePredict()
        {
            Random rand = new Random();
            int val = rand.Next(0, predictions.Count);
            this.TryModify();
            _predText = predictions[val];

        }


   
    }





}



