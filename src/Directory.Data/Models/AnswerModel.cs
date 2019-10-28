using System.Diagnostics.CodeAnalysis;

namespace Directory.Data.Models {
    [ExcludeFromCodeCoverage]
    public class AnswerModel {
        public int QuestionId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
