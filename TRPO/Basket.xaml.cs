using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для Basket.xaml
    /// </summary>
    public partial class Basket : Window
    {
        public static ObservableCollection<Product> AddedProducts = new ObservableCollection<Product>();
        public Basket(bool inAccount)
        {
            InitializeComponent();
            textblockWithStatus.Tag = inAccount;
            ProductsInBasket.ItemsSource = AddedProducts;
            float total = 0;
            for (int i = 0; i < AddedProducts.Count; i++)
            {
                total += AddedProducts[i].Price;
            }
            EndPrice.Text = total.ToString();
        }

        private void RemoveQuantity_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int i = Convert.ToInt32(button.Tag);
            AddedProducts[i].Quantity -= 1;
            if (AddedProducts[i].Quantity == 0)
            {
                AddedProducts.RemoveAt(i);
                MainWindow.j--;
            }
            ProductsInBasket.ItemsSource = AddedProducts;
            float total = 0;
            for (int j = 0; j < AddedProducts.Count; j++)
            {
                total += AddedProducts[j].TotalPrice;
            }
            EndPrice.Text = total.ToString();
        }

        private void AddQuantity_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int i = Convert.ToInt32(button.Tag);
            AddedProducts[i].Quantity += 1;
            ProductsInBasket.ItemsSource = AddedProducts;
            float total = 0;
            for (int j = 0; j < AddedProducts.Count; j++)
            {
                total += AddedProducts[j].TotalPrice;
            }
            EndPrice.Text = total.ToString();
        }
    }
}
