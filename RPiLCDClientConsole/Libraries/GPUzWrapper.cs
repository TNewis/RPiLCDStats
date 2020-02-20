using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RPiLCDClientConsole.Libraries
{
    class GPUzWrapper
    {
        [DllImport(@"GpuzShMem.dll")]
        private static extern int InitGpuzShMem();
        [DllImport(@"GpuzShMem.dll")]
        private static extern int RemGpuzShMem();
        [DllImport(@"GpuzShMem.dll")]
        private static extern IntPtr GetSensorName(int index);
        [DllImport(@"GpuzShMem.dll")]
        private static extern double GetSensorValue(int index);
        [DllImport(@"GpuzShMem.dll")]
        private static extern IntPtr GetSensorUnit(int index);
        [DllImport(@"GpuzShMem.dll")]
        private static extern IntPtr GetDataKey(int index);
        [DllImport(@"GpuzShMem.dll")]
        private static extern IntPtr GetDataValue(int index);

        /// <summary>
        /// Opens the shared memory interface for reading. Don't forget to close it if you don't need it anymore!
        /// </summary>
        /// <exception cref="Exception">If the shared memory could not be opened.</exception>
        public void Open()
        {
            if (InitGpuzShMem() != 0)
            {
                throw new Exception("An error occured while opening the GPUZ shared memory!");
            }
        }


        /// <summary>
        /// Closes the shared memory interface.
        /// </summary>
        public void Close()
        {
            RemGpuzShMem();
        }


        /// <summary>
        /// Gets the name of the specified sensor field (eg. "GPU Core Clock", "Fan Speed", ...).
        /// </summary>
        /// <param name="index">Index of sensor field needed.</param>
        /// <returns>Name of the sensor field.</returns>
        public string SensorName(int index)
        {
            return Marshal.PtrToStringUni(GetSensorName(index));
        }


        /// <summary>
        /// Gets the value of the specified sensor field (eg. 900.0, 56.0, ...).
        /// </summary>
        /// <param name="index">Index of sensor field needed.</param>
        /// <returns>Value of the sensor field.</returns>
        public double SensorValue(int index)
        {
            return GetSensorValue(index);
        }


        /// <summary>
        /// Gets the unit of the specified sensor field (e.g. "MHz", "°C", ...).
        /// </summary>
        /// <param name="index">Index of sensor field needed.</param>
        /// <returns>Unit of the sensor field.</returns>
        public string SensorUnit(int index)
        {
            return Marshal.PtrToStringUni(GetSensorUnit(index));
        }


        /// <summary>
        /// Gets the key (=name) of the specified data field (eg. "FillratePixel", "Vendor", ...).
        /// </summary>
        /// <param name="index">Index of data field needed.</param>
        /// <returns>Key of the data field.</returns>
        public string DataKey(int index)
        {
            return Marshal.PtrToStringUni(GetDataKey(index));
        }


        /// <summary>
        /// Gets the value of the specified data field (eg. "13.2", "ATI", ...).
        /// </summary>
        /// <param name="index">Index of data field needed.</param>
        /// <returns>Value of the data field.</returns>
        public string DataValue(int index)
        {
            return Marshal.PtrToStringUni(GetDataValue(index));
        }

        /// <summary>
        /// Returns a list of all sensor fields available.
        /// </summary>
        /// <returns>A formated string of all sensor names, values and units, each triple a line.</returns>
        public string ListAllSensors()
        {
            String s, res = String.Empty;

            for (int i = 0; (s = SensorName(i)) != String.Empty; i++)
                res += "[" + i + "]" + s + ": " + SensorValue(i) + " " + SensorUnit(i) + "\n";

            return res;
        }

        /// <summary>
        /// Returns a list of all data fields available.
        /// </summary>
        /// <returns>A formated string of all data keys and values, each pair a line.</returns>
        public string ListAllData()
        {
            String s, res = String.Empty;

            for (int i = 0; (s = DataKey(i)) != String.Empty; i++)
                res += "[" + i + "]" + s + ": " + DataValue(i) + "\n";

            return res;
        }
    }
}
