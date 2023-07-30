namespace Epub3Editor.Shared.Models;

public class Block
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public virtual Project Project { get; set; }
    public int Sort { get; set; }
    public string Content { get; set; }
}