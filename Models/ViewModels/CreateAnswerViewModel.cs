namespace BeQuiz.Models.ViewModels
{
	public class CreateAnswerViewModel
	{
		public int QuestionId { get; set; }

		public List<AnswerInputModel> Answers { get; set; } = new();

		public int CorrectAnswerIndex { get; set; }

		public class AnswerInputModel
		{
			public string Content { get; set; }
		}
	}
}
