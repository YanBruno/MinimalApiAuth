using MinimalApiAuth.Models;

namespace MinimalApiAuth.Repository
{
        public static class UserRepository{

        private static IList<User> _users;

        static UserRepository() => 
            _users = new List<User>{
                new(){Id = 1, Username = "batman", Password = "batman", Role = "manager"},
                new(){Id = 1, Username = "yansantos", Password = "123", Role = "manager"},
                new(){Id = 1, Username = "joao", Password = "123", Role = "employee"}
            };
        
        public async static Task<IEnumerable<User>> GetUsers(){
            await Task.Delay(5000);
            return _users;
        }
        
        public async static Task<User> GetUser(string username, string password) {
            await Task.Delay(3000);
            return _users.FirstOrDefault(
                u => u.Username == username
                && u.Password == password
            );
        }
    }
}