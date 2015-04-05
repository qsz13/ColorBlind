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
using System.Windows.Forms;

namespace ColorBlindess
{
    /// <summary>
    /// ChoosePage.xaml 的交互逻辑
    /// </summary>

    enum TransType { protanope, deuteranope, tritanope }
    public partial class ChoosePage : Page
    {
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenu contextMenu;

        private System.Windows.Forms.MenuItem protanopeMenuItem;    //红色盲
        private System.Windows.Forms.MenuItem deuteranopeMenuItem;  //绿色盲
        private System.Windows.Forms.MenuItem tritanopeMenuItem;   //蓝色盲
        //private Transformation transformation = new Transformation();
        private TransformationManager manager = new TransformationManager();
        TransType selectedType;
        private string[] colorList = new string[] { "红色盲模式", "绿色盲模式", "蓝色盲模式" };
        private bool isClicked = false;
        public ChoosePage()
        {
            InitializeComponent();
            InitializeContextMenu();
            InitialTray();
            manager.init();
            Submit.Visibility = System.Windows.Visibility.Visible;
            if(QuePage.pageType==1)
            {
                name.Content = "您的检测结果为：";
                ColorComboBox.Visibility = System.Windows.Visibility.Collapsed;
                result.Visibility = System.Windows.Visibility.Visible;
                int currentValue = QuePage.resultNum;
                switch (currentValue)
                {
                    case 0:
                        result.Content = "正常";
                        Submit.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case 1:
                        result.Content = "红色盲";
                        protanopeMenuItem.Checked = true;
                        deuteranopeMenuItem.Checked = false;
                        tritanopeMenuItem.Checked = false;
                        selectedType = TransType.protanope;
                        manager.setColorEffect(selectedType);
                        break;
                    case 2:
                        result.Content = "蓝色盲";
                        protanopeMenuItem.Checked = false;
                        deuteranopeMenuItem.Checked = false;
                        tritanopeMenuItem.Checked = true;
                        selectedType = TransType.protanope;
                        manager.setColorEffect(selectedType);
                        break;
                    case 3:
                        result.Content = "绿色盲";
                        protanopeMenuItem.Checked = false;
                        deuteranopeMenuItem.Checked = true;
                        tritanopeMenuItem.Checked = false;
                        selectedType = TransType.deuteranope;
                        break;
                }
            }
            else
            {
                name.Content = "请选择模式";
                ColorComboBox.Visibility = System.Windows.Visibility.Visible;
                result.Visibility = System.Windows.Visibility.Collapsed;
                for (int i = 0; i < colorList.Count(); i++)
                {
                    ColorComboBox.Items.Add(colorList[i]);
                }
            }
            
        }

         private void InitializeContextMenu()
        {
           
            this.contextMenu = new System.Windows.Forms.ContextMenu();
            contextMenu.MenuItems.Add(
                new System.Windows.Forms.MenuItem("退出", new System.EventHandler(Exit_Click)));
            protanopeMenuItem = new System.Windows.Forms.MenuItem("红色盲", new System.EventHandler(Type_Click));
            deuteranopeMenuItem = new System.Windows.Forms.MenuItem("绿色盲", new System.EventHandler(Type_Click));
            tritanopeMenuItem = new System.Windows.Forms.MenuItem("蓝色盲", new System.EventHandler(Type_Click));
            protanopeMenuItem.RadioCheck = true;
            protanopeMenuItem.Checked = true;
            deuteranopeMenuItem.RadioCheck = true;
            tritanopeMenuItem.RadioCheck = true;
            contextMenu.MenuItems.Add(protanopeMenuItem);
            contextMenu.MenuItems.Add(deuteranopeMenuItem);
            contextMenu.MenuItems.Add(tritanopeMenuItem);
   
        }

         protected void Type_Click(Object sender, System.EventArgs e)
         {


             if (sender == protanopeMenuItem)
             {
                 protanopeMenuItem.Checked = true;
                 deuteranopeMenuItem.Checked = false;
                 tritanopeMenuItem.Checked = false;
                 selectedType = TransType.protanope;
                 manager.setColorEffect(selectedType);

             }

             else if (sender == deuteranopeMenuItem)
             {
                 protanopeMenuItem.Checked = false;
                 deuteranopeMenuItem.Checked = true;
                 tritanopeMenuItem.Checked = false;
                 selectedType = TransType.deuteranope;
                 manager.setColorEffect(selectedType);
             }

             else if (sender == tritanopeMenuItem)
             {

                 protanopeMenuItem.Checked = false;
                 deuteranopeMenuItem.Checked = false;
                 tritanopeMenuItem.Checked = true;
                 selectedType = TransType.tritanope;
                 manager.setColorEffect(selectedType);

             }
         }
         protected void Exit_Click(Object sender, System.EventArgs e)
        {
            Environment.Exit(0);
        }

        private void InitialTray()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = ColorBlindess.Properties.Resources.MainIcon;
            notifyIcon.Visible = true;
            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_Click);
           // NotifyIcon
        }
        
        private void notifyIcon_Click(object Sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                manager.Toggle();
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            protanopeMenuItem.Checked = false;
            deuteranopeMenuItem.Checked = false;
            tritanopeMenuItem.Checked = false;
            selectedType = TransType.tritanope;
            manager.setColorEffect(selectedType);
            manager.Toggle();
            HomePage home = new HomePage();
            this.NavigationService.Navigate(home);
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ColorComboBox.Text=="红色盲模式")
            {
                protanopeMenuItem.Checked = true;
                deuteranopeMenuItem.Checked = false;
                tritanopeMenuItem.Checked = false;
                selectedType = TransType.protanope;
                manager.setColorEffect(selectedType);
            }
            if (ColorComboBox.Text == "绿色盲模式")
            {
                protanopeMenuItem.Checked = false;
                deuteranopeMenuItem.Checked = true;
                tritanopeMenuItem.Checked = false;
                selectedType = TransType.deuteranope;
                manager.setColorEffect(selectedType);
            }
            if (ColorComboBox.Text == "蓝色盲模式")
            {
                protanopeMenuItem.Checked = false;
                deuteranopeMenuItem.Checked = false;
                tritanopeMenuItem.Checked = true;
                selectedType = TransType.protanope;
                manager.setColorEffect(selectedType);
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if(isClicked==false)
            {
                manager.Toggle();
                isClicked = true;
            }
            
        }
    }
}
