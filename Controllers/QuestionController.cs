using BeQuiz.Data;
using BeQuiz.Models;
using BeQuiz.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class QuestionController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public QuestionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /Question/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var question = await _context.Questions
            .Include(q => q.Quiz)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null || question.Quiz.CreatorId != _userManager.GetUserId(User))
            return Forbid();

        return View(question);
    }

    // POST: /Question/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Question updated)
    {
        var question = await _context.Questions
            .Include(q => q.Quiz)
            .FirstOrDefaultAsync(q => q.Id == updated.Id);

        if (question == null || question.Quiz.CreatorId != _userManager.GetUserId(User))
            return Forbid();

        if (!ModelState.IsValid)
            return View(updated);

        question.Content = updated.Content;
        question.Points = updated.Points;

        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "Quiz", new { id = question.QuizId });
    }

    // GET: /Question/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var question = await _context.Questions
            .Include(q => q.Quiz)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null || question.Quiz.CreatorId != _userManager.GetUserId(User))
            return Forbid();

        return View(question);
    }

    // POST: /Question/Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var question = await _context.Questions
            .Include(q => q.Quiz)
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null || question.Quiz.CreatorId != _userManager.GetUserId(User))
            return Forbid();

        _context.Answers.RemoveRange(question.Answers);
        _context.Questions.Remove(question);

        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "Quiz", new { id = question.QuizId });
    }
    [HttpGet]
    public async Task<IActionResult> EditAnswers(int questionId)
    {
        var question = await _context.Questions
            .Include(q => q.Answers)
            .Include(q => q.Quiz)
            .FirstOrDefaultAsync(q => q.Id == questionId);

        if (question == null || question.Quiz.CreatorId != _userManager.GetUserId(User))
            return Forbid();

        var model = new EditAnswersViewModel
        {
            QuestionId = question.Id,
            QuestionContent = question.Content,
            CorrectAnswerId = question.CorrectAnswerId,
            Answers = question.Answers.Select(a => new EditAnswersViewModel.AnswerItem
            {
                AnswerId = a.Id,
                Content = a.Content
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAnswers(EditAnswersViewModel model)
    {
        var question = await _context.Questions
            .Include(q => q.Answers)
            .Include(q => q.Quiz)
            .FirstOrDefaultAsync(q => q.Id == model.QuestionId);

        if (question == null || question.Quiz.CreatorId != _userManager.GetUserId(User))
            return Forbid();

        for (int i = 0; i < model.Answers.Count; i++)
        {
            var answerModel = model.Answers[i];
            var answerEntity = question.Answers.FirstOrDefault(a => a.Id == answerModel.AnswerId);

            if (answerEntity != null)
            {
                answerEntity.Content = answerModel.Content;
            }
        }

        question.CorrectAnswerId = model.CorrectAnswerId;

        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Quiz", new { id = question.QuizId });
    }

}
