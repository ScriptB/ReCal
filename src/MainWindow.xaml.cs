using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ReCal.Models;
using ReCal.Security;

namespace ReCal
{
    public partial class MainWindow : Window
    {
        private readonly LockerCalculator _lockerCalculator;
        private readonly CigarCalculator _cigarCalculator;
        private DispatcherTimer _tipTimer;
        private readonly Random _tipRandom = new Random();
        private readonly string[] _tips = new[]
        {
            "Join the Discord: https://discord.gg/2CXVDasA6Z",
            "Developer: Asuneteric",
            "Be sure to give me your opinions in my Discord server.",
            "Try different locker counts to compare costs quickly.",
            "Tip: Smaller batches help you plan material runs.",
            "Want updates? Check the Discord announcements.",
            "If the UI feels cramped, widen the window for a clearer layout.",
            "Found a bug? Let me know on Discord.",
            "Suggestions and feedback are always welcome."
        };

        public MainWindow()
        {
            InitializeComponent();
            _lockerCalculator = new LockerCalculator();
            _cigarCalculator = new CigarCalculator();
            InitializeTipRotator();
            
            // Check if user has accepted terms (delayed to avoid XAML loading issues)
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IntegrityChecker.VerifyOrWarn(AppDomain.CurrentDomain.BaseDirectory);

            // Check for updates first
            await CheckForUpdatesAsync();
            
            // Check if user has accepted terms after window is fully loaded
            CheckTermsAcceptance();
            
            // Calculate for 1 locker by default
            CalculateButton_Click(null, null);
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                await AutoUpdater.CheckForUpdatesAsync();
            }
            catch
            {
                // Silently continue if update check fails
            }
        }

        private void CheckTermsAcceptance()
        {
            try
            {
                // Check if settings are available and terms have been accepted
                bool termsAccepted = false;
                try
                {
                    termsAccepted = Properties.Settings.Default.TermsAccepted;
                }
                catch
                {
                    // If settings are not available, default to showing terms
                    termsAccepted = false;
                }
                
                if (!termsAccepted)
                {
                    // Show terms of service window
                    var termsWindow = new TermsOfServiceWindow();
                    termsWindow.Owner = this;
                    
                    if (termsWindow.ShowDialog() != true)
                    {
                        // User declined terms, close application
                        MessageBox.Show("You must accept the Terms of Service to use this application.", 
                                      "Terms Required", 
                                      MessageBoxButton.OK, 
                                      MessageBoxImage.Warning);
                        this.Close();
                        return;
                    }
                }
            }
            catch
            {
                // If there's any issue, show terms anyway as a fallback
                try
                {
                    var termsWindow = new TermsOfServiceWindow();
                    termsWindow.Owner = this;
                    
                    if (termsWindow.ShowDialog() != true)
                    {
                        this.Close();
                    }
                }
                catch
                {
                    // Last resort - close if we can't even show terms
                    MessageBox.Show("Unable to initialize application properly.", 
                                  "Initialization Error", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Error);
                    this.Close();
                }
            }
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(LockerCountTextBox.Text, out int lockerCount) || lockerCount <= 0 || lockerCount > 1000)
                {
                    MessageBox.Show("Please enter a valid number between 1 and 1000.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = _lockerCalculator.CalculateMaterials(lockerCount);

                // Update materials and costs
                CansNeededText.Text = result.CansNeeded.ToString();
                CageLightsNeededText.Text = result.CageLightsNeeded.ToString();
                TotalCostText.Text = $"£{result.TotalCost:N2}";
                CostPerLockerText.Text = $"£{result.CostPerLocker:N2}";

                // Update leftovers
                ScrapLeftoverText.Text = result.ScrapLeftover.ToString();
                WireLeftoverText.Text = result.WireLeftover.ToString();
                MetalBarsProducedText.Text = result.MetalBarsProduced.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during calculation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateCigarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(CigarCountTextBox.Text, out int cigarCount) || cigarCount <= 0 || cigarCount > 10000)
                {
                    MessageBox.Show("Please enter a valid number between 1 and 10000.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = _cigarCalculator.CalculateCigars(cigarCount);

                // Update revenue and production
                CigarRevenueText.Text = $"£{result.TotalRevenue:N2}";
                CigarProductionTimeText.Text = $"{result.ProductionHours:F2}h";

                // Update materials
                CigarPlantsText.Text = result.PlantsNeeded.ToString();
                CigarTapeText.Text = result.TapeNeeded.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during cigar calculation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DiscordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://discord.gg/2CXVDasA6Z",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to open Discord link: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void YoutubeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://www.youtube.com/@Asuehh",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to open YouTube link: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 0;
        }

        private void OpenInfo_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 1;
        }

        private void OpenWallLockers_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 2;
        }

        private void OpenCigars_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 3;
        }

        private void InitializeTipRotator()
        {
            _tipTimer = new DispatcherTimer();
            _tipTimer.Tick += (s, e) => RotateTip();
            RotateTip();
        }

        private void RotateTip()
        {
            if (_tips.Length == 0)
            {
                return;
            }

            var tip = _tips[_tipRandom.Next(_tips.Length)];
            if (DashboardTipText != null)
            {
                DashboardTipText.Text = tip;
            }

            var nextSeconds = _tipRandom.Next(7, 91);
            _tipTimer.Interval = TimeSpan.FromSeconds(nextSeconds);
            if (!_tipTimer.IsEnabled)
            {
                _tipTimer.Start();
            }
        }

        private void TermsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var termsWindow = new TermsOfServiceWindow();
                termsWindow.Owner = this;
                termsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to open Terms of Service: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
