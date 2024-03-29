﻿namespace Arise.Core
{
    using Arise.Core.CsCore;
    using CSCore;
    using CSCore.SoundIn;
    using CSCore.Streams;
    using System.Linq;

    public class Class1
    {
        public Class1()
        {
            //create a new soundIn instance
            var soundIn = new WasapiCapture();
            //optional: set some properties 
            //soundIn.Device = ...
            //...

            soundIn.Device = new DeviceService().InputDevices().First();

            //initialize the soundIn instance
            soundIn.Initialize();

            //create a SoundSource around the the soundIn instance
            //this SoundSource will provide data, captured by the soundIn instance
            SoundInSource soundInSource = new SoundInSource(soundIn) { FillWithZeros = false };

            //create a source, that converts the data provided by the
            //soundInSource to any other format
            //in this case the "Fluent"-extension methods are being used
            IWaveSource convertedSource = soundInSource
                .ToStereo() //2 channels (for example)
                .ChangeSampleRate(8000) // 8kHz sample rate
                .ToSampleSource()
                .ToWaveSource(16); //16 bit pcm

            //register an event handler for the DataAvailable event of 
            //the soundInSource
            //Important: use the DataAvailable of the SoundInSource
            //If you use the DataAvailable event of the ISoundIn itself
            //the data recorded by that event might won't be available at the
            //soundInSource yet
            soundInSource.DataAvailable += (s, e) =>
            {
                //read data from the converedSource
                //important: don't use the e.Data here
                //the e.Data contains the raw data provided by the 
                //soundInSource which won't have your target format
                byte[] buffer = new byte[convertedSource.WaveFormat.BytesPerSecond / 2];
                int read;

                //keep reading as long as we still get some data
                //if you're using such a loop, make sure that soundInSource.FillWithZeros is set to false
                while ((read = convertedSource.Read(buffer, 0, buffer.Length)) > 0)
                {
                    
                    //your logic follows here
                    //for example: stream.Write(buffer, 0, read);

                }
            };

            //we've set everything we need -> start capturing data
            soundIn.Start();
        }
    }
}
