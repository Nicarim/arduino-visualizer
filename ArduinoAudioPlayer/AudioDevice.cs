using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace ArduinoAudioPlayer
{
    class AudioDevice
    {
        private int deviceId;
        private WASAPIPROC proc;
        public AudioDevice()
        {

        }
        public bool chooseDevice()
        {
            int deviceIterator = -1;
            BASS_WASAPI_DEVICEINFO[] infos = BassWasapi.BASS_WASAPI_GetDeviceInfos();
            foreach (BASS_WASAPI_DEVICEINFO info in infos)
            {
                deviceIterator++;
                if (info.IsDefault && !info.IsInput)
                {
                    deviceIterator = deviceIterator + 1; // Device loopback is always +1
                    break;
                }
            }

            if (deviceIterator < 0)
                return false;

            deviceId = deviceIterator;
            return true;
        }
        private void setupDevice()
        {
            Bass.BASS_Init(0, 0, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            proc = new WASAPIPROC(WasapiCallback);
            BassWasapi.BASS_WASAPI_Init(deviceId, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER | BASSWASAPIInit.BASS_WASAPI_AUTOFORMAT, 1f, 0f, proc, IntPtr.Zero);
            BASS_WASAPI_DEVICEINFO info = BassWasapi.BASS_WASAPI_GetDeviceInfo(deviceId);
            if (info.name.Contains("Conexant")) // buzzing fix for bug in conexant devices
            {
                BassWasapi.BASS_WASAPI_Init(deviceId - 1, 0, 0, BASSWASAPIInit.BASS_WASAPI_AUTOFORMAT, 0f, 0f, null, IntPtr.Zero);
            }
            BASSError _error = Bass.BASS_ErrorGetCode();
            if (_error != BASSError.BASS_OK)
                throw new Exception("Cannot initialize WASAPI device, error code: " + _error);
        }
        private void startDevice()
        {
            BassWasapi.BASS_WASAPI_SetDevice(deviceId);
            BassWasapi.BASS_WASAPI_Start();
        }
        /// <summary>
        ///     Gets audio level directly from WASAPI device
        /// </summary>
        /// <returns>Audio level - from 0 to 100</returns>
        public int GetAudioLevel()
        {
            float audioLevel = BassWasapi.BASS_WASAPI_GetDeviceLevel(deviceId, -1);
            return ((int)(audioLevel * 100));
        }
        /// <summary>
        /// Gets FFT data (fast fourier transformation) from audio device.
        /// </summary>
        /// <param name="dataType">BASSData - type of data</param>
        /// <param name="size">int - size of array (should always be equal to BASSData size)</param>
        /// <returns></returns>
        public float[] GetFFTData(BASSData dataType, int size)
        {
            float[] dataFromDevice = new float[size];
            BassWasapi.BASS_WASAPI_GetData(dataFromDevice, (int)dataType);
            return dataFromDevice;
        }
        /// <summary>
        /// Sets up an audio device. May throw Exception.
        /// </summary>
        /// <returns>Boolean indicating if choosing device was possible</returns>
        public bool Setup()
        {
            bool choosingDeviceSetup = chooseDevice();
            setupDevice();
            startDevice();
            return choosingDeviceSetup;
        }
        /// <summary>
        /// Returns device human readable name.
        /// </summary>
        /// <returns></returns>
        public string GetDeviceName()
        {
            return BassWasapi.BASS_WASAPI_GetDeviceInfo(deviceId).name;
        }
        public bool IsPlaying()
        {
            return !(BassWasapi.BASS_WASAPI_GetDeviceLevel(deviceId, 0) == 0f);
        }
        private static int WasapiCallback(IntPtr buffer, int length, IntPtr user)
        {
            return 1;
        }
    }
}
