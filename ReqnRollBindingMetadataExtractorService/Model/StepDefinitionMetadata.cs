namespace ReqnRollBindingMetadataExtractorService.Model;

public record BindingMetadata
{
    public required List<BindingClassMetadata> BindingClasses { get; set; }
    public required List<StepDefinitionMetadata> StepDefinitions { get; set; }
}

public record BindingClassMetadata
{
    public required string ClassName { get; set; }
    public required string ClassFullName { get; set; }
    public required string Assembly { get; set; }
    public required string Description { get; set; }
}

public record StepDefinitionMetadata
{
    public required StepDefinitionSourceMetadata Source { get; set; }
    public required string StepType { get; set; }
    public required string Pattern { get; set; }
    public required string PatternType { get; set; }
    public required string Description { get; set; }
    public required List<StepDefinitionParameterInfo> Parameters { get; set; }
}