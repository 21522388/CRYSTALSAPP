using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRYSTALSAPP
{
    internal class OrderRequest
    {
        public string CustomerName { get; set; }
        public string DeliveryAddress { get; set; }
        public List<OrderItem> OrderItems {get; set; }
    }
}
