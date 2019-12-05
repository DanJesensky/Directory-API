using Directory.Data.Models;

namespace Directory.Data {
    public partial class Answer {
        public AnswerModel ToAnswerModel() =>
            new AnswerModel {
                Id = Question.Id,
                Question = Question.QuestionText,
                Answer = AnswerText
            };
    }
}
