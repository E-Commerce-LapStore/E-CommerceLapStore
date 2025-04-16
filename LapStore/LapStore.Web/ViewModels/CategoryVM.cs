using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace LapStore.Web.ViewModels
{
    public class CategoryVM
    {
        #region Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; }
        public int? ParentCategoryId { get; set; }
        #endregion

        #region Methods
        public static CategoryVM FromCategory(Category category)
        {
            return new CategoryVM
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                ParentCategoryId = category.ParentCategoryId,
            };
        }
        public static Category FromCategoryVM(CategoryVM category)
        {
            return new Category
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                ParentCategoryId = category.ParentCategoryId,
            };
        }
        #endregion
    }
}
