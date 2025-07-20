namespace BeQuiz.Models
{
    public class QuizResult
    {
        public int Id { get; set; }

        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int TotalScore { get; set; }
        public DateTime CompletedAt { get; set; }

        public ICollection<UserAnswer> UserAnswers { get; set; }
    }
}
