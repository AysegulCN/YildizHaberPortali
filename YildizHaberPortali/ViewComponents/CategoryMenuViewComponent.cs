// ViewComponents/CategoryMenuViewComponent.cs

using Microsoft.AspNetCore.Mvc;
using YildizHaberPortali.Contracts;
using System.Threading.Tasks;

namespace YildizHaberPortali.ViewComponents
{
    // View Component'in tanımı
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryMenuViewComponent(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }
    }
}