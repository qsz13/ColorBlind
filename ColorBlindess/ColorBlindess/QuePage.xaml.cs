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
    /// QuePage.xaml 的交互逻辑
    /// </summary>
    public partial class QuePage : Page
    {
        private string[] picURLList = new string[] { "UI/red1.jpg", "UI/red2.jpg", "UI/blue1.jpg", "UI/blue2.jpg", "UI/green1.jpg", "UI/green2.jpg" };
        private string[] AList = new string[] { "C", "鱼", "F", "鸡", "顺时针分别是：688", "鸵鸟" };
        private string[] BList = new string[] { "N", "U", "7", "L", "顺时针分别是：699", "5" };
        private string[] CList = new string[] { "R", "A", "P", "K", "顺时针分别是：698", "6" };
        private string[] AnswerList = new string[] { "B", "C", "A", "C", "C", "B" };
        private int wrongInA = 0;
        private int wrongInB = 0;
        private int wrongInC = 0;
        private int currentNum = 0;
        private int answerTime = 0;
        public static int resultNum;
        public static int pageType;
        public QuePage()
        {
            InitializeComponent();
            currentNum = 0;
            answerTime = 0;
            wrongInA = 0;
            wrongInB = 0;
            wrongInC = 0;
            InitalQue(currentNum);
            pageType = 1;
        }
        public void InitalQue(int value)
        {
            img.Source = new BitmapImage(new Uri(picURLList[value],UriKind.Relative));
            AChoice.Text= AList[value];
            BChoice.Text = BList[value];
            CChoice.Text = CList[value];
            
        }
        public int result()
        {
            if(wrongInA>=wrongInB&&wrongInA>=wrongInC)
            {
                return 1; 
            }
            if(wrongInB>=wrongInA&&wrongInB>=wrongInC)
            {
                return 2;
            }
            if(wrongInC>=wrongInA&&wrongInC>=wrongInB)
            {
                return 3;
            }
            return 0;
        }
        private void ChooseA_Click(object sender, RoutedEventArgs e)
        {
            answerTime++;
            if (answerTime == 6)
            {
                resultNum = result();
                ChoosePage resultPage = new ChoosePage();
                this.NavigationService.Navigate(resultPage);
            }
            else {
                if (AnswerList[currentNum] == "A")
                {
                    currentNum++;
                    InitalQue(currentNum);
                }
                else
                {
                    wrongInA++;
                    currentNum++;
                    InitalQue(currentNum);
                }
            }
           
        }

        private void ChooseB_Click(object sender, RoutedEventArgs e)
        {
            answerTime++;
            if (answerTime == 6)
            {
                resultNum = result();
                ChoosePage resultPage = new ChoosePage();
                this.NavigationService.Navigate(resultPage);
            }
            else
            {
                if (AnswerList[currentNum] == "B")
                {
                    currentNum++;
                    InitalQue(currentNum);
                }
                else
                {
                    wrongInB++;
                    currentNum++;
                    InitalQue(currentNum);
                }
            }
           
        }

        private void ChooseC_Click(object sender, RoutedEventArgs e)
        {
            answerTime++;
            if (answerTime == 6)
            {
                resultNum = result();
                ChoosePage resultPage = new ChoosePage();
                this.NavigationService.Navigate(resultPage);
            }
            else
            {
                if (AnswerList[currentNum] == "C")
                {
                    currentNum++;
                    InitalQue(currentNum);
                }
                else
                {
                    wrongInC++;
                    currentNum++;
                    InitalQue(currentNum);
                }
            }
           
        }
    }
}
