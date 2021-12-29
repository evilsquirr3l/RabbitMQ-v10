namespace RabbitMQ_v10.Models;

public class Beer : IComparable
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public BeerType BeerType { get; set; }

    public bool IsAlcoholic { get; set; }

    public Manufacturer Manufacturer { get; set; }

    public List<string> Ingredients { get; set; } = new List<string>()
    {
        "water", "malt", "hops", "sugar"
    };

    public Chars Chars { get; set; }
    
    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(this, obj)) return 0;
        if (ReferenceEquals(this, null)) return -1;
        if (ReferenceEquals(obj, null)) return 1;
        return this.Id.CompareTo(((obj as Beer)!).Id);
    }
}