using System;

namespace ConsoleApp1 
{
    class Program
    {
        static void Main()
        {
            CESAR cesar = new CESAR();

            string originalText = "ПРИВЕТ ЦЕЗАРЬ";
            string encryptionKey = "КЛЮЧ"; // Ключ для шифрования

            string encryptedText = cesar.Encrypt(originalText, encryptionKey);
            string decryptedText = cesar.Decrypt(encryptedText, encryptionKey);

            Console.WriteLine($"Original: {originalText}");
            Console.WriteLine($"Encrypted: {encryptedText}");
            Console.WriteLine($"Decrypted: {decryptedText}");
        }

        //Метод для разделения текста на блоки
        //public static string[] SplitIntoBlocks(string text)
        //{
        //    int numBlocks = (text.Length + BlockSize - 1) / BlockSize;
        //    string[] blocks = new string[numBlocks];

        //    for (int i = 0; i < numBlocks; i++)
        //    {
        //        int start = i * BlockSize;
        //        blocks[i] = text.Substring(start, Math.Min(BlockSize, text.Length - start));
        //    }
        //    return blocks;
        //}
    }
}
