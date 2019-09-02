using System.Diagnostics.CodeAnalysis;

namespace Directory.Data {
    [ExcludeFromCodeCoverage]
    public partial class InactiveBrother {
        public int Id { get; set; }
        public string Reason { get; set; }

        public virtual Brother IdNavigation { get; set; }
    }
}
