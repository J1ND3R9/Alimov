using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
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

namespace TRPO
{
    public class Product
    {
        public int localID { get; set; }
        public long ID { get; set; }
        public string ImagePath { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Kkal { get; set; }
        public float Price { get; set; }
        public long Category { get; set; }
        public short Quantity { get; set; }

        public bool inBasket => Quantity > 0;
        public float TotalPrice => Price * Quantity;
    }
    public partial class MainWindow : Window
    {
        public static bool adminStatus = false;
        public static bool inAccount = false;
        public static int j = 0;

        public static string connectionString = "Data Source=localhost;Initial Catalog=Konditerka;Integrated Security=True;";
        public MainWindow()
        {
            InitializeComponent();
            debugTextBlockCategory.Text = "0";
            ListBoxItemsProduct.ItemsSource = GetProducts();
        }

        private IEnumerable GetProducts(int category = 0, string text = "")
        {
            List<Product> products = new List<Product>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Products";
                if (category > 0)
                {
                    query += $" WHERE Category = {category}";
                }
                if (text != "")
                {
                    if (category == 0)
                        query += $" WHERE Name LIKE ('%{text}%')";
                    else
                        query += $" AND Name LIKE ('%{text}%')";
                }
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    var reader = command.ExecuteReader();
                    int i = 0;
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            localID = i,
                            ID = reader.GetInt64(0),
                            Name = reader.GetString(1),
                            Kkal = (double)reader.GetDecimal(2),
                            ImagePath = FixedImagePath(reader.GetString(3)),
                            Price = (float)reader.GetDecimal(4),
                            Description = reader.GetString(5),
                            Category = reader.GetInt64(6),
                            Quantity = 0
                        });
                        i++;
                    }
                }
                connection.Close();
            }
            return products;
        }
        private string FixedImagePath(string imagePath)
        {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
        }
        private void AccountLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow w = new LoginWindow(Logo, GridHeader2);
            w.Owner = this;
            Effects.DarkEffect(this);

            w.ShowDialog();

            Effects.ClearEffect(this);
        }
        private void CategoryAll_Click(object sender, RoutedEventArgs e)
        {
            debugTextBlockCategory.Text = "0";
            ProductListCategories();
        }
        private void CategoryCakes_Click(object sender, RoutedEventArgs e)
        {
            debugTextBlockCategory.Text = "1";
            ProductListCategories();
        }
        private void CategoryMarshmellow_Click(object sender, RoutedEventArgs e)
        {
            debugTextBlockCategory.Text = "2";
            ProductListCategories();
        }
        private void CategoryChocolate_Click(object sender, RoutedEventArgs e)
        {
            debugTextBlockCategory.Text = "3";
            ProductListCategories();
        }

        private void ProductListCategories() => ListBoxItemsProduct.ItemsSource = GetProducts(Convert.ToInt32(debugTextBlockCategory.Text), SearchBox.Text);

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ProductListCategories();

        private void AddProductToBusket_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int i = Convert.ToInt32(button.Tag);
            Product product = (Product)ListBoxItemsProduct.Items[i];
            product.Quantity++;
            product.localID = j;
            Basket.AddedProducts.Add(product);
            j++;
            List<Product> products = (List<Product>)GetProducts();
            for (int j = 0; j < products.Count; j++)
            {
                if (Basket.AddedProducts.Any(s => s.ID == products[j].ID))
                    products[j].Quantity++;
            }
            ListBoxItemsProduct.ItemsSource = products;
        }


        private void BasketButton_Click(object sender, RoutedEventArgs e)
        {
            Basket w = new Basket(inAccount);
            w.Owner = this;
            Effects.DarkEffect(this);

            w.ShowDialog();

            Effects.ClearEffect(this);
            List<Product> products = (List<Product>)GetProducts();
            for (int j = 0; j < products.Count; j++)
            {
                if (Basket.AddedProducts.Any(s => s.ID == products[j].ID))
                    products[j].Quantity++;
            }
            ListBoxItemsProduct.ItemsSource = products;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            long id = (long)button.Tag;
            using (SqlConnection  conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = $"DELETE FROM Products WHERE ID = {id}";
                string query1 = $"SELECT * FROM Products WHERE ID = {id}";
                using (SqlCommand cmd1 = new SqlCommand(query1, conn))
                {
                    object result = cmd1.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Товар не существует", "Ошибка");
                        ProductListCategories();
                        conn.Close();
                        return;
                    }
                }
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Товар удален", "Успешно");
                    ProductListCategories();
                    conn.Close();
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string query = $"SELECT * FROM Products WHERE ID = {button.Tag}";
            Product product = new Product();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        product.ID = reader.GetInt64(0);
                        product.Name = reader.GetString(1);
                        product.Kkal = (double)reader.GetDecimal(2);
                        product.ImagePath = FixedImagePath(reader.GetString(3));
                        product.Price = (float)reader.GetDecimal(4);
                        product.Description = reader.GetString(5);
                        product.Category = reader.GetInt64(6);
                    }
                }
            }
            EditAddProduct w = new EditAddProduct(product);
            w.Owner = this;
            Effects.DarkEffect(this);

            w.ShowDialog();

            Effects.ClearEffect(this);
            ProductListCategories();
        }

        private void AddButtonProduct_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            EditAddProduct w = new EditAddProduct();
            w.Owner = this;
            Effects.DarkEffect(this);

            w.ShowDialog();

            Effects.ClearEffect(this);
            ProductListCategories();
        }
    }

}
