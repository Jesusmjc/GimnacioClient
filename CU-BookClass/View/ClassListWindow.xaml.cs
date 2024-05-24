using GimnacioClient.CU_BookClass.View.Controls;
using GimnacioClient.GimnacioService;
using System;
using System.Diagnostics;
using System.Windows;

namespace GimnacioClient.CU_BookClass.View
{
    
    public partial class ClassListWindow : Window
    {
        public ClassListWindow()
        {
            InitializeComponent();
            Loaded += ClassListWindow_Loaded;
        }

        private void ClassListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInformation(); 
        }

        private void LoadInformation()
        {
            Classes_StackPanel.Children.Clear();

            ClassManagerClient classClient = new ClassManagerClient();

            try
            {
                
                var classes = classClient.GetClasses();
                Debug.WriteLine(classes.Length + "El tamaño es "); 

                if(classes != null)
                {
                    foreach (var clazz in classes)
                    {
                        Classes_StackPanel.Children.Add(new ClassControl(clazz));
                    }
                }
                
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                classClient.Close(); 
            }
        }
    }
}
