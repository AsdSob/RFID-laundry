using Storage.Laundry.Models.Abstract;

namespace Storage.Laundry.Models
{
    public class AccountDetailsEntity : EntityBase
    {
        public int AccountId { get; set; }
        public int? ReaderId { get; set; }

        public virtual AccountEntity AccountEntity { get; set; }
        public virtual RfidReaderEntity RfidReaderEntity { get; set; }

    }
}
