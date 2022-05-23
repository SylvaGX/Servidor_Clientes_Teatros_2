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

namespace Client_Admin
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        bool hasBeenClicked = false;
        bool hasBeenClicked1 = false;
        bool hasBeenClicked2 = false;

        private void TextBox_Focus(object sender, RoutedEventArgs e)
        {
            if (!hasBeenClicked)
            {
                TextBox? box = sender as TextBox;
                if (box != null)
                    box.Text = string.Empty;
                hasBeenClicked = true;
            }
        }
        private void TextBox_Focus1(object sender, RoutedEventArgs e)
        {
            if (!hasBeenClicked1)
            {
                PasswordBox? box = sender as PasswordBox;
                if (box != null)
                    box.Password = string.Empty;
                hasBeenClicked1 = true;
            }
        }
        private void TextBox_Focus2(object sender, RoutedEventArgs e)
        {
            if (!hasBeenClicked2)
            {
                TextBox? box = sender as TextBox;
                if (box != null)
                    box.Text = string.Empty;
                hasBeenClicked2 = true;
            }
        }

    }
}
