using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class LongToInt32_mass
    {
        public static int[][] Convert(long[][] longArray)
        {
            // Проверка на null
            if (longArray == null) throw new ArgumentNullException(nameof(longArray));

            int[][] intArray = new int[longArray.Length][];

            for (int i = 0; i < longArray.Length; i++)
            {
                intArray[i] = new int[longArray[i].Length];
                for (int j = 0; j < longArray[i].Length; j++)
                {
                    if (longArray[i][j] < int.MinValue || longArray[i][j] > int.MaxValue)
                    {
                        throw new OverflowException($"Значение {longArray[i][j]} не может быть преобразовано в int.");
                    }
                    intArray[i][j] = (int)longArray[i][j];
                }
            }

            return intArray;
        }
    }
}
