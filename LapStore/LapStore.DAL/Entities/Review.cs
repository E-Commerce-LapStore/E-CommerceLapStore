using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.DAL.Entities
{
    internal class Review
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public char Rate {  get; set; }
        public string ReviewText { get; set; }
    }
}
