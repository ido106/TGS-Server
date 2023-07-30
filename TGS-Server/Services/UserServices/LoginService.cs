using Domain.DatabaseSettings;
using Domain.Solutions;
using Domain.User;
using MongoDB.Driver;

namespace Services.UserServices
{
    public class LoginService : ILoginService
    {
        private readonly IMongoCollection<User> _collection;

        public LoginService(ITgsSolverDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public string HandleLogin(string email)
        {
            if (GetByEmail(email) == null)
            {
                Create(new User { Email = email, SolutionsHistory = new List<Answer>() });
                User user = GetByEmail(email);
                return user.Id;
            }
            else
            {
                return GetByEmail(email).Id;
            }
        }

        public User Create(User user)
        {
            _collection.InsertOne(user);
            return user;
        }

        public List<User> Get()
        {
            return _collection.Find(user => true).ToList();
        }

        public User GetByEmail(string email)
        {
            return _collection.Find(user => user.Email == email).FirstOrDefault();
        }

        public User GetById(string id)
        {
            return _collection.Find(user => user.Id == id).FirstOrDefault();
        }

        public void RemoveById(string id)
        {
            _collection.DeleteOne(user => user.Id == id);
        }

        public void RemoveByEmail(string email)
        {
            _collection.DeleteOne(user => user.Email == email);
        }

        public void UpdateById(string id, User user)
        {
            _collection.ReplaceOne(user => user.Id == id, user);
        }

        public void UpdateByEmail(string email, User user)
        {
            _collection.ReplaceOne(user => user.Email == email, user);
        }
    }
}
