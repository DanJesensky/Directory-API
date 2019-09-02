using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Directory.Data {
    [ExcludeFromCodeCoverage]
    public partial class Extracurricular {
        public Extracurricular() {
            BrotherExtracurricular = new HashSet<BrotherExtracurricular>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BrotherExtracurricular> BrotherExtracurricular { get; set; }
    }
}
