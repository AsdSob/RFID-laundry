using System;
using System.Collections.Generic;
using System.Text;
using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class RfidReaderEntityViewModel : ViewModelBase
    {
        private int _id;
        private string _name;
        private string _readerIp;
        private int _readerPort;
        private double _antenna1Tx;
        private double _antenna1Rx;
        private double _antenna2Rx;
        private double _antenna2Tx;
        private double _antenna3Rx;
        private double _antenna3Tx;
        private double _antenna4Rx;
        private double _antenna4Tx;
        private RfidReaderEntity _originalObject;

        public RfidReaderEntity OriginalObject
        {
            get => _originalObject;
            set => Set(() => OriginalObject, ref _originalObject, value);
        }
        public double Antenna4Tx
        {
            get => _antenna4Tx;
            set => Set(() => Antenna4Tx, ref _antenna4Tx, value);
        }
        public double Antenna4Rx
        {
            get => _antenna4Rx;
            set => Set(() => Antenna4Rx, ref _antenna4Rx, value);
        }
        public double Antenna3Tx
        {
            get => _antenna3Tx;
            set => Set(() => Antenna3Tx, ref _antenna3Tx, value);
        }
        public double Antenna3Rx
        {
            get => _antenna3Rx;
            set => Set(() => Antenna3Rx, ref _antenna3Rx, value);
        }
        public double Antenna2Tx
        {
            get => _antenna2Tx;
            set => Set(() => Antenna2Tx, ref _antenna2Tx, value);
        }
        public double Antenna2Rx
        {
            get => _antenna2Rx;
            set => Set(() => Antenna2Rx, ref _antenna2Rx, value);
        }
        public double Antenna1Rx
        {
            get => _antenna1Rx;
            set => Set(() => Antenna1Rx, ref _antenna1Rx, value);
        }
        public double Antenna1Tx
        {
            get => _antenna1Tx;
            set => Set(() => Antenna1Tx, ref _antenna1Tx, value);
        }
        public int ReaderPort
        {
            get => _readerPort;
            set => Set(() => ReaderPort, ref _readerPort, value);
        }
        public string ReaderIp
        {
            get => _readerIp;
            set => Set(() => ReaderIp, ref _readerIp, value);
        }
        public string Name
        {
            get => _name;
            set => Set(() => Name, ref _name, value);
        }
        public int Id
        {
            get => _id;
            set => Set(() => Id, ref _id, value);
        }


        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        public RfidReaderEntityViewModel()
        {
            OriginalObject = new RfidReaderEntity();
        }

        public RfidReaderEntityViewModel(RfidReaderEntity originalObject) : this()
        {
            Update(originalObject);
        }


        private void Update(RfidReaderEntity originalObject)
        {
            OriginalObject = originalObject;

            Id = OriginalObject.Id;
            Name = OriginalObject.Name;
            ReaderPort = OriginalObject.ReaderPort;
            ReaderIp = OriginalObject.ReaderIp;
            Antenna1Rx = OriginalObject.Antenna1Rx;
            Antenna1Tx = OriginalObject.Antenna1Tx;
            Antenna2Rx = OriginalObject.Antenna2Rx;
            Antenna2Tx = OriginalObject.Antenna2Tx;
            Antenna3Rx = OriginalObject.Antenna3Rx;
            Antenna3Tx = OriginalObject.Antenna3Tx;
            Antenna4Rx = OriginalObject.Antenna4Rx;
            Antenna4Tx = OriginalObject.Antenna4Tx;

        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = Name;
            OriginalObject.ReaderIp = ReaderIp;
            OriginalObject.ReaderPort = ReaderPort;
            OriginalObject.Antenna1Rx = Antenna1Rx;
            OriginalObject.Antenna1Tx = Antenna1Tx;
            OriginalObject.Antenna2Rx = Antenna2Rx;
            OriginalObject.Antenna2Tx = Antenna2Tx;
            OriginalObject.Antenna3Rx = Antenna3Rx;
            OriginalObject.Antenna3Tx = Antenna3Tx;
            OriginalObject.Antenna4Rx = Antenna4Rx;
            OriginalObject.Antenna4Tx = Antenna4Tx;

        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(Name, OriginalObject.Name) ||
                                    !Equals(ReaderIp, OriginalObject.ReaderIp) ||
                                    !Equals(ReaderPort, OriginalObject.ReaderPort) ||
                                    !Equals(Antenna1Rx, OriginalObject.Antenna1Rx) ||
                                    !Equals(Antenna1Tx, OriginalObject.Antenna1Tx) ||
                                    !Equals(Antenna2Rx, OriginalObject.Antenna2Rx) ||
                                    !Equals(Antenna2Tx, OriginalObject.Antenna2Tx) ||
                                    !Equals(Antenna3Rx, OriginalObject.Antenna3Rx) ||
                                    !Equals(Antenna3Tx, OriginalObject.Antenna3Tx) ||
                                    !Equals(Antenna4Rx, OriginalObject.Antenna4Rx) ||
                                    !Equals(Antenna4Tx, OriginalObject.Antenna4Tx);


    }
}
