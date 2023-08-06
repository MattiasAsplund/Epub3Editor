using System.ComponentModel.DataAnnotations;

namespace Epub3Editor.Shared.Requests;

public class CreateBlockRequest
{
    public int? Id { get; set; }
    [Required]
    public Guid Guid { get; set; }
    [Required]
    public int ProjectId { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Content { get; set; }
    public int Sort { get; set; }
}