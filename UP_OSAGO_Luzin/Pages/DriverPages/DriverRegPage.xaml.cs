using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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
using System.Data.SqlClient;



namespace UP_OSAGO_Luzin.Pages.DriverPages
{
    /// <summary>
    /// Логика взаимодействия для DriverRegPage.xaml
    /// </summary>
    public partial class DriverRegPage : Page
    {
        private static readonly Regex FIOregex = new Regex(@"^[А-ЯЁ][а-яё]+$");
        private static readonly Regex PasswordRegex = new Regex(@"^(?=.*\d)[a-zA-Z0-9]{6,}$");
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        private readonly Entities db;

        private DateTime birthdate;
        private DateTime Pasdate;
        private DateTime Udodate;

        private Users currentuser = new Users();
        public DriverRegPage()
        {
            InitializeComponent();
            db = new Entities();

        }

        private void GoBackPage()
        {
            NavigationService?.Navigate(new Pages.DriverPages.DriverAuthPage());

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

        static string GenerateOsagoPolicyNumber()
        {
            Random random = new Random();
            StringBuilder policyNumber = new StringBuilder();

            for (int i = 0; i < 15; i++)
            {
                policyNumber.Append(random.Next(0, 10)); 
            }

            return policyNumber.ToString();
        }

        static void InsertDateIntoDatabase(DateTime date)
        {
            string connectionString = "your_connection_string_here";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO YourTable (DateColumn) VALUES (@Date)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
                    command.ExecuteNonQuery();
                }
            }
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
          
            string pasportnumber = PasBox.Text;
            string policyNumber = GenerateOsagoPolicyNumber();
            DateTime currentdate = DateTime.Now;
            string FIO = FIOBox.Text;
            string udoNumber = UdoNumberBox.Text;
            string region = RegionBox.Text;

            try
            {
                
                int pasnumber;
                int udoNum;

                if (int.TryParse(PasBox.Text, out pasnumber) && pasportnumber.Length == 10)
                {
                    
                }
                else
                {
                    MessageBox.Show("Серия и номер должны быть числом, состоящим из 10 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    PasBox.Text = "";
                    return;

                }

                if (db.Drivers.Any(u => u.PassportSeriesNumber == pasportnumber))
                {
                    MessageBox.Show("Водитель с таким паспортом уже зарегистрирован в системе!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    PasBox.Text = "";
                    return;
                }

                if (int.TryParse(UdoNumberBox.Text, out udoNum) && udoNumber.Length == 10)
                {
                    
                }
                else
                {
                    MessageBox.Show("Номер удостоверения должен быть числом, состоящим из 10 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    UdoNumberBox.Text = "";
                    return;
                }

                if (db.Drivers.Any(u => u.LicenseSeriesNumber == udoNumber))
                {
                    MessageBox.Show("Водитель с таким удостоверением уже зарегистрирован в системе!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    UdoNumberBox.Text = "";
                    return;
                }

                if (!FIOcheck(FIO))
                {
                    MessageBox.Show("ФИО введено некорректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string login = (string)App.Current.Resources["Login"];
                int? userId = db.Users
                        .AsNoTracking()
                        .Where(u => u.Login == login) 
                        .Select(u => (int?)u.ID)      
                        .FirstOrDefault();            

                int number = userId ?? -1;

                App.Current.Resources["CurrentUserId"] = userId;

                // Создание нового водителя
                var newDriver= new Drivers
                {
                    Region = region,
                    FullName = FIO,
                    DateOfBirth = birthdate,
                    LicenseSeriesNumber = udoNumber,
                    LicenseIssueDate = Udodate,
                    PassportSeriesNumber = pasportnumber,
                    PassportIssueDate = Pasdate,
                    UserID = userId,
                };

                db.Drivers.Add(newDriver);
                db.SaveChanges();
                GoBackPage();
                MessageBox.Show("Регистрация завершена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.Navigate(new Pages.DriverPages.CreateOSAGO());

            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityErrors in ex.EntityValidationErrors)
                {
                    foreach (var valError in entityErrors.ValidationErrors)
                    {
                        MessageBox.Show($"Ошибка: {valError.ErrorMessage}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private bool FIOcheck(string FIO)
        {
            var splitFIO = FIO.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Подразумевается, что у пользователя нет отчества. В качестве данного условия, пришлось разделить ФИО на три части.
            if (splitFIO.Length < 2 || splitFIO.Length > 3)
            {
                return false;
            }

            foreach (var part in splitFIO)
            {
                if (string.IsNullOrWhiteSpace(part) || !FIOregex.IsMatch(part) || part.Length > 50)
                {
                    return false;
                }
            }
            return true;
        }

        //Одностророннее хэширование
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }

        }


        private void PasDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (PasdatePicker.SelectedDate.HasValue)
            {
                Pasdate = PasdatePicker.SelectedDate.Value;

                selectedDateText.Text = Pasdate.ToString("yyyy.MM.dd");

            }
        }

        private void UdoDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (UdodatePicker.SelectedDate.HasValue)
            {
                Udodate = UdodatePicker.SelectedDate.Value;

                selectedDateText2.Text = Udodate.ToString("yyyy.MM.dd");

            }
        }

        private void BirthDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (BirthdaydatePicker.SelectedDate.HasValue)
            {
                birthdate = BirthdaydatePicker.SelectedDate.Value;

                selectedDateText3.Text = birthdate.ToString("yyyy.MM.dd");

            }
        }

        private void RegionBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(RegionBox.Text) && RegionBox.Text.Length > 0)
            {
                RegionBoxText.Visibility = Visibility.Collapsed;
            }
            else
            {
                RegionBoxText.Visibility = Visibility.Visible;
            }
        }
        private void RegBoxText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RegionBox.Focus();
        }
    }
}
