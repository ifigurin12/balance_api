using balanceSimple.Calculators;
using balanceSimple.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace balanceSimple.Services
{
    public class CalculatorService : ICalculatorService
    {
        private readonly ILogger<CalculatorService> _logger;

        public CalculatorService(ILogger<CalculatorService> logger)
        {
            _logger = logger;
        }

        public BalanceOutput Calculate(BalanceInput balanceInput)
        {
            _logger.LogInformation("Начат расчёт баланса. Входные данные: {@BalanceInput}", balanceInput);

            if (balanceInput.flows.Count == 0)
            {
                _logger.LogWarning("Входной массив потоков пуст!");
                throw new ValidationException("Message: Flow array is empty!");
            }

            // Экземпляр класса калькулятор для вычислений
            ICalculator calculator = new Calculators.Calculator();

            // Экземпляр выходных данных
            var outputData = new BalanceOutput();

            // Переменные для имён и начальных значений потока
            List<string> names = new List<string>();
            List<double> startResults = new List<double>();

            // Количество итераций
            int iterCount = 2000;

            // Массивы для расчёта 
            double[] x0 = new double[balanceInput.flows.Count];
            double[] errors = new double[balanceInput.flows.Count];
            byte[] I = new byte[balanceInput.flows.Count];
            double[] lb = new double[balanceInput.flows.Count];
            double[] ub = new double[balanceInput.flows.Count];

            _logger.LogInformation("Инициализация массивов для расчётов завершена.");

            int i = 0;
            int size = 0;

            foreach (var flow in balanceInput.flows.OrderBy(w => w.Id))
            {
                if (flow.LowerBound > flow.UpperBound)
                {
                    _logger.LogError("Нижняя граница больше верхней для потока {FlowIndex}!", i + 1);
                    throw new ValidationException($"Message: Upper bound less then lower bound in flow {i + 1}!");
                }

                if (flow.Id != i)
                {
                    _logger.LogError("Пропущен поток с индексом {FlowIndex}!", i + 1);
                    throw new ValidationException($"Message: Flow {i + 1} is missing!");
                }

                if (flow.Value < 0 || flow.Tols < 0)
                {
                    _logger.LogError("Некорректные значения Value или Tols для потока {FlowIndex}!", i + 1);
                    throw new ValidationException($"Message: Value or tols are incorrect in {i + 1} flow");
                }

                _logger.LogWarning("Инициализация данных для потока {FlowName}: {FlowData}", flow.Name, flow);

                names.Add(flow.Name);
                startResults.Add(flow.Value);

                // Заполнение данных
                x0[i] = flow.Value;
                errors[i] = flow.Tols;
                if (flow.IsUsed) I[i] = 1;

                lb[i] = (double)flow.LowerBound;
                ub[i] = (double)flow.UpperBound;

                // Определение размера матрицы Ab
                if (flow.DestNode > size) size = (int)flow.DestNode;
                i++;
            }

            double[,] Ab = new double[size, balanceInput.flows.Count + 1];
            i = 0;

            // Заполнение матрицы Ab
            foreach (var flow in balanceInput.flows.OrderBy(w => w.Id))
            {
                if (flow.SourceNode != -1) Ab[(int)flow.SourceNode - 1, i] = -1;
                if (flow.DestNode != -1) Ab[(int)flow.DestNode - 1, i] = 1;
                i++;
            }

            _logger.LogInformation("Матрица ограничений Ab успешно сформирована. Размер: {Rows}x{Columns}", Ab.GetLength(0), Ab.GetLength(1));

            // Запуск расчёта
            _logger.LogInformation("Запуск вычислений с помощью Calculator.");
            List<double> results = calculator.Calculate(iterCount, Ab, x0, errors, I, lb, ub);

            _logger.LogInformation("Вычисления завершены. Результаты: {@Results}", results);

            outputData.FlowsNames = names;
            outputData.InitValues = startResults;
            outputData.FinalValues = results;
            outputData.IsBalanced = checkBalanced(Ab, results);

            _logger.LogInformation("Результаты проверки баланса: IsBalanced = {IsBalanced}", outputData.IsBalanced);

            return outputData;
        }

        public bool checkBalanced(double[,] Ab, List<double> result)
        {
            bool isAppropriate = true;
            double tolerance = 1e-6; // Задаем допустимую погрешность
            double sum = 0;

            for (int i = 0; i < Ab.GetLength(0); i++)
            {
                sum = 0;
                for (int j = 0; j < Ab.GetLength(1) - 1; j++)
                {
                    sum += Ab[i, j] * result[j];
                }


                if (Math.Abs(sum) > tolerance) // Проверка с учетом погрешности
                {
                    _logger.LogWarning("Несоответствие баланса в узле {Node}: сумма = {Sum}.", i + 1, sum);
                    isAppropriate = false;
                    break;
                }
            }

            return isAppropriate;
        }

    }
}
