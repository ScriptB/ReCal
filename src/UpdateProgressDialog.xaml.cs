using System.Windows;

namespace ReCal
{
    public partial class UpdateProgressDialog : Window
    {
        public UpdateProgressDialog()
        {
            InitializeComponent();
        }

        public void UpdateProgress(int percentage)
        {
            ProgressBarFill.Width = (ProgressBarBackground.ActualWidth * percentage) / 100;
            ProgressText.Text = $"{percentage}%";
            
            if (percentage < 30)
                StatusText.Text = "Downloading update...";
            else if (percentage < 70)
                StatusText.Text = "Receiving files...";
            else if (percentage < 100)
                StatusText.Text = "Almost done...";
            else
                StatusText.Text = "Preparing installation...";
        }
    }
}
