using System;

namespace PALMS.Data.Objects
{
    public interface IEntity : IEntityBase
    {
        int Id { get; set; }
        bool IsNew { get; }
    }

    public interface IEntityBase
    {
        DateTime CreatedDate { get; set; }
        DateTime? DeletedDate { get; set; }
    }

    public interface IAuditable
    {
        int Id { get; set; }
    }
}