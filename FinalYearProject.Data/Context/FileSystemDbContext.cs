using FinalYearProject.Data.Domain.Config;
using FinalYearProject.Data.Domain.Entities;
using FinalYearProject.Data.Domain.Entities.Attributes;
using Microsoft.EntityFrameworkCore;
using Attribute = FinalYearProject.Data.Domain.Entities.Attributes.Attribute;


namespace FinalYearProject.Data.Context;

public class FileSystemDbContext(DbContextOptions<FileSystemDbContext> options, FileSystemConfig config)
: DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Upload> Uploads { get; set; }
    public DbSet<Policy> Policies { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Attribute> Attributes { get; set; }
    public DbSet<UserAttribute> Users_Attributes { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

  
    }

}
