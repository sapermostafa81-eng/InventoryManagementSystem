using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers
{
	public class ProductController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ProductController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Product
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var products = await _context.Products
				.Include(p => p.Category)
				.ToListAsync();

			return View(products);
		}

		// GET: Product/Details/5
		[HttpGet]
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
				return NotFound();

			var product = await _context.Products
				.Include(p => p.Category)
				.FirstOrDefaultAsync(p => p.ProductId == id);

			if (product == null)
				return NotFound();

			return View(product);
		}

		// GET: Product/Create
		[HttpGet]
		public async Task<IActionResult> Create()
		{
			await LoadCategories();
			return View();
		}

		// POST: Product/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Product product)
		{
			if (!ModelState.IsValid)
			{
				await LoadCategories(product.CategoryId);
				return View(product);
			}

			_context.Products.Add(product);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}

		// GET: Product/Edit/5
		[HttpGet]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
				return NotFound();

			var product = await _context.Products.FindAsync(id);

			if (product == null)
				return NotFound();

			await LoadCategories(product.CategoryId);

			return View(product);
		}

		// POST: Product/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, Product product)
		{
			if (id != product.ProductId)
				return NotFound();

			if (!ModelState.IsValid)
			{
				await LoadCategories(product.CategoryId);
				return View(product);
			}

			try
			{
				_context.Products.Update(product);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ProductExists(product.ProductId))
					return NotFound();

				throw;
			}

			return RedirectToAction(nameof(Index));
		}

		// GET: Product/Delete/5
		[HttpGet]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
				return NotFound();

			var product = await _context.Products
				.Include(p => p.Category)
				.FirstOrDefaultAsync(p => p.ProductId == id);

			if (product == null)
				return NotFound();

			return View(product);
		}

		// POST: Product/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var product = await _context.Products.FindAsync(id);

			if (product != null)
			{
				_context.Products.Remove(product);
				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Index));
		}

		private bool ProductExists(int id)
		{
			return _context.Products.Any(e => e.ProductId == id);
		}

		private async Task LoadCategories(int? selectedCategory = null)
		{
			ViewBag.CategoryId = new SelectList(
				await _context.Categories.ToListAsync(),
				"CategoryId",
				"Name",
				selectedCategory
			);
		}
	}
}