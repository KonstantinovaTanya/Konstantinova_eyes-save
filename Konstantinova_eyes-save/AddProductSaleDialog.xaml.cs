using System;
using System.Collections.Generic;
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

namespace Konstantinova_eyes_save
{
    /// <summary>
    /// Логика взаимодействия для AddProductSaleDialog.xaml
    /// </summary>
    public partial class AddProductSaleDialog : Window
    {
        public Product SelectedProduct { get; private set; }
        public int ProductCount { get; private set; }
        public DateTime SaleDate { get; private set; }
        public AddProductSaleDialog()
        {
            InitializeComponent();
            ProductCombo.ItemsSource = Konstantinova_eyesEntities.GetContext().Product.ToList();
            DatePicker.SelectedDate = DateTime.Now;
        }

        private void AddSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProductCombo.SelectedItem == null)
            {
                MessageBox.Show("Выберите продукт", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(CountTextBox.Text, out int count) || count <= 0)
            {
                MessageBox.Show("Введите корректное количество (целое положительное число)",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedProduct = (Product)ProductCombo.SelectedItem;
            ProductCount = count;
            SaleDate = DatePicker.SelectedDate.Value;

            DialogResult = true;
            Close();
        }

        private void TBoxSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            var currentProducts = Konstantinova_eyesEntities.GetContext().Product.ToList();

            string searchText = TBoxSearchProduct.Text?.ToLower() ?? "";

            currentProducts = currentProducts.Where(p =>(!string.IsNullOrEmpty(p.Title) && p.Title.ToLower().Contains(searchText))).ToList();

            ProductCombo.ItemsSource = currentProducts;
            ProductCombo.SelectedItem = null;

            if (!string.IsNullOrWhiteSpace(searchText) && currentProducts.Any())
            {
                ProductCombo.IsDropDownOpen = true;
            }
        }
    }
}
