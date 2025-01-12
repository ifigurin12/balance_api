namespace balanceSimple.Models
{
    public class BalanceOutput
    {
        public int Id { get; set; }
        public List<string> FlowsNames { get; set; }
        public List<double> InitValues { get; set; }
        public List<double> FinalValues { get; set; }
        public bool IsBalanced { get; set; }
    }
}
