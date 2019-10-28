using System;
using System.Diagnostics.CodeAnalysis;

namespace Directory.Data.Models {
    [ExcludeFromCodeCoverage]
    public class PositionHeldModel {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime HeldFrom { get; set; }
        public DateTime HeldTo { get; set; }
    }
}
