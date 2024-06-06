using GimnacioClient.CU_BookClass.View;
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

namespace GimnacioClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetContentByRole();
        }

        private void SetContentByRole()
        {
            switch (UserSingleton.Instance.UserType)
            {
                case "Admin":
                    btnRegisterClass.Visibility = Visibility.Visible;
                    break;

                case "Entrenador":
                    break;

                case "Miembro":
                    btnReserveClass.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Btn_Register_Class_Click(object sender, RoutedEventArgs e)
        {
            Window classWindow = new GUI_Class();
            classWindow.Show();
            this.Close();
        }

        private void Btn_Book_Class_Click(object sender, RoutedEventArgs e)
        {
            Window classWindow = new ClassListWindow();
            classWindow.Show();
            this.Close();
        }

        private void Btn_Log_Out_Click(object sender, RoutedEventArgs e)
        {
            Window loginWindow = new GUI_Login();
            loginWindow.Show();
            this.Close();

            UserSingleton.Instance.LogOut();
        }
    }
}
