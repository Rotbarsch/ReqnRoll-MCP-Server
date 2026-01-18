namespace ReqnRollBindingMetadataExtractorService.Model;

public record BindingSourceMetadata
{
    public required string Assembly { get; set; }
    public required string ClassName { get; set; }
    public required string MethodName { get; set; }
}