using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class wrap_C_HC_LCG
    {
        private const int BlockSize = 4;

        public static (string stream, long[][] state) Next(bool init_flag, long[][] state_in = null, string seed_in = null, long[][] set_in = null, Func<int[], string, int, int[]> oneWayFunction = null)
        {
            long[][] state = new long[BlockSize][];
            string stream = "";
            if (init_flag == true)
            {
                string seed = Check_Seed(seed_in, oneWayFunction);
                string[] sub_seed_in = new string[seed_in.Length / BlockSize]; // Задаем массив, в котором будет храниться 4 части нашего seed
                for (int i = 0; i < BlockSize; i++)
                {
                    sub_seed_in[i] = seed.Substring(i * BlockSize, BlockSize); // Делим входящий seed из 16 символов на 4 части и каждую часть добавляем в массив
                    int[] codes = Blocks.ConvertToTelegraphCode(sub_seed_in[i]); // Конвертируем каждую часть символов в телеграфный код из 32 символов
                    long[] longArray = codes.Select(x => (long)x).ToArray(); // преобразование int к long
                    state[i] = EnhancedLCG.make_seed(longArray, oneWayFunction); // Хранит 3 массива с 4-мя значениями long
                    if (i > 0)
                    {
                        for (int q = 0; q < i + 1; q++)
                        {
                            EnhancedLCG enhLCG = new EnhancedLCG(state[i], set_in);
                            var T = enhLCG.Next();
                            state[i] = T.Item2;
                        }
                    }
                }
            }
            else if (init_flag == false)
            {
                state = state_in;
            }


            for (int j = 0; j < BlockSize; j++)
            {

                long tmp = 0;
                long sign = 1;

                for (int i = 0; i < BlockSize; i++) // Аналог state <-- seed2nums... итерации создания H-LCG
                {
                    EnhancedLCG enhLCG = new EnhancedLCG(state[i], set_in); // Здесь должен создаваться новый enhLCG с новым state
                    var T = enhLCG.Next();
                    state[i] = T.Item2; // Новый state задаётся здесь
                    tmp = (1048576 + sign * T.Item1 + tmp) % 1048576;
                    sign = -sign;

                }
                stream = stream + Blocks.ConvertFromTelegraphCode(Blocks.LongToBlock(tmp));
            }
            return (stream, state);

        }

        public static string Check_Seed(string seed_in, Func<int[], string, int, int[]> oneWayFunction)
        {
            string C = "ОТВЕТСТВЕННЫЙ_ПОДХОД";
            string[] T = new string[BlockSize];
            for (int i = 0; i < BlockSize; i++)
            {
                T[i] = seed_in.Substring(i * BlockSize, BlockSize);
            }
            for (int j = 0; j < 3; j++)
            {
                for (int i = j+1; i < BlockSize; i++)
                {
                    if (T[i] == T[j])
                    {
                        int[] plainTextCodes = Blocks.ConvertToTelegraphCode(T[j]);
                        int[] resOneWay = oneWayFunction(plainTextCodes, C, j + 2 * i);
                        T[i] = Blocks.ConvertFromTelegraphCode(resOneWay);
                    }
                }
            }
            return T[0] + T[1] + T[2] + T[3];
        }
    }
}
