namespace Domain.Lines
{
    public class PerpendicularBisector : Node
    {
        public PerpendicularBisector(string key, string reason, Node node) : base(key, null, reason, node)
        {
            typeName = "אנך אמצעי";
        }
        public PerpendicularBisector(string key, string reason) : base(key, null, reason)
        {
            typeName = "אנך אמצעי";
        }
        public override string ToString()
        {
            return name;
        }
    }
}
