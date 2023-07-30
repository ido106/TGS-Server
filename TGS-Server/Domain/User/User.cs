using Domain.Solutions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.User
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("solutions")]
        public List<Answer> SolutionsHistory { get; set; } = new List<Answer>();
    }
}
