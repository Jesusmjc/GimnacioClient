using GimnacioClient.GimnacioService;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GimnacioClient.CU_BookClass.View.Controls
{
    
    public partial class ClassControl : UserControl
    {

        private Class clazz; 

        public ClassControl(Class clazz)
        {
            InitializeComponent();
            this.clazz = clazz;
            Loaded += ClassControl_Loaded;
        }

        private void ClassControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Title_TextBlock.Text = clazz.Type;
            Capacity_TextBlock.Text = clazz.Capacity.ToString(); 
            Date_TextBlock.Text = clazz.Date.ToString();
        }

        private void Book_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Window classWindow = new ClassDetailsWindow(clazz);
            classWindow.Show();
        }
    }
}
