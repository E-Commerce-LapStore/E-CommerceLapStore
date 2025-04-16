using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.BLL.Interfaces
{
    public interface IFileService
    {
        public Task<string> Upload(IFormFile file, string location);
        public bool DeletePhysicalFile(string path);
    }
}
