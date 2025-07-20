namespace BeQuiz.Models.ViewModels
{
    public class EditAnswersViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionContent { get; set; }

        public List<AnswerItem> Answers { get; set; } = new();

        public int CorrectAnswerId { get; set; }

        public class AnswerItem
        {
            public int AnswerId { get; set; } // = 0 jeśli nowa odpowiedź
            public string Content { get; set; }
        }
    }


}
