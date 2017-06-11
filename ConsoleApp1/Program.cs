using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace CryptMethods1
{
    class Program
    {
        public static void PrintMatrix(double[,] matrix, string path)
        {
            var result = new List<string>();
            for (int i = 0; i < 20; i++)
            {
                var sb = new StringBuilder();
                for (int j = 0; j < 20; j++)
                {
                    sb.Append($"{Math.Round(matrix[i, j], 2)}, ");
                }
                result.Add($"{sb.ToString()}");
            }
            File.WriteAllLines(path, result);
        }
        public static void PrintArray(double[] arr, string path)
        {
            var result = new List<string>();
            for (int i = 0; i < 20; i++)
            {
                var sb = new StringBuilder();
                sb.Append($"{Math.Round(arr[i], 2)}\t");
                result.Add($"{sb.ToString()}");
            }
            File.WriteAllLines(path, result);
        }

        public static int[,] ReadTable()
        {
            var result = new int[20, 20];
            var table = File.ReadAllLines("table_08.csv");
            var tablePrepared = table.Select(x => x.Split(',')).ToArray();
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    result[i, j] = int.Parse(tablePrepared[i][j]);
                }
            }
            return result;
        }
        public static double[,] ReadProb()
        {
            var result = new double[2, 20];
            var table = File.ReadAllLines("prob_08.csv");
            var tablePrepared = table.Select(x => x.Split(',')).ToArray();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    result[i, j] = double.Parse(tablePrepared[i][j], CultureInfo.InvariantCulture);
                }
            }
            return result;
        }
        static void Main(string[] args)
        {
            var probs = ReadProb();
            var table = ReadTable();

            double[,] PMK = new double[20,20];
            for (byte i = 0; i < 20; i++)
            {
                for (byte j = 0; j < 20; j++)
                {
                    PMK[i, j] = probs[0, i] * probs[1, j];
                }
            }

            double[] PC = new double[20];
            double[,] PMC = new double[20, 20];

            for (byte m = 0; m < 20; m++)
            {
                for (byte k = 0; k < 20; k++)
                {
                    PC[table[k, m]] += PMK[m, k];
                    PMC[m, table[k, m]] += PMK[m, k];
                }
            }

            Console.Write("---------table[m][k]---------");
            Console.Write("\n");
            for (byte m = 0; m < 20; m++)
            {
                for (byte c = 0; c < 20; c++)
                {
                    Console.Write(table[m, c]);
                    Console.Write(" ");
                }
                Console.Write("\n");
            }

            Console.Write("---------P(C)---------");
            Console.Write("\n");
            for (byte c = 0; c < 20; c++)
            {
                Console.Write(PC[c]);
                Console.Write(" ");
            }
            Console.Write("\n");
            Program.PrintArray(PC, @"P(C).txt");
            Console.Write("---------P(M,C)---------");
            Console.Write("\n");
            for (byte m = 0; m < 20; m++)
            {
                for (byte c = 0; c < 20; c++)
                {
                    Console.Write(PMC[m, c]);
                    Console.Write(" ");
                }
                Console.Write("\n");
            }
            Program.PrintMatrix(PMC, @"P(M,C).txt");
            Console.Write("---------P(M/C)---------");
            Console.Write("\n");
            double[,] PMFC = new double[20, 20];
            for (byte m = 0; m < 20; m++)
            {
                for (byte c = 0; c < 20; c++)
                {
                    PMFC[m, c] = PMC[m, c] / PC[c];
                }
            }
            Program.PrintMatrix(PMFC, @"P(M_C).txt");
            for (byte m = 0; m < 20; m++)
            {
                for (byte c = 0; c < 20; c++)
                {
                    Console.Write(PMFC[m, c]);
                    Console.Write(" ");
                }
                Console.Write("\n");
            }
            double[] det_func = new double[20];
            byte[] det_index = new byte[20];
            for (byte c = 0; c < 20; c++)
            {
                for (byte m = 0; m < 20; m++)
                {
                    if (det_func[c] < PMFC[m, c])
                    {
                        det_func[c] = PMFC[m, c];
                        det_index[c] = m;
                    }
                }
            }
            Console.Write("---------det_function---------");
            Console.Write("\n");

            for (byte c = 0; c < 20; c++)
            {
                Console.Write(det_func[c]);
                Console.Write(" ");
            }
            Console.Write("\n");

            Program.PrintArray(det_func, @"DetFuncrion.txt");
            Console.Write("---------det_indexes---------");
            Console.Write("\n");
 
            for (byte i = 0; i < 20; i++)
            {
                Console.Write((int)det_index[i]);
                Console.Write(" ");
            }
            Console.Write("\n");
            double[] b_i= new double[20];
            for (int i = 0; i < 20; i++)
            {
                b_i[i] = System.Convert.ToDouble(det_index[i]);
            }
            Program.PrintArray(b_i, @"DetIndex.txt");
            double[,] stoh = new double[20,20];
            for (byte i = 0; i < 20; i++)
            {
                byte count = 0;
                for (byte j = 0; j < 20; j++)
                {
                    if (PMFC[j, i] == det_func[i])
                    {
                        count++;
                    }
                }

                for (byte j = 0; j < 20; j++)
                {
                    if (PMFC[j, i] == det_func[i])
                    {
                        stoh[i, j] = 1.0 / ((double)count);
                    }
                }
            }

            Console.Write("---------stoh[c][m]---------");
            Console.Write("\n");
            for (byte j = 0; j < 20; j++)
            {
                for (byte i = 0; i < 20; i++)
                {
                    Console.Write(stoh[j, i]);
                    Console.Write(" ");
                }
                Console.Write("\n");
            }
            Program.PrintMatrix(stoh, @"StohasticMatrix.txt");
            double d_loss = 0;
            double s_loss = 0;

            for (byte c = 0; c < 20; c++)
            {
                d_loss += PC[c] * (1 - det_func[c]);
            }

            for (byte c = 0; c < 20; c++)
            {
                for (byte m = 0; m < 20; m++)
                {
                    s_loss += PMC[m,c] * stoh[c, m];
                }
            }
            s_loss = 1 - s_loss;
            Console.Write("det_loss->");
            Console.Write(d_loss);
            Console.Write("\n");
            Console.Write("stoh_loss->");
            Console.Write(s_loss);
            Console.Write("\n");
            System.Console.ReadKey(true);
        }
 


    }
    
}
