namespace Domain.Lines
{
    public class MeansSection : Node
    {
        public MeansSection(string key, string reason, Node node) : base(key, null, reason, node)
        {
            typeName = "קטע אמצעים";
        }
        public MeansSection(string key, string reason) : base(key, null, reason)
        {
            typeName = "קטע אמצעים";
        }
        public override string ToString()
        {
            return name;
        }
    }
}
