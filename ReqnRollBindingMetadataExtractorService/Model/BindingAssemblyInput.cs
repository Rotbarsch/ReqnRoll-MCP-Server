using System.Text.Json.Serialization;

namespace ReqnRollBindingMetadataExtractorService.Model
{
    public record BindingAssemblyInput
    {
        [JsonPropertyName("dll")]
        public required string Dll { get; set; }

        [JsonPropertyName("xml")]
        public string? Xml { get; set; }
    }
}
