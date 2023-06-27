using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRYSTALSAPP
{
    internal class OrderItem
    {
        public int ID { get; set; }
        [DefaultValue(0)] public int Amount { get; set; }
    }
}
