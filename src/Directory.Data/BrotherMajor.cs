using System.Diagnostics.CodeAnalysis;

namespace Directory.Data {
    [ExcludeFromCodeCoverage]
    public partial class BrotherMajor {
        public int BrotherId { get; set; }
        public int MajorId { get; set; }

        public virtual Brother Brother { get; set; }
        public virtual Major Major { get; set; }
    }
}
