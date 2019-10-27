#Simple lib for test Python API in FreeCad
#Tested on Python 3.7. and FreeCad 0.18.3
#Designed for article - https://habr.com/ru/post/464113/
import  FreeCAD, FreeCADGui, Part, Draft
class stamp:
    "This class creates a rectangle with text. Coordinates are set by entering into the console."
    def __init__(self):
        self.view = FreeCADGui.ActiveDocument.ActiveView
        x=float(input("set x point  "))
        y=float(input("set y point  "))
        mtext=str(input("input text  "))
        self.drawstamp(mtext, x , y )
        FreeCAD.Console.PrintMessage("\r\n Your object was successfully created")
        
        
    def drawstamp(self, mtext,x=0,y=0 ):
        "Method for placing a figure in model space."
        if (mtext==""):
            mtext="Hello Habr!"
        point=FreeCAD.Placement(FreeCAD.Vector(x,y,0), FreeCAD.Rotation(0,0,0), FreeCAD.Vector(0,0,0))
        text = Draft.makeText(mtext,point=FreeCAD.Vector(x+0.25,y+0.55,0.0))
        fontsize = 1.0
        textwidth=(len(mtext)*fontsize*0.6)+0.35
        textheight=fontsize+0.5
        text.ViewObject.FontSize = fontsize  
        text.ViewObject.FontName = "Courier"  
        rec = Draft.makeRectangle(length=textwidth,height=textheight,placement=point,face=False,support=None)
         
       
