const int btn1 = 7;
const int btn2 = 8;
const int btn3 = 9;
const int btn4 = 10;
const int btn5 = 11;
const int btn6 = 12;

void setup() {
  Serial.begin(9600);
  pinMode(btn1, INPUT);
  pinMode(btn2, INPUT);
  pinMode(btn3, INPUT);
  pinMode(btn4, INPUT);
  pinMode(btn5, INPUT);
  pinMode(btn6, INPUT);
}

void loop() {
  
  if (Serial.available() > 0) {
    String received = Serial.readStringUntil('\n');

    // If the received text is "Who Are You?", respond with "It is me!"
    if (received == "Who Are You?") {
      Serial.println("It is me!");
    }
  }

  String result = "";

  if(digitalRead(btn1) == HIGH){
    result += '1';
  }
    if(digitalRead(btn2) == HIGH){
    result += '2';
  }
    if(digitalRead(btn3) == HIGH){
    result += '3';
  }
    if(digitalRead(btn4) == HIGH){
    result += '4';
  }
    if(digitalRead(btn5) == HIGH){
    result += '5';
  }
    if(digitalRead(btn6) == HIGH){
    result += '6';
  }

  if(result != ""){
    Serial.println(result);
  }
}
