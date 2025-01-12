using balanceSimple.Models;

namespace balanceSimple.Services
{
    public interface ICalculatorService
    {
        public BalanceOutput Calculate(BalanceInput balanceInput);
    }
}
