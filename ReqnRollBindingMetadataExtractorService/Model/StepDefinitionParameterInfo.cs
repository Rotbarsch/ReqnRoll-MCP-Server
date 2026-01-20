namespace ReqnRollBindingMetadataExtractorService.Model;

public record StepDefinitionParameterInfo
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string ParameterType { get; set; }
}