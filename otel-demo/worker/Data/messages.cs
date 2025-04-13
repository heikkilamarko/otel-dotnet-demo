#pragma warning disable CS8981
namespace OtelDemo.Worker.Data;

public partial class messages
{
    public Guid id { get; set; }

    public string name { get; set; }

    public DateTime created_at { get; set; }
}
#pragma warning restore CS8981
