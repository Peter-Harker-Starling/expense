using Microsoft.AspNetCore.Mvc;
using expense.Models;
using expense.Data;
using expense.Services;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace expense.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class ExpenseRecordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ExpenseRecordService _expenseRecordService;

        public ExpenseRecordController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ExpenseRecordService expenseRecordService)
        {
            this._context = context;
            this._userManager = userManager;
            this._expenseRecordService = expenseRecordService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Analyze(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                ModelState.AddModelError("", "輸入不能為空");
                return View("Create");
            }

            try
            {
                ExpenseRecord record = await _expenseRecordService.AnalyzeExpenseRecord(input);
                return View("Create", record);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "分析失敗，請稍後再試。");
                return View("Create");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(ExpenseRecord record)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", record);
            }

            // 取得目前登入的使用者
            var user = await _userManager.GetUserAsync(User);
            // 如果 user 為 null，表示使用者未登入，回傳 401 Unauthorizeds
            if (user == null)
            {
                return Unauthorized();
            }
            record.UserId = user.Id;
            record.User = user;

            _context.ExpenseRecords.Add(record);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
