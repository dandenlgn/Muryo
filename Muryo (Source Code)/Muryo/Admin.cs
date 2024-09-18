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
    public partial class Admin : Form
    {

        string constring = "datasource=localhost;port=3306;username=root;database=store;password=dan5114";
        string selectedUserId, userName;

        bool goBack = false;

        Form form;


        public Admin(Form form, string userName)
        {
            InitializeComponent();
            this.form = form;
            this.userName = userName;

            FillUsers();
        }

        private void FillUsers()
        {            
            using (MySqlConnection sqlCon = new MySqlConnection(constring))
            {
                sqlCon.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter($"select userName from users where role='purchaser' OR role='vendor'", sqlCon);

                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);

                comboBox1.DataSource = dtbl;
                comboBox1.DisplayMember = "userName";
            }
        }

        private void Admin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!goBack)
               Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            goBack = true;
            this.Close();
            form.Show();
        }


        private void delete_acc_Click(object sender, EventArgs e)
        {
            MySqlConnection conDataBase = new MySqlConnection(constring);
            MySqlCommand cmdDatabase = new MySqlCommand($"delete from users where users_id='{selectedUserId}'", conDataBase);
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

        private void refreshbtn_Click(object sender, EventArgs e)
        {
            using (MySqlConnection sqlCon = new MySqlConnection(constring))
            {
                sqlCon.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter($"select * from users where role='purchaser' OR role='vendor'", sqlCon);

                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);

                dataGridView1.DataSource = dtbl;
                dataGridView1.Columns[0].HeaderCell.Value = "User Id";
                dataGridView1.Columns[1].HeaderCell.Value = "First Name";
                dataGridView1.Columns[2].HeaderCell.Value = "Last Name";
                dataGridView1.Columns[3].HeaderCell.Value = "Birthday";
                dataGridView1.Columns[4].HeaderCell.Value = "Sex";
                dataGridView1.Columns[5].HeaderCell.Value = "Address";
                dataGridView1.Columns[6].HeaderCell.Value = "City";
                dataGridView1.Columns[7].HeaderCell.Value = "Phone";
                dataGridView1.Columns[8].HeaderCell.Value = "Email";
                dataGridView1.Columns[9].HeaderCell.Value = "Username";
                dataGridView1.Columns[10].HeaderCell.Value = "Password";
                dataGridView1.Columns[11].HeaderCell.Value = "Role";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string Query = $"SELECT users_id FROM users WHERE userName='{comboBox1.Text}'";
            MySqlConnection conDataBase = new MySqlConnection(constring);
            MySqlCommand cmdDatabase = new MySqlCommand(Query, conDataBase);
            MySqlDataReader myReader;

            try
            {
                conDataBase.Open();
                myReader = cmdDatabase.ExecuteReader();
                while (myReader.Read())
                {
                    selectedUserId = myReader[0].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Error");
            }
            conDataBase.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Admin_edit_all_accounts editacc = new Admin_edit_all_accounts(this, userName);
            editacc.Show();
            this.Hide();
        }
    }
}
