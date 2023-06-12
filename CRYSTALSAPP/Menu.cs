using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRYSTALSAPP
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void ServerButton_Click(object sender, EventArgs e)
        {
            ServerForm form = new ServerForm();
            form.Show();
        }

        private void ClientButton_Click(object sender, EventArgs e)
        {
            ClientForm form = new ClientForm();
            form.Show();
        }
    }
}
