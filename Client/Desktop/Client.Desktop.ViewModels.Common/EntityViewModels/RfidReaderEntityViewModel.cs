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
        private RfidReaderEntity _originalObject;
        private ushort _tagPopulation;

        public ushort TagPopulation
        {
            get => _tagPopulation;
            set => Set(() => TagPopulation, ref _tagPopulation, value);
        }
        public RfidReaderEntity OriginalObject
        {
            get => _originalObject;
            set => Set(() => OriginalObject, ref _originalObject, value);
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
            TagPopulation = (ushort) OriginalObject.TagPopulation;

        }

        public void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = Name;
            OriginalObject.ReaderIp = ReaderIp;
            OriginalObject.ReaderPort = ReaderPort;
            OriginalObject.TagPopulation = (int) TagPopulation;
        }

        public bool HasChanges() => OriginalObject == null ||
                                    OriginalObject.IsNew ||
                                    !Equals(Name, OriginalObject.Name) ||
                                    !Equals(ReaderIp, OriginalObject.ReaderIp) ||
                                    !Equals(ReaderPort, OriginalObject.ReaderPort) ||
                                    !Equals(TagPopulation, OriginalObject.TagPopulation);


    }
}
