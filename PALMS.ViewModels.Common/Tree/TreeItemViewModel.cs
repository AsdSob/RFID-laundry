using System;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.EntityModel;

namespace PALMS.ViewModels.Common.Tree
{
    public class TreeItemViewModel : ViewModelBase
    {
        private int _id;
        private int? _parentId;
        private string _name;

        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        public int? ParentId
        {
            get => _parentId;
            set => Set(ref _parentId, value);
        }
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }


        public INameEntity OriginalObject { get; }

        public TreeItemViewModel(INameEntity entity)
        {
            OriginalObject = entity ?? throw new ArgumentNullException(nameof(entity));
            Name = entity.Name;

        }


    }
}
