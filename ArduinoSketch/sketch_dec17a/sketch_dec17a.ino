
byte analogLED = 3;
int lastLEDValue = 0;
byte Switcher = 0;
boolean isConnectedPC = false;
byte LEDDirection = 1;
int readValue = 0;
int readSign = 1;
char readCharacter = '0';
boolean shouldRead = true;

void setup(){
	Serial.begin(9600);
	pinMode(analogLED, OUTPUT);
}

void doPcWork()
{
	if(Serial.available() > 1)
	{
		int data = readDataFromSerial();
		if (data < 0)
		{
			if (data == -2)
			{
				isConnectedPC = false;
				return;
			} 
		}
		lastLEDValue = data;
	}

}

void doStandbyWork()
{
	if (Serial.available() > 1)
	{
		int readValue = readDataFromSerial();
		if (readValue == -1)
		isConnectedPC = true;
		lastLEDValue = 0;
		return;
	}
	delay(20);
	if (LEDDirection == 1)
		lastLEDValue += 1;
	else if (LEDDirection == 0)
		lastLEDValue -= 1;

	if (lastLEDValue > 255)
		LEDDirection = 0;
	if (lastLEDValue < 20)
		LEDDirection = 1;
}
int readDataFromSerial(){
	readValue = 0;
	readSign = 1;
	if (Serial.available() > 0)
	{
		shouldRead = true;
		while (shouldRead)
		{
			readCharacter = Serial.read();
			
			if (readCharacter >= '0' && readCharacter <= '9') // is this an ascii digit between 0 and 9?
				readValue = (readValue * 10) + (readCharacter - '0'); // yes, accumulate the value
			else if (readCharacter == '-')
				readSign = -1;
			else if (readCharacter == 10)
				shouldRead = false;

		}
	}
	readValue *= readSign;
	return readValue;
}
void loop(){

	if (isConnectedPC)
		doPcWork();
	else
		doStandbyWork();

	analogWrite(analogLED, constrain(lastLEDValue, 0, 255));
}
