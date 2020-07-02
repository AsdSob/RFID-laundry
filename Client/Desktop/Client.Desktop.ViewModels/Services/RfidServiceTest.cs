using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Client.Desktop.ViewModels.Common.EntityViewModels;
using Client.Desktop.ViewModels.Common.Services;

namespace Client.Desktop.ViewModels.Services
{
    public class RfidServiceTest : RfidService
    {
        public TestImpinj Reader = new TestImpinj();

        public RfidServiceTest(ILaundryService laundryService, IResolver resolver, IDialogService dialog) 
            : base(laundryService, resolver, dialog)
        {
            
        }

        public override void Connect()
        {
            if (SelectedReader == null) return;

            Connection(SelectedReader, SortedAntennas);
        }

        public bool Connection(RfidReaderEntityViewModel newReader, List<RfidAntennaEntityViewModel> antennas)
        {
            IsReading = false;
            return true;
        }

        public override void StartStopRead()
        {
            if (!IsReading)
            {
                StartRead();
            }
            else
            {
                StopRead();
            }
        }

        public override void StartRead()
        {
            Reader.UserEvent += DisplayTag;
            Reader.Start();

            IsReading = true;
        }

        public override void StopRead()
        {
            Reader.UserEvent -= DisplayTag;

            IsReading = false;
        }

        private void DisplayTag(List<Tuple<string, int>> tags)
        {
            _data = new ConcurrentDictionary<string, int>();

            foreach (var tag in tags)
            {
                AddData(tag.Item1, tag.Item2);
            }

            SetTagViewModels();
        }

        private void AddData(string epc, int antenna)
        {
            if (!_data.TryGetValue(epc, out int val))
            {
                _data.TryAdd(epc, antenna);
            }
            else
            {
                _data.TryUpdate(epc, antenna, val);
            }
        }
    }

    public class TestImpinj
    {
        public delegate void MyDelegate(List<Tuple<string, int>> val);

        public event MyDelegate UserEvent;

        private List<Tuple<string, int>> tages;

        public void Start()
        {
            tages = new List<Tuple<string, int>>();
            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                tages.Add(new Tuple<string, int>($"TagNumber - {i}", random.Next(1, 4)));
            }

            UserEvent?.Invoke(tages); ;
        }
    }
}
