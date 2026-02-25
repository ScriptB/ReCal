using System;
using System.Windows;

namespace ReCal
{
    public partial class TermsOfServiceWindow : Window
    {
        public TermsOfServiceWindow()
        {
            InitializeComponent();
        }

        private void AgreeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save agreement to settings
                Properties.Settings.Default.TermsAccepted = true;
                Properties.Settings.Default.Save();
            }
            catch
            {
                // If settings fail, continue anyway - user has agreed
            }
            
            this.DialogResult = true;
            this.Close();
        }

        private void DeclineButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You must accept the Terms of Service to use this application.", 
                          "Terms Required", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Warning);
            
            this.DialogResult = false;
            this.Close();
        }
    }
}
