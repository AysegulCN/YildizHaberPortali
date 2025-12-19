using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Models;

namespace YildizHaberPortali.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsRepository _newsRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _hostEnvironment;

        public NewsController(INewsRepository newsRepository, ICategoryRepository categoryRepository, IWebHostEnvironment hostEnvironment)
        {
            _newsRepository = newsRepository;
            _categoryRepository = categoryRepository;
            _hostEnvironment = hostEnvironment;
        }

        // 1. HABER LİSTELEME
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? categoryId, string status)
        {
            var newsList = await _newsRepository.GetAllWithCategoryAsync();

            if (categoryId.HasValue)
                newsList = newsList.Where(x => x.CategoryId == categoryId).ToList();

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "active") newsList = newsList.Where(x => x.IsPublished).ToList();
                else if (status == "passive") newsList = newsList.Where(x => !x.IsPublished).ToList();
            }

            return View(newsList.OrderByDescending(x => x.CreatedDate).ToList());
        }

        // 2. YENİ HABER EKLEME (GET)
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var viewModel = new NewsCreateViewModel
            {
                Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList()
            };
            return View(viewModel);
        }

        // 3. YENİ HABER EKLEME (POST)
        [HttpPost]
        [Authorize(Roles = "Admin,Editor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = "no-image.png";
                if (viewModel.ImageFile != null)
                {
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.ImageFile.FileName;
                    string path = Path.Combine(_hostEnvironment.WebRootPath, "uploads", uniqueFileName);
                    using (var stream = new FileStream(path, FileMode.Create)) { await viewModel.ImageFile.CopyToAsync(stream); }
                }

                var news = new News
                {
                    Title = viewModel.Title,
                    Content = viewModel.Content,
                    Author = User.Identity.Name ?? "Admin",
                    CategoryId = viewModel.CategoryId,
                    CreatedDate = DateTime.Now,
                    IsPublished = true,
                    Image = uniqueFileName
                };

                await _newsRepository.AddAsync(news);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // 4. HABER DÜZENLEME (GET) - [Düzeltilen Kritik Hata: ViewModel Uyuşmazlığı]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<IActionResult> Edit(int id)
        {
            var news = await _newsRepository.GetByIdAsync(id);
            if (news == null) return NotFound();

            var vm = new NewsCreateViewModel
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                CategoryId = news.CategoryId,
                ExistingImagePath = news.Image //
            };

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = categories.ToList();
            return View(vm);
        }

        // 5. HABER DÜZENLEME (POST)
        [HttpPost]
        [Authorize(Roles = "Admin,Editor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NewsCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var news = await _newsRepository.GetByIdAsync(viewModel.Id);
                if (news == null) return NotFound();

                news.Title = viewModel.Title;
                news.Content = viewModel.Content;
                news.CategoryId = viewModel.CategoryId;

                if (viewModel.ImageFile != null)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.ImageFile.FileName;
                    string path = Path.Combine(_hostEnvironment.WebRootPath, "uploads", uniqueFileName);
                    using (var stream = new FileStream(path, FileMode.Create)) { await viewModel.ImageFile.CopyToAsync(stream); }
                    news.Image = uniqueFileName;
                }

                await _newsRepository.UpdateAsync(news);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // 6. HABER SİLME (POST - AJAX) - [Düzeltilen Hata: Sil Butonu Çalışmama Sorunu]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _newsRepository.GetByIdAsync(id);
            if (news == null) return Json(new { success = false, message = "Haber bulunamadı!" });

            // Klasördeki görseli de silelim ki sunucu dolmasın
            if (!string.IsNullOrEmpty(news.Image) && news.Image != "no-image.png")
            {
                var path = Path.Combine(_hostEnvironment.WebRootPath, "uploads", news.Image);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }

            await _newsRepository.DeleteAsync(id);
            return Json(new { success = true });
        }

        // 7. DURUM DEĞİŞTİRME (POST - AJAX)
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var news = await _newsRepository.GetByIdAsync(id);
            if (news == null) return Json(new { success = false });

            news.IsPublished = !news.IsPublished;
            await _newsRepository.UpdateAsync(news);
            return Json(new { success = true });
        }
    }
}