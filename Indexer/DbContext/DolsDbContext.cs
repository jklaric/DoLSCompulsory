using Microsoft.EntityFrameworkCore;
namespace Indexer.DbContext;

public class Email
{
    public int EmailId { get; set; } 
    public string EmailName { get; set; }
    public string EmailContent { get; set; }

      
    public ICollection<Occurrence> Occurrences { get; set; } 
}

public class Word
{
    public int WordId { get; set; }
    public string WordValue { get; set; }
        
    public ICollection<Occurrence> Occurrences { get; set; } 
}
    
public class Occurrence
{
    public int OccurrenceId { get; set; }  
        
    public int WordId { get; set; }  
    public Word Word { get; set; }   
    public int EmailId { get; set; }  
    public Email Email { get; set; }  

    public int Count { get; set; } 
}

public class DoLsDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DoLsDbContext(DbContextOptions<DoLsDbContext> options)
        : base(options)
    { }

    public DbSet<Email> Emails { get; set; }
    public DbSet<Word> Words { get; set; }
    public DbSet<Occurrence> Occurrences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Occurrence>()
            .HasOne(o => o.Word)
            .WithMany(w => w.Occurrences)
            .HasForeignKey(o => o.WordId);

        modelBuilder.Entity<Occurrence>()
            .HasOne(o => o.Email)
            .WithMany(f => f.Occurrences)
            .HasForeignKey(o => o.EmailId);

        base.OnModelCreating(modelBuilder);
    }
}
