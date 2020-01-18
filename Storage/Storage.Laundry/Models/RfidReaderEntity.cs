using System;
using System.Collections.Generic;
using System.Text;
using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry.Models
{
    public class RfidReaderEntity : EntityBase
    {
        public string Name { get; set; }
        public string ReaderIp { get; set; }
        public int ReaderPort { get; set; }

        public double Antenna1Rx { get; set; }
        public double Antenna1Tx { get; set; }

        public double Antenna2Rx { get; set; }
        public double Antenna2Tx { get; set; }

        public double Antenna3Rx { get; set; }
        public double Antenna3Tx { get; set; }

        public double Antenna4Rx { get; set; }
        public double Antenna4Tx { get; set; }
    }
}
