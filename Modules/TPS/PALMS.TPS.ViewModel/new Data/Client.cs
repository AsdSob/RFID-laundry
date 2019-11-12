using PALMS.Data.Objects.EntityModel;

namespace PALMS.TPS.ViewModel.new_Data
{
    public 
        class Client : NameEntity
    {
        public string ShortName { get; set; }
        public bool Active { get; set; }
        public int? ParentId { get; set; }
    }
}
