using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class XOR
    {
        private const int BlockSize = 4;
        public static string subblocks_xor(string Block_A_IN, string Block_B_IN)
        {
            long decA = Blocks.BlockToLong(Blocks.ConvertToTelegraphCode(Block_A_IN));
            long decB = Blocks.BlockToLong(Blocks.ConvertToTelegraphCode(Block_B_IN));
            int[] binA = Blocks.ToBinaryArray(decA);
            int[] binB = Blocks.ToBinaryArray(decB);
            int[] binO = new int[binA.Length];
            for (int i = 0; i < binA.Length; i++)
            {
                binO[i] = (binA[i] + binB[i]) % 2;
            }
            long decO = Blocks.FromBinaryArray(binO);
            return Blocks.ConvertFromTelegraphCode(Blocks.LongToBlock(decO));
        }

        public static string block_xor(string Block_A_IN, string Block_B_IN) 
        {
            int nb = Block_A_IN.Length / BlockSize;
            string result = "";
            for (int i = 0; i < nb; i++)
            {
                string tmpA = Block_A_IN.Substring(i * 4, BlockSize);
                string tmpB = Block_B_IN.Substring(i * 4, BlockSize);
                result = result + subblocks_xor(tmpA, tmpB);
            }
            return result;
        }
    }
}
