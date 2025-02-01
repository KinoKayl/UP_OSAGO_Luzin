using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace UP_OSAGO_Luzin.Pages.DriverPages
{
    /// <summary>
    /// Логика взаимодействия для DriverAuthPage.xaml
    /// </summary>
    public partial class DriverAuthPage : Page
    {
        private readonly Entities db;
        public DriverAuthPage()
        {
            InitializeComponent();
            db = new Entities();
        }

        private void LoginBoxText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FIOBox.Focus();
        }
        private void PasswordBoxText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PasBox.Focus();
        }

        private void LoginBox_TextChanged(object sender, TextChangedEventArgs e)
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

        private void PassswordBox_PasswordChanged(object sender, RoutedEventArgs e)
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

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            string pasportnumber = PasBox.Text;
            string FIO = FIOBox.Text;
            int pasnumber;

            if (int.TryParse(PasBox.Text, out pasnumber) && pasportnumber.Length == 10)
            {
                return;
            }
            else
            {
                MessageBox.Show("Серия и номер должны быть числом, состоящим из 10 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                PasBox.Text = "";

            }

            var driver = db.Drivers.AsNoTracking().FirstOrDefault(u => u.FullName == FIO && u.PassportSeriesNumber == pasportnumber);

            if (driver == null)
            {
                MessageBox.Show("Водитель с такими данными не найден в системе!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Вы авторизовались!");

            int currentUserId = (int)App.Current.Resources["CurrentUserId"];
            var driver2 = db.Drivers.AsNoTracking().FirstOrDefault(u => u.UserID == currentUserId);

            if (driver2 != null)
            {
                var policy = db.Policies.FirstOrDefault(c => c.DriverID == driver2.DriverID);

                if (policy != null)
                {
                    MessageBox.Show("В системе уже есть ваш действующий полис");
                    NavigationService?.Navigate(new Pages.DriverPages.OSAGOPage());
                }
                else
                {
                    MessageBox.Show("В системе нет полиса вашего");
                    NavigationService?.Navigate(new Pages.DriverPages.CreateOSAGO());

                }
            }
            else
            {
                Console.WriteLine("Водитель не найден.");
            }


        }
        private bool IsValidInput(string input)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9]*$");
            return regex.IsMatch(input);
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            FIOBox.Text = string.Empty;
            PasBoxText.Text = string.Empty;
            NavigationService?.Navigate(new Pages.DriverPages.DriverRegPage());
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }
    }
}
