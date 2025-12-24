using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Data;
using YildizHaberPortali.Models;
using YildizHaberPortali.Models.ViewModels;

namespace YildizHaberPortali.Controllers
{
    [Authorize(Roles = "Admin,Yazar")]
    public class NewsController : Controller
    {
        private readonly INewsRepository _newsRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICommentRepository _commentRepository;
        private readonly ApplicationDbContext _context; 

        public NewsController(INewsRepository newsRepository,
                              ICategoryRepository categoryRepository,
                              IWebHostEnvironment hostEnvironment,
                              UserManager<AppUser> userManager,
                              ICommentRepository commentRepository,
                              ApplicationDbContext context) 
        {
            _newsRepository = newsRepository;
            _categoryRepository = categoryRepository;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _commentRepository = commentRepository;
            _context = context; 
        }

        public async Task<IActionResult> Index(int? categoryId, bool? isPublished) 
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            var categories = await _categoryRepository.GetAllAsync();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = categoryId;

            var news = await _newsRepository.GetAllWithCategoryAsync();

            if (isPublished.HasValue)
            {
                news = news.Where(x => x.IsPublished == isPublished.Value).ToList();
                ViewBag.IsPublishedFilter = isPublished.Value;
            }

            if (categoryId.HasValue)
            {
                news = news.Where(x => x.CategoryId == categoryId.Value).ToList();
            }

            if (roles.Contains("Admin"))
                return View(news);

            if (roles.Contains("Yazar"))
            {
                return View(news.Where(x => x.AuthorId == user.Id).ToList());
            }

            return View(new List<News>());
        }

        public async Task<IActionResult> CategoryNews(string slug)
        {
            var categories = await _categoryRepository.GetAllAsync();
            var category = categories.FirstOrDefault(c => c.Name.Replace(" ", "-").ToLower() == slug.ToLower());

            if (category != null)
            {
                return RedirectToAction("Index", "Home", new { categoryId = category.Id });
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");

            var model = new NewsCreateViewModel
            {
                Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                string uniqueFileName = "no-image.png";

                if (model.ImageFile != null)
                {
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string path = Path.Combine(_hostEnvironment.WebRootPath, "uploads", uniqueFileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }
                }

                var news = new News
                {
                    Title = model.Title,
                    Content = model.Content,
                    CategoryId = model.CategoryId,
                    IsPublished = model.IsPublished,
                    AuthorId = user.Id,
                    Author = user.FullName ?? "Yıldız Editör",
                    CreatedDate = DateTime.Now,
                    Image = uniqueFileName
                };

                await _newsRepository.AddAsync(news);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", model.CategoryId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var news = await _newsRepository.GetByIdAsync(id);
            if (news == null) return NotFound();

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", news.CategoryId);

            var model = new NewsUpdateViewModel
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                CategoryId = news.CategoryId,
                IsPublished = news.IsPublished,
                ExistingImage = news.Image,
                Author = news.Author
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NewsUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var news = await _newsRepository.GetByIdAsync(model.Id);
                if (news == null) return NotFound();

                string uniqueFileName = model.ExistingImage;

                if (model.ImageFile != null)
                {
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string path = Path.Combine(_hostEnvironment.WebRootPath, "uploads", uniqueFileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }
                }

                news.Title = model.Title;
                news.Content = model.Content;
                news.CategoryId = model.CategoryId;
                news.IsPublished = model.IsPublished;
                news.Image = uniqueFileName;
                news.Author = model.Author ?? news.Author;

                await _newsRepository.UpdateAsync(news);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", model.CategoryId);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _newsRepository.GetByIdAsync(id);
            if (news == null) return Json(new { success = false, message = "Haber bulunamadı!" });

            if (!string.IsNullOrEmpty(news.Image) && news.Image != "no-image.png")
            {
                var path = Path.Combine(_hostEnvironment.WebRootPath, "uploads", news.Image);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }

            await _newsRepository.DeleteAsync(id);
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var news = await _context.News
                .Include(x => x.Category)
                .Include(x => x.Comments.Where(c => c.IsApproved == true))
                .FirstOrDefaultAsync(m => m.Id == id);

            if (news == null) return NotFound();

            return View(news);
        }
    }
}