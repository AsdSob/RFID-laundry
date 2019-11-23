namespace PALMS.Data.Objects.EntityModel
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
