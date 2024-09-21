using SQLite;
using SQLiteNetExtensions.Attributes;

namespace LibraryTRU.Data;

public partial class Ticket : Object
{
    [PrimaryKey]
    public int Id { get; set; }

    public string Qrhash { get; set; } = null!;

    public string? Email { get; set; }

    [ForeignKey(typeof(Concert))]
    public int? ConcertId { get; set; }

    public DateTime? Timescanned { get; set; }

    [ManyToOne]
    public virtual Concert? Concert { get; set; }
}
