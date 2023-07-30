using Epub3Editor.Shared.Enums;

namespace Epub3Editor.Shared.Models;

public class TranslatedProject
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public virtual Project Project { get; set; }
    public virtual Culture Culture { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public TranslationStatusEnum TranslationStatus { get; set; }
}