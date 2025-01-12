namespace balanceSimple.Calculators
{
    public class Calculator
    {
        private readonly ICalculator _calculationStrategy;

        // Внедрение зависимости: передаём стратегию в конструктор
        public Calculator(ICalculator calculationStrategy)
        {
            _calculationStrategy = calculationStrategy;
        }

        // Метод для расчёта с использованием выбранной стратегии
        public List<double> Calculate(int iterCount, double[,] Ab, double[] x0, double[] errors, byte[] I, double[] lb, double[] ub)
        {
            return _calculationStrategy.Calculate(iterCount, Ab, x0, errors, I, lb, ub);
        }
    }
}
