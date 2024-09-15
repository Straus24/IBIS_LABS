using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class CESAR
    {
        private static TelegraphAlphabet alphabet;

        public CESAR()
        {
             alphabet = new TelegraphAlphabet();
        }

        public static char Encrypt(char input, char key)
        {
            byte inputCode = alphabet.GetCode(input);
            byte keyCode = alphabet.GetCode(key);
            int encryptedCode = alphabet.SumCode(inputCode, keyCode);
            return alphabet.GetSymbolByCode(encryptedCode);
        }

        public static char Decrypt(char input, char key)
        {
            byte inputCode = alphabet.GetCode(input);
            byte keyCode = alphabet.GetCode(key);
            int decryptedCode = alphabet.SubtractCode(inputCode, keyCode);
            return alphabet.GetSymbolByCode(decryptedCode);
        }
    }
}
