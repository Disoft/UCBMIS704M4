namespace MIS704.Infrastructure;
// Diap. 77: Pipeline: transformaciones composables.
public static class StudentImportPipeline
{
    public static IReadOnlyList<string> Import(IEnumerable<string> rows) => rows
        .Select(x => x.Trim())
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Select(x => x.ToUpperInvariant())
        .Distinct()
        .ToList();
}
// Diap. 78: Microkernel: contrato estable + extensiones.
public interface IReportPlugin { string Name { get; } string Render(string input); }
public sealed class TextReportPlugin : IReportPlugin
{
    public string Name => "TXT";
    public string Render(string input) => $"Report: {input}";
}
public sealed class ReportHost
{
    private readonly IReadOnlyDictionary<string, IReportPlugin> _plugins;
    public ReportHost(IEnumerable<IReportPlugin> plugins)
        => _plugins = plugins.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
    public string Generate(string format, string input)
        => _plugins.TryGetValue(format, out var plugin) ? plugin.Render(input) : throw new NotSupportedException(format);
}
