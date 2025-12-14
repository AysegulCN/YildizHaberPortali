using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

public class NewsController : Controller
{

    private readonly INewsRepository _newsRepository;
    private readonly ICategoryRepository _categoryRepository;

    public NewsController(INewsRepository newsRepository, ICategoryRepository categoryRepository)
    {
        _newsRepository = newsRepository;
        _categoryRepository = categoryRepository;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(int? categoryId)
    {
        IEnumerable<News> newsList;

        var categories = await _categoryRepository.GetAllAsync();
        ViewBag.Categories = categories;

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            newsList = await _newsRepository.GetByCategoryIdAsync(categoryId.Value);
            var category = categories.FirstOrDefault(c => c.Id == categoryId.Value);
            ViewData["Title"] = $"{category?.Name} Haberleri";
        }
        else
        {
            newsList = await _newsRepository.GetAllAsync();
            ViewData["Title"] = "Tüm Haberler";
        }

        return View(newsList);
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
        var categoryList = await _categoryRepository.GetAllAsync();
        viewModel.Categories = categoryList.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();

        if (ModelState.IsValid)
        {
            var news = new News
            {
                Title = viewModel.Title,
                Content = viewModel.Content,
                ImageUrl = viewModel.ImageUrl,
                Author = viewModel.Author,
                CategoryId = viewModel.CategoryId,
                PublishDate = DateTime.Now,
                IsPublished = true
            };
            await _newsRepository.AddAsync(news);
            return RedirectToAction(nameof(Index));
        }
        return View(viewModel);
    }

    [Authorize(Roles = "Admin,Editor")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        // Daha önceki yazım hatası buradan temizlenmiştir.
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
            // IsPublished alanını da eklemeyi unutmayın (ViewModel'de olmalı)
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
                // IsPublished'i de modelden almayı unutmayın
            };
            await _newsRepository.UpdateAsync(news);
            return RedirectToAction(nameof(Index));
        }
        ModelState.AddModelError("", "Haber güncellenirken bir hata oluştu.");
        return View(viewModel);
    }

    // GET: Haber Silme Onayı (Sadece Admin)
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var news = await _newsRepository.GetByIdAsync(id.GetValueOrDefault());
        if (news == null) return NotFound();
        return View(news);
    }

    // POST: Haber Silme İşlemi (Sadece Admin)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _newsRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> StatusManagement()
    {
        return View(); 
        throw new NotImplementedException(); 
    }
}