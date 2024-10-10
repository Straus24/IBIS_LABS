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
            int previousCipher = 0; // Вектор инициализации для первого символа

            for (int i = 0; i < BlockSize; i++)
            {
                // Применение ключа и холостого сдвига j
                cipherText[i] = (plainText[i] + key[i] + j + previousCipher) % Moduls;
                previousCipher = cipherText[i];
            }

            return cipherText;
        }

        //Метод расшифрования с учетом ключа и холостого сдвига j
        public static int[] Decrypt(int[] cipherText, int j)
        {
            int[] key = { 3, 1, 7, 4 };
            if (cipherText.Length != BlockSize)
            {
                throw new ArgumentException($"Cipher text must be {BlockSize} integers long");
            }

            int[] plainText = new int[BlockSize];
            int previousCipher = 0;

            for (int i = 0; i < BlockSize; i++)
            {
                // Обратный сдвиг с учетом ключа и холостого сдвига j
                plainText[i] = (cipherText[i] - key[i] - j - previousCipher + Moduls) % Moduls;
                previousCipher = cipherText[i];
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

        public static string OneWayFunction(int[] blockIn, string constIn, int nIn)
        {
            int[] data = blockIn;
            int c = constIn.Length;

            string C = "ТПУ" + constIn + constIn.Substring(0, 4);

            string key = C.Substring(3, 4);

            Console.WriteLine($"Начальные данные: {ConvertFromTelegraphCode(data)}");

            for (int i = 0; i < nIn; i++)
            {
                int q = (i * 4) % c + 3;
                int[] tmp = Encrypt(data, key, 0);

                Console.WriteLine($"Раунд {i + 1}, данные: {ConvertFromTelegraphCode(tmp)}");

                int s = (int)(BlockToLong(tmp) % 4);
                key = AddText(tmp, C.Substring(q - s, 4));

                data = tmp;
            }

            return BlockToString(data);
        }

        private static string AddText(int[] block, string str)
        {
            // Преобразуем блок в строковое представление
            char[] blockChars = block.Select(b => (char)(b % 32)).ToArray();
            string blockString = new string(blockChars);

            string updateKey = blockString + str;

            if (updateKey.Length > 4)
            {
                updateKey = updateKey.Substring(0, 4);
            }

            return updateKey;
        }

        private static string BlockToString(int[] block) 
        {
            // Преобразуем блок в строку
            return string.Join(" ", block.Select(b => b.ToString()));
        }

        // Проверка 1: Влияние позиции символа в блоке
        //public static void TestPositionInfluence(int[] plainText, int j)
        //{
        //    int[] key = { 3, 1, 7, 4 };
        //    Console.WriteLine("\nTesting position influence:");

        //    for (int i = 0; i < BlockSize; i++)
        //    {
        //        int[] modifiedText = (int[])plainText.Clone();
        //        modifiedText[i] = (modifiedText[i] + 1) % Moduls; // Изменяем один символ в блоке

        //        int[] modifiedEncryptedText = Encrypt(modifiedText, j);

        //        string originalWord = ConvertFromTelegraphCode(plainText);
        //        string modifiedWord = ConvertFromTelegraphCode(modifiedText);
        //        string encryptedWord = ConvertFromTelegraphCode(modifiedEncryptedText);

        //        Console.WriteLine($"Original text at position {i}: {string.Join(", ", plainText)} -> {originalWord}");
        //        Console.WriteLine($"Modified text at position {i}: {string.Join(", ", modifiedText)} -> {modifiedWord}");
        //        Console.WriteLine($"Encrypted result: {string.Join(", ", modifiedEncryptedText)} -> {encryptedWord}");
        //    }
        //}

        // Проверка 2: Влияние порядка применения ключей
        //public static void TestKeyOrder(int[] plainText, int j)
        //{
        //    Console.WriteLine("\nTesting key order:");

        //    // Шифрование с прямым ключом
        //    int[] encryptedText = Encrypt(plainText, j);
        //    string encryptedWord = ConvertFromTelegraphCode(encryptedText);

        //    Console.WriteLine("Encrypted text with original key order: " + string.Join(", ", encryptedText) + " -> " + encryptedWord);

        //    // Меняем порядок ключей
        //    Array.Reverse(key);
        //    int[] encryptedTextReversedKey = Encrypt(plainText, j);
        //    string encryptedWordReversed = ConvertFromTelegraphCode(encryptedTextReversedKey);

        //    Console.WriteLine("Encrypted text with reversed key order: " + string.Join(", ", encryptedTextReversedKey) + " -> " + encryptedWordReversed);

        //    // Восстаналиваем оригинальный ключ
        //    Array.Reverse(key);
        //}

        // Проверка 3: Эффект лавины
        //public static void TestAvalancheEffect(int[] plainText, int j)
        //{
        //    Console.WriteLine("\nTesting avalanche effect:");

        //    // Оригинальное шифрование
        //    int[] encryptedText = Encrypt(plainText, j);
        //    string encryptedWord = ConvertFromTelegraphCode(encryptedText);

        //    Console.WriteLine("Original encrypted text: " + string.Join(", ", encryptedText) + " -> " + encryptedWord);

        //    // Изменение одного символа в открытом тексте
        //    int[] modifiedText = (int[])plainText.Clone();
        //    modifiedText[0] = (modifiedText[0] + 1) % Moduls; // Изменение первого символа

        //    int[] modifiedEncryptedText = Encrypt(modifiedText, j);
        //    string modifiedEncryptedWord = ConvertFromTelegraphCode(modifiedEncryptedText);

        //    Console.WriteLine("Modified encrypted text: " + string.Join(", ", modifiedEncryptedText) + " -> " + modifiedEncryptedWord);

        //    // Сравнение результатов и подсчет измененных символов
        //    int changesCount = 0;
        //    Console.WriteLine("Comparing original and modified encrtypted text:");
        //    for (int i = 0; i < BlockSize; i++)
        //    {
        //        if (encryptedText[i] != modifiedEncryptedText[i])
        //        {
        //            Console.WriteLine($"Position {i} differs: {encryptedText[i]} -> {modifiedEncryptedText[i]}");
        //            changesCount++;
        //        }
        //    }

        //    // Проверяем, изменилось ли больше одного символа
        //    if (changesCount > 1)
        //    {
        //        Console.WriteLine($"\nAvalanche effect confirmed: {changesCount} characters were changed in the ciphertext.");
        //    }
        //    else
        //    {
        //        Console.WriteLine("\nNo avalanche effect: Only one character was changed in the ciphertext.");
        //    }
        //}
    }
}
