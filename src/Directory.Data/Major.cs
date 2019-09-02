using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Directory.Data {
    [ExcludeFromCodeCoverage]
    public partial class Major {
        public Major() {
            BrotherMajor = new HashSet<BrotherMajor>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BrotherMajor> BrotherMajor { get; set; }
    }
}
