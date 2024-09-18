using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Muryo
{
    public partial class LoadingScreen : Form
    {
        public LoadingScreen()
        {
            InitializeComponent();
        }
        int startpoint = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            startpoint += 5;
            myprogress.Value = startpoint;
            if(myprogress.Value == 100)
            {
                myprogress.Value = 0;
                timer1.Stop();
                Main log = new Main();
                this.Hide();
                log.Show();

            }
        }

        private void LodingScreen_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}
