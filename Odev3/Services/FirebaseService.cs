using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Firebase.Database.Query;
using Odev3.Models;

namespace Odev3.Services
{
    public static class FirebaseService
    {
        static string apiKey = "AIzaSyD8geEUBO8L9vM8vW38g1X5XExmRC7Lses";
        static string authDomain = "odev3-2566b.firebaseapp.com";

        static string dbUrl = "https://odev3-2566b-default-rtdb.firebaseio.com/";

        static FirebaseAuthConfig config = new FirebaseAuthConfig
        {
            ApiKey = apiKey,
            AuthDomain = authDomain,
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        };

        static FirebaseAuthClient authClient = new FirebaseAuthClient(config);
        static FirebaseClient dbClient = new FirebaseClient(dbUrl);

        public static async Task<bool> Register(string name, string email, string password)
        {
            try
            {
                await authClient.CreateUserWithEmailAndPasswordAsync(
                    email,
                    password,
                    name
                );
                return true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Firebase Register Hatası",
                    ex.Message,
                    "OK"
                );
                return false;
            }
        }



        public static async Task<string> Login(string email, string password)
        {
            try
            {
                var userCred = await authClient.SignInWithEmailAndPasswordAsync(email, password);
                return userCred.User.Uid;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LOGIN ERROR: " + ex.Message);
                return null;
            }
        }

        public static async Task<List<ToDoItem>> GetToDos()
        {
            try
            {
                var items = await dbClient
                    .Child("todos")
                    .OnceAsync<ToDoItem>();

                return items.Select(x =>
                {
                    x.Object.Key = x.Key;
                    return x.Object;
                }).ToList();
            }
            catch
            {
                return new List<ToDoItem>();
            }
        }

        public static async Task AddToDo(ToDoItem item)
        {
            await dbClient.Child("todos").PostAsync(item);
        }

        public static async Task UpdateToDo(ToDoItem item)
        {
            await dbClient.Child("todos").Child(item.Key).PutAsync(item);
        }

        public static async Task DeleteToDo(string key)
        {
            await dbClient.Child("todos").Child(key).DeleteAsync();
        }
    }
}
