using System.Diagnostics.CodeAnalysis;

namespace Directory.Data.Models {
    [ExcludeFromCodeCoverage]
    public class RelatedBrotherModel {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Visible { get; set; }
    }
}
