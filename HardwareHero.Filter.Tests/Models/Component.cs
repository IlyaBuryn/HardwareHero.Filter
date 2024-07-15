namespace HardwareHero.Filter.Tests.Models
{
    public class Component
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ComponentType? ComponentType { get; set; }
        public ICollection<ComponentAttribute> ComponentAttributes { get; set; } = new List<ComponentAttribute>(50);
    }
}
