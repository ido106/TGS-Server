namespace Domain
{
    public class Prove
    {
        public string Statement { get; set; }
        public List<Input> Inputs { get; set; }
        public Prove(string statement, List<Input> inputs)
        {
            Statement = statement;
            Inputs = inputs;
        }

    }
}
