using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.BassWasapi;
using System.IO.Ports;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Configuration;
namespace ArduinoAudioPlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            BassNet.Registration(ConfigurationManager.AppSettings["bass_email"], ConfigurationManager.AppSettings["bass_key"]);
            AudioDevice dev = new AudioDevice();
            dev.Setup();
            ArduinoDevice ard = new ArduinoDevice();
            bool couldSetupArduino = ard.Setup();

            if (!couldSetupArduino)
                ArduinoDevice.ForceUserToChoose(ard);
            ard.PutIntoStandby();
            List<string> dataFromArduino = new List<string>();
            while (true)
            {
                if (dev.IsPlaying() && ard.Status == ArduinoStatus.StandBy)
                    ard.WakeUp();
                else if (!dev.IsPlaying() && ard.Status == ArduinoStatus.Playing)
                    ard.PutIntoStandby();
                Console.Clear();
                Console.WriteLine("{0}", dev.GetDeviceName());
                Console.WriteLine("Device audio level: {0}", dev.GetAudioLevel());
                Console.WriteLine("Is connected to arduino?: {0}", ard.IsConnected() ? "Yes, on port " + ard.comPort : "No, some shit error, can't connect on: " + ard.comPort);
                float[] newAveraged = FFTAnalyzer.AvgValues(dev.GetFFTData(BASSData.BASS_DATA_FFT512, 512), 16, 10000);
                for (int i = 0; i < newAveraged.Length; i++)
                {
                    Console.WriteLine("FFT[{0}]: {1}", i, Math.Round(newAveraged[i], 4));
                    if (ard.Status == ArduinoStatus.Playing && i == 0)
                    {
                        ard.Write(((int)newAveraged[i]).ToString());
                    }
                }
                Thread.Sleep(50);
            }
        }
    }
}
