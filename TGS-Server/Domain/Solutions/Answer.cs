using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Solutions
{
    public class Answer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("question")]
        public List<PairWrapper<string, List<String>>> Quest { get; set; }
        //public Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> Quest { get; set; }

        [BsonElement("claims")]
        public Dictionary<string, string> ClaimAndReason { get; set; } = new Dictionary<string, string>();

        [BsonElement("star")]
        public bool Star { get; set; } = false;

        [BsonElement("time")]
        public string Time { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("svg")]
        public string Svg { get; set; }
    }
}
