namespace Epub3Editor.Shared.Models;

public class Project
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Css { get; set; }
}