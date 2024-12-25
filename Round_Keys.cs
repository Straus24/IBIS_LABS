using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Round_Keys
    {

        public static string produce_round_keys(string KEY_IN, int num_in, Func<int[], string, int, int[]> oneWayFunction = null)
        {
            long[][] set = {
                new long[] { 723482, 8677, 983609 },
                new long[] { 252564, 9109, 961193 },
                new long[] { 357630, 8971, 948209 }
            };

            var wrap_result = wrap_C_HC_LCG.Next(true, null, KEY_IN, set, oneWayFunction);
            string wrap_result_out = wrap_result.Item1;

            if (num_in > 1)
            {
                for (int i = 0; i < num_in - 1; i++)
                {
                    wrap_result = wrap_C_HC_LCG.Next(false, wrap_result.Item2, null, set, null);
                    wrap_result_out = wrap_result_out + wrap_result.Item1;
                }
            }
            return wrap_result_out;
        }
    }
}
