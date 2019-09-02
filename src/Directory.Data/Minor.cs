using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Directory.Data {
    [ExcludeFromCodeCoverage]
    public partial class Minor {
        public Minor() {
            BrotherMinor = new HashSet<BrotherMinor>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BrotherMinor> BrotherMinor { get; set; }
    }
}
