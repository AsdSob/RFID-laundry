using Client.Desktop.ViewModels.Common.ViewModels;

namespace Client.Desktop.ViewModels.Content
{
    public class DataViewModel : ViewModelBase
    {
        private string _text;

        public string Text
        {
            get => _text;
            set => Set(() => Text, ref _text, value);
        }

        //public DataViewModel()
        //{
        //    Text = "NEW Content of DataViewModel";
        //}


    }
}
