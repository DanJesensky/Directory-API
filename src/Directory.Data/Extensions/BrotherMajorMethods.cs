using Directory.Data.Models;

namespace Directory.Data {
    public partial class BrotherMajor {
        public BrotherStudyModel ToBrotherStudyModel() =>
            new BrotherStudyModel {
                Id = Major.Id,
                Name = Major.Name
            };
    }
}
