namespace balanceSimple.Calculators
{
    public class Calculator : ICalculator
    {
        public List<double> Calculate(int iterCount, double[,] Ab, double[] x0, double[] errors, byte[] I, double[] lb, double[] ub)
        {
            // решение
            double[] x = { };

            // Получение размерности
            int n = errors.GetLength(0);
            int m = Ab.GetLength(0);

            // Формирование матрицы H = W * I, W = 1/error[i]^2
            double[,] H = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        H[i, j] = I[i] / Math.Pow(errors[i], 2);
                    }

                    else
                    {
                        H[i, j] = 0;
                    }
                }
            }

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
                    // Переменные для алглиба
                    double[] s = new double[n]; // == 1
                    int[] ct = new int[m];// == 0
                    bool isupper = true;

                    for (int i = 0; i < n; i++) s[i] = 1;
                    for (int i = 0; i < m; i++) ct[i] = 0;


                    alglib.minqpstate state;
                    alglib.minqpreport rep;

                    //create solver
                    alglib.minqpcreate(n, out state);
                    alglib.minqpsetquadraticterm(state, H, isupper);
                    alglib.minqpsetlinearterm(state, d);
                    alglib.minqpsetlc(state, Ab, ct);

                    alglib.minqpsetbc(state, lb, ub);

                    // Set scale of the parameters.
                    alglib.minqpsetscale(state, s);

                    // Solve problem with the sparse interior-point method (sparse IPM) solver.
                    alglib.minqpsetalgosparseipm(state, 0.0);
                    alglib.minqpoptimize(state);
                    alglib.minqpresults(state, out x, out rep);

                }

                catch (alglib.alglibexception alglib_exception)
                {
                    System.Console.WriteLine("ALGLIB exception with message '{0}'", alglib_exception.msg);
                }

                for (int i = 0; i < n; i++)
                {
                    x0[i] = x[i];
                }

            }

            for (int i = 0; i < n; i++)
                x[i] = Math.Round(x[i], 3);

            return x.ToList();
        }
    }
}
