namespace TestConveyor.Svoyo.Data
{
    public class Client : NameEntity
    {
        public int ParentId { get; set; }
        public string ShortName { get; set; }
        public bool Active { get; set; }
    }
}
