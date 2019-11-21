using System;

namespace Storage.Laundry.Models.Abstract
{
    public abstract class EntityBase : IEntity<int>
    {
        public int Id { get; set; }
        public DateTime CreatedDateUtc { get; set; }
    }
}
