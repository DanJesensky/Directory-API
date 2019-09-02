using System.Diagnostics.CodeAnalysis;

namespace Directory.Data {
    [ExcludeFromCodeCoverage]
    public partial class Answer {
        public int QuestionId { get; set; }
        public int BrotherId { get; set; }
        public string AnswerText { get; set; }

        public virtual Brother Brother { get; set; }
        public virtual Question Question { get; set; }
    }
}
