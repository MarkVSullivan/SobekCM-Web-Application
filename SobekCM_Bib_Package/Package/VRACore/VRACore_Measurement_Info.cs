using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package.VRACore
{
    /// <summary> The physical size, shape, scale, dimensions, or format of the Work or Image, as encoded in VRACore </summary>
    public class VRACore_Measurement_Info
    {
        private string units, measurements;

        /// <summary> Constructor for a new instance of the VRACore_Measurement_Info class </summary>
        public VRACore_Measurement_Info()
        {
            // Do nothing
        }

        /// <summary> Constructor for a new instance of the VRACore_Measurement_Info class </summary>
        /// <param name="Measurements">The physical size, shape, scale, dimensions, or format of the Work or Image</param>
        /// <param name="Units">Units for the included measurement(s)</param>
        public VRACore_Measurement_Info(string Measurements, string Units)
        {
            measurements = Measurements;
            units = Units;
        }

        /// <summary> The physical size, shape, scale, dimensions, or format of the Work or Image </summary>
        public string Measurements
        {
            get { return measurements ?? String.Empty; }
            set { measurements = value; }
        }

        /// <summary> Units for the included measurement(s) </summary>
        public string Units
        {
            get { return units ?? String.Empty; }
            set { units = value; }
        }
    }
}
