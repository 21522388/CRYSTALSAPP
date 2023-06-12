using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRYSTALSAPP
{
    public partial class DevConsole : Form
    {
        public DevConsole()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            PrintView.Items.Clear();
        }

        public void Print(string message)
        {
            ListViewItem item = new ListViewItem(message);
            PrintView.Items.Add(item);
        }
    }
}
