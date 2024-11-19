using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Feistel
    {
        public static string Frw_P_Scitala(string blockIn) 
        {
            int length = blockIn.Length; // длина строки
            int q = length / 2; // половина длины строки
            int f = length % 2; // остаток длины строки при делении на 2

            // Разделение строки на две части
            string tmpA = blockIn.Substring(0, q + f);
            string tmpB = blockIn.Substring(q + f, q);

            // Выходная строка
            string output = "";

            // Основной цикл
            for (int i = 0; i < q; i++) 
            {
                if (i % 2 == 0) 
                {
                    output += tmpA[i];
                    output += tmpB[i];
                }
                else
                {
                    output += tmpB[i];
                    output += tmpA[i];
                }
            }

            if (f == 1)
            {
                output += tmpA[q];
            }
            return output;        
        }

        public static string Inv_P_Scitala(string blockIn)
        {
            int length = blockIn.Length; // Длина строки
            int q = length / 2; // Половина длины строки
            int f = length % 2; // Остаток длина строки при делении на 2

            // Временные строки для восстановления tmpA и tmpB
            string tmpA = "";
            string tmpB = "";

            // Основной цикл
            for (int i = 0; i < q; i++)
            {
                if (i % 2 == 0)
                {
                    tmpA += blockIn[2 * i];
                    tmpB += blockIn[2 * i + 1];
                }
                else
                {
                    tmpB += blockIn[2 * i];
                    tmpA += blockIn[2 * i + 1];
                } 
            }

            // Добавление последнего символа в tmpA, если длина строки нечетная
            if (f == 1)
            {
                tmpA += blockIn[2 * q];
            }

            return tmpA + tmpB;
        }
    }
}
