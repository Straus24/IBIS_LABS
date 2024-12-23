using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class CCM
    {        
        public static List<object> PreparePacket(string[] dataIn, string ivIn, string msgIn)
        {
            TelegraphAlphabet telegraphAlphabet = new TelegraphAlphabet();

            // Инициализация data
            string[] data = new string[5];
            Array.Copy(dataIn, data, 4); // Первые 4 элемента копируем

            // Добавляем подчеркивания к IV
            string iv = ivIn.PadRight(16, '_');

            // Пакуем сообщение
            string msg = Padding.PadMessage(msgIn);

            // Вычисляем длину сообщения в битах
            int L = telegraphAlphabet.Masg2Bin(msg).Length;

            // Формируем строку "а"
            string a = "";
            for (int i = 0; i < 5; i++)
            {
                int modeValue = L % 32;
                a = telegraphAlphabet.GetSymbolByCode(modeValue) + a;
                L /= 32;
            }

            data[4] = a; // Добавляем строку а в последний элемент data

            // MAC остается пустым
            string mac = "";

            return new List<object> { data, iv, msg, mac };
        }

        public static int[] Transmit(List<object> packetIn)
        {
            TelegraphAlphabet telegraphAlphabet = new TelegraphAlphabet();

            // Распаковка пакета
            string[] data = (string[])packetIn[0];
            string iv = (string)packetIn[1];
            string msg = (string)packetIn[2];
            string mac = (string)packetIn[3];

            // Формирование данных из частей data
            string outData = string.Concat(data);

            // Конкатенация всех частей пакета
            string fullPacket = outData + iv + msg + mac;

            // Преобразование в бинарный поток
            return telegraphAlphabet.Masg2Bin(fullPacket);
        }

        public static List<object> Recieve(int[] streamIn)
        {
            TelegraphAlphabet telegraphAlphabet = new TelegraphAlphabet();

            // Преобразование бинарного потока в сообщение
            string p = telegraphAlphabet.Bin2Msg(streamIn);

            // Длина сообщения
            int M = p.Length;

            // Извлечение полей пакета
            string type = p.Substring(0, 2);
            string sender = p.Substring(2, 8);
            string receiver = p.Substring(10, 8);
            string session = p.Substring(18, 9);
            string lengthField = p.Substring(27, 5);
            string iv = p.Substring(32, 16);

            // Восстановление длины сообщения
            int L = 0;
            for (int i = 0; i < 5; i++)
            {
                char t = lengthField[i];
                int l = telegraphAlphabet.GetCode(t);
                L = 32 * L + l;
            }

            L /= 5;

            // Извлечение сообщения
            string message = p.Substring(48, L);

            // Извлечение MAC
            string mac = p.Substring(48 + L, M - (48 + L));

            // Формирование результата
            var metadata = new List<string> { type, sender, receiver, session, lengthField };
            return new List<object> { metadata, iv, message, mac };
        }

        public static string TextXor(string A_IN, string B_IN)
        {
            // Результирующая строка
            string result = "";

            // Размер блока в символах
            int blockSizeInChars = 4;

            // Обработка каждого блока
            for (int i = 0; i < A_IN.Length / blockSizeInChars; i++)
            {
                // Извлечение подстрок блоков
                string aBlock = A_IN.Substring(i * blockSizeInChars, blockSizeInChars);
                string bBlock = B_IN.Substring(i * blockSizeInChars, blockSizeInChars);

                // Преобразование блоков в числа
                long aNumber = Blocks.BlockToLong(Blocks.ConvertToTelegraphCode(aBlock));
                long bNumber = Blocks.BlockToLong(Blocks.ConvertToTelegraphCode(bBlock));

                // Преобразование чисел в массивы бит
                int[] aBits = Blocks.ToBinaryArray(aNumber);
                int[] bBits = Blocks.ToBinaryArray(bNumber);

                // Выполнение побитового XOR
                int[] cBits = new int[aBits.Length];
                for (int j = 0; j < aBits.Length; j++)
                {
                    cBits[j] = aBits[j] ^ bBits[j]; // XOR между соответствующими битами
                }

                // Преобразование массива бит обратно в число
                long cNumber = Blocks.FromBinaryArray(cBits);

                // Преобразлвание числа обратно в блок символов
                int[] cBlocks = Blocks.LongToBlock(cNumber);
                result += Blocks.ConvertFromTelegraphCode(cBlocks);
            }

            return result;
        }
    }
}
