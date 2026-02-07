namespace ReqnRollBindingMetadataExtractorService.Model;

public record BindingMetadata
{
    public required BindingSourceMetadata Source { get; set; }
    public required string StepType { get; set; }
    public required string Expression { get; set; }
    public required string ExpressionType { get; set; }
    public required string Description { get; set; }
    public required List<BindingSourceParameterInfo> Parameters { get; set; }
}