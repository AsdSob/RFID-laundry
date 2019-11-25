using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConveyor.Svoyo.Data
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
}
