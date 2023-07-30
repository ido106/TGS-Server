namespace Domain.Nodes
{
    public class Diagonal : Node
    {
        public Diagonal(string key, string reason, Node node) : base(key, null, reason, node)
        {
            typeName = "אלכסון";
        }
        public Diagonal(string key, string reason) : base(key, null, reason)
        {
            typeName = "אלכסון";
        }
        public override string ToString()
        {
            return name;
        }
    }
}
