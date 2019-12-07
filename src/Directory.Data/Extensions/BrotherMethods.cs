using System.Collections;
using System.Collections.Generic;
using Directory.Data.Models;
using System.Linq;

namespace Directory.Data {
    public partial class Brother {
        public BrotherDetailModel ToBrotherDetailModel() =>
            new BrotherDetailModel {
                Id = Id,
                ZetaNumber = ZetaNumber,
                FirstName = FirstName,
                LastName = LastName,
                DateJoined = DateJoined,
                DateInitiated = DateInitiated,
                ExpectedGraduation = ExpectedGraduation,
                ChapterDesignation = ChapterDesignation,
                Majors = BrotherMajor.Select(major => major.ToBrotherStudyModel()),
                Minors = BrotherMinor.Select(minor => minor.ToBrotherStudyModel()),
                Questions = Answer.Select(answer => answer.ToAnswerModel()),
                Positions = BrotherPosition.Select(position => position.ToPositionHeldModel()),
                Visible = InactiveBrother == null
            };

        public RelatedBrotherModel ToRelatedBrotherModel() =>
            new RelatedBrotherModel {
                Id = Id,
                Name = $"{FirstName} {LastName}",
                // If an InactiveBrother record exists, the link should not be visible.
                // Alternatively, if the DateJoined is null, that means it's a dummy record just to show the name.
                Visible = InactiveBrother == null && DateJoined != null
            };
    }
}
