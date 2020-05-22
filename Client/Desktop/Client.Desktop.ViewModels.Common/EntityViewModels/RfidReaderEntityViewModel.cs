using System;
using System.ComponentModel;
using Client.Desktop.ViewModels.Common.Extensions;
using Client.Desktop.ViewModels.Common.ViewModels;
using Storage.Laundry.Models;

namespace Client.Desktop.ViewModels.Common.EntityViewModels
{
    public class RfidReaderEntityViewModel : ViewModelBase, IDataErrorInfo
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

        public string Error { get; set; }
        public string this[string columnName] => Validate(columnName);
        public Func<RfidReaderEntityViewModel, string, bool> NameUniqueValidationFunc { get; set; }

        private string Validate(string columnName)
        {
            //string error;

            //if (columnName == nameof(Name))
            //{
            //    if (!Name.ValidateRequired(out error) ||
            //        !Name.ValidateBySpaces(out error))
            //    {
            //        return error;
            //    }

            //}

            //if (columnName == nameof(ReaderPort))
            //{
            //    if (!ReaderPort.ValidateRequired(out error))
            //    {
            //        return error;
            //    }

            //}

            //if (columnName == nameof(ReaderIp))
            //{
            //    if (!ReaderIp.ValidateRequired(out error) ||
            //        !ReaderIp.ValidateBySpaces(out error))
            //    {
            //        return error;
            //    }

            //    if (NameUniqueValidationFunc != null && !NameUniqueValidationFunc(this, nameof(ReaderIp)))
            //    {
            //        return "Ip already exist";
            //    }
            //}
            return null;
        }

    }
}
