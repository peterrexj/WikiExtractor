using Pj.Library;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Text;
using WikiExtractor.Exts;
using WikiExtractor.Models;

namespace WikiExtractor.Process
{
    /// <summary>
    /// Generic extraction report writer. Works for any extractor (WorldLeaders, Saints, Popes, Countries…).
    /// Call WriteReports() after each extraction run. Image validation runs with a configurable per-request
    /// delay and appends its results to the success file.
    /// </summary>
    public class ExtractionReporter
    {
        public class ExtractionRecord
        {
            public WikiWhatToExtractModel Item { get; set; } = null!;
            public WikiPageModel? PageModel { get; set; }
            public List<MetaDataModel>? Metadatas { get; set; }
            public bool PageFetchFailed { get; set; }
            public int StoredMasterId { get; set; }

            // Optional label surfaced in reports — defaults to the first non-"All" tag if not set
            public string? GroupLabel { get; set; }
        }

        private readonly string _reportFolder;
        private readonly string _runLabel;

        public ExtractionReporter(string reportFolder, string runLabel)
        {
            _reportFolder = reportFolder;
            _runLabel = runLabel;
            Directory.CreateDirectory(_reportFolder);
        }

        // ─── Entry point ─────────────────────────────────────────────────────────

        public void WriteReports(List<ExtractionRecord> records, int imageValidationDelayMs = 500, bool skipImageValidation = false)
        {
            var timestamp = $"{DateTime.Now:yyyyMMdd_HHmmss}";
            var successPath = Path.Combine(_reportFolder, $"report_success_{_runLabel}_{timestamp}.txt");
            var failurePath = Path.Combine(_reportFolder, $"report_failure_{_runLabel}_{timestamp}.txt");

            var successRecords = records.Where(r => !IsFailure(r)).ToList();
            var failureRecords = records.Where(r =>  IsFailure(r)).ToList();

            WriteFailureReport(failurePath, failureRecords, records.Count);
            WriteSuccessReport(successPath, successRecords, records.Count);

            Console.WriteLine($"[REPORT] Success  → {successPath}");
            Console.WriteLine($"[REPORT] Failures → {failurePath}");

            if (!skipImageValidation && successRecords.Count > 0)
                ValidateImages(successPath, successRecords, imageValidationDelayMs);
            else if (skipImageValidation)
                Console.WriteLine("[REPORT] Image validation skipped.");
        }

        // ─── Failure report ───────────────────────────────────────────────────────

        private void WriteFailureReport(string path, List<ExtractionRecord> failures, int totalExtracted)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"EXTRACTION FAILURE REPORT — {_runLabel}");
            sb.AppendLine($"Generated      : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Total extracted: {totalExtracted}  |  Failures: {failures.Count}");
            sb.AppendLine(new string('=', 100));
            sb.AppendLine();

            if (failures.Count == 0)
            {
                sb.AppendLine("No failures detected.");
                File.WriteAllText(path, sb.ToString());
                return;
            }

            foreach (var r in failures.OrderBy(f => PrimaryTag(f)))
            {
                sb.AppendLine($"Item    : {r.Item.Title}");
                sb.AppendLine($"Group   : {PrimaryTag(r)}");
                sb.AppendLine($"Route   : {r.Item.Route}");

                var issues = BuildIssueList(r);
                sb.AppendLine($"Issues  : {string.Join(", ", issues)}");

                if (r.PageModel != null)
                {
                    var metaCount     = r.Metadatas?.Count(m => m.Type != MetadataType.Image) ?? 0;
                    var imgMetaCount  = r.Metadatas?.Count(m => m.Type == MetadataType.Image) ?? 0;
                    var h2Count       = r.PageModel.WikiParaCollection?.Count ?? 0;
                    var paraCount     = r.PageModel.WikiParaCollection?.Sum(p => p.ParagraghInternalModels.Count) ?? 0;
                    var inlineImg     = r.PageModel.WikiPictureCollection?.Count ?? 0;
                    sb.AppendLine($"Data    : metadata={metaCount}, meta-images={imgMetaCount}, H2={h2Count}, paragraphs={paraCount}, inline-images={inlineImg}");

                    var introCount = r.PageModel.MainParagraph?.Count ?? 0;
                    sb.AppendLine(introCount > 0
                        ? $"Intro   : {introCount} intro paragraph(s) found"
                        : "Intro   : no intro paragraph");
                }
                else
                {
                    sb.AppendLine("Data    : page model is null — page was not fetched or parsing failed entirely");
                }

                sb.AppendLine(new string('-', 80));
                sb.AppendLine();
            }

            File.WriteAllText(path, sb.ToString());
        }

        // ─── Success report ───────────────────────────────────────────────────────

        private void WriteSuccessReport(string path, List<ExtractionRecord> successes, int totalExtracted)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"EXTRACTION SUCCESS REPORT — {_runLabel}");
            sb.AppendLine($"Generated      : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Total extracted: {totalExtracted}  |  Succeeded: {successes.Count}");
            sb.AppendLine(new string('=', 130));
            sb.AppendLine();

            int totMeta = 0, totImg = 0, totH2 = 0, totH3 = 0, totPara = 0, totInlineImg = 0, totIntro = 0;

            sb.AppendLine($"  {"Item",-45} {"Group",-24} {"Meta",-6} {"Imgs",-6} {"H2",-5} {"H3",-5} {"Para",-6} {"InlineImg",-10} {"Intro"}");
            sb.AppendLine(new string('-', 130));

            foreach (var r in successes.OrderBy(f => PrimaryTag(f)).ThenBy(f => f.Item.Title))
            {
                var metaCount    = r.Metadatas?.Count(m => m.Type != MetadataType.Image) ?? 0;
                var imgMetaCount = r.Metadatas?.Count(m => m.Type == MetadataType.Image) ?? 0;
                var h2Count      = r.PageModel?.WikiParaCollection?.Count ?? 0;
                var h3Count      = r.PageModel?.WikiParaCollection?
                                     .Sum(p => p.ParagraghInternalModels
                                         .Select(d => d.SubHeader)
                                         .Where(s => s.HasValue())
                                         .Distinct().Count()) ?? 0;
                var paraCount    = r.PageModel?.WikiParaCollection?.Sum(p => p.ParagraghInternalModels.Count) ?? 0;
                var inlineImg    = r.PageModel?.WikiPictureCollection?.Count ?? 0;
                var introCount   = r.PageModel?.MainParagraph?.Count ?? 0;

                totMeta      += metaCount;
                totImg       += imgMetaCount;
                totH2        += h2Count;
                totH3        += h3Count;
                totPara      += paraCount;
                totInlineImg += inlineImg;
                totIntro     += introCount;

                sb.AppendLine($"  {r.Item.Title,-45} {PrimaryTag(r),-24} {metaCount,-6} {imgMetaCount,-6} {h2Count,-5} {h3Count,-5} {paraCount,-6} {inlineImg,-10} {introCount}");
            }

            sb.AppendLine(new string('-', 130));
            sb.AppendLine($"  {"TOTAL",-45} {"",-24} {totMeta,-6} {totImg,-6} {totH2,-5} {totH3,-5} {totPara,-6} {totInlineImg,-10} {totIntro}");
            sb.AppendLine();
            sb.AppendLine("Image validation will be appended below once the URL probe pass completes.");
            sb.AppendLine(new string('=', 130));
            sb.AppendLine();

            File.WriteAllText(path, sb.ToString());
        }

        // ─── Image validation ─────────────────────────────────────────────────────

        private void ValidateImages(string successPath, List<ExtractionRecord> successes, int minDomainGapMs)
        {
            Console.WriteLine($"[IMAGE VALIDATION] Starting URL probe (domain throttle: {minDomainGapMs}ms min gap per host)…");

            var results = new StringBuilder();
            results.AppendLine();
            results.AppendLine($"IMAGE VALIDATION RESULTS — {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            results.AppendLine(new string('-', 110));
            results.AppendLine($"  {"Item",-45} {"Group",-24} {"Status",-9} URL");
            results.AppendLine(new string('-', 110));

            int ok = 0, broken = 0, skipped = 0;
            // Tracks when the last request to each host completed (Environment.TickCount64 ms)
            var domainLastMs = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

            using var httpClient = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 5,
            });
            httpClient.Timeout = TimeSpan.FromSeconds(15);
            httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (compatible; WikiExtractorValidator/1.0)");

            foreach (var r in successes.OrderBy(f => PrimaryTag(f)).ThenBy(f => f.Item.Title))
            {
                var imageUrls = CollectImageUrls(r);

                if (imageUrls.Count == 0)
                {
                    results.AppendLine($"  {r.Item.Title,-45} {PrimaryTag(r),-24} {"SKIP",-9} no images");
                    skipped++;
                    continue;
                }

                bool anyBroken = false;
                foreach (var url in imageUrls)
                {
                    try
                    {
                        // Only sleep the remaining gap for this domain — if the last HTTP call
                        // already took longer than minDomainGapMs, no sleep is needed at all.
                        ThrottleForDomain(url, minDomainGapMs, domainLastMs);

                        var response = SendHead(httpClient, url);
                        domainLastMs[GetUrlHost(url)] = Environment.TickCount64;

                        // On 429, back off once and retry before marking broken
                        if ((int)response.StatusCode == 429)
                        {
                            var backoff = Math.Max(minDomainGapMs * 4, 4000);
                            Console.WriteLine($"  [429] {GetUrlHost(url)} — backing off {backoff}ms, retrying…");
                            Thread.Sleep(backoff);
                            response = SendHead(httpClient, url);
                            domainLastMs[GetUrlHost(url)] = Environment.TickCount64;
                        }

                        var code = (int)response.StatusCode;
                        if (response.IsSuccessStatusCode)
                        {
                            results.AppendLine($"  {r.Item.Title,-45} {PrimaryTag(r),-24} {$"HTTP {code}",-9} {url}");
                            ok++;
                        }
                        else
                        {
                            results.AppendLine($"  {r.Item.Title,-45} {PrimaryTag(r),-24} {"BROKEN",-9} [{code}] {url}");
                            broken++;
                            anyBroken = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        results.AppendLine($"  {r.Item.Title,-45} {PrimaryTag(r),-24} {"ERROR",-9} {ex.Message} | {url}");
                        broken++;
                        anyBroken = true;
                    }
                }

                Console.WriteLine(anyBroken
                    ? $"  [IMAGE WARN] {r.Item.Title} ({PrimaryTag(r)}) — one or more broken URLs"
                    : $"  [IMAGE OK  ] {r.Item.Title} ({PrimaryTag(r)})");
            }

            results.AppendLine(new string('-', 110));
            results.AppendLine($"SUMMARY: OK={ok}  BROKEN={broken}  SKIPPED={skipped}  TOTAL={ok + broken + skipped}");
            results.AppendLine(new string('=', 110));

            File.AppendAllText(successPath, results.ToString());
            Console.WriteLine($"[IMAGE VALIDATION] Done — OK={ok}, BROKEN={broken}, SKIPPED={skipped}");
            Console.WriteLine($"[IMAGE VALIDATION] Results appended to {successPath}");
        }

        private static System.Net.Http.HttpResponseMessage SendHead(HttpClient client, string url) =>
            client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)).GetAwaiter().GetResult();

        private static void ThrottleForDomain(string url, int minGapMs, Dictionary<string, long> domainLastMs)
        {
            if (minGapMs <= 0) return;
            var host = GetUrlHost(url);
            if (!domainLastMs.TryGetValue(host, out var lastMs)) return;
            var elapsed = (int)(Environment.TickCount64 - lastMs);
            var remaining = minGapMs - elapsed;
            if (remaining > 0)
                Thread.Sleep(remaining);
        }

        private static string GetUrlHost(string url)
        {
            try { return new Uri(url).Host; }
            catch { return url; }
        }

        // ─── Helpers ──────────────────────────────────────────────────────────────

        private static bool IsFailure(ExtractionRecord r)
        {
            if (r.PageFetchFailed) return true;
            if (r.PageModel == null) return true;
            bool hasMeta = r.Metadatas?.Any(m => m.Type != MetadataType.Image) == true;
            bool hasPara = r.PageModel.WikiParaCollection?.Count > 0;
            bool hasMain = r.PageModel.MainParagraph?.Count > 0;
            return !hasMeta && !hasPara && !hasMain;
        }

        private static List<string> BuildIssueList(ExtractionRecord r)
        {
            var issues = new List<string>();
            if (r.PageFetchFailed) { issues.Add("PAGE_FETCH_FAILED"); return issues; }
            if (r.PageModel == null) { issues.Add("NULL_PAGE_MODEL");  return issues; }

            bool hasMeta  = r.Metadatas?.Any(m => m.Type != MetadataType.Image) == true;
            bool hasImage = r.Metadatas?.Any(m => m.Type == MetadataType.Image) == true;
            bool hasPara  = r.PageModel.WikiParaCollection?.Count > 0;
            bool hasMain  = r.PageModel.MainParagraph?.Count > 0;

            if (!hasMeta)            issues.Add("NO_METADATA");
            if (!hasImage)           issues.Add("NO_IMAGES");
            if (!hasPara && !hasMain) issues.Add("NO_PARAGRAPH_CONTENT");
            if (!hasPara && hasMain)  issues.Add("INTRO_ONLY_NO_SECTIONS");
            if (!hasMeta && !hasPara && !hasMain) issues.Add("POSSIBLE_301_OR_EMPTY_PAGE");

            return issues;
        }

        private static List<string> CollectImageUrls(ExtractionRecord r)
        {
            var urls = new List<string>();

            if (r.Metadatas != null)
            {
                foreach (var m in r.Metadatas.Where(m => m.Type == MetadataType.Image))
                {
                    var db = m.ToImageDbModel();
                    if (db.Path.HasValue()) urls.Add(db.Path!);
                }
            }

            if (r.PageModel?.WikiPictureCollection != null)
            {
                foreach (var pic in r.PageModel.WikiPictureCollection)
                {
                    var db = pic.ToImageDbModel();
                    if (db.Path.HasValue()) urls.Add(db.Path!);
                }
            }

            return urls.Distinct().ToList();
        }

        private static string PrimaryTag(ExtractionRecord r) =>
            r.GroupLabel
            ?? r.Item.Tags?.FirstOrDefault(t => t != "All")
            ?? string.Empty;
    }
}
