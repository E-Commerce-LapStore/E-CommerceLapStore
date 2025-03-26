using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DAL.Entities
{
    internal class CartItem
    {
        public int ProductId { get; set; }
        public int CartId { get; set; }
    }
}
