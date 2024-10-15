using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Blocks
    {
        private const int BlockSize = 4; // Размер блока - 4 символа
        private const int Moduls = 32; // Числовой диапазон [0, 31]
        private const int BitSize = 20; // 20 бит для блока символов

        // Функция конвертации блока в 64-битное число
        public static long BlockToLong(int[] block)
        {
            if (block.Length != BlockSize)
            {
                throw new ArgumentException($"Длина блока должна быть {BlockSize}.");
            }

            long result = 0;
            for (int i = 0; i < BlockSize; i++)
            {
                result |= (long)block[i] << (5 * (BlockSize - 1 - i)); // Каждый символ занимает 5 бит
            }
            return result;
        }

        // Функция конвертации 64-битного числа обратно в блок символов
        public static int[] LongToBlock(long value)
        {
            int[] block = new int[BlockSize];
            for (int i = 0; i < BlockSize; i++)
            {
                block[i] = (int)((value >> (5 * (BlockSize - 1 - i))) & 0x1F); // Вытаскиваем по 5 бит для каждого символа
            }
            return block;
        }

        // Преобразование числа в массив двоичных значений (20 бит)
        public static int[] ToBinaryArray(long value)
        {
            int[] binaryArray = new int[BitSize];
            for (int i = 0; i < BitSize; i++)
            {
                binaryArray[BitSize - 1 - i] = (int)((value >> i) & 1); // Заполняем массив двоичных значений от старшего бита к младшему
            }
            return binaryArray;
        }

        // Преобразование массива двоичных значений обратно в число
        public static long FromBinaryArray(int[] binaryArray)
        {
            if (binaryArray.Length != BitSize)
            {
                throw new ArgumentException($"Длина массива должна быть {BitSize}.");
            }

            long result = 0;
            for (int i = 0; i < BitSize; i++)
            {
                result |= (long)binaryArray[BitSize - 1 - i] << i; // Читаем биты начиная с самого старшего
            }
            return result;
        }

        public static long[] SeedToNums(int[][] arrayIn)
        {
            long[] result = new long[arrayIn.Length];

            for (int i = 0; i < arrayIn.Length; i++)
            {
                result[i] = BlockToLong(arrayIn[i]);
            }

            return result;
        }

        // Основной ключ - массив целых чисел
       // private static readonly int[] key = { 3, 1, 7, 4 }; // Пример ключа для шифрования
        private static TelegraphAlphabet alphabet = new TelegraphAlphabet(); // Телеграфный алфавит

        //Метод шифрования с учетом ключа и холостого сдвига j
        public static int[] Encrypt(int[] plainText, string key, int j)
        {
            if (plainText.Length != BlockSize)
            {
                throw new ArgumentException($"Input text must be {BlockSize} integers long");
            }

            int[] cipherText = new int[BlockSize];
            int keyLength = key.Length;

            // Начальное значение элементарного ключа
            int elementaryKey = alphabet.GetCode('_');  // k(0) = '_'

            for (int i = 0; i < BlockSize; i++)
            {
                // Циклическое использование ключа
                int q = (i + j) % keyLength;
                int keyCharCode = alphabet.GetCode(key[q]);

                // Генерация элементарного ключа
                elementaryKey = (elementaryKey + keyCharCode) % Moduls;

                // Шифрование с использованием элементарного ключа
                cipherText[i] = (plainText[i] + elementaryKey) % Moduls;
            }

            return cipherText;
        }


        //Метод расшифрования с учетом ключа и холостого сдвига j
        public static int[] Decrypt(int[] cipherText, string key, int j)
        {
            if (cipherText.Length != BlockSize)
            {
                throw new ArgumentException($"Input text must be {BlockSize} integers long");
            }

            int[] plainText = new int[BlockSize];
            int keyLength = key.Length;

            // Начальное значение элементарного ключа
            int elementaryKey = alphabet.GetCode('_');  // k(0) = '_'

            for (int i = 0; i < BlockSize; i++)
            {
                // Циклическое использование ключа
                int q = (i + j) % keyLength;
                int keyCharCode = alphabet.GetCode(key[q]);

                // Генерация элементарного ключа
                elementaryKey = (elementaryKey + keyCharCode) % Moduls;

                // Расшифрование с использованием элементарного ключа
                plainText[i] = (cipherText[i] - elementaryKey + Moduls) % Moduls;
            }

            return plainText;
        }

        // Метод преобразования слова в числовой код и обратно
        public static int[] ConvertToTelegraphCode(string input)
        {
            int[] codes = new int[BlockSize];
            for (int i = 0; i < input.Length && i < BlockSize; i++)
            {
                codes[i] = alphabet.GetCode(input[i]);
            }

            return codes;
        }

        public static string ConvertFromTelegraphCode(int[] codes)
        {
            char[] chars = new char[BlockSize];
            for (int i = 0; i < codes.Length; i++)
            {
                chars[i] = alphabet.GetSymbolByCode(codes[i]);
            }
            return new string(chars);
        }

        public static int[][] ConvertKeyToTelegraphCode(string key)
        {
            int totalBlocks = (int)Math.Ceiling(key.Length / (double)BlockSize);
            int[][] keyBlocks = new int[totalBlocks][];

            for (int i = 0; i < totalBlocks; i++)
            {
                string keyBlock = key.Substring(i * BlockSize, Math.Min(BlockSize, key.Length -  i * BlockSize));
                keyBlocks[i] = ConvertToTelegraphCode(keyBlock);
            }

            return keyBlocks;
        }


        // Односторонняя функция
        public static int[] OneWayFunction(int[] blockIn, string constIn, int nIn)
        {
            if (constIn.Length < 4)
            {
                throw new ArgumentException("constIn must have at least 4 characters.");
            }

            // Исходное слово
            int[] data = new int[blockIn.Length];
            int[] tmp = new int[blockIn.Length];
            data = blockIn;

            int c = constIn.Length;

            // Создание константы C
            string C = "ТПУ" + constIn + constIn.Substring(0, 4);
            string key = C.Substring(3, 4); // Начальный ключ

            // Выполнение n раундов
            for (int i = 0; i < nIn; i++)
            {
                int q = ((i * 4) % c) + 3;

                // Шифруем данные с текущим ключом
                tmp = FrwCesarM(data, key, 0);

                // Получаем результат шифрования
                int s = (int)(BlockToLong(tmp) % 4);

                // Обновление ключа с использованием новой части ключа
                key = AddText(ConvertFromTelegraphCode(tmp), C.Substring(q - s, Math.Min(4, C.Length - (q - s))));
            }

            return tmp; // Возвращаем итоговые данные
        }

        public static string AddText(string str1, string str2)
        {
            if (str1.Length != str2.Length)
            {
                throw new ArgumentException("Strings must have the same length");
            }

            string result = "";
            for (int i = 0; i < str1.Length; i++)
            {
                // Корректное сложение кодов символов
                int code1 = alphabet.GetCode(str1[i]);
                int code2 = alphabet.GetCode(str2[i]);
                int sum = (code1 + code2) % 32;

                result += alphabet.GetSymbolByCode(sum);
            }
            return result;
        }


        // Объединение обычного преобразования и модицифированного (необходимо для односторонней функции)
        public static int[] FrwCesarM(int[] blockIn, string keyIn, int jIn)
        {
            int[] tmp = Encrypt(blockIn, keyIn, jIn);
            return ImprovedEncrypt(tmp, keyIn, jIn);
        }

        // Модифицированное преобразование S-блока

        public static int[] ImprovedEncrypt(int[] plainText, string key, int j)
        {
            if (plainText.Length != BlockSize)
            {
                throw new ArgumentException($"Input text must be {BlockSize} integers long");
            }

            // Удвоение ключа, если j больше длины ключа
            string t = key;
            while (j > t.Length - 4)
            {
                t += t;
            }

            // Получение подстроки ключа длиной 4 символа
            string keySubstring = t.Substring(j, 4);
            int[] k = ConvertToTelegraphCode(keySubstring); // Преобразуем подстроку в массив телеграфных кодов
            int[] b = plainText; // Входной текст (прошедший через Encrypt)

            // Вычисление q: сумма элементов ключа mod 4
            int q = (k[0] + k[1] + k[2] + k[3]) % 4;

            // Шифрование
            for (int i = 0; i < 3; i++)  
            {
                int jIndex = (q + i + 1) % 4;  // Индекс следующего символа
                int lIndex = (q + i) % 4;      // Индекс текущего символа

                // Применение суммы кодов с циклическим сдвигом
                b[jIndex] = (b[jIndex] + b[lIndex]) % 32; // Меняем символы на основе текущего и предыдущего
            }

            // Возвращаем зашифрованный текст
            //Console.WriteLine(ConvertFromTelegraphCode(b));
            return b;
        }

        // Проверка 1: Влияние позиции символа в блоке
        public static void TestPositionInfluence(int[] plainText, string key, int j)
        {
            Console.WriteLine("\nTesting position influence:");

            for (int i = 0; i < BlockSize; i++)
            {
                int[] modifiedText = (int[])plainText.Clone();
                modifiedText[i] = (modifiedText[i] + 1) % Moduls; // Изменяем один символ в блоке

                int[] modifiedEncryptedText = Encrypt(modifiedText, key, j);

                string originalWord = ConvertFromTelegraphCode(plainText);
                string modifiedWord = ConvertFromTelegraphCode(modifiedText);
                string encryptedWord = ConvertFromTelegraphCode(modifiedEncryptedText);

                Console.WriteLine($"Original text at position {i}: {string.Join(", ", plainText)} -> {originalWord}");
                Console.WriteLine($"Modified text at position {i}: {string.Join(", ", modifiedText)} -> {modifiedWord}");
                Console.WriteLine($"Encrypted result: {string.Join(", ", modifiedEncryptedText)} -> {encryptedWord}");
            }
        }

        // Проверка 2: Влияние порядка применения ключей
        public static void TestKeyOrder(int[] plainText, string key, int j)
        {
            Console.WriteLine("\nTesting key order:");

            // Шифрование с прямым ключом
            int[] encryptedText = Encrypt(plainText, key, j);
            string encryptedWord = ConvertFromTelegraphCode(encryptedText);

            Console.WriteLine("Encrypted text with original key order: " + string.Join(", ", encryptedText) + " -> " + encryptedWord);

            // Меняем порядок ключа
            char[] reversedKeyArray = key.ToCharArray();
            Array.Reverse(reversedKeyArray);
            string reversedKey = new string(reversedKeyArray);

            int[] encryptedTextReversedKey = Encrypt(plainText, reversedKey, j);
            string encryptedWordReversed = ConvertFromTelegraphCode(encryptedTextReversedKey);

            Console.WriteLine("Encrypted text with reversed key order: " + string.Join(", ", encryptedTextReversedKey) + " -> " + encryptedWordReversed);
        }

        // Проверка 3: Эффект лавины
        public static void TestAvalancheEffect(int[] plainText, string key, int j)
        {
            Console.WriteLine("\nTesting avalanche effect:");

            // Оригинальное шифрование
            int[] encryptedText = Encrypt(plainText, key, j);
            string encryptedWord = ConvertFromTelegraphCode(encryptedText);

            Console.WriteLine("Original encrypted text: " + string.Join(", ", encryptedText) + " -> " + encryptedWord);

            // Изменение одного символа в открытом тексте
            int[] modifiedText = (int[])plainText.Clone();
            modifiedText[0] = (modifiedText[0] + 1) % Moduls; // Изменение первого символа

            int[] modifiedEncryptedText = Encrypt(modifiedText, key, j);
            string modifiedEncryptedWord = ConvertFromTelegraphCode(modifiedEncryptedText);

            Console.WriteLine("Modified encrypted text: " + string.Join(", ", modifiedEncryptedText) + " -> " + modifiedEncryptedWord);

            // Сравнение результатов и подсчет измененных символов
            int changesCount = 0;
            Console.WriteLine("Comparing original and modified encrypted text:");
            for (int i = 0; i < BlockSize; i++)
            {
                if (encryptedText[i] != modifiedEncryptedText[i])
                {
                    Console.WriteLine($"Position {i} differs: {encryptedText[i]} -> {modifiedEncryptedText[i]}");
                    changesCount++;
                }
            }

            // Проверяем, изменилось ли больше одного символа
            if (changesCount > 1)
            {
                Console.WriteLine($"\nAvalanche effect confirmed: {changesCount} characters were changed in the ciphertext.");
            }
            else
            {
                Console.WriteLine("\nNo avalanche effect: Only one character was changed in the ciphertext.");
            }
        }
    }
}
