using Epub3Editor.Shared.Interfaces;
using Epub3Editor.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace EpubEditor.System.Tests;

public class SqliteDbContext : DbContext, IEpub3Database
{
    public SqliteDbContext(DbContextOptions<SqliteDbContext> connBuilderOptions)  : base(connBuilderOptions)
    {
    }

    public DbSet<Culture> Culture { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<TranslatedProject> TranslatedProjects { get; set; }
    public DbSet<ProjectAuthor> ProjectAuthors { get; set; }
    public DbSet<Block> Blocks { get; set; }
    public DbSet<TranslatedBlock> TranslatedBlocks { get; set; }
}