using Microsoft.EntityFrameworkCore;

public class LudoDbContext(DbContextOptions<LudoDbContext> options) : DbContext(options)
{
  public DbSet<Board> Boards => Set<Board>();
  public DbSet<Player> Players => Set<Player>();
  public DbSet<Peg> Pegs => Set<Peg>();
}