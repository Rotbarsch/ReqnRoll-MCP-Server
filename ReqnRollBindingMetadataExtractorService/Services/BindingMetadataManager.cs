using ReqnRollBindingMetadataExtractorService.Model;

namespace ReqnRollBindingMetadataExtractorService.Services;

public static class BindingMetadataManager
{
    public static List<BindingMetadata> GetAll(string currentWorkingDirectory)
    {
        var result = new List<BindingMetadata>();
        var filePairs = GetFilePairs(currentWorkingDirectory);

        foreach (var fp in filePairs)
        {
            using var service = new BindingMetadataExtractorService(fp.DllFile, fp.XmlFile);
            var metadata = service.LoadMetadata();
            result.AddRange(metadata);
        }

        return result;
    }

    private static List<(string DllFile, string XmlFile)> GetFilePairs(string currentWorkingDirectory)
    {
        var result = new List<(string DllFile, string XmlFile)>();

        var dllFiles = Directory.EnumerateFiles(currentWorkingDirectory, "*", SearchOption.AllDirectories)
            .Where(x => x.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            .DistinctBy(Path.GetFileNameWithoutExtension)
            .ToArray();

        foreach (var dll in dllFiles)
        {
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(dll);
            var dir = Path.GetDirectoryName(dll);
            var possibleXmlPath = Path.Join(dir, nameWithoutExtension + ".xml");
            result.Add((dll, possibleXmlPath));
        }

        return result;
    }
}