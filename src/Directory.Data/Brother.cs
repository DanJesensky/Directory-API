using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Directory.Data {
    [ExcludeFromCodeCoverage]
    public partial class Brother {
        public Brother() {
            Answer = new HashSet<Answer>();
            BrotherExtracurricular = new HashSet<BrotherExtracurricular>();
            BrotherMajor = new HashSet<BrotherMajor>();
            BrotherMinor = new HashSet<BrotherMinor>();
            BrotherPosition = new HashSet<BrotherPosition>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[]? Picture { get; set; }
        public DateTime? DateJoined { get; set; }
        public DateTime? DateInitiated { get; set; }
        public string ChapterDesignation { get; set; }
        public int? ZetaNumber { get; set; }
        public DateTime? ExpectedGraduation { get; set; }
        public int BigBrotherId { get; set; }

        public virtual InactiveBrother InactiveBrother { get; set; }
        public virtual ICollection<Answer> Answer { get; set; }
        public virtual ICollection<BrotherExtracurricular> BrotherExtracurricular { get; set; }
        public virtual ICollection<BrotherMajor> BrotherMajor { get; set; }
        public virtual ICollection<BrotherMinor> BrotherMinor { get; set; }
        public virtual ICollection<BrotherPosition> BrotherPosition { get; set; }
    }
}
