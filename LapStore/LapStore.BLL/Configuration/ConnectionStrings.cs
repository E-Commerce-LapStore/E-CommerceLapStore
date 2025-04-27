using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.BLL.Configuration
{
    public class ConnectionStrings
    {
        [Required(ErrorMessage = "DefaultConnection string is required")]
        public string DefaultConnection { get; set; }
    }
}
