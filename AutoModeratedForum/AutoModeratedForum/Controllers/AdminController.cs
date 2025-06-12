using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly RoleManager<IdentityRole> roleManager;

    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    public IActionResult Index()
    {
        var users = userManager.Users.ToList();
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> AddRole(string userId, string role)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user != null && await roleManager.RoleExistsAsync(role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveRole(string userId, string role)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user != null && await roleManager.RoleExistsAsync(role))
        {
            await userManager.RemoveFromRoleAsync(user, role);
        }
        return RedirectToAction("Index");
    }
}
