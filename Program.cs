using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GetNomenclatureConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //User user = new User() { Login = "demo", Password = "demo" };

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://f3bus.test.pharmadata.ru/");
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    // - Get the JWT  
                    httpClient.DefaultRequestHeaders.Add(
                        "Authorization", 
                        "Bearer " + await AuthenticateAsync(httpClient, new { Login = "demo", Password = "demo" }));

                    // - Get first (any) user department  
                    User user = await GetUserAnyDepartment(httpClient);

                    //if (user.Departments == null)
                    //{
                    //    Console.WriteLine("Department not specified");
                    //    Console.WriteLine();
                    //}
                    //else
                    //{
                    //    var nomenclatureList = await GetNomenclature(httpClient, user.Departments.FirstOrDefault()?.Id);
                    //}



                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }




            }




            //foreach (var dept in departments) {
            //    Console.WriteLine($"{dept.Id} - {dept.Name}");

            //    var nomenclatureList = await GetNomenclature(dept.Id);
            //}







            Console.WriteLine("Enter for continue");
            Console.ReadKey();
        }


        private static async Task<string> AuthenticateAsync(HttpClient httpClient, object auth)
        {
            var response = await httpClient.PostAsJsonAsync(@"/User/auth/F3bus", auth);
            TokenResponse? token = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return token.Token;
        }



        private static async Task<Department[]> GetUserAnyDepartment(HttpClient httpClient)
        {
            User? user = null;
            using (var response = await httpClient.GetAsync(@"/user/departments"))
            {
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<User>(json);
            }


            //IEnumerable<Department>? departaments = await httpClient.GetFromJsonAsync<List<Department>?>(@"/user/departments");
            return user?.Departments == null 
                ? new Department[0]
                : user.Departments;
        }



        private static async Task<Goods?> GetNomenclature(HttpClient httpClient, string id)
        {
            var nomenclatureList = await httpClient.GetFromJsonAsync<List<Goods>>(string.Format(@"/goods/{0}", id));

            return null;
        }

    }
}