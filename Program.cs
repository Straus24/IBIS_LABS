using System;

namespace ConsoleApp1 
{
    class Program
    {
        static void Main()
        {
            CESAR cesar = new CESAR();

            string keyWord = "ШИФР"; // Пример ключевого слова
            char key = keyWord[0]; // Берем первый символ как ключ
            char originalChar = 'А';

            char encryptedChar = CESAR.Encrypt(originalChar, key);
            char decryptedChar = CESAR.Decrypt(encryptedChar, key);

            Console.WriteLine($"Original: {originalChar}");
            Console.WriteLine($"Encrypted: {encryptedChar}");
            Console.WriteLine($"Decrypted: {decryptedChar}");
        }

        //Метод для разделения текста на блоки
        public static string[] SplitIntoBlocks(string text)
        {
            int numBlocks = (text.Length + BlockSize - 1) / BlockSize;
            string[] blocks = new string[numBlocks];

            for (int i = 0; i < numBlocks; i++)
            {
                int start = i * BlockSize;
                blocks[i] = text.Substring(start, Math.Min(BlockSize, text.Length - start));
            }
            return blocks;
        }
    }
}
