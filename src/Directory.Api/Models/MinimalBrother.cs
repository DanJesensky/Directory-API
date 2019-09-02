using System;

namespace Directory.Api.Models {
    public class MinimalBrother {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateJoined { get; set; }
        public int? ZetaNumber { get; set; }
    }
}
