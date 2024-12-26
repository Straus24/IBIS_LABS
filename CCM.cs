using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Cryptography.X509Certificates;
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
            string[] metadata = { type, sender, receiver, session, lengthField };
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

        public static string enc_CTR(string MSG_IN, string IV_IN, string KEY_IN, Func<int[], string, int, int[]> sFun ,int r_in)
        {
            int m = MSG_IN.Length/16;
            string IV_starter = IV_IN.Substring(0, 12);
            int ctr = 0;
            string result = "";
            for (int i = 0; i < m; i++)
            {
                string IV_ender = Blocks.ConvertFromTelegraphCode(Blocks.LongToBlock(ctr));
                string IV = IV_starter + IV_ender;
                string keystream = Feistel.Frw_Feistel(IV, KEY_IN, sFun ,r_in);
                string inp = MSG_IN.Substring(i * 16, 16);
                result = result + TextXor(inp, keystream);
                ctr = ctr + 1;
            }
            return result;
        }

        public static string mac_CBC(string MSG_IN, string IV_IN, string KEY_IN, Func<int[], string, int, int[]> sFun, int r_in)
        {
            int m = MSG_IN.Length / 16;
            int ctr = 0;
            string result = "";
            string feedback = IV_IN;
            for (int i = 0; i < m; i++)
            {
                string inp = MSG_IN.Substring(i * 16, 16);
                string temp = TextXor(feedback, inp);
                feedback = Feistel.Frw_Feistel(temp, KEY_IN, sFun ,r_in);
                result = result + feedback;
            }
            return feedback; // Возможно ошибка и нужно возвращать result
        }

        public static string combine(string[] STRSET_IN)
        {
            string result = "";
            for (int i = 0; i < STRSET_IN.Length; i++)
            {
                result = result + STRSET_IN[i];
            }
            return result;
        }

        public static List<object> CCM_frw(string[] AD, string IV_IN, string INPUTS_ARRAY, string KEY_IN,  int onlymac, Func<int[], string, int, int[]> sFun, int r_in)
        {
            string data = combine(AD);
            int M = INPUTS_ARRAY.Length;
            string mac = mac_CBC(data + INPUTS_ARRAY, IV_IN, KEY_IN, sFun, r_in);
            string MSG;
            string MAC;
            if (onlymac == 0)
            {
                string msg = enc_CTR(INPUTS_ARRAY+mac, IV_IN, KEY_IN, sFun, r_in);
                MSG = msg.Substring(0, M);
                MAC = msg.Substring(M, 16);
            }
            else
            {
                MSG = INPUTS_ARRAY.ToString();
                MAC = mac;
            }
            List<object> result = new List<object>();
            result.Add(AD);
            result.Add(IV_IN);
            result.Add(MSG);
            result.Add(MAC);
            return result;
        }

        public static List<object> CCM_inv(string[] AD, string IV_IN, string INPUTS_ARRAY, string MAC_IN ,string KEY_IN, int onlymac, Func<int[], string, int, int[]> sFun, int r_in)
        {
            string data = combine(AD);
            int M = INPUTS_ARRAY.Length;
            string MSG;
            string MAC;
            if (onlymac == 0)
            {
                string msg = enc_CTR(INPUTS_ARRAY + MAC_IN, IV_IN, KEY_IN, sFun, r_in);
                 MSG = msg.Substring(0, M);
                 MAC = msg.Substring (M, 16);
            }
            else
            {
                 MSG = INPUTS_ARRAY;
                 MAC = MAC_IN;
            }
            string mac = mac_CBC(data + MSG, IV_IN, KEY_IN, sFun, r_in);
            MAC = TextXor(MAC, mac);
            List<object> result = new List<object>();
            result.Add(AD);
            result.Add(IV_IN);
            result.Add(MSG);
            result.Add(MAC);
            return result;
        }

        public static int[][] CCM_SEND(string[] ASS_DATA, string[] MSG_ARRAY, string KEY_IN, string nonce, Func<int[], string, int, int[]> sFun)
        {
            string t1 = ASS_DATA[2] + ASS_DATA[1];
            string t2 = ASS_DATA[0] + ASS_DATA[3] + "____";
            string t3 = Blocks.add_txt(Blocks.add_txt(t1, t2), nonce);
            string IV0 = t3.Substring(0, 8) + t3.Substring(12, 4) + t3.Substring(12, 4);
            int msg_counter = -1;
            string keyset = Round_Keys.produce_round_keys(KEY_IN, 8, sFun);
            int[][] result = new int[MSG_ARRAY.Length][];
            List<object> tmp_packet = new List<object>();
            List<object> sec_packet = new List<object>();
            for (int i = 0; i < MSG_ARRAY.Length; i++)
            {
                string msg_sec = ASS_DATA[0];
                msg_counter = msg_counter + 1;
                string IV1 = "________" + Blocks.ConvertFromTelegraphCode(Blocks.LongToBlock(msg_counter)) + "____";
                string IV = TextXor(IV0, IV1);
                tmp_packet = PreparePacket([msg_sec, ASS_DATA[1], ASS_DATA[2], ASS_DATA[3]], IV, MSG_ARRAY[i]);
                if (msg_sec == "В_")
                {
                    result[i] = Transmit(tmp_packet);
                }
                if (msg_sec == "ВА")
                {
                   sec_packet = CCM_frw([msg_sec, ASS_DATA[1], ASS_DATA[2], ASS_DATA[3]], IV, MSG_ARRAY[i], keyset, 1, sFun, 6);
                   result[i] = Transmit(sec_packet);
                }
                if (msg_sec=="ВБ")
                {
                    sec_packet = CCM_frw([msg_sec, ASS_DATA[1], ASS_DATA[2], ASS_DATA[3]], IV, MSG_ARRAY[i], keyset, 0, sFun, 6);
                    result[i] = Transmit(sec_packet);
                }

            }
            return result;
        }
        public static List<object> CCM_RECEIVE(string[] ASS_DATA, int[][] MSG_ARRAY, string KEY_IN, string nonce, Func<int[], string, int, int[]> sFun)
        {
            List<object> result = new List<object>();
            long last = -1;
            for (int i = 0; i < MSG_ARRAY.Length; i++)
            {
                List<object> rec_packet = new List<object>();
                List<object> tmp_packet = Recieve(MSG_ARRAY[i]);
                string[] rdata = (string[])tmp_packet[0];
                string x1 = tmp_packet[1].ToString().Substring(12,4);
                string x2 = tmp_packet[1].ToString().Substring(8, 4);
                long current = Blocks.BlockToLong(Blocks.ConvertToTelegraphCode(XOR.block_xor(x1, x2)));
                if (current > last)
                {
                    if (rdata[0] == "ВБ")
                    {
                        rec_packet = CCM_inv((string[])tmp_packet[0], (string)tmp_packet[1], (string)tmp_packet[2], (string)tmp_packet[3] ,KEY_IN, 0, sFun, 6);
                        rec_packet[2] = Padding.UnpadMessage((string)rec_packet[2]);
                        if (rec_packet[3] == "________________")
                        {
                            last = current;
                            rec_packet[3] = "OK";
                        }
                    }
                    else if (rdata[0] == "ВА" && ASS_DATA[0] != "ВБ")
                    {
                        rec_packet = CCM_inv((string[])tmp_packet[0], (string)tmp_packet[1], (string)tmp_packet[2], (string)tmp_packet[3], KEY_IN, 1, sFun, 6);
                        rec_packet[2] = Padding.UnpadMessage((string)rec_packet[2]);
                        if (rec_packet[3] == "________________")
                        {
                            last = current;
                            rec_packet[3] = "OK";
                        }
                    }
                    else if (rdata[0] == "В_" && ASS_DATA[0] == "В_")
                    {
                        rec_packet = tmp_packet;
                        rec_packet[2] = Padding.UnpadMessage((string)rec_packet[2]);
                        if (rec_packet[3] == "")
                        {
                            last = current;
                            rec_packet[3] = "N/A";
                        }
                    }
                    else
                    {
                        rec_packet = tmp_packet;
                    }
                    result.Add(rec_packet);
                }

            }
            return result;
        }
    }
}
