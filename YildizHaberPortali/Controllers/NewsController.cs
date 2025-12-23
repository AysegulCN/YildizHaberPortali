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

        public NewsController(INewsRepository newsRepository,
                              ICategoryRepository categoryRepository,
                              IWebHostEnvironment hostEnvironment,
                              UserManager<AppUser> userManager,
                              ICommentRepository commentRepository)
        {
            _newsRepository = newsRepository;
            _categoryRepository = categoryRepository;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _commentRepository = commentRepository;
        }

        // 📰 Haber Listesi (Index)
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            // Kategori bilgileriyle birlikte tüm haberleri çek
            var news = await _newsRepository.GetAllWithCategoryAsync();

            if (roles.Contains("Admin"))
                return View(news);

            if (roles.Contains("Yazar"))
            {
                // Yazara sadece kendi haberlerini göster
                return View(news.Where(x => x.AuthorId == user.Id).ToList());
            }

            return View(new List<News>());
        }

        // ✨ Yeni Haber Ekle (GET)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllAsync();
            // 🚀 Kategorilerin dropdown'da görünmesi için tek satır:
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name");

            var model = new NewsCreateViewModel
            {
                Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList()
            };
            return View(model);
        }

        // ✨ Yeni Haber Ekle (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                string uniqueFileName = "no-image.png";

                // 📸 Görsel Yükleme Mantığı
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

            // Hata varsa kategorileri tekrar doldur
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", model.CategoryId);
            return View(model);
        }

        // ✏️ Haber Düzenle (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var news = await _newsRepository.GetByIdAsync(id);
            if (news == null) return NotFound();

            var categories = await _categoryRepository.GetAllAsync();
            // 🚀 Düzenle sayfasında kategori seçili gelsin diye:
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

        // ✏️ Haber Düzenle (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NewsUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var news = await _newsRepository.GetByIdAsync(model.Id);
                if (news == null) return NotFound();

                string uniqueFileName = model.ExistingImage;

                // 📸 Yeni Görsel Yükleme
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
                // Yazar adını koru veya güncelle
                news.Author = model.Author ?? news.Author;

                await _newsRepository.UpdateAsync(news);
                return RedirectToAction(nameof(Index));
            }

            // 🚀 Hata durumunda kategorileri tekrar doldur (Burası bozulmayı önler!)
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", model.CategoryId);
            return View(model);
        }

        // 🗑️ Haber Sil (Ajax)
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

        // 👁️ Haber Detay
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var news = await _newsRepository.GetByIdAsync(id);
            if (news == null) return NotFound();

            var comments = (await _commentRepository.GetAllAsync())
                .Where(x => x.NewsId == id)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

            ViewBag.Comments = comments;
            return View(news);
        }
    }
}