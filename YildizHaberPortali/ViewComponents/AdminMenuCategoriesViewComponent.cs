using Microsoft.AspNetCore.Mvc;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Repositories; 

namespace YildizHaberPortali.ViewComponents
{
    public class AdminMenuCategoriesViewComponent : ViewComponent
    {
        private readonly ICategoryRepository _categoryRepository;

        public AdminMenuCategoriesViewComponent(ICategoryRepository categoryRepository)
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