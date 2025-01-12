namespace balanceSimple.Calculators
{
    // Стратегия для решения задачи с использованием ALGLIB
    public class AlglibCalculationStrategy : ICalculator
    {
        public List<double> Calculate(int iterCount, double[,] Ab, double[] x0, double[] errors, byte[] I, double[] lb, double[] ub)
        {
            double[] x = new double[errors.Length];

            int n = errors.Length;
            int m = Ab.GetLength(0);

            // Формирование матрицы H = W * I, W = 1/error[i]^2
            double[,] H = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    H[i, j] = (i == j) ? I[i] / Math.Pow(errors[i], 2) : 0;
                }
            }

            // Итерируем по количеству итераций
            for (int iter = 0; iter < iterCount; iter++)
            {
                // Формирование вектора d = -H * x0
                double[] d = new double[n];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        d[i] += -H[i, j] * x0[j];
                    }
                }

                try
                {
                    double[] s = new double[n]; // == 1
                    int[] ct = new int[m]; // == 0
                    bool isupper = true;

                    // Инициализация векторов
                    for (int i = 0; i < n; i++) s[i] = 1;
                    for (int i = 0; i < m; i++) ct[i] = 0;

                    // ALGLIB minqpstate для оптимизации
                    alglib.minqpstate state;
                    alglib.minqpreport rep;

                    // Создание решателя
                    alglib.minqpcreate(n, out state);
                    alglib.minqpsetquadraticterm(state, H, isupper);
                    alglib.minqpsetlinearterm(state, d);
                    alglib.minqpsetlc(state, Ab, ct);
                    alglib.minqpsetbc(state, lb, ub);

                    // Масштабирование параметров
                    alglib.minqpsetscale(state, s);

                    // Оптимизация с использованием Sparse IPM
                    alglib.minqpsetalgosparseipm(state, 0.0);
                    alglib.minqpoptimize(state);
                    alglib.minqpresults(state, out x, out rep);
                }
                catch (alglib.alglibexception ex)
                {
                    Console.WriteLine($"ALGLIB exception: {ex.msg}");
                    return null;
                }

                // Обновление x0 для следующей итерации
                Array.Copy(x, x0, n);
            }

            // Округление результата до 3 знаков после запятой
            return x.Select(val => Math.Round(val, 3)).ToList();
        }
    }
}
