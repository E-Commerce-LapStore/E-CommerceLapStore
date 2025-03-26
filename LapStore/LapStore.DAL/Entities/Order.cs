using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DAL.Entities
{
    internal class Order
    {
        public int Id { get; set; }
        public DateOnly OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public int TotalAmount {  get; set; }
        public int UserId { get; set; }

    }
}
