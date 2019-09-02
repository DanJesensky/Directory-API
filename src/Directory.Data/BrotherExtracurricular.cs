using System.Diagnostics.CodeAnalysis;

namespace Directory.Data {
    [ExcludeFromCodeCoverage]
    public partial class BrotherExtracurricular {
        public int BrotherId { get; set; }
        public int ExtracurricularId { get; set; }

        public virtual Brother Brother { get; set; }
        public virtual Extracurricular Extracurricular { get; set; }
    }
}
