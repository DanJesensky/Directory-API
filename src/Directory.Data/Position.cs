using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Directory.Data {
    [ExcludeFromCodeCoverage]
    public partial class Position {
        public Position() {
            BrotherPosition = new HashSet<BrotherPosition>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<BrotherPosition> BrotherPosition { get; set; }
    }
}
