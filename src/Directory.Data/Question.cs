using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Directory.Data {
    [ExcludeFromCodeCoverage]
    public partial class Question {
        public Question() {
            Answer = new HashSet<Answer>();
        }

        public int Id { get; set; }
        public string QuestionText { get; set; }

        public virtual ICollection<Answer> Answer { get; set; }
    }
}
