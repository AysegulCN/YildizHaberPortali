using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YildizHaberPortali.Models;
using YildizHaberPortali.Contracts;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace YildizHaberPortali.Controllers
{
    public class ContactController : Controller
    {
        private readonly IGenericRepository<Contact> _contactRepo;
        private readonly IWebHostEnvironment _env;

        public ContactController(IGenericRepository<Contact> contactRepo, IWebHostEnvironment env)
        {
            _contactRepo = contactRepo;
            _env = env;
        }

        [AllowAnonymous]
        public IActionResult Index() => View();

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Contact model, IFormFile? Photo)
        {
            if (ModelState.IsValid)
            {
                if (Photo != null)
                {
                    string folder = "uploads/contact/";
                    string fileName = Guid.NewGuid().ToString() + "_" + Photo.FileName;
                    string serverFolder = Path.Combine(_env.WebRootPath, folder);
                    if (!Directory.Exists(serverFolder)) Directory.CreateDirectory(serverFolder);

                    string filePath = Path.Combine(serverFolder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Photo.CopyToAsync(fileStream);
                    }
                    model.PhotoPath = "/" + folder + fileName;
                }

                await _contactRepo.AddAsync(model);
                TempData["SuccessMessage"] = "Mesajınız veya ihbarınız başarıyla iletildi!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Messages()
        {
            var messages = await _contactRepo.GetAllAsync();
            return View(messages.OrderByDescending(x => x.CreatedDate).ToList());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var message = await _contactRepo.GetByIdAsync(id);
            if (message == null) return NotFound();
            return View(message);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _contactRepo.DeleteAsync(id);
            return Json(new { success = true });
        }
    }
}