#based on https://proglib.io/p/real-time-object-detection/

# import the necessary packages
from imutils.video import VideoStream
from imutils.video import FPS
import numpy as np
import imutils
import time
import cv2
import os


path=os.path.join(os.path.abspath(os.curdir) , 'my_resnet18.onnx')
args_confidence = 0.2

# initialize the list of class labels 
CLASSES = ['raspberry', 'someduino']

# load our serialized model from disk
print("[INFO] loading model...")
net = cv2.dnn.readNetFromONNX (path)

# initialize the video stream, allow the c
#cammera sensor to warmup,
# and initialize the FPS counter
print("[INFO] starting video stream...")
vs = VideoStream(src=0).start()
time.sleep(2.0)
fps = FPS().start()

frame = vs.read()
frame = imutils.resize(frame, width=400)

# loop over the frames from the video stream
while True:
	# grab the frame from the threaded video stream and resize it
	# to have a maximum width of 400 pixels
	frame = vs.read()
	frame = imutils.resize(frame, width=400)

	# grab the frame dimensions and convert it to a blob
	(h, w) = frame.shape[:2]
	blob = cv2.dnn.blobFromImage(cv2.resize(frame, (224, 224)),scalefactor=1.0/224
                              , size=(224, 224), mean= (104, 117, 123), swapRB=True)
	cv2.imshow("Cropped image", cv2.resize(frame, (224, 224)))

	# pass the blob through the network and obtain the detections and
	# predictions
	net.setInput(blob)
	detections = net.forward()
	print(list(zip(CLASSES,detections[0])))
	# loop over the detections

		# extract the confidence (i.e., probability) associated with
		# the prediction
	confidence = abs(detections[0][0]-detections[0][1])
	print(confidence)
		# filter out weak detections by ensuring the `confidence` is
		# greater than the minimum confidence
	if (confidence > args_confidence) :
		
		class_mark=np.argmax(detections)
		cv2.putText(frame, CLASSES[class_mark], (30,30),cv2.FONT_HERSHEY_SIMPLEX, 0.6, (242, 230, 220), 2)

	# show the output frame
	cv2.imshow("Web camera view", frame)
	key = cv2.waitKey(1) & 0xFF

	# if the `q` key was pressed, break from the loop
	if key == ord("q"):
		break

	# update the FPS counter
	fps.update()

# stop the timer and display FPS information
fps.stop()
# do a bit of cleanup
cv2.destroyAllWindows()
vs.stop()