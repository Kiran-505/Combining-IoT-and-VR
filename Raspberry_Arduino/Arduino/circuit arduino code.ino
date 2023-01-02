#include <ezBuzzer.h>

#define NOTE_B0 31
#define NOTE_C1 33
#define NOTE_CS1 35
#define NOTE_D1 37
#define NOTE_DS1 39
#define NOTE_E1 41
#define NOTE_F1 44
#define NOTE_FS1 46
#define NOTE_G1 49
#define NOTE_GS1 52
#define NOTE_A1 55
#define NOTE_AS1 58
#define NOTE_B1 62
#define NOTE_C2 65
#define NOTE_CS2 69
#define NOTE_D2 73
#define NOTE_DS2 78
#define NOTE_E2 82
#define NOTE_F2 87
#define NOTE_FS2 93
#define NOTE_G2 98
#define NOTE_GS2 104
#define NOTE_A2 110
#define NOTE_AS2 117
#define NOTE_B2 123
#define NOTE_C3 131
#define NOTE_CS3 139
#define NOTE_D3 147
#define NOTE_DS3 156
#define NOTE_E3 165
#define NOTE_F3 175
#define NOTE_FS3 185
#define NOTE_G3 196
#define NOTE_GS3 208
#define NOTE_A3 220
#define NOTE_AS3 233
#define NOTE_B3 247
#define NOTE_C4 262
#define NOTE_CS4 277
#define NOTE_D4 294
#define NOTE_DS4 311
#define NOTE_E4 330
#define NOTE_F4 349
#define NOTE_FS4 370
#define NOTE_G4 392
#define NOTE_GS4 415
#define NOTE_A4 440
#define NOTE_AS4 466
#define NOTE_B4 494
#define NOTE_C5 523
#define NOTE_CS5 554
#define NOTE_D5 587
#define NOTE_DS5 622
#define NOTE_E5 659
#define NOTE_F5 698
#define NOTE_FS5 740
#define NOTE_G5 784
#define NOTE_GS5 831
#define NOTE_A5 880
#define NOTE_AS5 932
#define NOTE_B5 988
#define NOTE_C6 1047
#define NOTE_CS6 1109
#define NOTE_D6 1175
#define NOTE_DS6 1245
#define NOTE_E6 1319
#define NOTE_F6 1397
#define NOTE_FS6 1480
#define NOTE_G6 1568
#define NOTE_GS6 1661
#define NOTE_A6 1760
#define NOTE_AS6 1865
#define NOTE_B6 1976
#define NOTE_C7 2093
#define NOTE_CS7 2217
#define NOTE_D7 2349
#define NOTE_DS7 2489
#define NOTE_E7 2637
#define NOTE_F7 2794
#define NOTE_FS7 2960
#define NOTE_G7 3136
#define NOTE_GS7 3322
#define NOTE_A7 3520
#define NOTE_AS7 3729
#define NOTE_B7 3951
#define NOTE_C8 4186
#define NOTE_CS8 4435
#define NOTE_D8 4699
#define NOTE_DS8 4978
#define FORCE_SENSOR_PIN A0  // the FSR and 10K pulldown are connected to A0
#define HEART_RATE_PIN A2
#define POTENTIOMETER_PIN A5

// constants won't change. They're used here to set pin numbers:
const int BUTTON_PIN = 3;  // the number of the pushbutton pin
const int LED_PIN = 10;    // the number of the LED pin
const int BUZZER_PIN = 6;  // Arduino pin connected to Buzzer's pin
ezBuzzer buzzer(BUZZER_PIN);


// Variables will change:
int lastState = HIGH;  // the previous state from the input pin
int currentState;      // the current reading from the input pin

// variables will change:
int buttonState = 0;  // variable for reading the pushbutton status
bool sentButtonSocket = false;

int melody[] = {
  NOTE_E5, NOTE_E5, NOTE_E5,
  NOTE_E5, NOTE_E5, NOTE_E5,
  NOTE_E5, NOTE_G5, NOTE_C5, NOTE_D5,
  NOTE_E5,
  NOTE_F5, NOTE_F5, NOTE_F5, NOTE_F5,
  NOTE_F5, NOTE_E5, NOTE_E5, NOTE_E5, NOTE_E5,
  NOTE_E5, NOTE_D5, NOTE_D5, NOTE_E5,
  NOTE_D5, NOTE_G5
};

// note durations: 4 = quarter note, 8 = eighth note, etc, also called tempo:
int noteDurations[] = {
  8, 8, 4,
  8, 8, 4,
  8, 8, 8, 8,
  2,
  8, 8, 8, 8,
  8, 8, 8, 16, 16,
  8, 8, 8, 8,
  4, 4
};

void setup() {

  // initialize serial communication at 9600 bits per second:
  Serial.begin(9600);

  pinMode(BUTTON_PIN, INPUT_PULLUP);  // set arduino pin to input pull-up mode

  // initialize the pushbutton pin as an pull-up input
  // the pull-up input pin will be HIGH when the switch is open and LOW when the switch is closed.
  pinMode(BUTTON_PIN, INPUT_PULLUP);
  // initialize the LED pin as an output:
  pinMode(LED_PIN, OUTPUT);
  // initialize the pushbutton pin as an pull-up input:
  // the pull-up input pin will be HIGH when the switch is open and LOW when the switch is closed.
  pinMode(BUZZER_PIN, OUTPUT);  // set arduino pin to output mode
}


void loop() {

  buzzer.loop();

  // read the state of the switch/button:
  currentState = digitalRead(BUTTON_PIN);

  if (lastState == LOW && currentState == HIGH) {
    // Serial.println("The state changed from LOW to HIGH");
  }
  // save the last state
  lastState = currentState;
  // read the state of the pushbutton value:
  buttonState = digitalRead(BUTTON_PIN);

  // control LED according to the state of button
  if (buttonState == LOW) {          // If button is pressing
    digitalWrite(LED_PIN, HIGH);     // turn on LED
    digitalWrite(BUZZER_PIN, HIGH);  // turn on

    if (sentButtonSocket == false) {
      Serial.println("Button was pressed");
      sentButtonSocket = true;
      int length = sizeof(noteDurations) / sizeof(int);
      buzzer.playMelody(melody, noteDurations, length);
    }

  } else {                          // otherwise, button is not pressing
    digitalWrite(LED_PIN, LOW);     // turn off LED
    digitalWrite(BUZZER_PIN, LOW);  // turn off
    if (sentButtonSocket == true) {
      sentButtonSocket = false;
    }
  }
  int analogReading = analogRead(FORCE_SENSOR_PIN);
  int heartReading = analogRead(HEART_RATE_PIN);
  int potentiometerReading = analogRead(POTENTIOMETER_PIN);

  Serial.print("Heart Rate Reading = ");
  Serial.println(heartReading);

  Serial.print("Potentiometer Reading = ");
  Serial.println(potentiometerReading);

  Serial.print("Force sensor reading = ");
  Serial.println(analogReading);
  Serial.println("----------------------");
  delay(200);
}
