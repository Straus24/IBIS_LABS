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
                    "\n5. Работа модифицированного LCG-генератора\n6. Работа wrap_C_HC_LCG\n7. Работа XOR + Round_Keys\n8. Шифр перестановки Скитала");

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

                        case ConsoleKey.D7:

                            // Проверка работы XOR + Round_Keys
                            string inA = "АГАТ";
                            string inB = "ТАГА";
                            string inA1 = "КОЛЕНЬКА";
                            string inB1 = "МТВ_ТЛЕН";
                            string inA2 = "ТОРТ_ХОЧЕТ_ГОРКУ";
                            string inB2 = "МТВ_ВСЕ_ЕЩЕ_ТЛЕН";
                            Console.WriteLine("subblocks_xor: " + XOR.subblocks_xor(inA, inB));
                            Console.WriteLine("block_xor: " + XOR.block_xor(inA2, inB2));
                            Console.WriteLine("block_xor: " + XOR.block_xor(XOR.block_xor(inA2, inB2), inB2));
                            Console.WriteLine("block_xor: " + XOR.block_xor(XOR.block_xor(inA2, inB2), inA2));

                            string round_key = "ПОЛИМАТ_ТЕХНОБОГ";
                            Console.WriteLine("Round_Keys: " + Round_Keys.produce_round_keys(round_key, 5, oneWayFunction));
                            break;

                        case ConsoleKey.D8:
                            Console.WriteLine("Работа функции Скитала");
                            Console.WriteLine(Feistel.Frw_P_Scitala("ДЖИГУРДА"));
                            Console.WriteLine(Feistel.Frw_P_Scitala("ДЖИГУРДАЯ"));
                            Console.WriteLine(Feistel.Frw_P_Scitala("АЭРОСМИТ"));
                            Console.WriteLine(Feistel.Frw_P_Scitala("БАЭРОСМИТ"));

                            Console.WriteLine();

                            Console.WriteLine("Обратное преобразование");
                            Console.WriteLine(Feistel.Inv_P_Scitala("ДУРЖИДАГ"));
                            Console.WriteLine(Feistel.Inv_P_Scitala("ДРДЖИАЯГУ"));
                            Console.WriteLine(Feistel.Inv_P_Scitala("АСМЭРИТО"));
                            Console.WriteLine(Feistel.Inv_P_Scitala("БСМАЭИТРО"));

                            break;

                        case ConsoleKey.D9:
                            Console.WriteLine("Работа рутинной функции Фейстеля");
                            string in1 = "ГОР_СВЕТ";
                            string in2 = "ЕГОР_КОТ";
                            string Feistel_key = "ЗОЛОТУХА";
                            Console.WriteLine(Feistel.Frw_Routine_Feistel(in1, Feistel_key, 0));
                            Console.WriteLine(Feistel.Frw_Routine_Feistel(in2, Feistel_key, 0));

                            Console.WriteLine(Feistel.Inv_Routine_Feistel("СВЕТЛРЩН", Feistel_key, 0));
                            Console.WriteLine(Feistel.Inv_Routine_Feistel("_КОТДДАЗ", Feistel_key, 0));

                            Console.WriteLine(Feistel.Frw_Inner_Feistel(in1, Feistel_key, 2));
                            Console.WriteLine(Feistel.Frw_Inner_Feistel(in2, Feistel_key, 2));

                            Console.WriteLine(Feistel.Inv_Inner_Feistel("ЛРЩНУЭЭХ", Feistel_key, 2));
                            Console.WriteLine(Feistel.Inv_Inner_Feistel("ДДАЗЬТМЦ", Feistel_key, 2));

                            Console.WriteLine(Feistel.Frw_Inner_FeistelM(in1, Feistel_key, 2));
                            Console.WriteLine(Feistel.Frw_Inner_FeistelM(in2, Feistel_key, 2));

                            Console.WriteLine(Feistel.Inv_Inner_FeistelM("ЕЖБЩНЬЯТ", Feistel_key, 2));
                            Console.WriteLine(Feistel.Inv_Inner_FeistelM("ОЧДАЗЙАТ", Feistel_key, 2));


                            Console.WriteLine("Работа раундовой функции Фейстеля");
                            in1 = "КОРЫСТЬ_СЛОНА_ЭХ";
                            in2 = "НУЖНО_БОЛЬШЕ_ПЫЩ";
                            Feistel_key = "МТВ_ВСЕ_ЕЩЕ_ТЛЕН";

                            Console.WriteLine(Feistel.round_Feistel(in1, Feistel_key));
                            Console.WriteLine(Feistel.round_Feistel(in2, Feistel_key));


                            string tmp1c = Feistel.swap_blocks(Feistel.round_Feistel(in1, Feistel_key));
                            string tmp2c = Feistel.swap_blocks(Feistel.round_Feistel(in2, Feistel_key));

                            Console.WriteLine(tmp1c);
                            Console.WriteLine(tmp2c);

                            string ltmp1c = Feistel.round_Feistel(tmp1c, Feistel_key);
                            string ltmp2c = Feistel.round_Feistel(tmp2c, Feistel_key);

                            Console.WriteLine(ltmp1c);
                            Console.WriteLine(ltmp2c);

                            string lout1c = Feistel.swap_blocks(ltmp1c);
                            string lout2c = Feistel.swap_blocks(ltmp2c);

                            Console.WriteLine(lout1c);
                            Console.WriteLine(lout2c);

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
