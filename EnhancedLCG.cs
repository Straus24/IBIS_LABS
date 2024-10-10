//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ConsoleApp1
//{
//    class EnhancedLCG
//    {
//        private LCG lcg1, lcg2, lcg3;
//        private long hiddenState; // Скрытое управление
//        private const int u = 20;
//        private const long maxCharCode = 1114111;

//        public EnhancedLCG(long a, long c, long m, long initialState)
//        {
//            long Seed1 = Blocks.OneWayFunction(initialState, "АПЧХ", 10);
//            long Seed2 = Blocks.OneWayFunction(initialState, "Ч_ОК", 10);
//            long Seed3 = Blocks.OneWayFunction(initialState, "ШУРА", 10);

//            lcg1 = new LCG(a, c, m, Seed1);
//            lcg2 = new LCG(a, c, m, Seed2);
//            lcg3 = new LCG(a, c, m, Seed3);

//            hiddenState = initialState; // Инициализация скрытого состояния
//        }

//        // Усиленное преобразование с скрытым управлением
//        public long Next(long seed)
//        {
//            // Генерация скрытого состояние 
//            hiddenState = lcg1.Next();

//            long Sa = lcg1.Next();
//            long Sb = lcg2.Next();
//            long Sc = lcg3.Next();

//            Console.WriteLine($"Sa {Sa} Sb {Sb} Sc {Sc} ");

//            int countOnes = CountOnes(Sc);
//            bool isEven = (countOnes % 2 == 0);

//            long output;
//            if (isEven)
//            {
//                output = CombineBits(Sa, Sb);
//            }
//            else
//            {
//                output = CombineBits(Sb, Sc);
//            }

//            // Применение модуля для ограничения значения output
//            output = Math.Abs(output % maxCharCode);

//            // Добавление скрытого состояния с корректировкой через модуль
//            output = (output + Math.Abs(hiddenState % maxCharCode)) % maxCharCode;

//            // Проверка на недопустимые значения
//            if (output <= 0)
//            {
//                output = 1; // Заменяем на минимально допустимый символ
//            }

//            if (output < 0 || output > maxCharCode)
//            {
//                throw new Exception($"Код '{output}' не соответствует ни одному символу.");
//            }

//            return output;
//        }

//        private int CountOnes(long value)
//        {
//            int count = 0;
//            while (value != 0)
//            {
//                count += (int)(value & 1);
//                value >>= 1;
//            }
//            return count;
//        }

//        private long CombineBits(long first, long second)
//        {
//            long mask = (1L << u) - 1; // Маска на 20 бит
//            long firstPart = first & mask; // Берем 20 младших битов первого числа
//            long secondPart = second & mask; // Берем также 20 бит второго числа
//            return (firstPart << u) | secondPart; // Сливаем их в одно значение
//        }
//    }

//    // Пример вызова
//    // EnhancedLCG enhancedGenerator = new EnhancedLCG(5, 3, 1048576, 999); // Инициализация скрытого состояние
//    // long newSeed = enhancedGenerator.Next(12345); // Усиленное преобразование
//}
