using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class EnhancedLCG
    {
        private LCG lcg1, lcg2, lcg3;        

        public EnhancedLCG(long[] state, long[][] set)
        {         
            lcg1 = new LCG(set[0][0], set[0][1], set[0][2], state[0]);
            lcg2 = new LCG(set[1][0], set[1][1], set[1][2], state[1]);
            lcg3 = new LCG(set[2][0], set[2][1], set[2][2], state[2]);
        }

        // Усиленное преобразование с скрытым управлением
        public (long output, long[] stateOut) Next()
        {
            long first = lcg1.Next();
            long second = lcg2.Next();
            long control = lcg3.Next();

            Console.WriteLine($"Sa {first} Sb {second} Sc {control} ");

            int n = count_unity_bits(control);
            long output;

            if (control % 2 == 0)
            {
                output = Compose_num(first, second, n);
            }
            else
            {
                output = Compose_num(second, first, n);
            }
            long[] stateOut = new long[] { first, second, control };

            return (output, stateOut);
        }

        private int count_unity_bits(long value)
        {
            long rem = value;
            int count = 0;
            while (rem != 0)
            {
                long tmp = rem % 2;
                rem = rem / 2;
                count += (int)tmp;
            }
            return count;
        }

        private long Compose_num(long num1_in, long num2_in, int cont_in)
        {
            if (cont_in > 0 && cont_in < 20)
            {
                int[] arr1 = Blocks.ToBinaryArray(num1_in);
                int[] arr2 = Blocks.ToBinaryArray(num2_in);
                int[] tmp = new int[arr1.Length];

                // Копируем первые count_in битов из arr1
                for (int i = 0; i < cont_in; i++)
                {
                    tmp[i] = arr1[i];
                }

                // Копируем оставшиеся биты из arr2
                for (int i = cont_in; i < arr2.Length; i++)
                {
                    tmp[i] = arr2[i];
                }

                // Преобразуем результат обратно в число
                return Blocks.FromBinaryArray(tmp);
            }
            else if (cont_in == 0)
            {
                return num1_in;
            }
            else
            {
                return num2_in;
            }
        }

        public static long[][] make_seed(long[] blockIn, Func<int[], string, int, int[]> osFun)
        {
            string[] generatorNames = { "ПЕРВЫЙ_ГЕНЕРАТОР", "ВТОРОЙ_ГЕНЕРАТОР", "ТРЕТИЙ_ГЕНЕРАТОР" };

            long[][] seeds = new long[3][];

            for (int i = 0; i < 3; i++)
            {
                int[] resultBlock = osFun(ConvertLongToIntArray(blockIn), generatorNames[i], 10); // Вызов односторонней функции
                seeds[i] = new long[] { Blocks.BlockToLong(resultBlock) };
            }

            return seeds;
        }


        public static int[] ConvertLongToIntArray(long[] blockIn)
        {
            // Создаем массив int такой же длины, как и blockIn
            int[] result = new int[blockIn.Length];

            // Проходим по каждому элементу blockIn и преобразуем в int
            for (int i = 0; i < blockIn.Length; i++)
            {
                // Проверяем, не превышает ли значение диапазон int
                if (blockIn[i] < int.MinValue || blockIn[i] > int.MaxValue)
                {
                    throw new OverflowException($"Значение {blockIn[i]} не может быть преобразовано в int.");
                }

                // Преобразуем и сохраняем в результат
                result[i] = (int)blockIn[i];
            }

            return result; // Возвращаем преобразованный массив
        }
    }
}
