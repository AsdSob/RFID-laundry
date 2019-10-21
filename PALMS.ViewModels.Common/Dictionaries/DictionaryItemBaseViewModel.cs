using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace PALMS.ViewModels.Common.Dictionaries
{
    public abstract class DictionaryItemBaseViewModel : ViewModelBase, IDataErrorInfo, IChangeTracking
    {
        #region IDataErrorInfo

        public string this[string columnName] => Validate(columnName);

        public string Error { get; set; }

        protected abstract string Validate(string columnName);

        #endregion

        #region IChangeTracking

        private bool _notifyingObjectIsChanged;
        private readonly object _notifyingObjectIsChangedSyncRoot = new object();

        /// <summary>
        /// Resets the object’s state to unchanged by accepting the modifications.
        /// </summary>
        public virtual void AcceptChanges()
        {
            IsChanged = false;
        }

        /// <summary>
        /// Gets the object's changed status.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the object’s content has changed since the last call to <see cref="M:PALMS.ViewModels.Common.Dictionaries.DictionaryItemBaseViewModel.AcceptChanges" />; otherwise, <see langword="false" />. 
        /// The initial value is <see langword="false" />.
        /// </value>
        public bool IsChanged
        {
            get
            {
                lock (_notifyingObjectIsChangedSyncRoot)
                {
                    return _notifyingObjectIsChanged;
                }
            }

            protected set
            {
                lock (_notifyingObjectIsChangedSyncRoot)
                {
                    Set(ref _notifyingObjectIsChanged, value);
                }
            }
        }

        #endregion
    }
}