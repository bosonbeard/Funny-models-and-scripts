import serial
import time
from tkinter import *
from tkinter import ttk
from tkinter import messagebox

serialport = serial.Serial("/dev/ttyS0", baudrate=9600, timeout=1.0)
 
  
window = Tk()  
window.title("Security system for my pen")  
window.geometry('400x170')
#Presure sensor label
lbl_ps = ttk.Label(window, text="Pressure sensor", font=("Arial Bold", 20))
lbl_ps.grid(column=0, row=0)
lbl_ps_status = ttk.Label(window, text=" ",  font=("Arial Bold", 20))
lbl_ps_status.grid(column=1, row=0)
#Motion sensor label
lbl_ms = ttk.Label(window, text="Motion sensor", font=("Arial Bold", 20))
lbl_ms.grid(column=0, row=1)
lbl_ms_status = ttk.Label(window, text=" ", font=("Arial Bold", 20))
lbl_ms_status.grid(column=1, row=1)

while True:
   counter = 0
   rcv = serialport.readline().decode('utf-8').replace("\r\n","").split('  ')

   if (len(rcv)==2):
       ps=rcv[0]
       ms=rcv[1]
       print (ps+ " " +ms)
       if (int(ps)<380):
           lbl_ps_status.config(text = " Warning!")
           counter += 1
       else:
           lbl_ps_status.config(text = " Ok")

       if (int(ms)>0):
           lbl_ms_status['text']=" Warning!"
           counter += 1
       else:
           lbl_ms_status['text']=" Ok"  
       window.update_idletasks()  
       window.update()
       if (counter == 2):
            messagebox.showinfo("Alarm!", "Сheck your pen!")

       
       time.sleep(1)
