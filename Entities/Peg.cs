using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Peg
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; private set; }
    [ForeignKey(nameof(Owner))]
    public int Owner { get; private set; }
    public int Position { get; set; }
    public int Order { get; set; }
    private Peg()
    {
        Position = 0;
        Order = 0;
    }
    public static Peg Create(int ownerId, int order = 0, int position = 0)
    {
        return new() { Owner = ownerId, Order = order, Position = position };
    }
}