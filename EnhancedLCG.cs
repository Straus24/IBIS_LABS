using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class EnhancedLCG
    {
        private LCG baseGenerator;
        private long hiddenState; //Скрытое управление

        public EnhancedLCG(long a, long c, long m, long initialState)
        {
            baseGenerator = new LCG(a, c, m);
            hiddenState = initialState; // Инициализация скрытого состояния
        }

        // Усиленное преобразование с скрытым управлением
        public long Next(long seed)
        {
            // Генерация скрытого состояние 
            hiddenState = baseGenerator.Next(hiddenState);

            // Основное состояние зависит от скрытого состояния
            long result = baseGenerator.Next(seed + hiddenState);

            return result;
        }
    }

    // Пример вызова
    // EnhancedLCG enhancedGenerator = new EnhancedLCG(5, 3, 1048576, 999); // Инициализация скрытого состояние
    // long newSeed = enhancedGenerator.Next(12345); // Усиленное преобразование
}
