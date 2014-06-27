using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Helia_1_5_server
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            Run();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Run();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
        }

        void Run()
        {
            manager.start();
        }

        void Stop()
        {
            manager.stop();
        }

       
    }
}
