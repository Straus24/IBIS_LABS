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

            Func<int[], string, int, int[]> oneWayFunction = Blocks.OneWayFunction;

            // Параметры для генераторов
            long[][] set = {
                new long[] { 723482, 8677, 983609 },
                new long[] { 252564, 9109, 961193 },
                new long[] { 357630, 8971, 948209 }
            };

            while (true)
            {
                Console.WriteLine("\nВыберите номер задания:");
                Console.WriteLine("1. Шифрование текста\n2. Шифрование текста методом S-блоков\n3. Работа односторонней функции\n4. Работа LCG-генератора" +
                    "\n5. Работа модифицированного LCG-генератора\n6. Работа wrap_C_HC_LCG");

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
                            //Console.WriteLine($"Блок в 64-битное число = {value}");
                            //Console.WriteLine($"Целое число в битах = {string.Join("", BitArray)}");
                            //Console.WriteLine($"Биты в 64-битное число = {Blocks.FromBinaryArray(BitArray)}");
                            //Console.WriteLine($"64-битное в блок = {string.Join("", Blocks.LongToBlock(Blocks.FromBinaryArray(BitArray)))}");

                            // Работа односторонней функции
                            Console.Write("\nРабота односторонней функции: \n");
                            string constant = "ББББ";
                            int rounds = 5;
                            int[] resOneWay = Blocks.OneWayFunction(plainTextCodes2, constant, rounds);
                            Console.WriteLine($"{string.Join("", Blocks.ConvertFromTelegraphCode(resOneWay))}");

                            break;
                        case ConsoleKey.D4:
                            // Работа базового LCG
                            string TestString = "ЛУЛУ";
                            int a = 723482;
                            int c = 8677;
                            int m = 983609;
                            long Seed = Blocks.BlockToLong(Blocks.ConvertToTelegraphCode(TestString));
                            LCG BaseLCG = new LCG(a, c, m, Seed);
                            //Console.WriteLine("\nРабота базового LCG");
                            Console.WriteLine($"Входное слово: {TestString}");
                            Console.Write($"Результат: {Blocks.ConvertFromTelegraphCode(Blocks.LongToBlock(BaseLCG.Next()))}\n");

                            break;
                        case ConsoleKey.D5:
                            // Работа EnhancedLCG
                            //Console.WriteLine("\nРабота модифицированного LCG");
                            string inputBlock = "КОЛА";
                            

                            int[] codes = Blocks.ConvertToTelegraphCode(inputBlock);
                            long[] longArray = codes.Select(x => (long)x).ToArray();
                            long[] seed2 = EnhancedLCG.make_seed(longArray, oneWayFunction);

                            Console.WriteLine($"Входное слово: {inputBlock}");
                            // Вывод сгенерированных сидов
                            Console.WriteLine("\nSeed Blocks: ");
                            foreach (var seed in seed2)
                            {
                                Console.WriteLine($"Seed: {string.Join("", Blocks.ConvertFromTelegraphCode(Blocks.LongToBlock(seed)))}");  
                            }
                            Console.WriteLine();



                            // Основная часть работы LCG
                            EnhancedLCG enhLCG = new EnhancedLCG(seed2, set);

                            for (int i = 0; i < 10; i++)
                            {
                                var result = enhLCG.Next();

                                int[] blockOutput = Blocks.LongToBlock(result.Item1);  // Преобразуем вывод обратно в блок
                                Console.WriteLine($"\nIteration {i + 1}:");
                                Console.WriteLine($"Output Block: {string.Join(", ", Blocks.ConvertFromTelegraphCode(blockOutput))}");  // Выводим блок
                                Console.WriteLine($"Internal States: {string.Join(", ", result.Item2)}");  // Выводим внутренние состояния
                            }

                            break;

                        case ConsoleKey.D6:

                            // Работа wrap_C_HC_LCG
                            string seed_for_wrap = "ААААББББВВВВГГГГ";
                            var wrap_result = wrap_C_HC_LCG.Next(true, null, seed_for_wrap, set, oneWayFunction);

                            string wrap_result_out = wrap_result.Item1;

                            for (int i = 0; i < 8; i++)
                            {
                                wrap_result = wrap_C_HC_LCG.Next(false, wrap_result.Item2, null, set, null);
                                 wrap_result_out = wrap_result_out + wrap_result.Item1;
                            }
                            Console.WriteLine("Результат финальной обёртки: " + wrap_result_out);


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
