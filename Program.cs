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
            TelegraphAlphabet telegraphAlphabet = new TelegraphAlphabet();

            string plainText = "ВАСЯ";
            string key = "РОЗА"; // Пример ключа для шифрования
            int j = 0; // Число холостых сдвигов j

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
                            int[] BlockencryptedText = Blocks.Encrypt(plainTextCodes, key, j);
                            Console.WriteLine("Encrypted text (C): " + string.Join(", ", BlockencryptedText));

                            // Обратное преобразование шифротекста в символы
                            string encryptedWord = string.Concat(BlockencryptedText.Select(code => telegraphAlphabet.GetSymbolByCode(code)));
                            Console.WriteLine($"Encrypted word: {encryptedWord}");

                            //    // Расшифрование
                            int[] BlockdecryptedText = Blocks.Decrypt(BlockencryptedText, key, j);
                            Console.WriteLine("Decrypted text (P): " + string.Join(", ", BlockdecryptedText));

                            //    // Обратное преобразование расшифрованного текста в символы
                            string decryptedWord = Blocks.ConvertFromTelegraphCode(BlockdecryptedText);
                            Console.WriteLine("Decrypted word (P): " + decryptedWord);

                            // Модифицированный S-блок
                            Console.WriteLine("Результат: " + string.Join(", ", Blocks.FrwCesarM(plainTextCodes, key, j)));

                            Console.WriteLine("Check S-blocks");
                            Blocks.TestPositionInfluence(plainTextCodes, key, j);
                            Blocks.TestKeyOrder(plainTextCodes, key, j);
                            Blocks.TestAvalancheEffect(plainTextCodes, key, j);

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
                            Console.WriteLine("Работа односторонней функции: ");
                            string constant = "ББББ";
                            int rounds = 5;
                            Blocks.OneWayFunction(plainTextCodes2, constant, rounds);

                            // Работа базового LCG
                            string TestString = "ЛУЛУ";
                            int a = 723482;
                            int c = 8677;
                            int m = 983609;
                            long Seed = Blocks.BlockToLong(Blocks.ConvertToTelegraphCode(TestString));
                            LCG BaseLCG = new LCG(a, c, m, Seed);
                            Console.WriteLine("Работа базового LCG");
                            Console.WriteLine($"Входное слово: {TestString}");
                            Console.Write($"Результат: {Blocks.ConvertFromTelegraphCode(Blocks.LongToBlock(BaseLCG.Next()))}");

                            // Работа EnhancedLCG
                            string inputBlock = "КОЛА";
                            Func<int[], string, int, int[]> oneWayFunction = Blocks.OneWayFunction;

                            // Исправлено на long[][]
                            int[] codes = Blocks.ConvertToTelegraphCode(inputBlock);
                            long[] longArray = codes.Select(x => (long)x).ToArray();
                            long[][] seed2 = EnhancedLCG.make_seed(longArray, oneWayFunction);

                            Console.WriteLine("Seed Blocks: ");
                            foreach (var seed in seed2)
                            {
                                Console.WriteLine($"Seed: {string.Join("", Blocks.LongToBlock(seed[0]))}");  // Вывод блока, преобразованного из long
                            }
                            int[][] forSeedToNums = LongToInt32_mass.Convert(seed2);
                            long[] s2 = Blocks.SeedToNums(forSeedToNums);  // Преобразование seed2 в массив long
                            Console.WriteLine("\nInitial states: ");
                            Console.WriteLine(string.Join(", ", s2));  // Вывод начальных состояний

                            // Пример параметров для генераторов
                            long[][] set = {
                                new long[] { 723482, 8677, 983609 },
                                new long[] { 541223, 13571, 746517 },
                                new long[] { 324982, 24107, 999999 }
                            };

                            // Основная часть работы LCG
                            EnhancedLCG enhLCG = new EnhancedLCG(s2, set);
                            for (int i = 0; i < 10; i++)
                            {
                                var result = enhLCG.Next();

                                string blockOutput = Blocks.LongToBlock(result.Item1).ToString();  // Преобразуем вывод обратно в блок
                                Console.WriteLine($"\nIteration {i + 1}:");
                                Console.WriteLine($"Output Block: {blockOutput}");  // Выводим блок
                                Console.WriteLine($"Internal States: {string.Join(", ", result.Item2)}");  // Выводим внутренние состояния
                            }

                            //long a = 723482, c = 8677, m = 983609;
                            //long initialState = value;



                            //Console.WriteLine("Тест работы модифицированного генератора: ");
                            //for (int i = 0; i < 10; i++)
                            //{
                            //    long output = enhGen.Next(initialState);
                            //    int[] block = Blocks.LongToBlock(output);
                            //    string text = Blocks.ConvertFromTelegraphCode(block);

                            //    Console.WriteLine($"Итерация {i + 1}: {output} -> {text}");
                            //}

                            break;
                        case ConsoleKey.Escape:
                            return;
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
        }        
    }
}
