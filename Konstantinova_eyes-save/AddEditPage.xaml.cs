using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Konstantinova_eyes_save
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Agent currentAgents = new Agent();
        private List<ProductSale> productSales = new List<ProductSale>();
        public AddEditPage(Agent SelectedAgent)
        {
            InitializeComponent();

            if (SelectedAgent != null)
            {
                currentAgents = SelectedAgent;
                DeleteBtn.Visibility = Visibility.Visible;
                HistoryTB_ProductSalesListView.Visibility = Visibility.Visible;
                NameTB_ProductSalesListView.Visibility = Visibility.Visible;
                ProductSalesListView.Visibility = Visibility.Visible;
                Buttons_ProductSalesListView.Visibility = Visibility.Visible;
            }

            var agentTypes = new List<string> { "МФО", "ООО", "ЗАО", "МКК", "ОАО", "ПАО" };
            ComboType.ItemsSource = agentTypes;



            DataContext = currentAgents;
        }
        
        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            if (myOpenFileDialog.ShowDialog() == true)
            {
                string fullPath = myOpenFileDialog.FileName;
                string fileName = System.IO.Path.GetFileName(fullPath);

                string projectPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\.."));
                string agentsFolder = System.IO.Path.Combine(projectPath, "agents");

                string destPath = System.IO.Path.Combine(agentsFolder, fileName);

                System.IO.File.Copy(fullPath, destPath, true);

                string relativePath = $"\\agents\\{fileName}";

                currentAgents.Logo = relativePath;

                LogoImage.Source = new BitmapImage(new Uri(fullPath));
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(currentAgents.Title))
                errors.AppendLine("Укажите наименование агента");

            if (Konstantinova_eyesEntities.GetContext().Agent
            .Any(a => a.Title == currentAgents.Title && a.ID != currentAgents.ID))
                errors.AppendLine("Агент с таким наименованием уже существует");

            if (string.IsNullOrWhiteSpace(currentAgents.Address))
                errors.AppendLine("Укажите адрес агента");

            if (string.IsNullOrWhiteSpace(currentAgents.DirectorName))
                errors.AppendLine("Укажите ФИО директора");

            if (ComboType.SelectedItem == null)
                errors.AppendLine("Укажите тип агента");
            else
            {
                currentAgents.AgentTypeID = ComboType.SelectedIndex + 1;
                currentAgents.AgentType = Konstantinova_eyesEntities.GetContext().AgentType
            .FirstOrDefault(at => at.ID == currentAgents.AgentTypeID);
            }

            if (string.IsNullOrWhiteSpace(currentAgents.Priority.ToString()))
                errors.AppendLine("Укажите приоритет агента");

            if (currentAgents.Priority < 0)
                errors.AppendLine("Укажите положительный приоритет агента");

            if (string.IsNullOrWhiteSpace(currentAgents.INN))
                errors.AppendLine("Укажите ИНН агента");
            else
            {
                if (currentAgents.INN.Length != 10 && currentAgents.INN.Length != 12)
                    errors.AppendLine("ИНН должен содержать 10 или 12 цифр");
            }
            
            if (string.IsNullOrWhiteSpace(currentAgents.KPP))
                errors.AppendLine("Укажите КПП агента");
            else
            {
                if (currentAgents.KPP.Length != 9)
                    errors.AppendLine("КПП должен содержать 9 цифр");
            }
            

            if (string.IsNullOrWhiteSpace(currentAgents.Phone))
                errors.AppendLine("Укажите телефон агента");
            else
            {
                string ph = new string(currentAgents.Phone.Where(char.IsDigit).ToArray());
                if (ph.Length < 10 || ph.Length > 12)
                {
                    errors.AppendLine("Телефон должен содержать 10-12 цифр");
                }
                else
                {
                    char firstDigit = ph[0];
                    if (firstDigit != '7' && firstDigit != '8' && firstDigit != '3')
                    {
                        errors.AppendLine("Телефон должен начинаться с 7, 8 или 3");
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(currentAgents.Email))
                errors.AppendLine("Укажите почту агента");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (currentAgents.ID == 0)
                Konstantinova_eyesEntities.GetContext().Agent.Add(currentAgents);

            try
            {
                Konstantinova_eyesEntities.GetContext().SaveChanges();

                DataContext = null;
                DataContext = currentAgents;

                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            currentAgents = (sender as Button).DataContext as Agent;
            var currentAgentsProductSale = Konstantinova_eyesEntities.GetContext().ProductSale.ToList();

            currentAgentsProductSale = currentAgentsProductSale.Where(p => p.AgentID == currentAgents.ID).ToList();
            if (currentAgentsProductSale.Count != 0)
                MessageBox.Show("Невозможно удалить агента!");
            else
            {
                if (MessageBox.Show("Вы точно хотите удалить агента?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Konstantinova_eyesEntities.GetContext().Agent.Remove(currentAgents);
                        Konstantinova_eyesEntities.GetContext().SaveChanges();
                        MessageBox.Show("Информация сохранена!");
                        Manager.MainFrame.GoBack();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void AddSale_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddProductSaleDialog();
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true)
            {
                var productSale = new ProductSale
                {
                    AgentID = currentAgents.ID,
                    ProductID = dialog.SelectedProduct.ID,
                    ProductCount = dialog.ProductCount,
                    SaleDate = dialog.SaleDate
                };

                productSale.Product = Konstantinova_eyesEntities.GetContext().Product
            .FirstOrDefault(p => p.ID == productSale.ProductID);

                if (currentAgents.ID > 0)
                {
                    Konstantinova_eyesEntities.GetContext().ProductSale.Add(productSale);
                    Konstantinova_eyesEntities.GetContext().SaveChanges();
                }

                productSales.Add(productSale);

                ProductSalesListView.ItemsSource = null;
                ProductSalesListView.ItemsSource = productSales;

            }
        }

        private void DeleteSale_Click(object sender, RoutedEventArgs e)
        {
            var selectedSale = ProductSalesListView.SelectedItem as ProductSale;

            if (selectedSale == null)
            {
                MessageBox.Show("Выберите продажу для удаления");
                return;
            }

            if (MessageBox.Show("Удалить продажу?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (selectedSale.ID > 0)
                {
                    Konstantinova_eyesEntities.GetContext().ProductSale.Remove(selectedSale);
                    Konstantinova_eyesEntities.GetContext().SaveChanges();
                }

                productSales.Remove(selectedSale);

                ProductSalesListView.ItemsSource = null;
                ProductSalesListView.ItemsSource = productSales;
            }
        }

    }
}
