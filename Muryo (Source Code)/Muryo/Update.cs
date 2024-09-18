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
    public partial class Update : Form
    {
        string constring = "datasource=localhost;port=3306;username=root;database=store;password=dan5114";
        string userName;

        bool goBack = false;
        Form form;

        public Update(Form form, string userName)
        {
            InitializeComponent();
            password_txt.PasswordChar = '*';
            password_txt.MaxLength = 15;

            this.userName = userName;
            this.form = form;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string buildQuery = $"update users set "; //"update "+ role + " set";
            if (!string.IsNullOrWhiteSpace(fname_txt.Text))
                buildQuery += $"first_name = '{fname_txt.Text}',";
            if (!string.IsNullOrWhiteSpace(lname_txt.Text))
                buildQuery += $"last_name = '{lname_txt.Text}',";
            if (!string.IsNullOrWhiteSpace(sex_txt.Text))
                buildQuery += $"sex = '{sex_txt.Text}',";
            if (!string.IsNullOrWhiteSpace(address_txt.Text))
                buildQuery += $"address = '{address_txt.Text}',";
            if (!string.IsNullOrWhiteSpace(city_txt.Text))
                buildQuery += $"city = '{city_txt.Text}',";
            if (!string.IsNullOrWhiteSpace(phone_txt.Text))
                buildQuery += $"phone = '{phone_txt.Text}',";
            if (!string.IsNullOrWhiteSpace(email_txt.Text))
                buildQuery += $"email = '{email_txt.Text}',";
            if (!string.IsNullOrWhiteSpace(username_txt.Text))
                buildQuery += $"userName = '{username_txt.Text}',";
            if (!string.IsNullOrWhiteSpace(password_txt.Text))
                buildQuery += $"pass = '{password_txt.Text}',";

            buildQuery += $"birth_day = '{bday_txt.Value.ToString("yyyy-MM-dd")}'";
            buildQuery += $"where userName='{userName}'";
           
            MySqlConnection conDataBase = new MySqlConnection(constring);
            MySqlCommand cmdDatabase = new MySqlCommand(buildQuery, conDataBase);
            MySqlDataReader myReader;

            try
            {
                conDataBase.Open();
                myReader = cmdDatabase.ExecuteReader();
                MessageBox.Show("Your Action has been Saved");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (MySqlConnection sqlCon = new MySqlConnection(constring))
            {
                sqlCon.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter($"select first_name, last_name, birth_day, sex, address, city, phone, email, userName, pass from users where userName='{userName}'", sqlCon);
                
                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);
                
                dgv2.DataSource = dtbl;
                dgv2.Columns[0].HeaderCell.Value = "First Name";
                dgv2.Columns[1].HeaderCell.Value = "Last Name";
                dgv2.Columns[2].HeaderCell.Value = "Birthday";
                dgv2.Columns[3].HeaderCell.Value = "Sex";
                dgv2.Columns[4].HeaderCell.Value = "Address";
                dgv2.Columns[5].HeaderCell.Value = "City";
                dgv2.Columns[6].HeaderCell.Value = "Phone";
                dgv2.Columns[7].HeaderCell.Value = "Email";
                dgv2.Columns[8].HeaderCell.Value = "Username";
                dgv2.Columns[9].HeaderCell.Value = "Password";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            goBack = true;
            this.Close();
            form.Show();
        }

        private void Update_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!goBack) // goBack == false
                Application.Exit();
        }
    }
}
