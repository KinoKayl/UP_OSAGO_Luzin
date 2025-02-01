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

namespace UP_OSAGO_Luzin.Pages.DriverPages
{
    /// <summary>
    /// Логика взаимодействия для OSAGOPaymentPage.xaml
    /// </summary>
    public partial class OSAGOPaymentPage : Page
    {

        private readonly Entities db;


        private Users currentuser = new Users();
        public OSAGOPaymentPage()
        {
            InitializeComponent();
            int currentUserId = (int)App.Current.Resources["CurrentUserId"];
            var driver2 = db.Drivers.AsNoTracking().FirstOrDefault(u => u.UserID == currentUserId);
            var policy = db.Policies.FirstOrDefault(c => c.DriverID == driver2.DriverID);
            string Cost = policy.Cost.ToString();

            if (policy != null)
            {
                CostBoxText.Text = Cost;
            }
        }

        private void GoBackPage()
        {
            NavigationService?.Navigate(new Pages.DriverPages.CreateOSAGO());

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            UdoNumberBox.Text = string.Empty;
            PasBox.Text = string.Empty;
            FIOBox.Text = string.Empty;
            GoBackPage();
        }

        private void PasBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PasBox.Text) && PasBox.Text.Length > 0)
            {
                PasBoxText.Visibility = Visibility.Collapsed;
            }
            else
            {
                PasBoxText.Visibility = Visibility.Visible;
            }
        }

        private void PasBoxText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PasBox.Focus();
        }

        private void UdoNumberBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(UdoNumberBox.Text) && UdoNumberBox.Text.Length > 0)
            {
                UdoNumberBoxText.Visibility = Visibility.Collapsed;
            }
            else
            {
                UdoNumberBoxText.Visibility = Visibility.Visible;
            }
        }

        private void UdoNumberBoxText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UdoNumberBox.Focus();
        }

        private void FIOBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(FIOBox.Text) && FIOBox.Text.Length > 0)
            {
                FIOBoxText.Visibility = Visibility.Collapsed;
            }
            else
            {
                FIOBoxText.Visibility = Visibility.Visible;
            }
        }

        private void FIOBoxText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FIOBox.Focus();
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            string pasportnumber = PasBox.Text;
            string FIO = FIOBox.Text;
            string udoNumber = UdoNumberBox.Text;

            int pasnumber;
            int udonumber;
            int fionumber;

            if (int.TryParse(PasBox.Text, out pasnumber) && pasportnumber.Length == 16)
            {
                
            }
            else
            {
                MessageBox.Show("Номер должен быть числом, состоящим из 16 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                PasBox.Text = "";
                return;
            }

            if (int.TryParse(PasBox.Text, out udonumber) && udoNumber.Length == 4)
            {

            }
            else
            {
                MessageBox.Show("Срок действия должен быть числом, состоящим из 4 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                PasBox.Text = "";
                return;
            }

            if (int.TryParse(PasBox.Text, out fionumber) && FIO.Length == 3)
            {

            }
            else
            {
                MessageBox.Show("CVC должен быть числом, состоящим из 3 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                PasBox.Text = "";
                return;
            }

            MessageBox.Show("Операция проведена успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService?.Navigate(new Pages.DriverPages.OSAGOPage());

        }
    }
}
