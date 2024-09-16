using System;
using System.Reflection;

namespace ConsoleApp1 
{
    class Program
    {
        static void Main()
        {
            CESAR cesar = new CESAR();

            string originalText = "ПРИВЕТ ЦЕЗАРЬ";
            string encryptionKey = "КЛЮЧ"; // Ключ для шифрования

            Blocks blocks = new Blocks();

            string plainText = "ВЕГА";
            int j = 5; // Число холостых сдвигов j

            while (true)
            {
                Console.WriteLine("\nВыберите номер задания:");
                Console.WriteLine("1. Шифрование текста\n2. Шифрование текста методом S-блоков");

                ConsoleKeyInfo choice = Console.ReadKey();
                Console.WriteLine();

                try
                {
                    switch (choice.Key)
                    {
                        case ConsoleKey.D1:
                            string encryptedText = cesar.Encrypt(originalText, encryptionKey);
                            string decryptedText = cesar.Decrypt(encryptedText, encryptionKey);

                            Console.WriteLine($"Original: {originalText}");
                            Console.WriteLine($"Encrypted: {encryptedText}");
                            Console.WriteLine($"Decrypted: {decryptedText}");
                            break;
                        case ConsoleKey.D2:
                            Console.WriteLine("Original text (P): " + string.Join(", ", plainText));

                            // Преобразование в телеграфные коды
                            int[] plainTextCodes = Blocks.ConvertToTelegraphCode(plainText);
                            Console.WriteLine("Telegraph codes: " + string.Join(", ", plainTextCodes));

                            // Шифрование 
                            int[] BlockencryptedText = Blocks.Encrypt(plainTextCodes, j);
                            Console.WriteLine("Encrypted text (C): " + string.Join(", ", BlockencryptedText));

                            // Обратное преобразование шифротекста в символы
                            string encryptedWord = Blocks.ConvertFromTelegraphCode(BlockencryptedText);
                            Console.WriteLine("Encrypted word (C): " + encryptedWord);

                            // Расшифрование
                            int[] BlockdecryptedText = Blocks.Decrypt(BlockencryptedText, j);
                            Console.WriteLine("Decrypted text (P): " + string.Join(", ", BlockdecryptedText));

                            // Обратное преобразование расшифрованного текста в символы
                            string decryptedWord = Blocks.ConvertFromTelegraphCode(BlockdecryptedText);
                            Console.WriteLine("Decrypted word (P): " + decryptedWord);

                            Console.WriteLine("Check S-blocks");
                            Blocks.TestPositionInfluence(plainTextCodes, j);
                            Blocks.TestKeyOrder(plainTextCodes, j);
                            Blocks.TestAvalancheEffect(plainTextCodes, j);

                            break;
                        case ConsoleKey.Escape:
                            break;
                        default:
                            Console.WriteLine("Неверный выбор. Попробуйте еще раз.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
            }
            

            //Проверки
            //TestPositionInfluence(plainText, j);
            //TestKeyOrder(plainText, j);
            //TestAvalancheEffect(plainText, j);
        }

        
    }
}
