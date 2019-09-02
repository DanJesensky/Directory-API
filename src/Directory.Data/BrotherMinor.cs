using System.Diagnostics.CodeAnalysis;

namespace Directory.Data {
    [ExcludeFromCodeCoverage]
    public partial class BrotherMinor {
        public int BrotherId { get; set; }
        public int MinorId { get; set; }

        public virtual Brother Brother { get; set; }
        public virtual Minor Minor { get; set; }
    }
}
