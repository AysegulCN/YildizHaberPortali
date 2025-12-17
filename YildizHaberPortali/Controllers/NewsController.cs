using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Hosting;

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

    [AllowAnonymous] // Eğer admin paneliyse bunu kaldırabilirsin, site tarafıysa kalsın
    public async Task<IActionResult> Index(int? categoryId)
    {
        // 1. ADIM: Yeni yazdığımız metotla verileri (Kategorileriyle birlikte) çek
        var newsList = await _newsRepository.GetAllWithCategoryAsync();

        // 2. ADIM: Eğer bir kategori seçildiyse LİSTE ÜZERİNDE filtrele
        if (categoryId.HasValue)
        {
            newsList = newsList.Where(x => x.CategoryId == categoryId).ToList();
            ViewBag.CurrentCategory = categoryId;
        }

        // 3. ADIM: Tarihe göre sırala (Yeniden eskiye) ve sayfaya gönder
        return View(newsList.OrderByDescending(x => x.CreatedDate).ToList());
    }

    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> Create()
    { 
        var categoryList = await _categoryRepository.GetAllAsync();

        var viewModel = new NewsCreateViewModel
        {
            Categories = categoryList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList()
        };
        return View(viewModel);
    }
    [Authorize(Roles = "Admin,Editor")]
    [HttpPost]
    public async Task<IActionResult> Create(NewsCreateViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            string uniqueFileName = null;

            if (viewModel.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.ImageFile.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.ImageFile.CopyToAsync(fileStream);
                }
            }

            var news = new News
            {
                Title = viewModel.Title,
                Content = viewModel.Content,
                Author = viewModel.Author ?? "Admin", 
                CategoryId = viewModel.CategoryId,
                PublishDate = DateTime.Now,
                IsPublished = true,

                Image = uniqueFileName,  

                ImageUrl = uniqueFileName
            };

            await _newsRepository.AddAsync(news);
            return RedirectToAction(nameof(Index));
        }

        var categoryList = await _categoryRepository.GetAllAsync();
        viewModel.Categories = categoryList.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

        return View(viewModel);
    }

    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var news = await _newsRepository.GetByIdAsync(id.GetValueOrDefault());
        if (news == null) return NotFound();

        var categoryList = await _categoryRepository.GetAllAsync();

        var viewModel = new NewsCreateViewModel
        {
            Id = news.Id,
            Title = news.Title,
            Content = news.Content,
            ImageUrl = news.ImageUrl,
            Author = news.Author,
            CategoryId = news.CategoryId,
            Categories = categoryList.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList()
        };
        return View(viewModel);
    }

    [Authorize(Roles = "Admin,Editor")]
    [HttpPost]
    public async Task<IActionResult> Edit(NewsCreateViewModel viewModel)

    {
        var categoryList = await _categoryRepository.GetAllAsync();
        viewModel.Categories = categoryList.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

        if (ModelState.IsValid)
        {
            var news = new News
            {
                Id = viewModel.Id,
                Title = viewModel.Title,
                Content = viewModel.Content,
                ImageUrl = viewModel.ImageUrl,
                Author = viewModel.Author,
                CategoryId = viewModel.CategoryId,
                PublishDate = DateTime.Now,
            };
            await _newsRepository.UpdateAsync(news);
            return RedirectToAction(nameof(Index));
        }
        ModelState.AddModelError("", "Haber güncellenirken bir hata oluştu.");
        return View(viewModel);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var news = await _newsRepository.GetByIdAsync(id.GetValueOrDefault());
        if (news == null) return NotFound();
        return View(news);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _newsRepository.DeleteAsync(id);

            return Json(new { success = true, message = "Haber başarıyla silindi." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Silme işleminde hata oluştu." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var news = await _newsRepository.GetByIdAsync(id);
        if (news == null)
        {
            return Json(new { success = false, message = "Haber bulunamadı." });
        }

        // Resmi klasörden de silelim ki çöp birikmesin
        if (!string.IsNullOrEmpty(news.Image))
        {
            string imagePath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", news.Image);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }

        await _newsRepository.DeleteAsync(id);
        return Json(new { success = true });
    }

    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> StatusManagement()
    {
        return View(); 
        throw new NotImplementedException(); 
    }
    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var news = await _newsRepository.GetByIdAsync(id);
        if (news == null)
        {
            return Json(new { success = false, message = "Haber bulunamadı." });
        }

        // Durumu tersine çevir (Açıksa kapa, kapalıysa aç)
        news.IsPublished = !news.IsPublished;
        await _newsRepository.UpdateAsync(news);

        // Yeni durumu geri gönder ki sayfadaki renk değişsin
        return Json(new { success = true, isPublished = news.IsPublished });
    }
}