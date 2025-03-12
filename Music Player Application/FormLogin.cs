using Guna.UI2.WinForms;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Music_Player_Application
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
            this.pn_Login.Visible = true;
            this.pn_Register.Visible = false;
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            string username = guna2TextBox1.Text;
            string password = guna2TextBox2.Text;

            // Replace with your actual connection string
            string connectionString = "Data Source=DESKTOP-P6UD69C;Initial Catalog=MusicPlayerDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM Users WHERE Username=@Username AND PasswordHash=@PasswordHash";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@PasswordHash", password);

                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count == 1)
                {
                    Form1 form1 = new Form1();
                    form1.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid username or password");
                }
            }
        }

        private void btn_CreateAccount_Click(object sender, EventArgs e)
        {
            pn_Login.Visible = false;
            pn_Register.Visible = true;
        }

        private void btn_CreateAccountDone_Click(object sender, EventArgs e)
        {
            string username = guna2TextBox3.Text;
            string password = guna2TextBox4.Text;
            string email = guna2TextBox6.Text;

            // Replace with your actual connection string
            string connectionString = "Data Source=DESKTOP-P6UD69C;Initial Catalog=MusicPlayerDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Users (Username, Email, PasswordHash) VALUES (@Username, @Email, @PasswordHash)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@PasswordHash", password);

                connection.Open();
                command.ExecuteNonQuery();
                MessageBox.Show("Account created successfully");

                pn_Register.Visible = false;
                pn_Login.Visible = true;
            }
        }
    }
}