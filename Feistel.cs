using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
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

        public static string Frw_Routine_Feistel (string blockIn, string keyIn, int jIn)
        {
            int l = blockIn.Length;
            string left = blockIn.Substring(0, l/2);
            string right = blockIn.Substring(l/2, l/2);

            int[] right_int = Blocks.ConvertToTelegraphCode(right);
            int[] tmp = Blocks.FrwCesarM(right_int, keyIn, jIn);

            string str_tmp = Blocks.ConvertFromTelegraphCode(tmp);
            left = Blocks.add_txt(str_tmp, left);
            return right + left;
        }

        public static string Inv_Routine_Feistel(string blockIn, string keyIn, int jIn)
        {
            int l = blockIn.Length;
            string left = blockIn.Substring(0, l / 2);
            string right = blockIn.Substring(l / 2, l / 2);

            int[] left_int = Blocks.ConvertToTelegraphCode(left);
            int[] tmp = Blocks.FrwCesarM(left_int, keyIn, jIn);

            string str_tmp = Blocks.ConvertFromTelegraphCode(tmp);
            right = Blocks.sub_txt(right, str_tmp);
            return right + left;
        }

        public static string Frw_Inner_Feistel(string blockIn, string keyIn, int rIn)
        {
            string intermediate = blockIn;
            for (int i = 0; i < rIn; i++)
            {
                intermediate = Frw_Routine_Feistel(intermediate, keyIn, i * 4);
            }
            return intermediate;
        }

        public static string Inv_Inner_Feistel(string blockIn, string keyIn, int rIn)
        {
            string intermediate = blockIn;
            for (int i = 0; i < rIn; i++)
            {
                intermediate = Inv_Routine_Feistel(intermediate, keyIn, 4 * (rIn - i - 1));
            }
            return intermediate;
        }

        public static string Frw_Inner_FeistelM(string blockIn, string keyIn, int rIn)
        {
            string intermediate = blockIn;
            string tmp;
            for (int i = 0; i < rIn; i++)
            {
                tmp = Frw_Routine_Feistel(intermediate, keyIn, i * 4);
                intermediate = Frw_P_Scitala(tmp);
            }
            return intermediate;
        }

        public static string Inv_Inner_FeistelM(string blockIn, string keyIn, int rIn)
        {
            string intermediate = blockIn;
            string tmp;
            for (int i = 0; i < rIn; i++)
            {
                tmp = Inv_P_Scitala(intermediate);
                intermediate = Inv_Routine_Feistel(tmp, keyIn, 4 * (rIn - i - 1));
            }
            return intermediate;
        }

        public static string round_Feistel(string blockIn, string keyIn)
        {
            string left = blockIn.Substring(0, 8);
            string right = blockIn.Substring(8, 8);
            string tmp = Frw_Inner_FeistelM(right, keyIn, 4);
            left = XOR.block_xor(tmp, left);
            return right + left;
        }

        public static string swap_blocks(string blockIn)
        {
            string left = blockIn.Substring(0, 8);
            string right = blockIn.Substring(8, 8);
            return right + left;
        }

        public static string Frw_Feistel(string blockIn, string keyIn, Func<int[], string, int, int[]> sFun, int rounds)
        {
            // Проверка длины входного слова и ключа
            if (blockIn.Length < 16)
            {
                throw new ArgumentException("Входное слово должно содержать не менее 16 символов");
            }
            if (keyIn.Length < 16)
            {
                throw new ArgumentException("Ключ должен содержать не менее 16 символов");
            }

            string[] roundKeys = new string[rounds + 2]; // Генерация раундовых ключей
            string keys = Round_Keys.produce_round_keys(keyIn, rounds + 2, sFun);

            // Разбиение строки ключей на отдельные ключи
            int blockLength = blockIn.Length;
            for (int i = 0; i < roundKeys.Length; i++)
            {
                roundKeys[i] = keys.Substring(i * blockLength, blockLength);
            }

            string block = XOR.block_xor(blockIn, roundKeys[0]); // Предварительный XOR с первым ключом

            // Проведение раундов
            for (int i = 1; i <= rounds; i++)
            {
                block = round_Feistel(block, roundKeys[i]);
            }

            // Заключительный XOR с последним ключом
            block = XOR.block_xor(block, roundKeys[rounds + 1]);

            return block;
        }

        public static string Inv_Feistel(string blockIn, string keyIn, Func<int[], string, int, int[]> sFun, int rounds)
        {
            // Проверка длины входного слова и ключа
            if (blockIn.Length < 16)
            {
                throw new ArgumentException("Входное слово должно содержать не менее 16 символом");
            }
            if (keyIn.Length < 16)
            {
                throw new ArgumentException("Ключ должен содержать не менее 16 символов");
            }

            string keys = Round_Keys.produce_round_keys(keyIn, rounds + 2, sFun); // Генерация всех раундовых ключей

            // Разбиение строки ключей на отдельные ключи
            int blockLength = blockIn.Length;
            string[] roundKeys = new string[rounds + 2];
            for (int i = 0; i < roundKeys.Length; i++)
            {
                roundKeys[i] = keys.Substring(i * blockLength, blockLength);
            }

            // Начальный XOR с последним ключом
            string block = XOR.block_xor(blockIn, roundKeys[rounds + 1]);

            // Проведение обратных раундов
            for (int i = rounds; i >= 1; i--)
            {
                block = swap_blocks(round_Feistel(swap_blocks(block), roundKeys[i]));
            }

            // Заключительный XOR с первым ключом
            block = XOR.block_xor(block, roundKeys[0]);

            return block;

        }
    }
}
