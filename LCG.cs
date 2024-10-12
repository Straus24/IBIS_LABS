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
        private long currentState;

        public LCG(long a, long c, long m, long seed)
        {
            this.a = a;
            this.c = c;
            this.m = m;
            currentState = seed;
        }

        // Функция генерации нового состояния
        public long Next()
        {
            long state = (a * currentState + c) % m;
            return state;
        }
    }
}
