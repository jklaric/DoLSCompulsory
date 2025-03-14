using Microsoft.EntityFrameworkCore;
namespace Indexer.DbContext;

public class Email
{
    public int emailid { get; set; } 
    public string emailname { get; set; }
    public string emailcontent { get; set; }

      
    public ICollection<Occurrence> Occurrences { get; set; } 
}

public class Word
{
    public int wordid { get; set; }
    public string wordvalue { get; set; }
        
    public ICollection<Occurrence> Occurrences { get; set; } 
}
    
public class Occurrence
{
    public int occurrenceid { get; set; }  
        
    public int wordid { get; set; }  
    public Word word { get; set; }   
    public int emailid { get; set; }  
    public Email email { get; set; }  

    public int count { get; set; } 
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
        modelBuilder.Entity<Email>().ToTable("emails");
        modelBuilder.Entity<Word>().ToTable("words");
        modelBuilder.Entity<Occurrence>().ToTable("occurrences");
        
        modelBuilder.Entity<Occurrence>()
            .HasOne(o => o.word)
            .WithMany(w => w.Occurrences)
            .HasForeignKey(o => o.wordid);

        modelBuilder.Entity<Occurrence>()
            .HasOne(o => o.email)
            .WithMany(f => f.Occurrences)
            .HasForeignKey(o => o.emailid);

        base.OnModelCreating(modelBuilder);
    }
}
