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
    public partial class UI : Form
    {
        string constring = "datasource=localhost;port=3306;username=root;database=store;password=dan5114";
        string userName, role, category, id, selectedProductId;

        bool goBack = false;

        Form form;


        public UI(Form form, string id, string userName, string role)
        {
            InitializeComponent();

            this.userName = userName;
            this.role = role;
            this.id = id;
            this.form = form;

            label1.Text = userName.ToUpper();
            label2.Text = "Role: " + role;

            panel1.BringToFront();
            adminbtn.Visible = false;
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            ShowProducts();

            if(role == "vendor")
            {
                buyproductbtn.Visible = false;
                vieworderbtn.Visible = false;
                viewhistorybtn.Visible = false;
                panel4.Visible = false;
                panel5.Visible = false;
                panel6.Visible = false;

                FillProducts($"where users_id='{id}'", selectproduct);
            }
            else if(role == "admin")
            {
                addprodbtn.Visible = false;
                adminbtn.Location = addprodbtn.Location;
                adminbtn.Size = addprodbtn.Size;
                adminbtn.Visible = true;
                buyproductbtn.Visible = false;
                receivedbtn.Enabled = false;

                panel2.Visible = false;
                panel4.Visible = false;

                FillProducts("", selectproduct);
                FillProducts("", buyproduct);
                FillOrderNumbers();

            }
            else
            {
                FillProducts("", buyproduct);

                FillOrderNumbers();

                addprodbtn.Visible = false;
                editproductbtn.Visible = false;
                panel2.Visible = false;
                panel3.Visible = false;
            }
        }
        void FillOrderNumbers()
        {
            using (MySqlConnection sqlCon = new MySqlConnection(constring))
            {
                sqlCon.Open();

                string isAdmin = (role == "admin") ? "" : $"where users_id='{id}'";
                MySqlDataAdapter sqlDa = new MySqlDataAdapter($"select ordernumber from orders {isAdmin}", sqlCon);

                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);

                comboBox3.DataSource = dtbl;
                comboBox3.DisplayMember = "ordernumber";
            }
        }
        void FillProducts(string additonalQuery, ComboBox cb)
        {
            using (MySqlConnection sqlCon = new MySqlConnection(constring))
            {
                sqlCon.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter($"select prod_name from products {additonalQuery}", sqlCon);

                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);

                cb.DataSource = dtbl;
                cb.DisplayMember = "prod_name";
            }
        }

        void UpdateProduct()
        {
            string buildQuery = $"update products set ";
            if (!string.IsNullOrWhiteSpace(productname.Text))
                buildQuery += $"prod_name = '{productname.Text}',";
            if (!string.IsNullOrWhiteSpace(productqty.Text))
                buildQuery += $"prodQty = '{productqty.Text}',";
            if (!string.IsNullOrWhiteSpace(productdesc.Text))
                buildQuery += $"prodDesc = '{productdesc.Text}',";
           
            buildQuery += $"prodcategory = '{prodcategory.Text}'";
            buildQuery += $"where product_id = '{selectedProductId}'";

            ExecuteSQL(buildQuery, true, "Product updated", false);
        }
        void ShowProducts()
        {
            string isAdmin = (role == "admin") ? "" : $"where users_id='{id}'";
            if (role == "vendor" || role == "admin")
                FillProducts(isAdmin, selectproduct);

            category = (comboBox1.SelectedIndex > 0) ? $"where prodCategory = '{comboBox1.Text}'" : "";
            using (MySqlConnection sqlCon = new MySqlConnection(constring))
            {
                sqlCon.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter($"select prod_name, prodQty, prodCategory, prodDesc from products {category}", sqlCon);

                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);

                dataGridView1.DataSource = dtbl;
                dataGridView1.Columns[0].HeaderCell.Value = "Product Name";
                dataGridView1.Columns[1].HeaderCell.Value = "Quantity";
                dataGridView1.Columns[2].HeaderCell.Value = "Category";
                dataGridView1.Columns[3].HeaderCell.Value = "Description";
                dataGridView1.Columns[3].Width = 300;
            }
        }
        void ShowOrders()
        {
            using (MySqlConnection sqlCon = new MySqlConnection(constring))
            {
                sqlCon.Open();
                
                string isAdmin = (role == "admin") ? "select ordernumber, productqty, product_id, users_id from orders" : $"select ordernumber, productqty, product_id from orders where users_id='{id}'";
                MySqlDataAdapter sqlDa = new MySqlDataAdapter(isAdmin, sqlCon);

                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);

                dtbl.Columns.Add("Product Name", typeof(string));
                if (role == "admin")
                    dtbl.Columns.Add("Username", typeof(string));

                foreach (DataRow row in dtbl.Rows)
                {
                    string query = $"select prod_name from products where product_id='{row["product_id"]}'"; 
                    row["Product Name"] = ExecuteSQL(query, false, "", true);
                    if (role == "admin")
                    {
                        string query2 = $"select userName from users where users_id='{row["users_id"]}'";
                        row["Username"] = ExecuteSQL(query2, false, "", true);
                    }
                }

                dgv3.DataSource = dtbl;
                dgv3.Columns[0].HeaderCell.Value = "Order number";
                dgv3.Columns[1].HeaderCell.Value = "Quantity";
                dgv3.Columns.Remove("product_id");
                if(role=="admin")
                    dgv3.Columns.Remove("users_id");
                dgv3.Columns[0].Width = 120;
            }
        }
        void ShowHistory()
        {
            using (MySqlConnection sqlCon = new MySqlConnection(constring))
            {
                sqlCon.Open();
                string isAdmin = (role == "admin") ? "select ordernumber, productqty, prod_name, users_id from history" : $"select ordernumber, productqty, prod_name from history where users_id='{id}'";
                MySqlDataAdapter sqlDa = new MySqlDataAdapter(isAdmin, sqlCon);

                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);

                if (role == "admin")
                {
                    dtbl.Columns.Add("Username", typeof(string));
                    foreach (DataRow row in dtbl.Rows)
                    {
                        string query2 = $"select userName from users where users_id='{row["users_id"]}'";
                        row["Username"] = ExecuteSQL(query2, false, "", true);
                    }
                }

                dgv2.DataSource = dtbl;
                dgv2.Columns[0].HeaderCell.Value = "Order number";
                dgv2.Columns[1].HeaderCell.Value = "Quantity";
                dgv2.Columns[2].HeaderCell.Value = "Product Name";
                if(role == "admin")
                dgv2.Columns.Remove("users_id");
                dgv2.Columns[0].Width = 120;
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowProducts();
        }

        private void addprodbtn_Click(object sender, EventArgs e)
        {
            panel2.BringToFront();
        }

        private void showstore_Click(object sender, EventArgs e)
        {
            panel1.BringToFront();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ShowProducts();
        }

        private void editproductbtn_Click(object sender, EventArgs e)
        {
            panel3.BringToFront();
            ShowProducts();
        }

        private void deleteproduct_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure you want to delete?", "Confirmation", MessageBoxButtons.YesNo);

            if (dr == DialogResult.Yes)
            {
                string Query = $"DELETE FROM products WHERE product_id ='{selectedProductId}';";
                ExecuteSQL(Query, true, "Product deleted", false);
            }
        }

        private void savechanges_Click(object sender, EventArgs e)
        {
            UpdateProduct();
        }

        private void buyproductbtn_Click(object sender, EventArgs e)
        {
            panel4.BringToFront();
        }

        string ExecuteSQL(string Query, bool showMessage, string customMessage, bool readValue)
        {
            MySqlConnection conDataBase = new MySqlConnection(constring);
            MySqlCommand cmdDatabase = new MySqlCommand(Query, conDataBase);
            MySqlDataReader myReader;

            string returnVal="";

            try
            {
                
                conDataBase.Open();
                myReader = cmdDatabase.ExecuteReader();
                if(showMessage)
                    MessageBox.Show(customMessage);
                if (readValue)
                {
                    while (myReader.Read())
                    {
                         returnVal= myReader[0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),"line 192");
            }
            conDataBase.Close();

            return returnVal;
        }

        private void buybtn_Click(object sender, EventArgs e)
        {
            string Query = $"SELECT product_id FROM products WHERE prod_name='{buyproduct.Text}';";
            GetProductId(Query);

            int inputqty = 0;
            if (int.TryParse(buyerqty.Text, out inputqty))
            {
                try
                {
                    string query = $"SELECT prodQty FROM products WHERE product_id='{selectedProductId}'";
                    int prodRealQty = Convert.ToInt32(ExecuteSQL(query, false, "", true));

                    if (inputqty <= prodRealQty)
                    {
                        if (prodRealQty == 0)
                        {
                            MessageBox.Show("Out of stock");
                        }
                        else
                        {
                            int newVal = prodRealQty - inputqty;

                            string query2 = $"UPDATE products SET prodQty='{newVal}' WHERE product_id='{selectedProductId}'";
                            ExecuteSQL(query2, false, "", false);

                            Random rnd = new Random();
                            int ordernum = rnd.Next(10000, 99999);

                            string query3 = $"insert into store.orders (ordernumber, productqty, product_id, users_id) values('{ordernum}', '{inputqty}','{selectedProductId}', '{id}');";
                            ExecuteSQL(query3, false, "", false);

                            string query4 = $"insert into store.history (ordernumber, productqty, prod_name, users_id) values('{ordernum}', '{inputqty}','{ExecuteSQL($"select prod_name from products where product_id='{selectedProductId}'", false, "", true)}', '{id}');";
                            ExecuteSQL(query4, true, "Order submitted", false);
                        }
                    }
                    else
                        MessageBox.Show("Product doesnt have enough stock");
                }catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "Error");
                }
            }
            else
                MessageBox.Show("Invalid product quantity");
        }
        void GetProductId(string Query)
        {
            MySqlConnection conDataBase = new MySqlConnection(constring);
            MySqlCommand cmdDatabase = new MySqlCommand(Query, conDataBase);
            MySqlDataReader myReader;

            try
            {
                conDataBase.Open();
                myReader = cmdDatabase.ExecuteReader();
                while (myReader.Read())
                {
                    selectedProductId = myReader[0].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Error");
            }
            conDataBase.Close();
        }

        private void buyproduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            string Query = $"SELECT product_id FROM products WHERE prod_name='{buyproduct.Text}';";
            GetProductId(Query);
        }

        private void vieworderbtn_Click(object sender, EventArgs e)
        {
            FillOrderNumbers();
            ShowOrders();
            panel6.BringToFront();
        }

        private void refreshbtn_Click(object sender, EventArgs e)
        {
            ShowHistory();
        }

        private void label16_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure you want to logout?", "Confirmation", MessageBoxButtons.YesNo);

            if (dr == DialogResult.Yes)
            {
                goBack = true;
                this.Close();
                form.Show();
            }
        }

        private void receivedbtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(comboBox3.Text))
            {
                string deleteQuery = $"DELETE FROM orders WHERE ordernumber ='{comboBox3.Text}';";
                ExecuteSQL(deleteQuery, true, "Product has received", false);
                ShowOrders();
            }
        }

        private void historybtn_Click(object sender, EventArgs e)
        {
            panel5.BringToFront();
        }

        private void adminbtn_Click(object sender, EventArgs e)
        {
            Admin admin = new Admin(this, userName);
            admin.Show();
            this.Hide();
        }


        private void selectproduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            string isAdmin = (role == "admin") ? "" : $"AND users_id = '{id}'";
            string Query = $"SELECT product_id FROM products WHERE prod_name='{selectproduct.Text}' {isAdmin};";
            GetProductId(Query);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int userVal = 0;
            if (int.TryParse(textBox2.Text, out userVal))
            {
                string Query = "insert into store.products (prod_name, prodQty, prodCategory, prodDesc, users_id) values('" + this.textBox1.Text + "','" + this.textBox2.Text + "','" + this.comboBox2.Text + "','" + this.textBox3.Text.Replace("'", "") + "','"+ id +"') ;";
                ExecuteSQL(Query, true, "Product submitted", false);
            }
            else
                MessageBox.Show("Invalid product quantity");
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Update updated = new Update(this, userName);
            updated.Show();
        }

        private void UI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!goBack)
                Application.Exit();
        }
    }
}
