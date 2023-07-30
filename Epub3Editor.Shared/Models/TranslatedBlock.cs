using Epub3Editor.Shared.Enums;

namespace Epub3Editor.Shared.Models;

public class TranslatedBlock
{
    public int Id { get; set; }
    public virtual Block Block { get; set; }
    public virtual Culture Culture { get; set; }
    public TranslationStatusEnum TranslationStatus { get; set; }
}