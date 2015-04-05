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

namespace ColorBlindess
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            QuePage.pageType = 0;
        }
        private void Test_Click(object sender, RoutedEventArgs e)
        {
            QuePage que = new QuePage();
            this.NavigationService.Navigate(que);
        }
        private void Choose_Click(object sender, RoutedEventArgs e)
        {
            ChoosePage choose = new ChoosePage();
            this.NavigationService.Navigate(choose);
        }
    }
}
