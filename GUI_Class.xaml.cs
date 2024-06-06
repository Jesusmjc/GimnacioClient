using GimnacioClient.GimnacioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
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
    /// Interaction logic for GUI_Class.xaml
    /// </summary>
    public partial class GUI_Class : Window
    {
        Dictionary<int, string> trainersDictionary = new Dictionary<int, string>();

        public GUI_Class()
        {
            InitializeComponent();
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            GimnacioService2.ClassManagerClient client = new GimnacioService2.ClassManagerClient();

            List<string> trainerNames = new List<string>();
            var trainersFromDB = client.GetTrainers();

            foreach (var user in trainersFromDB)
            {
                string trainerName = user.Name + " " + user.MiddleName + " " + user.LastName;
                trainersDictionary[user.UserId] = trainerName;

                trainerNames.Add(trainerName);
            }
            cbTrainer.ItemsSource = trainerNames;

            List<TimeSpan> timesList = new List<TimeSpan>();
            for (int hour = 10; hour < 21; hour++)
            {
                timesList.Add(new TimeSpan(hour, 0, 0));
            }
            cbTime.ItemsSource = timesList;

            List<string> classTypesList = new List<string>();
            classTypesList.Add("Zumba");
            classTypesList.Add("Pilates");
            classTypesList.Add("Aeróbicos");
            classTypesList.Add("Yoga");
            classTypesList.Add("Tai Chi");
            classTypesList.Add("Otra...");
            cbClassType.ItemsSource = classTypesList;

            List<int> capacityList = new List<int>();
            capacityList.Add(10);
            capacityList.Add(15);
            capacityList.Add(20);
            cbCapacity.ItemsSource = capacityList;
        }

        private bool ValidateFields()
        {
            bool isTrainerValid = false;
            bool isTimeValid = false;
            bool isClassTypeValid = false;
            bool isCapacityValid = false;
            bool isCommentsValid = false;

            string trainerName = "";
            TimeSpan time = new TimeSpan();
            string classType = "";
            int capacity = 0;
            string comments = "";

            if (cbTrainer.SelectedItem != null)
            {
                trainerName = cbTrainer.SelectedItem.ToString();
                isTrainerValid = true;
            }

            if (cbTime.SelectedItem != null)
            {
                time = (TimeSpan)cbTime.SelectedItem;
                isTimeValid = true;
            }

            if (cbClassType.SelectedItem != null)
            {
                classType = cbClassType.SelectedItem.ToString();
                isClassTypeValid = true;
            }
            
            if (cbCapacity.SelectedItem != null)
            {
                capacity = (int)cbCapacity.SelectedItem;
                isCapacityValid = true;
            }
            
            if (!string.IsNullOrEmpty(tbComments.Text.ToString()))
            {
                comments = tbComments.Text.ToString();
                isCommentsValid = true;
            }
            

            if (!string.IsNullOrEmpty(trainerName) && time != null && !string.IsNullOrEmpty(classType) && capacity >= 10 && !string.IsNullOrEmpty(comments))
            {
                lbEmptyFields.Visibility = Visibility.Hidden;
            }
            else
            {
                lbEmptyFields.Visibility = Visibility.Visible;
            }

            return isTrainerValid && isTimeValid && isClassTypeValid && isCapacityValid && isCommentsValid && ValidateDatePicker();
        }

        private bool ValidateDatePicker()
        {
            bool isDateValid = false;

            if (datePicker.SelectedDate.HasValue && datePicker.SelectedDate.Value > DateTime.Now)
            {
                isDateValid = true;
                lbInvalidDate.Visibility = Visibility.Hidden;
            }
            else if (!datePicker.SelectedDate.HasValue)
            {
                lbEmptyFields.Visibility = Visibility.Visible;
            }
            else if (datePicker.SelectedDate.Value <= DateTime.Now)
            {
                lbInvalidDate.Visibility = Visibility.Visible;
            }

            return isDateValid;
        }

        public bool ValidateDateIsAvailable()
        {
            bool isDateAvailable = false;
            DateTime classDateTime = datePicker.SelectedDate.Value.Add((TimeSpan)cbTime.SelectedItem);
            GimnacioService.ClassManagerClient client = new GimnacioService.ClassManagerClient();
            try
            {
                int result = client.ValidateDateIsAvailable(classDateTime);

                if (result == 1)
                {
                    MessageBox.Show("Por favor seleccione una fecha/hora distintas.", "Horario ocupado.", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (result == 0)
                {
                    isDateAvailable = true;
                }
                else if (result == -1)
                {
                    MessageBox.Show("Ocurrió un error al conectar con la Base de Datos. Por favor intente de nuevo más tarde.", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("Ha ocurrido un error al intentar conectar con el Servidor. Por favor intente de nuevo más tarde.", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                isDateAvailable = false;
            }

            return isDateAvailable;
        }

        private void Btn_Register_Class_Click(object sender, RoutedEventArgs e)
        {
            int result = 0;

            if (UserSingleton.Instance.UserId <= 0)
            {
                MessageBox.Show("Ocurrió un error al asociar el entrenador.", "Error en el Cliente", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (ValidateFields() && ValidateDateIsAvailable())
                {

                    Class newClass = new Class
                    {
                        Date = datePicker.SelectedDate.Value.Add((TimeSpan)cbTime.SelectedItem),
                        Time = (TimeSpan)cbTime.SelectedItem,
                        Type = cbClassType.SelectedItem.ToString(),
                        Capacity = (int)cbCapacity.SelectedItem,
                        Comments = tbComments.Text.ToString(),
                        TrainerId = trainersDictionary.FirstOrDefault(x => x.Value == cbTrainer.SelectedItem.ToString()).Key,
                    };

                    GimnacioService.ClassManagerClient client = new GimnacioService.ClassManagerClient();
                    try
                    {
                        result = client.RegisterClass(newClass);

                        if (result > 0)
                        {
                            MessageBox.Show("Se ha registrado la clase correctamente.", "Operación exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Ocurrió un error al registrar la clase.", "Error en el Servidor", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    catch (EndpointNotFoundException)
                    {
                        MessageBox.Show("Ha ocurrido un error al intentar conectar con el Servidor. Por favor intente de nuevo más tarde.", "Error de conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window mainMenuWindow = new MainWindow();
            mainMenuWindow.Show();
            this.Close();
        }
    }
}
