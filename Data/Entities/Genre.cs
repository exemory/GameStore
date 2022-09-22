using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class Genre : EntityBase
{
    public string Name { get; set; } = default!;

    public Guid? ParentId { get; set; }
    public Genre? Parent { get; set; }
    
    public IEnumerable<Game> Games { get; set; } = default!;
}