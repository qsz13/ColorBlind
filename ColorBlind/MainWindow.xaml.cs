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

namespace ColorBlind
{
    enum TransType { protanope, deuteranope, tritanope }
  

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenu contextMenu;

        private System.Windows.Forms.MenuItem protanopeMenuItem;    //红色盲
        private System.Windows.Forms.MenuItem deuteranopeMenuItem;  //绿色盲
        private System.Windows.Forms.MenuItem tritanopeMenuItem;   //蓝色盲
        //private Transformation transformation = new Transformation();
        private TransformationManager manager = new TransformationManager();
        TransType selectedType;


        public MainWindow()
        {
            
            InitializeComponent();
            InitializeContextMenu();
            InitialTray();
            manager.init();
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
               // manager.Toggle();
                //manager.DoMagnifierApiInvoke();
            }

            else if(sender == deuteranopeMenuItem)
            {
                protanopeMenuItem.Checked = false;
                deuteranopeMenuItem.Checked = true;
                tritanopeMenuItem.Checked = false;
                selectedType = TransType.deuteranope;
                manager.setColorEffect(selectedType);
              //  transformations.transform(selectedType);
            }

            else if(sender == tritanopeMenuItem)
            {

                protanopeMenuItem.Checked = false;
                deuteranopeMenuItem.Checked = false;
                tritanopeMenuItem.Checked = true;
                selectedType = TransType.tritanope;
                manager.setColorEffect(selectedType);
                //transformations.transform(selectedType);
                //manager.InvokeColorEffect(new ScreenColorEffect());
            }
            




        }


        protected void Exit_Click(Object sender, System.EventArgs e)
        {

            Close();
        }
        private void InitialTray()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = ColorBlind.Properties.Resources.MainIcon;
            notifyIcon.Visible = true;
            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.Click += new System.EventHandler(this.notifyIcon_Click);
        }

        private void notifyIcon_Click(object Sender, EventArgs e)
        {
            manager.Toggle();
        }


    }
}
