using System.IO.Compression;
using System.Text;
using System.Xml;
using Epub3Editor.Shared.Enums;
using Epub3Editor.Shared.Interfaces;
using Epub3Editor.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Epub3Editor.xSystem;

public class Epub3System
{
    private readonly IEpub3Database _database;

    public Epub3System(IEpub3Database database)
    {
        _database = database;
    }
    public void CreateProject(Guid guid, string bookName, string description, List<string>? authors)
    {
        var project = new Project
        {
            Guid = guid,
            Name = bookName,
            Description = description
        };
        _database.Projects.Add(project);
        authors.ForEach(author =>
        {
            _database.ProjectAuthors.Add(new ProjectAuthor { Project = project, Author = author });
        });
        ((DbContext)_database).SaveChanges();
    }

    public void CreateCulture(Guid cultureGuid, string code)
    {
        var culture = new Culture()
        {
            Guid = cultureGuid,
            Code = code
        };
        _database.Culture.Add(culture);
        ((DbContext)_database).SaveChanges();
    }

    public void CreateProjectTranslation(Guid swedishBookTranslationGuid, Guid bookGuid, Guid swedishCultureGuid, string testbok, string anTrevligBokMedBalltInnehåll, List<string> list)
    {
        var culture = _database.Culture.Single(c => c.Guid == swedishCultureGuid);
        var project = _database.Projects.Single(p => p.Guid == bookGuid);
        var translatedProject = new TranslatedProject
        {
            Guid = swedishBookTranslationGuid,
            Culture = culture,
            Project = project,
            Name = testbok,
            Description = anTrevligBokMedBalltInnehåll
        };
        _database.TranslatedProjects.Add(translatedProject);
        ((DbContext)_database).SaveChanges();
    }

    public void CreateBlock(Guid guid, Guid projectGuid, string content)
    {
        var project = _database.Projects.Single(p => p.Guid == projectGuid);
        var maxSort = _database.Blocks.Any() ? _database.Blocks.Max(b => b.Sort) : 1;
        var block = new Block
        {
            Guid = guid,
            Project = project,
            Content = content,
            Sort = maxSort + 1
        };
        _database.Blocks.Add(block);
        ((DbContext)_database).SaveChanges();
    }

    public void MoveBlock(Guid guid, MoveBlockEnum moveBlockEnum)
    {
        var block = _database.Blocks.Single(b => b.Guid == guid);
        var blocks = _database.Blocks.Where(b => b.Project.Id == block.Project.Id);
        Block? compareBlock;
        if (moveBlockEnum == MoveBlockEnum.Up)
        {
            compareBlock = blocks.OrderBy(b => b.Sort).LastOrDefault(b => b.Sort < block.Sort);
        }
        else
        {
            compareBlock = blocks.OrderByDescending(b => b.Sort).FirstOrDefault(b => b.Sort > block.Sort);
        }

        if (compareBlock != null)
        {
            var blockSort = block.Sort;
            block.Sort = compareBlock.Sort;
            compareBlock.Sort = blockSort;
            ((DbContext)_database).Update(compareBlock);
            ((DbContext)_database).Update(block);
            ((DbContext)_database).SaveChanges();
        }
    }

    public Stream GetBook(Guid projectGuid)
    {
        var project = _database.Projects.Single(p => p.Guid == projectGuid);
        
        var stm = new MemoryStream();
        using (var archive = new ZipArchive(stm, ZipArchiveMode.Create, true))
        {
            var entry = archive.CreateEntry("content.opf");
            var entryStm = entry.Open();
            var contentTw = new StreamWriter(entryStm);
            var contentXml = new XmlTextWriter(contentTw);
            contentXml.WriteStartDocument();
            contentXml.WriteStartElement("package");
            contentXml.WriteAttributeString("version", "3.0");
            contentXml.WriteAttributeString("unique-identifier", "BookId");
            contentXml.WriteAttributeString("xmlns", "http://www.idpf.org/2007/opf");
            
                // <metadata xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:opf="http://www.idpf.org/2007/opf">
                // <dc:identifier id="BookId">urn:uuid:3019ce2d-8731-4519-a032-3640e5c1eddc</dc:identifier>
                // <dc:language>en</dc:language>
                // <dc:title>[Main title here]</dc:title>
                // <meta property="dcterms:modified">2023-07-27T21:13:02Z</meta>
                // <meta content="1.9.2" name="Sigil version" />
                // </metadata>
                
            contentXml.WriteStartElement("metadata");
            
            contentXml.WriteAttributeString("dc", "http://purl.org/dc/elements/1.1/", "");
            contentXml.WriteAttributeString("opf", "xmlns", "http://www.idpf.org/2007/opf");
            
            contentXml.WriteStartElement("dc", "identifier", "xmlns");
            contentXml.WriteAttributeString("id", "BookId");
            contentXml.WriteString($"urn:{projectGuid}");
            
            contentXml.WriteStartElement("dc", "language", "http://purl.org/dc/elements/1.1/");
            contentXml.WriteString("en");
            contentXml.WriteEndElement();
            
            contentXml.WriteEndElement();
            contentXml.WriteEndElement();
            
            contentXml.WriteEndElement();
            contentXml.WriteEndDocument();
            contentTw.Flush();
            contentTw.Close();
            
            var textEntry = archive.CreateEntry("hello.txt");
            var textStm = textEntry.Open();
            var contentTextWriter = new StreamWriter(textStm);
            contentTextWriter.WriteLine("Hello world");
            contentTextWriter.Flush();
        }

        return stm;
    }
}