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
    public partial class Signup : Form
    {
        string constring = "datasource=localhost;port=3306;username=root;password=dan5114";
        bool goBack = false;

        Form form1;
        

        public Signup(Form form)
        {
            InitializeComponent();
            password_txt.PasswordChar = '*';
            password_txt.MaxLength = 15;

            this.form1 = form;
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(fname_txt.Text) || 
              string.IsNullOrWhiteSpace(lname_txt.Text) ||
              string.IsNullOrWhiteSpace(dtp.Text) ||
              string.IsNullOrWhiteSpace(sex_txt.Text) ||
              string.IsNullOrWhiteSpace(address_txt.Text) ||
              string.IsNullOrWhiteSpace(city_txt.Text) ||
              string.IsNullOrWhiteSpace(phone_txt.Text) ||
              string.IsNullOrWhiteSpace(email_txt.Text) ||
              string.IsNullOrWhiteSpace(username_txt.Text) ||
              string.IsNullOrWhiteSpace(password_txt.Text))
            {
                MessageBox.Show("Please complete the signup form!");


            }
            else
            {
                string Query1 = "SELECT * FROM store.users where userName='"+this.username_txt.Text+"'";
                MySqlConnection conDataBase1 = new MySqlConnection(constring);
                MySqlCommand cmdDatabase1 = new MySqlCommand(Query1, conDataBase1);
                MySqlDataReader myReader1;
                int count = 0;
                try
                {
                    conDataBase1.Open();
                    myReader1 = cmdDatabase1.ExecuteReader();
                    while (myReader1.Read())
                    {
                        count = count + 1;
                    }
                } catch { }

                conDataBase1.Close();

                if (count > 0)
                {
                    MessageBox.Show("Username already exist"); 
                }
                else {
                    string Query = "insert into store.users (first_name, last_name, birth_day, sex, address, city, phone, email, userName, pass, role) values('" + this.fname_txt.Text + "','" + this.lname_txt.Text + "','" + dtp.Value.ToString("yyyy-MM-dd") + "','" + this.sex_txt.Text + "','" + this.address_txt.Text + "','" + this.city_txt.Text + "','" + this.phone_txt.Text + "','" + this.email_txt.Text + "','" + this.username_txt.Text + "','" + this.password_txt.Text + "','"+this.comboBox1.Text.ToLower()+"') ;";
                    MySqlConnection conDataBase = new MySqlConnection(constring);
                    MySqlCommand cmdDatabase = new MySqlCommand(Query, conDataBase);
                    MySqlDataReader myReader;
                    {

                    }
                    try
                    {
                        conDataBase.Open();
                        myReader = cmdDatabase.ExecuteReader();
                        MessageBox.Show("Congratulations! You have an account!");
                        MessageBox.Show("Proceed on Log In!");

                        goBack = true;
                        this.Close();
                        form1.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    conDataBase.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            goBack = true;
            this.Close();
            form1.Show();
        }

        private void Signup_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!goBack)
                Application.Exit();
        }
    }
}
