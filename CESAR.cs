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

        public string Encrypt(string text, string key)
        {
            StringBuilder encryptedText = new StringBuilder();
            int keyIndex = 0;
            foreach (char c in text)
            {
                if (alphabet.GetCode(c) != 0) // Проверяем, есть ли код
                {
                    char keyChar = key[keyIndex % key.Length]; // Повторяем ключ
                    byte textCode = alphabet.GetCode(c);
                    byte keyCode = alphabet.GetCode(keyChar);
                    int encryptedCode = alphabet.SumCode(textCode, keyCode);
                    encryptedText.Append(alphabet.GetSymbolByCode(encryptedCode));

                    keyIndex++;
                }
                else
                {
                    encryptedText.Append(c); // Неизменяем символы, не входящие в алфавит
                }
            }
            return encryptedText.ToString();
        }

        public string Decrypt(string text, string key)
        {
            StringBuilder decryptedText = new StringBuilder();
            int keyIndex = 0;
            foreach (char c in text)
            {
                if (alphabet.GetCode(c) != 0) // Проверяем, есть ли код
                {
                    char keyChar = key[keyIndex % key.Length]; // Повторяем ключ
                    byte textCode = alphabet.GetCode(c);
                    byte keyCode = alphabet.GetCode(keyChar);
                    int decryptedCode = alphabet.SubtractCode(textCode, keyCode);
                    decryptedText.Append(alphabet.GetSymbolByCode(decryptedCode));

                    keyIndex++;
                }
                else
                {
                    decryptedText.Append(c); // Неизменяем символы, не входящие в алфавит
                }
            }
            return decryptedText.ToString();
        }
    }
}
