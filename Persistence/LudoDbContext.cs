using Microsoft.EntityFrameworkCore;

public class LudoDbContext : DbContext
{
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Peg> Pegs => Set<Peg>();

    public LudoDbContext(DbContextOptions<LudoDbContext> options) : base(options)
    {

    }

}