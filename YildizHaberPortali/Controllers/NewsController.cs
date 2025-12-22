using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
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

        public NewsController(INewsRepository newsRepository, ICategoryRepository categoryRepository,
                              IWebHostEnvironment hostEnvironment, UserManager<AppUser> userManager)
        {
            _newsRepository = newsRepository;
            _categoryRepository = categoryRepository;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager; 
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            var news = await _newsRepository.GetAllAsync();

            if (roles.Contains("Admin")) return View(news);

            if (roles.Contains("Yazar"))
            {
                return View(news.Where(x => x.AuthorId == user.Id).ToList());
            }

            return View(new List<News>());
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Yazar")]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var viewModel = new NewsCreateViewModel
            {
                Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Yazar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var news = new News
                {
                    Title = model.Title,
                    Content = model.Content,
                    CategoryId = model.CategoryId,
                    IsPublished = model.IsPublished,
                    AuthorId = user.Id,
                    CreatedDate = DateTime.Now
                };

                await _newsRepository.AddAsync(news);
                return RedirectToAction(nameof(Index));
            }

            var allCategories = await _categoryRepository.GetAllAsync();
            model.Categories = allCategories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

            return View(model);
        }



        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> Edit(int id)
        {
            var news = await _newsRepository.GetByIdAsync(id);
            if (news == null) return NotFound();

            var categories = await _categoryRepository.GetAllAsync();

            var vm = new NewsUpdateViewModel
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                CategoryId = news.CategoryId,
                Author = news.Author,
                ExistingImage = news.Image,
                IsPublished = news.IsPublished,
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Yazar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NewsUpdateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var news = await _newsRepository.GetByIdAsync(viewModel.Id);
                if (news == null) return NotFound();

                string uniqueFileName = viewModel.ExistingImage;

                if (viewModel.ImageFile != null)
                {
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.ImageFile.FileName;
                    string path = Path.Combine(_hostEnvironment.WebRootPath, "uploads", uniqueFileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await viewModel.ImageFile.CopyToAsync(stream);
                    }
                }

                news.Title = viewModel.Title;
                news.Content = viewModel.Content;
                news.CategoryId = viewModel.CategoryId;
                news.IsPublished = viewModel.IsPublished;
                news.Author = viewModel.Author ?? "Ayşegül";
                news.Image = uniqueFileName;

                await _newsRepository.UpdateAsync(news);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllAsync();
            viewModel.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return View(viewModel);
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

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var news = await _newsRepository.GetByIdAsync(id);
            if (news == null) return Json(new { success = false });

            news.IsPublished = !news.IsPublished;
            await _newsRepository.UpdateAsync(news);
            return Json(new { success = true });
        }

        [Authorize(Roles = "Admin,Yazar")]
        public async Task<IActionResult> StatusManagement(int? categoryId)
        {

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectListItem[] { new SelectListItem { Value = "", Text = "Tüm Kategoriler" } }
                .Concat(categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }));

            var newsQuery = await _newsRepository.GetAllWithCategoryAsync();

            if (categoryId.HasValue)
            {
                newsQuery = newsQuery.Where(x => x.CategoryId == categoryId.Value).ToList();
                ViewBag.SelectedCategory = categoryId.Value;
            }

            return View(newsQuery.OrderByDescending(x => x.CreatedDate));
        }

    }
}