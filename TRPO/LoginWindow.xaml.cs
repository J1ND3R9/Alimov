using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TRPO
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public Image Logo;
        public Grid InAcc;
        public LoginWindow(Image logo, Grid grid)
        {
            InitializeComponent();
            this.PreviewKeyDown += CheckEscapeButton;
            Logo = logo;
            InAcc = grid;
        }

        private void CheckEscapeButton(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = TextBoxLogin.Text.ToLower();
            using (SqlConnection connection = new SqlConnection(MainWindow.connectionString))
            {
                connection.Open();
                string query = "SELECT Username FROM Users WHERE Username = @username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", SqlDbType.NVarChar).Value = username;
                    object result = command.ExecuteScalar();
                    connection.Close();
                    if (result == null)
                    {
                        LoginErrorTextBlock.Visibility = Visibility.Visible;
                        return;
                    }
                }
            }
            string password = TextBoxPassword.Text;
            if (password == null || password == "" || password == " ")
            {
                PasswordErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashPassword = SHA256.Create().ComputeHash(passwordBytes);
            using (SqlConnection connection1 = new SqlConnection(MainWindow.connectionString))
            {
                connection1.Open();
                string query = "SELECT Username, Password FROM Users WHERE Username = @username AND Password = @password";
                using (SqlCommand cmd = new SqlCommand(query, connection1))
                {
                    string hashPasswordString = "";
                    for (int i = 0; i < hashPassword.Length; i++)
                    {
                        hashPasswordString += hashPassword[i];
                    }
                    cmd.Parameters.AddWithValue("@username", SqlDbType.NVarChar).Value = username;
                    cmd.Parameters.AddWithValue("@password", SqlDbType.Char).Value = hashPasswordString;
                    object result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        connection1.Close();
                        
                        PasswordErrorTextBlock.Visibility = Visibility.Visible;
                        return;
                    }

                }
                query = "SELECT AdminStatus FROM Users WHERE Username = @username";
                using (SqlCommand cmd = new SqlCommand(query, connection1))
                {
                    cmd.Parameters.AddWithValue("@username", SqlDbType.NVarChar).Value = username;
                    object result = cmd.ExecuteScalar();
                    connection1.Close();
                    if ((bool)result)
                    {
                        MainWindow.adminStatus = true;
                        Logo.Tag = MainWindow.adminStatus;
                    }

                }
            }
            MainWindow.inAccount = true;
            InAcc.Tag = MainWindow.inAccount;
            Close();
        }
    }
}
