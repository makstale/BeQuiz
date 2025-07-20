namespace BeQuiz.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }

        public int QuizResultId { get; set; }
        public QuizResult QuizResult { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public int SelectedAnswerId { get; set; }
        public Answer SelectedAnswer { get; set; }
    }
}
