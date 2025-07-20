using BeQuiz.Data;
using BeQuiz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BeQuiz.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StatisticsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> QuizStats(int quizId)
        {
            var userId = _userManager.GetUserId(User);

            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null || quiz.CreatorId != userId)
                return NotFound();

            var results = await _context.QuizResults
                .Include(r => r.UserAnswers)
                .Where(r => r.QuizId == quizId)
                .ToListAsync();

            var totalAttempts = results.Count;
            var averageScore = results.Count > 0 ? results.Average(r => r.TotalScore) : 0;

            var questionStats = quiz.Questions.Select(q => new QuestionStatsViewModel
            {
                QuestionId = q.Id,
                QuestionContent = q.Content,
                Answers = q.Answers.Select(a => new AnswerStatsViewModel
                {
                    AnswerId = a.Id,
                    AnswerContent = a.Content,
                    SelectedPercentage = totalAttempts == 0 ? 0 :
                        results.SelectMany(r => r.UserAnswers)
                               .Count(ua => ua.QuestionId == q.Id && ua.SelectedAnswerId == a.Id) * 100.0 / totalAttempts
                }).ToList()
            }).ToList();

            var model = new QuizStatisticsViewModel
            {
                QuizTitle = quiz.Title,
                TotalAttempts = totalAttempts,
                AverageScore = averageScore,
                QuestionStats = questionStats
            };

            return View(model);
        }
    }
