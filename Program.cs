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

            string plainText = "КОЛА";
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

                        case ConsoleKey.D3:
                            int[] plainTextCodes2 = Blocks.ConvertToTelegraphCode(plainText);
                            long value = Blocks.BlockToLong(plainTextCodes2);
                            int[] BitArray = Blocks.ToBinaryArray(value);
                            Console.WriteLine($"Блок = {string.Join("", plainTextCodes2)}");
                            Console.WriteLine($"Блок в 64-битное число = {value}");
                            Console.WriteLine($"Целое число в битах = {string.Join("", BitArray)}");
                            Console.WriteLine($"Биты в 64-битное число = {Blocks.FromBinaryArray(BitArray)}");
                            Console.WriteLine($"64-битное в блок = {string.Join("", Blocks.LongToBlock(Blocks.FromBinaryArray(BitArray)))}");

                            // Работа односторонней функции
                            string constant = "КОНСТАНТА";
                            int rounds = 5;
                            long oneWayResult = Blocks.OneWayFunction(value, constant, rounds);
                            Console.WriteLine($"Результат односторонней функции после {rounds} раундов: {oneWayResult}");

                            //// Работа LCG
                            //long a = 5, c = 3, m = 1048576; // Коэффициенты для LCG
                            //LCG generator = new LCG(a, c, m);
                            //long lcgResult = generator.Next(value);
                            //Console.WriteLine($"Результат работы LCG: {lcgResult}");

                            // Работа EnhancedLCG
                            long a = 723482, c = 8677, m = 983609;
                            long initialState = value;

                            EnhancedLCG enhGen = new EnhancedLCG(a, c, m, initialState);

                            Console.WriteLine("Тест работы модифицированного генератора: ");
                            for (int i = 0; i < 10; i++)
                            {
                                long output = enhGen.Next(initialState);
                                int[] block = Blocks.LongToBlock(output);
                                string text = Blocks.ConvertFromTelegraphCode(block);

                                Console.WriteLine($"Итерация {i + 1}: {output} -> {text}");
                            }

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
