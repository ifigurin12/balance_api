namespace balanceSimple.Models
{
    public class Flow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public double Tols { get; set; }
        public bool IsUsed { get; set; }
        public double LowerBound { get; set; }
        public double UpperBound { get; set; }
        public int SourceNode { get; set; }
        public int DestNode { get; set; }
    }
}
