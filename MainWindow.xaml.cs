using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PasswordGenerator
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            addDataGrid();
        }
        //list for option without database
        //ObservableCollection<Password> listPassword = new ObservableCollection<Password>(); 
        string chars;
        SqlConnection connection = new SqlConnection(@"Server=DESKTOP-2I2V4OF; Database=Password; Trusted_Connection=True;");

        private void generateBTN_Click(object sender, RoutedEventArgs e)
        {
            if((bool)lowercaseCB.IsChecked || (bool)uppercaseCB.IsChecked || (bool)specialSignsCB.IsChecked || (bool)numbersCB.IsChecked)
            {
                if (lengthPasswordTB.Text == string.Empty)
                {
                    lengthPasswordTB.Background = new SolidColorBrush(Colors.OrangeRed);
                    MessageBox.Show("Enter the password length.");
                }
                else
                {
                    string lengthPassword = lengthPasswordTB.Text;
                    int lengthPasswordInt = int.Parse(lengthPassword);

                    //string chars = "abcdefghijklmnopqrstuwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    Random random = new Random();
                    String result = new string(Enumerable.Repeat(chars, lengthPasswordInt).Select(x => x[random.Next(x.Length)]).ToArray());

                    passwordLBL.FontStyle = FontStyles.Normal;
                    passwordLBL.FontSize = 16;
                    passwordLBL.Text = result;
                }
            }
            else
            {
                MessageBox.Show("Choose the password option.");
            }
        }

        private void lengthPasswordTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            lengthPasswordTB.Background = new SolidColorBrush(Colors.White);
        }

        private void addDataGrid()
        {
            connection.Open();
            String sql = "select id, name, generatedPassword from passwords";
            SqlCommand command = new SqlCommand(sql, connection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable("Password");
            dataAdapter.Fill(dataTable);
            passwordDG.ItemsSource = dataTable.DefaultView;
            connection.Close();
        }
        private void saveBTN_Click(object sender, RoutedEventArgs e)
        {
            if (namePasswordTB.Text == string.Empty || passwordLBL.Text=="password")
            {
                namePasswordTB.Background = new SolidColorBrush(Colors.OrangeRed);
                MessageBox.Show("Enter the password name or generate new.");
            }
            else
            {
                //option without database check xaml code
                string passwordGenerated = passwordLBL.Text;
                string passwordName = namePasswordTB.Text;
                /*listPassword.Add(new Password() { generatedPassword = passwordGenerated, name = passwordName });
                passwordLV.ItemsSource = listPassword;*/

                //option with database
                connection.Open();
                String sql = "insert into passwords (name, generatedPassword) values (@name, @generatedPassword)";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("name", passwordName);
                command.Parameters.AddWithValue("generatedPassword", passwordGenerated);
                command.ExecuteNonQuery();
                connection.Close();
                addDataGrid();
                namePasswordTB.Text = string.Empty;
            }
        }
        private void namePasswordTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            namePasswordTB.Background = new SolidColorBrush(Colors.White);
        }

        private void deletePassword_Click(object sender, RoutedEventArgs e)
        {
            //option without database check xaml code
            /*Button button = (Button)sender;
            Password deletePassword = (Password)button.DataContext;
            listPassword.Remove(deletePassword);*/

            //option with database
            DataRowView row = (DataRowView)passwordDG.SelectedItems[0];
            int id = (int)row["id"];
            string name = (string)row["name"];
            connection.Open();
            MessageBoxResult result = MessageBox.Show("Delete password for " + name,"Are you sure?", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    String sql = "delete from passwords where id ="+id;
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                    addDataGrid();
                    break;
                case MessageBoxResult.No:
                    connection.Close();
                    break;
            }
        }

        //lowercase options
        private void lowercase_Checked(object sender, RoutedEventArgs e)
        {
            string lowercase = "qwertyuiopasdfghjklzxcvbnm";
            chars = chars + lowercase;
        }
        private void lowercase_UnChecked(object sender, RoutedEventArgs e)
        {
            chars = chars.Replace("qwertyuiopasdfghjklzxcvbnm", "");
        }

        //uppercase options
        private void uppercase_Checked(object sender, RoutedEventArgs e)
        {
            string uppercase = "QWERTYUIOPASDFGHJKLZXCVBNM";
            chars = chars + uppercase;
        }
        private void uppercase_UnChecked(object sender, RoutedEventArgs e)
        {
            chars = chars.Replace("QWERTYUIOPASDFGHJKLZXCVBNM", "");
        }

        //special signs options
        private void specialSigns_Checked(object sender, RoutedEventArgs e)
        {
            string specialSigns = "!$%&*^.,><";
            chars = chars + specialSigns;
        }
        private void specialSigns_UnChecked(object sender, RoutedEventArgs e)
        {
            chars = chars.Replace("!$%&*^.,><", "");
        }
        //numbers options
        private void numbers_Checked(object sender, RoutedEventArgs e)
        {
            string numbers = "1234567890";
            chars = chars + numbers;
        }

        private void numbers_UnChecked(object sender, RoutedEventArgs e)
        {
            chars = chars.Replace("1234567890", "");
        }
    }
}
