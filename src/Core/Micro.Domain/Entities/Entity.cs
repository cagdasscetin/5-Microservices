using System.ComponentModel.DataAnnotations.Schema;

namespace Micro.Domain.Entities;

public abstract class Entity<TKey>
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public TKey Id { get; set; }
}