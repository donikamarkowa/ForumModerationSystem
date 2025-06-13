using AutoModeratedForum.Data;
using AutoModeratedForum.Entities;
using AutoModeratedForum.Models;
using AutoModeratedForum.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

[Route("[controller]/[action]")]
public class CommentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SentimentAnalysisService _sentimentService;

    public CommentsController(ApplicationDbContext context, SentimentAnalysisService sentimentService)
    {
        _context = context;
        _sentimentService = sentimentService;
    }

    [HttpGet]
    public IActionResult AddComment()
    {
        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddComment(string content)
    {
        var prediction = _sentimentService.Predict(content);

        var comment = new Comment
        {
            Content = content,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
            CreatedAt = DateTime.UtcNow,
            IsFlagged = prediction.isRude
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var comments = await _context.Comments
            .Where(c => !c.IsFlagged)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return View(comments);
    }

    [Authorize(Roles = "Moderator")]
    [HttpGet]
    public async Task<IActionResult> GetFlaggedComments()
    {
        var flaggedComments = await _context.Comments
            .Where(c => c.IsFlagged)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return Ok(flaggedComments);
    }

    [Authorize(Roles = "Moderator")]
    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveComment(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
            return NotFound();

        comment.IsFlagged = false;
        await _context.SaveChangesAsync();

        return Ok(comment);
    }

    [Authorize(Roles = "Moderator")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
            return NotFound();

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
