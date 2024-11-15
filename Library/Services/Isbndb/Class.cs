using System.Net;
using System.Text;

namespace Library.Services.Isbndb
{
    public class Api
    {
        public static async Task Main(string[] args)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://api2.isbndb.com");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "54525_f998566a066fc1456965169e594256ef");

                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "/books");

                string[] isbns = { "9781492666868", "9781616555719" };
                message.Content = new StringContent("isbns=" + String.Join(',', isbns), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.SendAsync(message);

                var jsonResponse = await response.Content.ReadAsStringAsync();

                Console.Write(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
