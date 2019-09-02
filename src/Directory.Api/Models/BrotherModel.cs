using System;
using System.Diagnostics.CodeAnalysis;
using Directory.Data;

namespace Directory.Api.Models {
    [ExcludeFromCodeCoverage]
    public class BrotherModel {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateJoined { get; set; }
        public DateTime? DateInitiated { get; set; }
        public DateTime ExpectedGraduation { get; set; }
        public int? ZetaNumber { get; set; }
        public int BigBrotherId { get; set; }
        public Question[] Question { get; set; }
        public Extracurricular[] Extracurricular { get; set; }
        public Major[] Major { get; set; }
        public Minor[] Minor { get; set; }
        public Position[] Position { get; set; }
    }
}
