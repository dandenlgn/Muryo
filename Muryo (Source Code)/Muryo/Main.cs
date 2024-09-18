using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace Muryo
{
    public partial class Main : Form
    {
        #region DRAGGABLE FORM 
       
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        string id;

        public Main()
        {
            InitializeComponent();
            password_txt.PasswordChar = '*';
            password_txt.MaxLength = 15;
            comboBox1.SelectedIndex = 0;

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                string myConnection = "datasource=localhost;port=3306;username=root;password=dan5114";
                MySqlConnection myConn = new MySqlConnection(myConnection);
                MySqlCommand SCommand = new MySqlCommand("select * from store.users where userName='" + this.username_txt.Text + "' and pass='" + this.password_txt.Text + "'and role= '" + comboBox1.Text.ToLower() + "';", myConn);
                MySqlDataReader myReader;

                myConn.Open();
                myReader = SCommand.ExecuteReader();
                int count = 0;
                while (myReader.Read())
                {
                    count = count + 1;
                    id = myReader[0].ToString();
                }
                if (count == 1)
                {
                    MessageBox.Show("Username and Password is correct");
                    this.Hide();
                    UI frontpage = new UI(this, id, username_txt.Text, comboBox1.Text.ToLower());
                    frontpage.Show();
             
                }
                else
                    MessageBox.Show("Username and Password are incorrect ... Please try again");
                myConn.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.BackColor = Color.FromArgb(100, 0, 0, 0);
        }

        private void label4_Click(object sender, EventArgs e)
        {
            Signup registerform = new Signup(this);
            registerform.Show();
            this.Hide();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #region DRAGGABLE FORM
        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion
    }
}
