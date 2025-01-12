using balanceSimple.Models;

namespace balanceSimple.Services
{
    public class CalculatorServiceWithLoggingDecorator : ICalculatorService
    {
        private readonly ICalculatorService _calculatorService;
        private readonly ILogger<CalculatorServiceWithLoggingDecorator> _logger;

        public CalculatorServiceWithLoggingDecorator(
            ICalculatorService calculatorService,
            ILogger<CalculatorServiceWithLoggingDecorator> logger)
        {
            _calculatorService = calculatorService;
            _logger = logger;
        }

        public BalanceOutput Calculate(BalanceInput balanceInput)
        {
            _logger.LogInformation("Начат расчёт баланса. Входные данные: {@BalanceInput}", balanceInput);

            var outputData = _calculatorService.Calculate(balanceInput);

            _logger.LogInformation("Вычисления завершены. Результаты: {@Results}", outputData);

            return outputData;
        }
    }
}
