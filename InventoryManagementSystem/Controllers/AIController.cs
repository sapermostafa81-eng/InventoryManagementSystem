using InventoryManagementSystem.Data;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace InventoryManagementSystem.Controllers
{
    public class AIController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AIService _aiService;

        public AIController(
            ApplicationDbContext context,
            AIService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ask(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                ModelState.AddModelError(
                    "",
                    "Please enter a question."
                );

                return View("Index");
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            var inventoryData = string.Join(
                "\n",
                products.Select(p =>
                    $"Product: {p.Title}, " +
                    $"Price: {p.Price}, " +
                    $"Quantity: {p.Quantity}, " +
                    $"Category: {p.Category?.Name ?? "No Category"}, " +
                    $"Description: {p.Description}"
                )
            );

            if (string.IsNullOrEmpty(inventoryData))
            {
                inventoryData = "There are currently no products in the inventory.";
            }

            try
            {
                var answer = await _aiService.AskAsync(
                    question,
                    inventoryData
                );

                ViewBag.Question = question;
                ViewBag.Answer = answer;

                return View("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    "An error occurred while communicating with the AI service."
                );

                return View("Index");
            }
        }
    }
}