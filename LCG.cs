using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class LCG
    {
        private long a;
        private long c;
        private long m;

        public LCG(long a, long c, long m)
        {
            this.a = a;
            this.c = c;
            this.m = m;
        }

        // Функция генерации нового состояния
        public long Next(long seed)
        {
            return (a * seed + c) % m;
        }
    }

    // Пример вызова:
    /*LCG generator = new LCG(5, 3, 1048576);*/ // a, c, m
    /*long newSeed = generator.Next(12345);*/ // Генерация нового состояния
}
