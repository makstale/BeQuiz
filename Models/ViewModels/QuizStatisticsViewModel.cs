namespace BeQuiz.Models.ViewModels
{
    public class QuizStatisticsViewModel
    {
        public string QuizTitle { get; set; }
        public int TotalAttempts { get; set; }
        public double AverageScore { get; set; }
        public List<QuestionStatsViewModel> QuestionStats { get; set; }
    }

    public class QuestionStatsViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionContent { get; set; }
        public List<AnswerStatsViewModel> Answers { get; set; }
    }

    public class AnswerStatsViewModel
    {
        public int AnswerId { get; set; }
        public string AnswerContent { get; set; }
        public double SelectedPercentage { get; set; }
    }

}
