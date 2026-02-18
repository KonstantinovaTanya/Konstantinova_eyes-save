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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Konstantinova_eyes_save
{
    /// <summary>
    /// Логика взаимодействия для AgentPage.xaml
    /// </summary>
    public partial class AgentPage : Page
    {
        public AgentPage()
        {
            InitializeComponent();
            var currentAgents = Konstantinova_eyesEntities.GetContext().Agent.ToList();
            AgentListView.ItemsSource = currentAgents;

            ComboType.SelectedIndex = 0;
            SortComboBox.SelectedIndex = 0;

            UpdateAgents();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void UpdateAgents()
        {
            var currentAgents = Konstantinova_eyesEntities.GetContext().Agent.ToList();

            string searchText = TBoxSearch.Text?.ToLower() ?? "";

            string cleanSearchText = new string(searchText.Where(c => char.IsDigit(c)).ToArray());

            if (cleanSearchText.StartsWith("8"))
            {
                cleanSearchText = "7" + cleanSearchText.Substring(1);
            }

            currentAgents = currentAgents.Where(p =>
                   
                    (!string.IsNullOrEmpty(p.Title) && p.Title.ToLower().Contains(searchText)) ||
                  
                    (!string.IsNullOrEmpty(p.Email) && p.Email.ToLower().Contains(searchText)) ||
                 
                    (!string.IsNullOrEmpty(p.Phone) && !string.IsNullOrEmpty(cleanSearchText) &&
                     new string(p.Phone.Where(c => char.IsDigit(c)).ToArray()).Contains(cleanSearchText))).ToList();


            switch (SortComboBox.SelectedIndex)
            {
                case 1: 
                    currentAgents = currentAgents.OrderBy(p => p.Title).ToList(); break;
                case 2: 
                    currentAgents = currentAgents.OrderByDescending(p => p.Title).ToList(); break;
                case 3: 
                    currentAgents = currentAgents.OrderBy(p => p.Discount).ToList(); break;
                case 4: 
                    currentAgents = currentAgents.OrderByDescending(p => p.Discount).ToList(); break;
                case 5: 
                    currentAgents = currentAgents.OrderBy(p => p.Priority).ToList(); break;
                case 6: 
                    currentAgents = currentAgents.OrderByDescending(p => p.Priority).ToList(); break;
            }

            
            string selectedType = (ComboType.SelectedItem as TextBlock)?.Text;

            if (ComboType.SelectedIndex > 0)
                currentAgents = currentAgents.Where(p => p.AgentType.Title == selectedType).ToList();


            AgentListView.ItemsSource = currentAgents;
        }


        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void AgentListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
