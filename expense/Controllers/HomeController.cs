using expense.Data;
using expense.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace expense.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string month)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            bool isValidMonth = DateTime.TryParseExact(
                month, "yyyy-MM",
                System.Globalization.CultureInfo.InvariantCulture, // 規定使用固定格式解讀，不受地區設定影響
                System.Globalization.DateTimeStyles.None, // 不要額外套用其他特殊的日期處理
                out var selectedDate); // 如果解析日期成功，將解析後的日期存入 selectedDate

            if (!isValidMonth)
            {
                selectedDate = DateTime.Today;
            }

            var startOfMonth = new DateOnly(selectedDate.Year, selectedDate.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);

            var expensesInMonth = await _context.ExpenseRecords
                .Where(e => e.UserId == user.Id && e.Date >= startOfMonth && e.Date < endOfMonth)
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            ViewBag.SelectedMonth = selectedDate.ToString("yyyy-MM");
            ViewBag.TotalAmount = expensesInMonth.Sum(e => e.Amount);
            return View(expensesInMonth);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var expenses = await _context.ExpenseRecords
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

            if (expenses == null)
            {
                return NotFound();
            }

            return View(expenses);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ExpenseRecord expense)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                return View(expense);
            }

            var expenses = await _context.ExpenseRecords
                .FirstOrDefaultAsync(e => e.Id == expense.Id && e.UserId == user.Id);

            if (expenses == null)
            {
                return NotFound();
            }

            expenses.Item = expense.Item;
            expenses.Date = expense.Date;
            expenses.Amount = expense.Amount;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var expenses = await _context.ExpenseRecords
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

            if (expenses == null)
            {
                return NotFound();
            }

            _context.ExpenseRecords.Remove(expenses);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
