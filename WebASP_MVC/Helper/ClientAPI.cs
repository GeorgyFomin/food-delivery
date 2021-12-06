namespace WebASP_MVC.Helper
{
    public class ClientAPI
    {
        public HttpClient Init()
        {
            HttpClient client = new();
            client.BaseAddress = new Uri("http://localhost:5234/");
            return client;
        }
    }
}
