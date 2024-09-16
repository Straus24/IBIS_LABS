using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Blocks
    {
        private const int BlockSize = 4; // Размер блока - 4 символа
        private const int Moduls = 32; // Числовой диапазон [0, 31]

        // Основной ключ - массив целых чисел
        private static readonly int[] key = { 3, 1, 7, 4 }; // Пример ключа для шифрования
        private static TelegraphAlphabet alphabet = new TelegraphAlphabet(); // Телеграфный алфавит

        //Метод шифрования с учетом ключа и холостого сдвига j
        public static int[] Encrypt(int[] plainText, int j)
        {
            if (plainText.Length != BlockSize)
            {
                throw new ArgumentException($"Input text must be {BlockSize} integers long");
            }

            int[] cipherText = new int[BlockSize];

            for (int i = 0; i < BlockSize; i++)
            {
                // Применение ключа и холостого сдвига j
                cipherText[i] = (plainText[i] + key[i] + j) % Moduls;
            }

            return cipherText;
        }

        //Метод расшифрования с учетом ключа и холостого сдвига j
        public static int[] Decrypt(int[] cipherText, int j)
        {
            if (cipherText.Length != BlockSize)
            {
                throw new ArgumentException($"Cipher text must be {BlockSize} integers long");
            }

            int[] plainText = new int[BlockSize];

            for (int i = 0; i < BlockSize; i++)
            {
                // Обратный сдвиг с учетом ключа и холостого сдвига j
                plainText[i] = (cipherText[i] - key[i] - j) % Moduls;
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

        // Проверка 1: Влияние позиции символа в блоке
        public static void TestPositionInfluence(int[] plainText, int j)
        {
            Console.WriteLine("\nTesting position influence:");

            for (int i = 0; i < BlockSize; i++)
            {
                int[] modifiedText = (int[])plainText.Clone();
                modifiedText[i] = (modifiedText[i] + 1) % Moduls; // Изменяем один символ в блоке

                int[] modifiedEncryptedText = Encrypt(modifiedText, j);

                string originalWord = ConvertFromTelegraphCode(plainText);
                string modifiedWord = ConvertFromTelegraphCode(modifiedText);
                string encryptedWord = ConvertFromTelegraphCode(modifiedEncryptedText);

                Console.WriteLine($"Original text at position {i}: {string.Join(", ", plainText)} -> {originalWord}");
                Console.WriteLine($"Modified text at position {i}: {string.Join(", ", modifiedText)} -> {modifiedWord}");
                Console.WriteLine($"Encrypted result: {string.Join(", ", modifiedEncryptedText)} -> {encryptedWord}");
            }
        }

        // Проверка 2: Влияние порядка применения ключей
        public static void TestKeyOrder(int[] plainText, int j)
        {
            Console.WriteLine("\nTesting key order:");

            // Шифрование с прямым ключом
            int[] encryptedText = Encrypt(plainText, j);
            string encryptedWord = ConvertFromTelegraphCode(encryptedText);

            Console.WriteLine("Encrypted text with original key order: " + string.Join(", ", encryptedText) + " -> " + encryptedWord);

            // Меняем порядок ключей
            Array.Reverse(key);
            int[] encryptedTextReversedKey = Encrypt(plainText, j);
            string encryptedWordReversed = ConvertFromTelegraphCode(encryptedTextReversedKey);

            Console.WriteLine("Encrypted text with reversed key order: " + string.Join(", ", encryptedTextReversedKey) + " -> " + encryptedWordReversed);

            // Восстаналиваем оригинальный ключ
            Array.Reverse(key);
        }

        // Проверка 3: Эффект лавины
        public static void TestAvalancheEffect(int[] plainText, int j)
        {
            Console.WriteLine("\nTesting avalanche effect:");

            // Оригинальное шифрование
            int[] encryptedText = Encrypt(plainText, j);
            string encryptedWord = ConvertFromTelegraphCode(encryptedText);

            Console.WriteLine("Original encrypted text: " + string.Join(", ", encryptedText) + " -> " + encryptedWord);

            // Изменение одного символа в открытом тексте
            int[] modifiedText = (int[])plainText.Clone();
            modifiedText[0] = (modifiedText[0] + 1) % Moduls; // Изменение первого символа

            int[] modifiedEncryptedText = Encrypt(modifiedText, j);
            string modifiedEncryptedWord = ConvertFromTelegraphCode(modifiedEncryptedText);

            Console.WriteLine("Modified encrypted text: " + string.Join(", ", modifiedEncryptedText) + " -> " + modifiedEncryptedWord);

            // Сравнение результатов 
            Console.WriteLine("Comparing original and modified encrtypted text:");
            for (int i = 0; i < BlockSize; i++)
            {
                if (encryptedText[i] != modifiedEncryptedText[i])
                {
                    Console.WriteLine($"Position {i} differs: {encryptedText[i]} -> {modifiedEncryptedText[i]}");
                }
            }
        }
    }
}
