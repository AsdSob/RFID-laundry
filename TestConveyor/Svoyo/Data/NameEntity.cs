using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConveyor.Svoyo.Data
{
    public abstract class NameEntity : Entity, INameEntity
    {
        public string Name { get; set; }
    }

    public interface INameEntity : IEntity
    {
        string Name { get; set; }
    }
}
