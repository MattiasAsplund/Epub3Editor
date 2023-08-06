using System.IO.Compression;
using System.Reflection;
using System.Xml;
using Epub3Editor.Shared.Enums;
using Epub3Editor.Shared.Interfaces;
using Epub3Editor.Shared.Models;
using Markdig;
using Microsoft.EntityFrameworkCore;

namespace Epub3Editor.xSystem;

public class Epub3System
{
    private readonly IEpub3Database _database;

    public Epub3System(IEpub3Database database)
    {
        _database = database;
    }

    public int CreateProject(Guid guid, string bookName, string description, List<string>? authors)
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
        return project.Id;
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

    public void CreateProjectTranslation(Guid swedishBookTranslationGuid, Guid bookGuid, Guid swedishCultureGuid,
        string testbok, string anTrevligBokMedBalltInnehåll, List<string> list)
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

    public int CreateBlock(Guid guid, int projectId, string name, string content, int sort)
    {
        var project = _database.Projects.Single(p => p.Id == projectId);
        var maxSort = _database.Blocks.Any() ? _database.Blocks.Max(b => b.Sort) : 1;
        var block = new Block
        {
            Guid = guid,
            Project = project,
            Name = name,
            Content = content,
            Sort = maxSort + 1
        };
        _database.Blocks.Add(block);
        ((DbContext)_database).SaveChanges();
        return block.Id;
    }

    public void MoveBlock(int id, MoveBlockEnum moveBlockEnum)
    {
        var block = _database.Blocks.Single(b => b.Id == id);
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

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            var writer = XmlTextWriter.Create(contentTw, settings);

            // Write the XML document
            writer.WriteStartDocument();
            writer.WriteStartElement("package", "http://www.idpf.org/2007/opf");
            writer.WriteAttributeString("version", "3.0");
            writer.WriteAttributeString("unique-identifier", "BookId");
            writer.WriteAttributeString("xmlns", "http://www.idpf.org/2007/opf");

            // Write the metadata element
            writer.WriteStartElement("metadata", "http://www.idpf.org/2007/opf");
            writer.WriteAttributeString("xmlns", "dc", null, "http://purl.org/dc/elements/1.1/");
            writer.WriteAttributeString("xmlns", "opf", null, "http://www.idpf.org/2007/opf");

            writer.WriteStartElement("dc", "identifier", null);
            writer.WriteAttributeString("id", "BookId");
            writer.WriteString($"urn:uuid:{projectGuid}");
            writer.WriteEndElement(); // Close dc:identifier

            writer.WriteElementString("dc", "language", null, "en");
            writer.WriteElementString("dc", "title", null, $"{project.Name}");
            writer.WriteElementString("meta", null, "2023-07-27T21:13:02Z");
            writer.WriteStartElement("meta");
            writer.WriteAttributeString("name", "Epub3Editor");
            writer.WriteAttributeString("content", "Development version");
            writer.WriteEndElement(); // Close meta

            writer.WriteEndElement(); // Close metadata

            // Write the manifest element
            writer.WriteStartElement("manifest");

            var sortedItems = _database.Blocks.Where(b => b.Project.Guid == projectGuid).OrderBy(b => b.Sort);

            foreach (var sortedItem in sortedItems)
            {
                // Write the individual item elements
                WriteItemElement(writer, $"{sortedItem.Guid.ToString()}.xhtml", $"Text/{sortedItem.Guid.ToString()}.xhtml", "application/xhtml+xml");
            }

            writer.WriteEndElement(); // Close manifest

            // Write the spine element
            writer.WriteStartElement("spine");

            foreach (var sortedItem in sortedItems)
            {
                // Write the individual itemref elements
                WriteItemRefElement(writer, $"{sortedItem.Guid.ToString()}.xhtml");
            }
            
            writer.WriteEndElement(); // Close spine

            writer.WriteEndElement(); // Close package

            writer.WriteEndDocument();

            writer.Flush();
            writer.Close();

            contentTw.Flush();
            contentTw.Close();
            
            foreach (var sortedItem in sortedItems)
            {
                var textEntry = archive.CreateEntry($"Text/{sortedItem.Guid.ToString()}.xhtml");
                var textStm = textEntry.Open();
                var contentTextWriter = new StreamWriter(textStm);
                var content = GetEntryTemplate();
                var newContent = content.Replace("@Body", Markdown.ToHtml(sortedItem.Content));
                contentTextWriter.WriteLine(newContent);
                contentTextWriter.Flush();
                textStm.Flush();
                textStm.Close();
            }
        }

        return stm;
    }

    // Helper method to write the item elements
    static void WriteItemElement(XmlWriter writer, string id, string href, string mediaType, string properties = null)
    {
        writer.WriteStartElement("item");
        writer.WriteAttributeString("id", id);
        writer.WriteAttributeString("href", href);
        writer.WriteAttributeString("media-type", mediaType);
        if (properties != null)
            writer.WriteAttributeString("properties", properties);
        writer.WriteEndElement();
    }

// Helper method to write the itemref elements
    static void WriteItemRefElement(XmlWriter writer, string idref, string linear = null)
    {
        writer.WriteStartElement("itemref");
        writer.WriteAttributeString("idref", idref);
        if (linear != null)
            writer.WriteAttributeString("linear", linear);
        writer.WriteEndElement();
    }

    static string GetEntryTemplate()
    {
        string resourceName = "Epub3Editor.xSystem.Templates.entry.xhtml";

        // Get the assembly containing the embedded resource (usually the executing assembly).
        Assembly assembly = Assembly.GetExecutingAssembly();

        string content;

        // Read the embedded resource stream.
        using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
        {
            if (resourceStream == null)
            {
                Console.WriteLine("Embedded resource not found.");
                throw new FileNotFoundException();
            }

            using (StreamReader reader = new StreamReader(resourceStream))
            {
                // Read and display the contents of the embedded resource.
                content = reader.ReadToEnd();
            }
        }

        return content;
    }

    public IEnumerable<Project> GetProjects()
    {
        return _database.Projects.OrderBy(p => p.Name);
    }
    
    public Project GetProject(int id)
    {
        return _database.Projects.Single(p => p.Id == id);
    }

    public void UpdateProject(int id, string name, string description, string css)
    {
        var project = _database.Projects.Single(p => p.Id == id);
        project.Name = name;
        project.Description = description;
        project.Css = css;
        ((DbContext)_database).Update(project);
        ((DbContext)_database).SaveChanges();
    }

    public IEnumerable<Block> GetBlocks(int id)
    {
        return _database.Blocks.OrderBy(b => b.Sort).Where(b => b.Project.Id == id);
    }
}
