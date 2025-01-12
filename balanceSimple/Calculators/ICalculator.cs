namespace balanceSimple.Calculators
{
    public interface ICalculator
    {
        public List<double> Calculate(int iterCount, double[,] Ab, double[] x0, double[] errors, byte[] I, double[] lb, double[] ub);
    }
}
