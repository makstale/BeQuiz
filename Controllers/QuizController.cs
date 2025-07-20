using BeQuiz.Data;
using BeQuiz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeQuiz.Controllers
{
    public class QuizController
    {
        [Authorize]
        public class QuizController : Controller
        {
            private readonly ApplicationDbContext _context;
            private readonly UserManager<ApplicationUser> _userManager;

            public QuizController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            {
                _context = context;
                _userManager = userManager;
            }

            // GET
            public async Task<IActionResult> Index()
            {
                var quizzes = await _context.Quizzes
                    .Include(q => q.Creator)
                    .ToListAsync();

                return View(quizzes);
            }

            // GET
            public async Task<IActionResult> MyQuizzes()
            {
                var userId = _userManager.GetUserId(User);
                var quizzes = await _context.Quizzes
                    .Where(q => q.CreatorId == userId)
                    .ToListAsync();

                return View(quizzes);
            }

            // GET
            public IActionResult Create()
            {
                return View();
            }

            // POST
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(Quiz quiz)
            {
                if (ModelState.IsValid)
                {
                    quiz.CreatorId = _userManager.GetUserId(User);
                    _context.Quizzes.Add(quiz);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(MyQuizzes));
                }
                return View(quiz);
            }

            // GET
            public async Task<IActionResult> Details(int id)
            {
                var quiz = await _context.Quizzes
                    .Include(q => q.Questions)
                        .ThenInclude(q => q.Answers)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (quiz == null)
                {
                    return NotFound();
                }

                return View(quiz);
            }
        }
        public async Task<IActionResult> Solve(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
                return NotFound();

            return View(quiz);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int quizId, [FromForm] Dictionary<int, int> answers)
        {
            var userId = _userManager.GetUserId(User);

            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null) return NotFound();

            int totalScore = 0;
            var result = new QuizResult
            {
                QuizId = quiz.Id,
                UserId = userId,
                CompletedAt = DateTime.UtcNow,
                UserAnswers = new List<UserAnswer>()
            };

            foreach (var question in quiz.Questions)
            {
                if (answers.TryGetValue(question.Id, out int selectedAnswerId))
                {
                    bool isCorrect = selectedAnswerId == question.CorrectAnswerId;
                    if (isCorrect)
                        totalScore += question.Points;

                    result.UserAnswers.Add(new UserAnswer
                    {
                        QuestionId = question.Id,
                        SelectedAnswerId = selectedAnswerId
                    });
                }
            }

            result.TotalScore = totalScore;

            _context.QuizResults.Add(result);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Result), new { id = result.Id });
        }
        public async Task<IActionResult> Result(int id)
        {
            var result = await _context.QuizResults
                .Include(r => r.Quiz)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (result == null || result.UserId != _userManager.GetUserId(User))
                return NotFound();

            return View(result);
        }
		[HttpGet]
		public IActionResult CreateQuestion(int quizId)
		{
			var question = new Question { QuizId = quizId };
			return View(question);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateQuestion(Question question)
		{
			if (ModelState.IsValid)
			{
				_context.Questions.Add(question);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(CreateAnswer), new { questionId = question.Id });
			}

			return View(question);
		}
		[HttpGet]
		public IActionResult CreateAnswer(int questionId)
		{
			var model = new CreateAnswerViewModel { QuestionId = questionId };
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateAnswer(CreateAnswerViewModel model)
		{
			if (ModelState.IsValid)
			{
				var question = await _context.Questions
					.Include(q => q.Answers)
					.FirstOrDefaultAsync(q => q.Id == model.QuestionId);

				if (question == null)
					return NotFound();

				var answers = model.Answers.Select(a => new Answer
				{
					QuestionId = model.QuestionId,
					Content = a.Content
				}).ToList();

				_context.Answers.AddRange(answers);
				await _context.SaveChangesAsync();

				// Ustawiamy poprawną odpowiedź
				if (model.CorrectAnswerIndex >= 0 && model.CorrectAnswerIndex < answers.Count)
				{
					question.CorrectAnswerId = answers[model.CorrectAnswerIndex].Id;
					await _context.SaveChangesAsync();
				}

				return RedirectToAction(nameof(Details), new { id = question.QuizId });
			}

			return View(model);
		}
        public async Task<IActionResult> MyQuizzes()
        {
            var userId = _userManager.GetUserId(User);
            var quizzes = await _context.Quizzes
                .Where(q => q.CreatorId == userId)
                .ToListAsync();

            return View(quizzes);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null || quiz.CreatorId != userId)
                return Forbid();

            return View(quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Quiz updatedQuiz)
        {
            var userId = _userManager.GetUserId(User);
            var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == updatedQuiz.Id);

            if (quiz == null || quiz.CreatorId != userId)
                return Forbid();

            if (!ModelState.IsValid)
                return View(updatedQuiz);

            quiz.Title = updatedQuiz.Title;
            quiz.Description = updatedQuiz.Description;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyQuizzes));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null || quiz.CreatorId != userId)
                return Forbid();

            return View(quiz);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null || quiz.CreatorId != userId)
                return Forbid();

            _context.Answers.RemoveRange(quiz.Questions.SelectMany(q => q.Answers));
            _context.Questions.RemoveRange(quiz.Questions);
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyQuizzes));
        }

    }
}
