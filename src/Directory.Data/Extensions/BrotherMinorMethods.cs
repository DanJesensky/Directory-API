using Directory.Data.Models;

namespace Directory.Data {
    public partial class BrotherMinor {
        public BrotherStudyModel ToBrotherStudyModel() =>
            new BrotherStudyModel {
                Id = Minor.Id,
                Name = Minor.Name
            };
    }
}
