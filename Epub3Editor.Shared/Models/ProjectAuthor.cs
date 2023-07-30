namespace Epub3Editor.Shared.Models;

public class ProjectAuthor
{
    public int Id { get; set; }
    public virtual Project Project { get; set; }
    public string Author { get; set; }
}