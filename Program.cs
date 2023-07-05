/*
3)	Необходимо написать приложение на C# (вид не важен, консольное, или как хочется), 
которое будет выполнять авторизацию и скачивать целиком всю номенклатуру по любому из доступных демонстрационной учетной записи департаментов (аптек)

метод - /Goods/{depId} (/User/departments), сваггер - http://f3bus.test.pharmadata.ru/swagger/index.html
Сохранять результат можно куда угодно (например, в файлы на диск). 


PS: исходя из задачи, реализация простейшая.

*/

using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GetNomenclatureConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
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

                    // - Get user departments  
                    var users = await GetUserAnyDepartment(httpClient);

                    foreach (var user in users)
                    {
                        var selectedDepartment = user?.Department;
                        if (selectedDepartment == null)
                        {
                            Console.WriteLine($"User departament {user?.Department?.Name} not specified");
                            Console.WriteLine();
                        }
                        else
                        {
                            var nomenclatureList = await GetNomenclature(httpClient, selectedDepartment.Id);
                            var fileName = SaveToFile(selectedDepartment, nomenclatureList);
                            Console.WriteLine($"User departament {user?.Department?.Name} nomenclature saved to {fileName}");
                            Console.WriteLine();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Console.WriteLine("Enter for continue");
            Console.ReadKey();
        }



        private static string SaveToFile(Department selectedDepartment, List<Goods>? nomenclatureList)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,  "Nomenclatures");
            var filePath = Path.Combine(path, $"nomenclature_{selectedDepartment.Id}_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffffff")}.json");
            Directory.CreateDirectory(path);

            string json = JsonConvert.SerializeObject(nomenclatureList, Formatting.Indented);
            File.WriteAllText(filePath, json);
            return filePath;
        }



        private static async Task<string?> AuthenticateAsync(HttpClient httpClient, object auth)
        {
            var response = await httpClient.PostAsJsonAsync(@"/User/auth/F3bus", auth);
            TokenResponse? token = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return token?.Token;
        }



        private static async Task<List<User>> GetUserAnyDepartment(HttpClient httpClient)
        {
            List<User>? users = null;
            using (var response = await httpClient.GetAsync(@"/user/departments"))
            {
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                users = JsonConvert.DeserializeObject<List<User>>(json);
            }
            return users;
        }



        private static async Task<List<Goods>?> GetNomenclature(HttpClient httpClient, string id)
        {
            var nomenclatureList = await httpClient.GetFromJsonAsync<List<Goods>>(string.Format(@"/goods/{0}", id));
            return nomenclatureList;
        }

    }
}