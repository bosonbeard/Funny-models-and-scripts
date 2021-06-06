/*
Synhronized Randomize DAC.
See more https://habr.com/ru/post/561148/
*/

int ADC_pin = 5;
int input_pin = 3;
int output_pin = 7;
int synch_signa = 0;
int v_min = 10;
int v_max = 2550;
int synch_signal = 0;
float rand_voltage = 0;
// the setup function runs once when you press reset or power the board
void setup() {
  pinMode(ADC_pin, OUTPUT);
  pinMode(output_pin, OUTPUT);
  digitalWrite(output_pin,HIGH);
  pinMode(input_pin, INPUT);
  Serial.begin(9600);
}

// the loop function runs over and over again forever
void loop() {

synch_signal = digitalRead(input_pin);      // read signal from another device

if (synch_signal) {
 rand_voltage=random(v_min, v_max) / 10;
 analogWrite(ADC_pin, rand_voltage);
 Serial.println(rand_voltage); 
 delay(2500);   // wait for seconds
}
else
{
 delay(500);   // wait for seconds
}
                
}
