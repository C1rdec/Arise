namespace Arise.Console
{
    using Arise.Core.CsCore;
    using Core.Services;
    using CSCore;
    using CSCore.Codecs.WAV;
    using CSCore.SoundIn;
    using CSCore.Streams;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            RecordSample().Wait();
        }

        public static void GoogleDriveSample()
        {
            var googleDriveService = new GoogleDriveService();
            var file = googleDriveService.CreateFolder("Arise").Result;
            var file2 = googleDriveService.CreateFolder("Folder1", file.Id).Result;

            var file3 = googleDriveService.CreateFolder("Folder3", file2.Id);
        }

        public static void DeviceSample()
        {
            var deviceService = new DeviceService();
            foreach (var device in deviceService.InputDevices())
            {
                System.Console.WriteLine(device.DeviceID);
            }
        }

        public static async Task RecordSample()
        {
            //create a new soundIn instance
            var soundIn = new WasapiCapture();
            //optional: set some properties 
            //soundIn.Device = ...
            //...
            soundIn.Device = new DeviceService().InputDevices().First();
            soundIn.Initialize();

            var waveWriter = new WaveWriter(@"C:\Users\Cedric Lampron\Desktop\Test Record\dump.wav", soundIn.WaveFormat); ;

            await Task.Run(() =>
            {
                //create a SoundSource around the the soundIn instance
                //this SoundSource will provide data, captured by the soundIn instance
                var soundInSource = new SoundInSource(soundIn) { FillWithZeros = false };

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
                    waveWriter.Write(e.Data, e.Offset, e.ByteCount);
                };

                //we've set everything we need -> start capturing data
                soundIn.Start();
            });

            await Task.Delay(5000);
            soundIn.Stop();
            waveWriter.Dispose();
            waveWriter = null;
            soundIn.Dispose();
            soundIn = null;
        }
    }
}
