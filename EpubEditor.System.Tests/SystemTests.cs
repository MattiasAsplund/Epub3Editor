using Epub3Editor.Shared.Enums;
using Epub3Editor.Shared.Interfaces;
using Epub3Editor.xSystem;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace EpubEditor.System.Tests;

public class SystemTests
{
    [Fact]
    public void HappyPath()
    {
        var conn = new SqliteConnection("Data Source=:memory:");
        conn.Open();
        var connBuilder = new DbContextOptionsBuilder<SqliteDbContext>();
        connBuilder.UseSqlite(conn);
        SqliteDbContext sqliteConn = new SqliteDbContext(connBuilder.Options);
        sqliteConn.Database.EnsureCreated();
        var sut = new Epub3System(sqliteConn);
        var swedishCultureGuid = Guid.NewGuid();
        sut.CreateCulture(swedishCultureGuid, "sv-SE");
        Guid projectGuid = Guid.NewGuid();
        var projectName = "Test book";
        var projectId = sut.CreateProject(projectGuid, projectName, "A nice book with cool content", new List<string>() { "Mattias" });
        Guid swedishBookTranslationGuid = Guid.NewGuid();
        sut.CreateProjectTranslation(swedishBookTranslationGuid, projectGuid, swedishCultureGuid, "Testbok", "An trevlig bok med ballt innehåll", new List<string>() { "Mattias" });
        Guid backgroundGuid = Guid.NewGuid();
        var backgroundId = sut.CreateBlock(backgroundGuid, projectId, "Background", "# Background\n\nFrom the beginning, there was a need.", 1);
        Guid introductionGuid = Guid.NewGuid();
        var introductionId = sut.CreateBlock(introductionGuid, projectId, "Introduction", "# Introduction\n\nThe system fulfills that need.", 2);
        sut.MoveBlock(introductionId, MoveBlockEnum.Up);
        var book = sut.GetBook(projectGuid);
        var bookStream = File.Create($"{projectName}.zip");
        book.Seek(0, SeekOrigin.Begin);
        book.CopyTo(bookStream);
        bookStream.Flush();
        bookStream.Close();
    }
}