using Directory.Data.Models;

namespace Directory.Data {
    public partial class BrotherPosition {
        public PositionHeldModel ToPositionHeldModel() =>
            new PositionHeldModel {
                Id = Position.Id,
                Name = Position.Name,
                HeldFrom = Start,
                HeldTo = End
            };
    }
}
