using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Directory.Data.Models {
    [ExcludeFromCodeCoverage]
    public class BrotherDetailModel {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ChapterDesignation { get; set; }
        public int? ZetaNumber { get; set; }
        public DateTime? DateJoined { get; set; }
        public DateTime? DateInitiated { get; set; }
        public DateTime? ExpectedGraduation { get; set; }
        public RelatedBrotherModel? BigBrother { get; set; }
        public IEnumerable<AnswerModel> Questions { get; set; }
        public IEnumerable<PositionHeldModel> Positions { get; set; }
        public IEnumerable<BrotherStudyModel> Majors { get; set; }
        public IEnumerable<BrotherStudyModel> Minors { get; set; }
        public IEnumerable<RelatedBrotherModel> LittleBrothers { get; set; }
        public bool Visible { get; set; }
    }
}
