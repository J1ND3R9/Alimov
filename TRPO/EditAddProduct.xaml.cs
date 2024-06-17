using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace TRPO
{
    /// <summary>
    /// Логика взаимодействия для EditAddProduct.xaml
    /// </summary>
    public partial class EditAddProduct : Window
    {
        public Product product = null;
        public EditAddProduct(Product product = null)
        {
            InitializeComponent();
            cbCategories.ItemsSource = GetCategories();
            if (product == null)
            {
                textBlockTitle.Text = "Добавление продукта";
                this.Title = "Добавление";
            }
            else
            {
                this.product = product;
                textBlockTitleProduct.Text = product.Name;
                textBlockDescription.Text = product.Description;
                priceTextBlock.Text = product.Price.ToString();
                AddButton.Content = "Изменить";
                AddButton.Tag = product.ID;
                caloriesTextBlock.Text = product.Kkal.ToString();
            }

        }

        private IEnumerable<string> GetCategories()
        {
            var categories = new List<string>();
            string query = "SELECT Name FROM Categories ORDER BY ID";
            using (SqlConnection con = new SqlConnection(MainWindow.connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        categories.Add(reader.GetString(0));
                    }
                }
            }
            return categories;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = textBlockTitleProduct.Text;
            if (!isUnique(name))
            {
                if (this.product == null)
                {
                    MessageBox.Show("Товар с таким наименованием уже есть в базе данных");
                    textBlockTitle.Focus();
                    return;
                }
            }
            if (cbCategories.SelectedIndex == -1)
            {
                MessageBox.Show("Вы не выбрали категорию товара");
                cbCategories.Focus();
                return;
            }
            string category = cbCategories.SelectedItem.ToString();
            float price;
            try
            {
                price = (float)Convert.ToDecimal(priceTextBlock.Text);
            }
            catch
            {
                MessageBox.Show("Ошибка преобразования цены");
                priceTextBlock.Focus();
                return;
            }
            if (price <= 0)
            {
                MessageBox.Show("Цена не может быть менее 1 рубля!");
                priceTextBlock.Focus();
                return;

            }
            double kkal;
            try
            {
                kkal = Convert.ToDouble(caloriesTextBlock.Text);
            }
            catch
            {
                MessageBox.Show("Ошибка преобразования калорий");
                caloriesTextBlock.Focus();
                return;
            }
            if (kkal <= 0)
            {
                MessageBox.Show("Калории не могут быть меньше 1!");
                caloriesTextBlock.Focus();
                return;
            }
            long id = GetCategoryID(category);
            if (id == -1)
            {
                MessageBox.Show("Категория не доступна. Обновление...");
                cbCategories.ItemsSource = GetCategories();
                cbCategories.Focus();
                return;
            }

            string query = "";
            if (this.product != null)
            {
                query = "UPDATE Products " +
                    "SET Name = @name, Calories = @calories, Price = @price, Description = @description, Category = @category " +
                    "WHERE ID = @id";
            }
            else
            {
                query = "INSERT INTO Products VALUES (@name, @calories, @image, @price, @description, @category)";
            }
            using (SqlConnection con = new SqlConnection(MainWindow.connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.AddWithValue("@calories", SqlDbType.Decimal).Value = (decimal)kkal;
                    cmd.Parameters.AddWithValue("@image", SqlDbType.NVarChar).Value = "-";
                    cmd.Parameters.AddWithValue("@price", SqlDbType.Decimal).Value = (decimal)price;
                    cmd.Parameters.AddWithValue("@description", SqlDbType.NVarChar).Value = textBlockDescription.Text;
                    cmd.Parameters.AddWithValue("@category", SqlDbType.BigInt).Value = GetCategoryID(cbCategories.SelectedItem.ToString());
                    if (product != null)
                        cmd.Parameters.AddWithValue("@id", SqlDbType.BigInt).Value = AddButton.Tag;
                    cmd.ExecuteNonQuery();
                }
                if (product == null)
                    MessageBox.Show("Товар добавлен");
                else
                    MessageBox.Show("Товар изменен");
                Close();
            }

        }

        private long GetCategoryID(string category)
        {
            long id;
            string query = "SELECT ID FROM Categories WHERE Name = @name";
            using (SqlConnection con = new SqlConnection(MainWindow.connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@name", SqlDbType.NVarChar).Value = category;
                    object result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        id = -1;
                    }
                    else
                    {
                        id = (long)result;
                    }
                }
            }
            return id;
        }

        private bool isUnique(string name)
        {
            string query = "SELECT * FROM Products WHERE Name = @name";
            using (SqlConnection con = new SqlConnection(MainWindow.connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@name", SqlDbType.NVarChar).Value = name;
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        return false;
                }
            }
            return true;
        }
    }
}
