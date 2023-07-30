using Domain.DatabaseSettings;
using Domain.Solutions;
using Domain.User;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Services.UserServices
{
    public class AnswerService : IAnswerService
    {
        private readonly IMongoCollection<User> _collection;

        public AnswerService(ITgsSolverDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public List<Answer> GetAllAnswers(string id)
        {
            return _collection.Find(user => user.Id == id).FirstOrDefault().SolutionsHistory;
        }

        public List<Answer> GetSavedAnswers(string id)
        {
            return _collection.Find(user => user.Id == id).FirstOrDefault().SolutionsHistory.FindAll(answer => answer.Star == true);
        }

        public string SaveAnswer(string id, Answer answer)
        {
            answer.Id = ObjectId.GenerateNewId().ToString();
            var filter = Builders<User>.Filter.Eq("Id", id);
            var update = Builders<User>.Update.Push("SolutionsHistory", answer);
            _collection.UpdateOne(filter, update);

            return answer.Id;
        }

        // star answer
        public void StarAnswer(string user_id, string answer_id)
        {
            var filter = Builders<User>.Filter.Eq("Id", user_id);
            var user = _collection.Find(filter).FirstOrDefault();
            if (user == null)
            {
                throw new Exception("User not found, StarAnswer");
            }

            var answer = user.SolutionsHistory.Find(answer => answer.Id == answer_id);
            if (answer == null)
            {
                throw new Exception("Answer not found, StarAnswer");
            }

            answer.Star = true;
            var update = Builders<User>.Update.Set("SolutionsHistory", user.SolutionsHistory);
            _collection.UpdateOne(filter, update);
        }

        // get answer
        public Answer GetAnswer(string user_id, string answer_id)
        {
            var filter = Builders<User>.Filter.Eq("Id", user_id);
            var user = _collection.Find(filter).FirstOrDefault();
            if (user == null)
            { return null; }

            return user.SolutionsHistory.Find(answer => answer.Id == answer_id);
        }

        // remove star
        public List<Answer> RemoveStar(string user_id, string answer_id)
        {
            var filter = Builders<User>.Filter.Eq("Id", user_id);
            var user = _collection.Find(filter).FirstOrDefault();
            if (user == null)
            {
                throw new Exception("User not found, RemoveStar");
            }

            var answer = user.SolutionsHistory.Find(answer => answer.Id == answer_id);
            if (answer == null)
            {
                throw new Exception("Answer not found, RemoveStar");
            }

            answer.Star = false;
            var update = Builders<User>.Update.Set("SolutionsHistory", user.SolutionsHistory);
            _collection.UpdateOne(filter, update);

            // return the solution history where star is true
            var new_list = user.SolutionsHistory.FindAll(answer => answer.Star == true);
            return new_list;
        }

        // remove answer
        public List<Answer> RemoveAnswer(string user_id, string answer_id)
        {
            var filter = Builders<User>.Filter.Eq("Id", user_id);
            var user = _collection.Find(filter).FirstOrDefault();
            if (user == null)
            {
                throw new Exception("User not found, RemoveAnswer");
            }

            var answer = user.SolutionsHistory.Find(answer => answer.Id == answer_id);
            if (answer == null)
            {
                throw new Exception("Answer not found, RemoveAnswer");
            }

            user.SolutionsHistory.Remove(answer);
            var update = Builders<User>.Update.Set("SolutionsHistory", user.SolutionsHistory);
            _collection.UpdateOne(filter, update);

            return user.SolutionsHistory;
        }
    }
}
