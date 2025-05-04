using LapStore.DAL.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace LapStore.BLL.DTOs
{
    public class CategoryDTO
    {
        #region Properties
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; }

        public int? ParentCategoryId { get; set; }
        #endregion

        #region Methods
        public static CategoryDTO FromCategory(Category category)
        {
            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                ParentCategoryId = category.ParentCategoryId,
            };
        }
        public static Category FromCategoryVM(CategoryDTO category)
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
