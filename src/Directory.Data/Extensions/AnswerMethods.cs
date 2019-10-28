using Directory.Data.Models;

namespace Directory.Data {
    public partial class Answer {
        public AnswerModel ToAnswerModel() =>
            new AnswerModel {
                QuestionId = Question.Id,
                Question = Question.QuestionText,
                Answer = AnswerText
            };
    }
}
