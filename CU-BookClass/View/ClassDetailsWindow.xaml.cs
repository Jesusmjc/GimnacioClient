using GimnacioClient.GimnacioService;
using System;
using System.Diagnostics;
using System.Windows;

namespace GimnacioClient.CU_BookClass.View
{
    public partial class ClassDetailsWindow : Window
    {
        private Class clazz; 
        private Class[] BookedClasses;
        private int TotalAssistantsToClass; 

        public ClassDetailsWindow(Class clazz)
        {
            InitializeComponent();
            this.clazz = clazz;

            Loaded += ClassDetailsWindow_Loaded;

        }

        private void ClassDetailsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Title_TextBlock.Text = clazz.Type;
            Date_TextBlock.Text = clazz.Date.ToShortDateString();
            Hour_TextBlock.Text = clazz.Date.ToShortTimeString(); 

            UserManagerClient client = new UserManagerClient();
            ClassManagerClient classClient = new ClassManagerClient();

            try
            {
                var trainer = client.GetUserById(clazz.TrainerId);
                this.BookedClasses = classClient.GetBookClassesByMember(UserSingleton.Instance.UserId);
                TotalAssistantsToClass = classClient.GetTotalAssistantsToClass(clazz.ClassId); 
                
                if (trainer != null && BookedClasses != null && TotalAssistantsToClass != -1)
                {
                    Instructor_TextBlock.Text = trainer.Name + " " + trainer.LastName + " " + trainer.MiddleName;
                }
                else
                {
                    MessageBox.Show("Ocurrio un error al intentar recuperar la información, intentelo mas tarde", "Error con la base de datos", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close(); 
                }

            }catch(Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Ocurrio un error al intentar recuperar la información, intentelo mas tarde", "Error de conexión con el servidor", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close(); 
            }
            finally
            {
                client.Close();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) 
        {
            bool canSave = true;

            if (TotalAssistantsToClass >= clazz.Capacity)
            {
                canSave = false;
                MessageBox.Show("Lo sentimos, pero al parecer esta clase ya se encuentra llena", "Cupo lleno", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); 
                return; 
            }

            foreach(var bookedClass in BookedClasses)
            {
                if (bookedClass.ClassId == clazz.ClassId)
                {
                    canSave = false;
                    MessageBox.Show( "Lo sentimos, pero al parecer ya se encuentra inscrito a esta clase", "Clase inscrita", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }
            }

            foreach (var bookedClass in BookedClasses)
            {
                if(bookedClass.Date == clazz.Date)
                {
                    canSave = false;
                    MessageBox.Show("Lo sentimos, pero al parecer usted ya se encuentra inscrito a otra clase en la misma fecha y hora", "Clase sobrepuesta", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                    return;
                }
            }

            SaveChanges();
        }

        private void SaveChanges()
        {

            ClassManagerClient client = new ClassManagerClient();

            try
            {
                int result = client.BookClass(clazz.ClassId, UserSingleton.Instance.UserId);

                if (result == 1)
                {
                    MessageBox.Show("Su clase ah sido reservada correctamente", "Clase reservada", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Lo sentimos, ocurrio un error al intentar conectarse con la base de datos", "Error con la base de datos", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }

            }catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Lo sentimos, ocurrio un error al intentar contectarse con el servidor", "Erro con el servidor", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            finally { client.Close(); }

            
        }
    }
}
