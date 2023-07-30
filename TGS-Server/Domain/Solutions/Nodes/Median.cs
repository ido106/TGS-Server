namespace Domain.Lines
{
    public class Median : Node
    {
        public Median(string key, string reason, Node node) : base(key, null, reason, node)
        {
            typeName = "תיכון";
        }
        public Median(string key, string reason) : base(key, null, reason)
        {
            typeName = "תיכון";
        }
        public override string ToString()
        {
            return name;
        }
    }
}
