using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace Music_Player_Application
{
    public partial class FormLogin : Form
    {
        private string verificationCode;

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

        private void btn_VerifyEmail_Click(object sender, EventArgs e)
        {
            string email = guna2TextBox5.Text.Trim();
            
            // Validate email
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please enter your email address.", "Validation Error");
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error");
                return;
            }

            // Generate and send verification code
            verificationCode = GenerateVerificationCode();
            SendVerificationCode(email, verificationCode);
            
            // Enable verification code input
            txt_VerificationCode.Enabled = true;
            btn_CreateAccountDone.Enabled = true;
            
            MessageBox.Show("Verification code has been sent to your email.", "Success");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void btn_CreateAccountDone_Click(object sender, EventArgs e)
        {
            string username = guna2TextBox3.Text.Trim();
            string password = guna2TextBox4.Text.Trim();
            string email = guna2TextBox5.Text.Trim();
            string enteredCode = txt_VerificationCode.Text.Trim();

            // Validate inputs
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || 
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(enteredCode))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error");
                return;
            }

            if (enteredCode != verificationCode)
            {
                MessageBox.Show("Invalid verification code. Please check your email and try again.", "Validation Error");
                return;
            }

            try
            {
                // Replace with your actual connection string
                string connectionString = "Data Source=DESKTOP-P6UD69C;Initial Catalog=MusicPlayerDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Check if username already exists
                    string checkQuery = "SELECT COUNT(1) FROM Users WHERE Username=@Username";
                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@Username", username);
                    
                    connection.Open();
                    int existingUsers = Convert.ToInt32(checkCommand.ExecuteScalar());
                    
                    if (existingUsers > 0)
                    {
                        MessageBox.Show("Username already exists. Please choose another username.", "Registration Error");
                        return;
                    }

                    // Insert new user
                    string insertQuery = "INSERT INTO Users (Username, Email, PasswordHash) VALUES (@Username, @Email, @PasswordHash)";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@Username", username);
                    insertCommand.Parameters.AddWithValue("@Email", email);
                    insertCommand.Parameters.AddWithValue("@PasswordHash", password); // Consider using proper password hashing

                    insertCommand.ExecuteNonQuery();
                    MessageBox.Show("Account created successfully!", "Success");

                    // Reset and switch to login panel
                    ResetRegistrationForm();
                    pn_Register.Visible = false;
                    pn_Login.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while creating your account.\nError: {ex.Message}", 
                    "Database Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        private void ResetRegistrationForm()
        {
            guna2TextBox3.Text = "";
            guna2TextBox4.Text = "";
            guna2TextBox5.Text = "";
            txt_VerificationCode.Text = "";
            verificationCode = null;
            txt_VerificationCode.Enabled = false;
            btn_CreateAccountDone.Enabled = false;
        }

        private string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private void SendVerificationCode(string recipientEmail, string code)
        {
            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    // Cấu hình SMTP
                    smtpClient.Host = EmailConfig.SmtpServer;
                    smtpClient.Port = EmailConfig.SmtpPort;
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(EmailConfig.SenderEmail, EmailConfig.SenderPassword);

                    // Tạo email message
                    using (var message = new MailMessage())
                    {
                        message.From = new MailAddress(EmailConfig.SenderEmail, EmailConfig.SenderName);
                        message.Subject = "SoundSphere - Email Verification Code";
                        message.Body = $@"
                            <h2>Welcome to SoundSphere!</h2>
                            <p>Your verification code is: <strong>{code}</strong></p>
                            <p>This code will expire in 10 minutes.</p>
                            <p>If you didn't request this code, please ignore this email.</p>";
                        message.IsBodyHtml = true;
                        message.To.Add(recipientEmail);

                        // Gửi email
                        smtpClient.Send(message);
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Failed to send verification code.\n\nError Details:\n{ex.Message}";
                MessageBox.Show(errorMessage, "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Log error for debugging
                Console.WriteLine($"Email Error: {ex}");
            }
        }
    }
}