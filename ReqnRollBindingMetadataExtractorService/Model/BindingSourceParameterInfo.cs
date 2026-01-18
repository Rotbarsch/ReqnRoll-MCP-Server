namespace ReqnRollBindingMetadataExtractorService.Model;

public record BindingSourceParameterInfo
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string ParameterType { get; set; }
}