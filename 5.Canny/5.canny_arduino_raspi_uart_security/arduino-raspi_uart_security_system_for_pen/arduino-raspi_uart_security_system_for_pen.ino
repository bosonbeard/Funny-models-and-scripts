byte photoPin = A0;

void setup() {
    Serial.begin(9600);
    pinMode(photoPin, INPUT);
 //  pinMode(8, OUTPUT); 
    pinMode(5, INPUT); 
    pinMode(13, OUTPUT);

}

void loop() {
  int adjustment = 250;
  int pressure_sensor = analogRead(A1) - adjustment;
  int motion_sensor = digitalRead(5);
  Serial.print(pressure_sensor);
  Serial.print("  ");
  Serial.println(motion_sensor);
  if ((pressure_sensor<380) && (motion_sensor==1))
  {
    digitalWrite(LED_BUILTIN, HIGH); 
  }
  else {
    digitalWrite(LED_BUILTIN, LOW); 
  }
  delay(1000);
}
