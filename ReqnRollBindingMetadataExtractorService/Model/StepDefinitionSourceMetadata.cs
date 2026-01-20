namespace ReqnRollBindingMetadataExtractorService.Model;

public record StepDefinitionSourceMetadata
{
    public required string Assembly { get; set; }
    public required string ClassName { get; set; }
    public required string ClassFullName { get; set; }
    public required string MethodName { get; set; }
}