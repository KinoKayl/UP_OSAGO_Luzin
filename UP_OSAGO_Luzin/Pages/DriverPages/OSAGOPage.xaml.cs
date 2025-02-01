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

namespace UP_OSAGO_Luzin.Pages.DriverPages
{
    /// <summary>
    /// Логика взаимодействия для OSAGOPage.xaml
    /// </summary>
    public partial class OSAGOPage : Page
    {
        private static readonly Regex FIOregex = new Regex(@"^[А-ЯЁ][а-яё]+$");
        private static readonly Regex PasswordRegex = new Regex(@"^(?=.*\d)[a-zA-Z0-9]{6,}$");
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        private readonly Entities db;


        private Users currentuser = new Users();
        public OSAGOPage()
        {
            InitializeComponent();
            db = new Entities();

            db = new Entities();

            int currentUserId = (int)App.Current.Resources["CurrentUserId"];
            var driver2 = db.Drivers.AsNoTracking().FirstOrDefault(u => u.UserID == currentUserId);
            var policy = db.Policies.FirstOrDefault(c => c.DriverID == driver2.DriverID);


            if (policy != null)
            {
                UdoNumberBox.Text = policy.LicenseSeriesNumber;
                FIOBox.Text = policy.f;
                TCNumberBox.Text = policy.LicensePlate;
            }
        }

        private void GoBackPage()
        {
            NavigationService?.Navigate(new Pages.DriverPages.DriverAuthPage());

        }

       

        

    }
}
