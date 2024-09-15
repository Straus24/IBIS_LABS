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
    }
}
