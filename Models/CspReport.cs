namespace CspReportMiddleware.Models
{
    public class CspReportWrapper
    {
        public CspReport? CspReport { get; set; }
    }

    public class CspReport
    {
        public string? DocumentUri { get; set; }
        public string? Referrer { get; set; }
        public string? BlockedUri { get; set; }
        public string? ViolatedDirective { get; set; }
        public string? OriginalPolicy { get; set; }
    }
}
