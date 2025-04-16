using LapStore.BLL.Interfaces;
using LapStore.DAL;
using Microsoft.AspNetCore.Mvc;

namespace LapStore.Web.Controllers
{
    public class ProductImageController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        public ProductImageController(IUnitOfWork unitOfWork, IFileService fileService) { 
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }


        #region GetAll
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region Add
        
        #endregion

        #region Edit
        
        #endregion

        #region Delete
        
        #endregion

        #region GetById
        
        #endregion

    }
}
