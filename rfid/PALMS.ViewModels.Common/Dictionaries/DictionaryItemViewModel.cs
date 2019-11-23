using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.ViewModels.Common.Dictionaries
{
    public abstract class DictionaryItemViewModel<TEntity> : DictionaryItemBaseViewModel where TEntity : NameEntity, new()
    {
        private string _name;
        private int _id;

        public TEntity OriginalObject { get; private set; }

        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public bool IsNew => OriginalObject == null || OriginalObject.IsNew;

        /// <summary>
        /// проверка OrifginalObject == null или OriginalObject.isNew
        /// </summary>
        /// <returns></returns>
        public virtual bool HasChanges => IsNew;

        public void Reset()
        {
            Update(OriginalObject);
        }

        public override void AcceptChanges()
        {
            if (OriginalObject == null) return;

            OriginalObject.Name = Name;
        }

        protected DictionaryItemViewModel()
        {
            OriginalObject = new TEntity();
        }

        protected DictionaryItemViewModel(TEntity entity)
        {
            Update(entity);
        }

        protected override string Validate(string columnName)
        {
            string error = null;

            if (columnName == nameof(Name))
            {
                if (!Name.ValidateRequired(out error) ||
                    !Name.ValidateBySpaces(out error) ||
                    !Name.ValidateByMaxLength(out error))
                {
                    return error;
                }
            }

            return error;
        }

        protected virtual void Update(TEntity originalObject)
        {
            OriginalObject = originalObject;

            if (OriginalObject == null) return;

            Id = OriginalObject.Id;
            Name = OriginalObject.Name;
        }
    }
}