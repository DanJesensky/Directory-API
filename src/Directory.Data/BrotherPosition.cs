using System;
using System.Diagnostics.CodeAnalysis;

namespace Directory.Data {
    [ExcludeFromCodeCoverage]
    public partial class BrotherPosition {
        public int BrotherId { get; set; }
        public int PositionId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public virtual Brother Brother { get; set; }
        public virtual Position Position { get; set; }
    }
}
