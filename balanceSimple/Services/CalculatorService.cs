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
        public BalanceOutput Calculate(BalanceInput balanceInput)
        {

            if (balanceInput.flows.Count == 0)
            {
                throw new ValidationException("Message: Flow array is empty!");
            }

            ICalculator calculationStrategy = new AlglibCalculationStrategy();

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

            int i = 0;
            int size = 0;

            foreach (var flow in balanceInput.flows.OrderBy(w => w.Id))
            {
                if (flow.LowerBound > flow.UpperBound)
                {
                    throw new ValidationException($"Message: Upper bound less then lower bound in flow {i + 1}!");
                }

                if (flow.Id != i)
                {
                    throw new ValidationException($"Message: Flow {i + 1} is missing!");
                }

                if (flow.Value < 0 || flow.Tols < 0)
                {
                    throw new ValidationException($"Message: Value or tols are incorrect in {i + 1} flow");
                }

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

            List<double> results = calculationStrategy.Calculate(iterCount, Ab, x0, errors, I, lb, ub);

            outputData.FlowsNames = names;
            outputData.InitValues = startResults;
            outputData.FinalValues = results;
            outputData.IsBalanced = checkBalanced(Ab, results);

            return outputData;
        }

        public bool checkBalanced(double[,] Ab, List<double> result)
        {
            bool isAppropriate = true;

            double sum = 0;
            for (int i = 0; i < Ab.GetLength(0); i++)
            {
                sum = 0;
                for (int j = 0; j < Ab.GetLength(1) - 1; j++)
                {
                    sum += Ab[i, j] * result[j];
                }

                if (Math.Round(sum, 1) != 0)
                {
                    isAppropriate = false;
                    break;
                }
            }

            return isAppropriate;
        }


    }
}
