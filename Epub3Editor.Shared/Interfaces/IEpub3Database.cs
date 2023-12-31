using Epub3Editor.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Epub3Editor.Shared.Interfaces;

public interface IEpub3Database
{
    public DbSet<Culture> Culture { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<TranslatedProject> TranslatedProjects { get; set; }
    public DbSet<ProjectAuthor> ProjectAuthors { get; set; }
    public DbSet<Block> Blocks { get; set; }
    public DbSet<TranslatedBlock> TranslatedBlocks { get; set; }
}