using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;
namespace ArduinoAudioPlayer
{
    class ArduinoDevice
    {
        private SerialPort arduinoPort;
        public string comPort;
        public ArduinoStatus Status = ArduinoStatus.Unknown;
        public ArduinoDevice(string _comPort = "default")
        {
            if (_comPort == "default" && ArduinoDevice.GetDevices().Length > 0)
            {
                comPort = ArduinoDevice.GetDevices()[0];
            }
            else if (ArduinoDevice.GetDevices().Length == 0) 
            {
                ForceUserToChoose(this);
            }
            else
                comPort = _comPort;
        }
        public static string[] GetDevices()
        {
            return SerialPort.GetPortNames();
        }
        private void connect()
        {
            arduinoPort = new SerialPort(comPort, 9600);
            arduinoPort.Open();
        }
        public bool Setup()
        {
            try
            {
                this.connect();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        public void Write(string data)
        {
            try
            {
                List<char> prepareData = data.ToCharArray().ToList();
                prepareData.Add((char)0x0A);
                char[] charData = prepareData.ToArray();
                arduinoPort.Write(charData, 0, charData.Length);
            }
            catch (Exception e)
            {
                ForceUserToChoose(this);
            }
        }
        public void WakeUp()
        {
            this.Write("-1");
            Status = ArduinoStatus.Playing;
        }
        public void PutIntoStandby()
        {
            this.Write("-2");
            Status = ArduinoStatus.StandBy;
        }
        public bool CanRead()
        {
            return arduinoPort.BytesToRead > 0;
        }
        public string Read()
        {
            return arduinoPort.ReadLine();
        }
        public bool IsConnected()
        {
            return arduinoPort.IsOpen;
        }
        public static void ForceUserToChoose(ArduinoDevice dev)
        {
            while (true)
            {
                Console.Clear();
                string[] devices = ArduinoDevice.GetDevices();
                Console.WriteLine("Some horrible error happened, choose a device (again): ");
                for (int i = 0; i < devices.Length; i++)
                {
                    Console.WriteLine("[{0}] {1}", i, devices[i]);
                }
                int choosenDeviceId = 0;
                Int32.TryParse(Console.ReadLine(), out choosenDeviceId);
                if (devices.Length == 0)
                    continue;
                dev.comPort = devices[choosenDeviceId];
                bool attemptSuccess = dev.Setup();
                if (attemptSuccess) break;
            }
        }
    }
    enum ArduinoStatus
    {
        Unknown,
        StandBy,
        Playing
    }
}
