namespace Domain.Lines
{
    public class AngleBisector : Node
    {
        public AngleBisector(string key, string reason, Node node) : base(key, null, reason, node)
        {
            typeName = "חוצה זווית";
        }
        public AngleBisector(string key, string reason) : base(key, null, reason)
        {
            typeName = "חוצה זווית";
        }
        public override string ToString()
        {
            return name;
        }
    }
}
