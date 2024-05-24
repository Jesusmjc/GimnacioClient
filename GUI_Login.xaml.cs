using GimnacioClient.GimnacioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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

namespace GimnacioClient
{
    /// <summary>
    /// Interaction logic for GUI_Login.xaml
    /// </summary>
    public partial class GUI_Login : Window
    {
        public GUI_Login()
        {
            InitializeComponent();
        }

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            string password = Utility.ComputeSha256Hash(pwbPassword.Password.ToString());
            string email = tbxEmail.Text.ToString();

            if (ValidateFields(email, password))
            {
                GimnacioService.UserManagerClient client = new GimnacioService.UserManagerClient();
                User user = new User();
                try
                {
                    user = client.GetUser(email, password);

                    if (user.UserId > 0)
                    {
                        UserSingleton.Instance.UserId = user.UserId;
                        UserSingleton.Instance.UserType = user.Type;

                        Window mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                    else if (user.UserId == 0)
                    {
                        MessageBox.Show("No se ha encontrado una cuenta con las credenciales ingresadas.", "Inicio de sesión incorrecto", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show("Ocurrió un error al intentar iniciar sesión.", "Error en el Servidor", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (EndpointNotFoundException)
                {
                    MessageBox.Show("Ha ocurrido un error al intentar conectar con el Servidor. Por favor intente de nuevo más tarde.", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                }  
            }
        }

        private bool ValidateFields(string email, string password)
        {
            bool areFieldsValid = false;

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                areFieldsValid = true;
                lbInvalidFields.Visibility = Visibility.Hidden;
            }
            else
            {
                lbInvalidFields.Visibility = Visibility.Visible;
            }

            return areFieldsValid;
        }
    }
}
