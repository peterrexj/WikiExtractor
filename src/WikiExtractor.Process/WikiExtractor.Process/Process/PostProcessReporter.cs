using Pj.Library;
using WikiExtractor.ViewModels;

namespace WikiExtractor.Process
{
    public class PostProcessReporter
    {
        private readonly WikiAppController _ctrl;
        private readonly string _reportFolder;
        private readonly string _runLabel;

        public PostProcessReporter(WikiAppController ctrl, string dbPath, string reportFolder)
        {
            _ctrl = ctrl;
            _runLabel = Path.GetFileNameWithoutExtension(dbPath);
            _reportFolder = reportFolder;
            Directory.CreateDirectory(_reportFolder);
        }

        public void Write()
        {
            var timestamp = $"{DateTime.Now:yyyyMMdd_HHmmss}";
            var path = Path.Combine(_reportFolder, $"postprocess_{_runLabel}_{timestamp}.csv");

            var items = _ctrl.GetListOfWikiItems().ToList();
            Console.WriteLine($"[POST-PROCESS REPORT] Building CSV for {items.Count} items…");

            var rows = new List<PostProcessReportRow>();

            foreach (var item in items)
            {
                PersonaViewModel full;
                try { full = _ctrl.GetViewModelByIdAsync(item.Id).GetAwaiter().GetResult(); }
                catch (Exception ex)
                {
                    Console.WriteLine($"  [POST-PROCESS REPORT WARN] Could not load '{item.Name}': {ex.Message}");
                    continue;
                }
                if (full == null) continue;

                var hasPrimaryImage = full.PicturePrimaryPath.HasValue()
                    && !full.PicturePrimaryPath.Contains("NoImageAvailable", StringComparison.OrdinalIgnoreCase);

                var textBuf = new System.Text.StringBuilder();
                if (full.MainContent.HasValue()) textBuf.Append(full.MainContent).Append(' ');
                foreach (var p2 in full.Paragraphs ?? new List<Paragraph2ContentViewModel>())
                {
                    if (p2.Content.HasValue()) textBuf.Append(p2.Content).Append(' ');
                    foreach (var container in p2.Para3Containers ?? new List<Paragraph3ContainerViewModel>())
                        foreach (var p3 in container.Para3s ?? new List<Paragraph3ContentViewModel>())
                            if (p3.Content.HasValue()) textBuf.Append(p3.Content).Append(' ');
                }

                var wordCount = textBuf.Length > 0
                    ? textBuf.ToString().Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length
                    : 0;

                var paragraphs = full.Paragraphs ?? new List<Paragraph2ContentViewModel>();
                var totalContent = paragraphs.Sum(p =>
                    (p.Content.HasValue() ? 1 : 0) +
                    (p.Para3Containers ?? new List<Paragraph3ContainerViewModel>())
                        .Sum(c => (c.Para3s ?? new List<Paragraph3ContentViewModel>())
                            .Count(p3 => p3.Content.HasValue())));

                rows.Add(new PostProcessReportRow
                {
                    ItemId              = full.Id,
                    ItemName            = full.Name ?? string.Empty,
                    HasPrimaryImageLink = hasPrimaryImage,
                    PrimaryImageLink    = full.PicturePrimaryPath ?? string.Empty,
                    HasPrimaryPara      = full.MainContent.HasValue(),
                    PrimaryParaData     = (full.MainContent ?? string.Empty).Replace('\n', ' ').Replace('\r', ' ').Trim(),
                    TotalH1             = full.Name.HasValue() ? 1 : 0,
                    TotalH2             = paragraphs.Count,
                    TotalContent        = totalContent,
                    TotalWordCount      = wordCount,
                    TotalImages         = (full.Pictures ?? new List<PictureViewModel>()).Count,
                });
            }

            CsvHelperEx.WriteToCsv(rows, path, hasHeaderRecords: true);
            Console.WriteLine($"[POST-PROCESS REPORT] {rows.Count} rows → {path}");
        }
    }

    public class PostProcessReportRow
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public bool HasPrimaryImageLink { get; set; }
        public string PrimaryImageLink { get; set; } = string.Empty;
        public bool HasPrimaryPara { get; set; }
        public string PrimaryParaData { get; set; } = string.Empty;
        public int TotalH1 { get; set; }
        public int TotalH2 { get; set; }
        public int TotalContent { get; set; }
        public int TotalWordCount { get; set; }
        public int TotalImages { get; set; }
    }
}
