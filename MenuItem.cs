using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRYSTALSAPP
{
    internal class MenuItem
    {
        public int ID { get; set; }
        public string ItemName { get; set; }
        public int Price { get; set; }
        public bool Available { get; set; }
        [DefaultValue(0)] public int Amount { get; set; }
    }
}
