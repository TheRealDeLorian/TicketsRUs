using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Text.Json.Serialization;

namespace LibraryTRU.Data;

public partial class Concert : Object
{
    [PrimaryKey]
    public int Id { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string EventName { get; set; } = null!;

    public string? Description { get; set; }
    [JsonIgnore]
    [OneToMany]
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
