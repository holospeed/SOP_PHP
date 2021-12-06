using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

namespace SOP_FRONT
{
    /// <summary>
    /// Interaction logic for AuthScreen.xaml
    /// </summary>
    public partial class AuthScreen : Page
    {

        // String URL = "http://aries.ektf.hu/~ksanyi/restapi/v1/";
        //  String ROUTE = "index.php";

        string URL = Constans.URL;
        string ROUTE = "auth.php";

       

        public AuthScreen()
        {
            InitializeComponent();
            Button login = FindName("SAVE") as Button;
            login.Click += Login_Click;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {

            TextBox username = FindName("InputUsername") as TextBox;
            TextBox password = FindName("InputPassword") as TextBox;
            TextBlock error = FindName("error") as TextBlock;
            TextBlock userError = FindName("userError") as TextBlock;

            string usernamValue = username.Text;
            string passwordValue = password.Text;

            if (usernamValue.Length < 4 && passwordValue.Length < 4)
            {
                userError.Visibility = Visibility.Visible;
            }
            else
            {
                userError.Visibility = Visibility.Hidden;

                var client = new RestClient(URL);
                ROUTE += "?username=" + usernamValue + "&password=" + passwordValue;
                var request = new RestRequest(ROUTE, Method.GET);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                if (response.StatusCode == HttpStatusCode.OK)
                {

                    AuthData data = new JsonDeserializer().Deserialize<AuthData>(response);

                    string jwt = data.jwt;
                    NavigationService.Navigate(new MainScreen(jwt, data.id));

                }
                else
                {
                    error.Visibility = Visibility.Visible;
                }

            }

           


        }
    }

    internal class HomeTimeline
    {
    }

    public class AuthData
    {
        public string message { get; set; }
        public string jwt { get; set; }
        public string username { get; set; }
        public decimal expireAt { get; set; }
        public int id { get; set; }
    }
}
