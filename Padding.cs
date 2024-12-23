using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Padding
    {
        public static (bool isValid, int numBlocks, int padLength) CheckPadding(int[] binMsgIn)
        {
            int[] bins = binMsgIn;
            int M = bins.Length;

            // Расчет количества блоков и остатка
            int blocks = M / 80;
            int remainder = M % 80;

            // Если остатка нет, начинаем проверку на подложку
            if (remainder == 0)
            {
                // Извлекаем последние 20 бит для проверки структуры и подложки
                int[] tb = bins[^20..];

                // Проверяем концевик "001"
                if (tb[^3..].SequenceEqual(new int[] { 0, 0, 1 }))
                {
                    // Извлекаем 9 бит числа блоков и 7 бит размера подложки
                    int[] NB = tb[7..17];
                    int[] PL = tb[0..7];

                    // Преобразуем биты в числа
                    int padLength = ConvertBitsToInt(PL);
                    int numBlocks = ConvertBitsToInt(NB);

                    Console.WriteLine($"numBlocks: {numBlocks}, padLength: {padLength}, blocks: {blocks}");

                    // Проверяем ограничения на подложку
                    if (numBlocks == blocks && padLength >= 23 && padLength < 103)
                    {
                        // Начало подложки (включая 20 бит структуры)
                        int padStartIndex = M - padLength;
                        int padEndIndex = M - 20;

                        // Извлекаем саму подложку
                        if (padStartIndex >= 0 && padEndIndex > padStartIndex)
                        {
                            int[] paddingSection = bins[padStartIndex..padEndIndex];

                            // Проверяем стартер "100"
                            if (paddingSection.Length > 2 &&
                                paddingSection[0] == 1 && paddingSection[1] == 0 && paddingSection[2] == 0)
                            {
                                // Проверяем, чтобы остальная часть подложки (кроме стартера) состояла из нулей
                                for (int i = 3; i < paddingSection.Length; i++)
                                {
                                    if (paddingSection[i] != 0)
                                    {
                                        return (false, 0, 0); // Найден ненулевой бит - подложка некорректна
                                    }
                                }
                                return (true, numBlocks, padLength);
                            }
                        }
                    }
                }
            }

            // Если проверки не прошли, возвращаем некорректную подложку
            return (false, 0, 0);
        }


        private static int ConvertBitsToInt(int[] bits)
        {
            int result = 0;
            foreach (int bit in bits)
            {
                result = (result << 1) | bit;
            }
            return result;
        }

        public static int[] ProducePadding(int remainder, int blocksIn)
        {
            int r, b;
            if (remainder == 0) // Сообщение уже кратно 80 битам
            {
                b = blocksIn + 1; // Добавляем один блок
                r = 80;
            }
            else if (remainder <= 57) // Остаток <= 57 бит
            {
                r = 80 - remainder;
                b = blocksIn + 1;
            }
            else // Остаток > 57 бит
            {
                r = 160 - remainder;
                b = blocksIn + 2;
            }

            int[] pad = new int[r];
            pad[0] = 1; // Стартовый бит "100"
            pad[1] = 0;
            pad[2] = 0;

            // Заполняем нулями пространство подложки до позиции длины подложки
            for (int i = 3; i < r - 20; i++)
            {
                pad[i] = 0;
            }

            // Добавляем размер подложки (ровно 7 бит)
            int rt = r;
            for (int i = 6; i >= 0; i--) // 7 бит: от 6 до 0
            {
                pad[r - 20 + i] = rt % 2; // Заполняем справа налево
                rt /= 2;
            }

            // Добавляем число блоков (ровно 10 бит)
            int bTemp = b;
            for (int i = 9; i >= 0; i--) // 10 бит: от 9 до 0
            {
                pad[r - 13 + i] = bTemp % 2;
                bTemp /= 2;
            }

            // Добавляем концевик "001"
            pad[r - 3] = 0;
            pad[r - 2] = 0;
            pad[r - 1] = 1;

            return pad;
        }

        public static string PadMessage(string msgIn)
        {
            TelegraphAlphabet telegraphAlphabet = new TelegraphAlphabet();

            // Преобразуем сообщение в бинарное представление
            List<int> bins = telegraphAlphabet.Masg2Bin(msgIn).ToList();
            int M = bins.Count;

            // Расчитываем количество блоков и остаток
            int blocks = M / 80;
            int remainder = M % 80;


            // Проверяем существование корректной подложки
            bool hasValidPadding = false;
            if (remainder == 0)
            {
                // Проверяем, только если остаток равен 0
                hasValidPadding = CheckPadding(bins.ToArray()).isValid;

            }

            // Если подложки нет или она некорректна, добавляем новую подложку
            if (!hasValidPadding)
            {
                int[] padding = ProducePadding(remainder, blocks);
                bins.AddRange(padding);
            }

            // Коневертируем бинарные данные обратно в сообщение
            return telegraphAlphabet.Bin2Msg(bins.ToArray());
        }

        public static string UnpadMessage(string msgIn)
        {
            TelegraphAlphabet telegraphAlphabet = new TelegraphAlphabet();

            // Преобразуем сообщение в бинарное представление
            int[] bins = telegraphAlphabet.Masg2Bin(msgIn);
            int M = bins.Length;

            // Проверяем подложку
            var checkResult = CheckPadding(bins);

            if (checkResult.isValid)
            {
                // Если подложка корректна, удаляем её
                int padLength = checkResult.padLength;
                int[] truncatedBins = bins.Take(M - padLength).ToArray();

                // Преобразуем укороченный бинарный массив обратно в сообщение
                return telegraphAlphabet.Bin2Msg(truncatedBins);
            }
            else
            {
                // Если подложка отсутствует или некорректна, возвращаем исходное сообщение
                return msgIn;
            }
        }
    }
}
