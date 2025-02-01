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
    public partial class CreateOSAGO : Page
    {
        private static readonly Regex FIOregex = new Regex(@"^[А-ЯЁ][а-яё]+$");
        private static readonly Regex PasswordRegex = new Regex(@"^(?=.*\d)[a-zA-Z0-9]{6,}$");
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        private readonly Entities db;


        private Users currentuser = new Users();
        public CreateOSAGO()
        {
            InitializeComponent();
            db = new Entities();

            int currentUserId = (int)App.Current.Resources["CurrentUserId"];
            var driver = db.Drivers.AsNoTracking().FirstOrDefault(u => u.UserID == currentUserId);
            string GosNumber = "";

            if (driver != null)
            {
                // Находим автомобиль, связанный с водителем
                var car = db.Cars.FirstOrDefault(c => c.DriverID == driver.DriverID);

                if (car != null)
                {
                    // Записываем номер автомобиля в переменную GosNumber
                    GosNumber = car.LicensePlate;
                    Console.WriteLine($"Номер автомобиля: {GosNumber}");
                }
                else
                {
                    Console.WriteLine("Автомобиль не найден.");
                }
            }
            else
            {
                Console.WriteLine("Водитель не найден.");
            }

            if (driver != null)
            {
                UdoNumberBox.Text = driver.LicenseSeriesNumber;
                FIOBox.Text = driver.FullName;
                TCNumberBox.Text = GosNumber;
            }

        }

        private void GoBackPage()
        {
            NavigationService?.Navigate(new Pages.DriverPages.DriverAuthPage());

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            UdoNumberBox.Text = string.Empty;
            FIOBox.Text = string.Empty;
            GoBackPage();
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

        private void RegNumberBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(RegNumberBox.Text) && RegNumberBox.Text.Length > 0)
            {
                RegNumberBoxText.Visibility = Visibility.Collapsed;
            }
            else
            {
                RegNumberBoxText.Visibility = Visibility.Visible;
            }
        }

        private void RegNumberBoxText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RegNumberBox.Focus();
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
            string policyNumber = GenerateOsagoPolicyNumber();
            DateTime currentdate = DateTime.Now;
            DateTime expiryDate = currentdate.AddYears(1);
            string FIO = FIOBox.Text;
            string org = OrgComboBox.Text;
            string license = UdoNumberBox.Text;
            int company = OrgComboBox.SelectedIndex;
            string GosNumber = TCNumberBox.Text;
            Random random = new Random();
            decimal cost = random.Next(5000, 7001);

            int userid = (int)App.Current.Resources["CurrentUserId"];

            int? userId = db.Drivers
                        .AsNoTracking()
                        .Where(u => u.UserID == userid)
                        .Select(u => (int?)u.DriverID)
                        .FirstOrDefault();

            try
            {
                
                // Создание нового пользователя
                var policy = new Policies
                {
                    PolicyNumber = policyNumber,
                    CompanyID = company,
                    DriverID = userId,
                    IssueDate = currentdate,
                    ExpiryDate = expiryDate,
                    LicenseSeriesNumber = license,
                    LicensePlate = GosNumber,
                    Cost = cost,
                };

                db.Policies.Add(policy);
                db.SaveChanges();
                GoBackPage();
                MessageBox.Show("Заявка завершена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.Navigate(new Pages.DriverPages.OSAGOPaymentPage());


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

       

    }
}
