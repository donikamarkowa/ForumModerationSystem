using AutoModeratedForum.Data;
using AutoModeratedForum.Entities;
using AutoModeratedForum.Enums;
using AutoModeratedForum.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Route("[controller]/[action]")]
public class CommentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SentimentAnalysisService _sentimentService;
    private readonly UserManager<IdentityUser> _userManager;

    public CommentsController(ApplicationDbContext context, SentimentAnalysisService sentimentService, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _sentimentService = sentimentService;
        _userManager = userManager;
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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var comment = new Comment
        {
            Content = content,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            IsFlagged = prediction.isRude
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        if (comment.IsFlagged)
        {
            var moderationRequest = new ModerationRequest
            {
                CommentId = comment.Id,
                Decision = ModerationDecision.Pending
            };

            _context.ModerationRequests.Add(moderationRequest);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> UserComments()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var comments = await _context.Comments
           .Where(c => c.UserId == userId)
           .Include(c => c.ModerationRequest)
           .OrderByDescending(c => c.CreatedAt)
           .ToListAsync();

        return View(comments);
    }


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var comments = await _context.Comments
         .Where(c => !c.IsFlagged ||
             c.ModerationRequest != null && c.ModerationRequest.Decision == ModerationDecision.Approved)
         .OrderByDescending(c => c.CreatedAt)
         .ToListAsync();

        return View(comments);
    }

    [Authorize(Roles = "Moderator")]
    [HttpGet]
    public async Task<IActionResult> Flagged()
    {
        var flagged = await _context.ModerationRequests
            .Include(r => r.Comment)
            .Where(r => r.Decision == ModerationDecision.Pending)
            .OrderBy(r => r.Comment.CreatedAt)
            .ToListAsync();

        return View(flagged);
    }

    [Authorize(Roles = "Moderator")]
    [HttpGet]
    public async Task<IActionResult> Reviewed()
    {
        var reviewedRequests = await _context.ModerationRequests
            .Include(r => r.Comment)
            .Include(r => r.Moderator)
            .Where(r => r.Decision != ModerationDecision.Pending)
            .OrderByDescending(r => r.ReviewedAt)
            .ToListAsync();

        return View(reviewedRequests);
    }

    [Authorize(Roles = "Moderator")]
    [HttpPost]
    public async Task<IActionResult> ApproveComment(int id)
    {
        var request = await _context.ModerationRequests
            .Include(r => r.Comment)
            .FirstOrDefaultAsync(r => r.CommentId == id && r.Decision == ModerationDecision.Pending);

        if (request == null)
            return NotFound();

        request.Decision = ModerationDecision.Approved;
        request.ModeratorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        request.ReviewedAt = DateTime.UtcNow;

        request.Comment.IsFlagged = false;

        await _context.SaveChangesAsync();

        return Ok(request.Comment);
    }

    [Authorize(Roles = "Moderator")]
    [HttpDelete]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var request = await _context.ModerationRequests
            .Include(r => r.Comment)
            .FirstOrDefaultAsync(r => r.CommentId == id && r.Decision == ModerationDecision.Pending);

        if (request == null)
            return NotFound();

        var comment = request.Comment;

        request.Decision = ModerationDecision.Rejected;
        request.ModeratorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        request.ReviewedAt = DateTime.UtcNow;

        comment.IsFlagged = true;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Moderator")]
    [HttpPost]
    public async Task<IActionResult> ModerateComment(int moderationRequestId, string decision, string? notes)
    {
        var request = await _context.ModerationRequests
            .Include(r => r.Comment)
            .FirstOrDefaultAsync(r => r.Id == moderationRequestId);

        if (request == null)
            return NotFound();

        var moderatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        request.ReviewedAt = DateTime.UtcNow;
        request.ModeratorId = moderatorId;
        request.Notes = notes;

        if (decision == "Approved")
        {
            request.Decision = ModerationDecision.Approved;
            request.Comment.IsFlagged = false;
        }
        else if (decision == "Rejected")
        {
            request.Decision = ModerationDecision.Rejected;
            request.Comment.IsFlagged = true; 
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Flagged));
    }
}
