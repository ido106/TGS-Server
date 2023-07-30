namespace Domain.Lines
{
    public class Height : Node
    {
        public Height(string key, string reason, Node node) : base(key, null, reason, node)
        {
            typeName = "גובה";
        }
        public Height(string key, string reason) : base(key, null, reason)
        {
            typeName = "גובה";
        }
        public override string ToString()
        {
            return name;
        }
    }
}
