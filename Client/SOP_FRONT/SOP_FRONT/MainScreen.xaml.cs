using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serialization.Json;
using System;
using System.Linq;
using System.Net;
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

namespace SOP_FRONT
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainScreen : Page
    {

        string URL  = Constans.URL;
       
        private static GetBody GetMethod(string URL, string jwt)
        {
            var client = new RestClient(URL);
            client.AddDefaultHeader("Authorization", "Barer " + jwt);
            RestRequest request = new RestRequest("post.php", Method.GET);
            IRestResponse<GetBody> response = client.Execute<GetBody>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }
            else
            {
                return new GetBody();
            }
           
        }

        [Obsolete]
        private static bool PostMethod(string URL, string jwt, string comment) 
        {

            var client = new RestClient(URL);
            client.AddDefaultHeader("Authorization", "Barer " + jwt);
            RestRequest request = new RestRequest("post.php", Method.POST);

            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddBody(new NewPost
            {
                comment = comment
             
            });
            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
           
        }

        private static bool DeleteMethod(string URL, string jwt, int comment_id)
        {

            var client = new RestClient(URL);
            client.AddDefaultHeader("Authorization", "Barer " + jwt);
            RestRequest request = new RestRequest("post.php?comment_id=" + comment_id, Method.DELETE);

       
            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private static bool PutMethod(string URL, string jwt, int comment_id, string comment)
        {

            var client = new RestClient(URL);
            client.AddDefaultHeader("Authorization", "Barer " + jwt);
            RestRequest request = new RestRequest("post.php?comment_id=" + comment_id + "&comment=" + comment, Method.PUT);


            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        public MainScreen(string jwt, int id)
        {
            InitializeComponent();
           

            Button addNewPostBTN = FindName("addNewPost") as Button;
            addNewPostBTN.Tag = "id#" + id + "#jwt#" + jwt;
            addNewPostBTN.Click += AddNewPost_Click;


            GetBody b = GetMethod(URL, jwt);
            foreach (Message data in b.message)
            {

                StackPanel stack = FindName("ProjectMainStackPanel") as StackPanel;
                StackPanel sMain = new StackPanel();
                StackPanel sFirst = new StackPanel();
                StackPanel sSecond = new StackPanel();
                StackPanel sThird = new StackPanel();
                Button edit = new Button();
                Button delete = new Button();

                TextBox     tbox = new TextBox();
                TextBlock tblock = new TextBlock();



                sMain.Orientation = Orientation.Horizontal;
                sFirst.Height = 50;
                sFirst.Width = 590;
                sSecond.Height = 50;
                sSecond.Width = 90;
                sThird.Height = 50;
                sThird.Width = 50;

                edit.Content = "MÓDOSÍT";
                delete.Content = "TÖRÖL";

                if (data.user_id == id)
                {
 
                    tbox.Text = data.content;
                    tbox.Name = "tbox_" + data.id;
                    RegisterName(tbox.Name, "tbox_" + data.id);
                    tbox.Tag = "id#" + id + "#jwt#" + jwt;
                    edit.Name = "edit_" + data.id;
                    edit.Tag = "id#" + id + "#jwt#" + jwt + "#comment_id#" + data.id;
                    edit.Click += Edit_Click;


                    delete.Name = "delete_" + data.id;
                    delete.Tag = "id#" + id + "#jwt#" + jwt + "#comment_id#" + data.id;
                    delete.Click += Delete_Click;

                    sFirst.Children.Add(tbox);
                    sSecond.Children.Add(edit);
                    sThird.Children.Add(delete);
                }
                else
                {
                    tblock.Text = data.content;
                    tblock.Name = "tblock_" + data.id;


                    sFirst.Children.Add(tblock);
                }

                sMain.Children.Add(sFirst);
                sMain.Children.Add(sSecond);
                sMain.Children.Add(sThird);

                stack.Children.Add(sMain);

            }



        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            string BTNName = ((Button)sender).Name;
            string textName = BTNName.Replace("edit_", "tbox_");
            StackPanel sp = FindName("ProjectMainStackPanel") as StackPanel;
            TextBox textBox =  (TextBox)LogicalTreeHelper.FindLogicalNode(sp, textName);

            Console.WriteLine(textBox.Text);

            string BtnTag = (string)((Button)sender).Tag;
            string[] subs = BtnTag.Split('#');
            int id = int.Parse(subs[1]);
            string jwt = subs[3];
            int comment_id = int.Parse(subs[5]);

            if(textBox.Text.Length > 0 && PutMethod(URL, jwt, comment_id, textBox.Text)) 
            {
                NavigationService.Navigate(new MainScreen(jwt, id));
            }

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            string BtnTag = (string)((Button)sender).Tag;
            string[] subs = BtnTag.Split('#');
            int id = int.Parse(subs[1]);
            string jwt = subs[3];
     
            int comment_id = int.Parse(subs[5]);

            if (DeleteMethod(URL, jwt, comment_id))
            {
                NavigationService.Navigate(new MainScreen(jwt, id));
            }



        }

      
        private void AddNewPost_Click(object sender, RoutedEventArgs e)
        {


            string BtnTag = (string)((Button)sender).Tag;
            string[] subs = BtnTag.Split('#');
            int id = int.Parse(subs[1]);
            string jwt = subs[3];

            TextBox addNewPostText = FindName("addNewPostText") as TextBox;
           
               if(addNewPostText.Text.Length > 0 && PostMethod(URL, jwt, addNewPostText.Text)) 
              {
                   NavigationService.Navigate(new MainScreen(jwt, id));
               }
        }

    }
}
