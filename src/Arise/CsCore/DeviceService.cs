namespace Arise.Core.CsCore
{
    using CSCore.CoreAudioAPI;
    using System.Collections.Generic;
    using System.Linq;

    public class DeviceService
    {
        #region Fields

        private MMDeviceEnumerator _deviceEnumerator;

        #endregion

        #region Constructors

        public DeviceService()
        {
            this._deviceEnumerator = new MMDeviceEnumerator();
        }

        #endregion

        #region Methods

        public IEnumerable<MMDevice> InputDevices()
        {
            return this._deviceEnumerator.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active);
        }

        public IEnumerable<MMDevice> OutputDevices()
        {
            return this._deviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active);
        }

        public MMDevice GetInputDevice(string deviceId)
        {
            var device = this.InputDevices().Where(d => d.DeviceID == deviceId).FirstOrDefault();
            if (device == null)
            {
                throw new System.InvalidOperationException($"The device ­{deviceId} was not found");
            }
            
            return device;
        }

        public MMDevice GetOutputDevice(string deviceId)
        {
            var device = this.OutputDevices().Where(d => d.DeviceID == deviceId).FirstOrDefault();
            if (device == null)
            {
                throw new System.InvalidOperationException($"The device ­{deviceId} was not found");
            }

            return device;
        }

        #endregion
    }
}
