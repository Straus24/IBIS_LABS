using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ConsoleApp1
{
    class TelegraphAlphabet
    {
        private Dictionary<char, byte> _alphabet;

        public TelegraphAlphabet()
        {
            _alphabet = new Dictionary<char, byte>
{
                { '_', 0b00000 },
                { 'А', 0b00001 },
                { 'Б', 0b00010 },
                { 'В', 0b00011 },
                { 'Г', 0b00100 },
                { 'Д', 0b00101 },
                { 'Е', 0b00110 },
                { 'Ж', 0b00111 },
                { 'З', 0b01000 },
                { 'И', 0b01001 },
                { 'Й', 0b01010 },
                { 'К', 0b01011 },
                { 'Л', 0b01100 },
                { 'М', 0b01101 },
                { 'Н', 0b01110 },
                { 'О', 0b01111 },
                { 'П', 0b10000 },
                { 'Р', 0b10001 },
                { 'С', 0b10010 },
                { 'Т', 0b10011 },
                { 'У', 0b10100 },
                { 'Ф', 0b10101 },
                { 'Х', 0b10110 },
                { 'Ц', 0b10111 },
                { 'Ч', 0b11000 },
                { 'Ш', 0b11001 },
                { 'Щ', 0b11010 },
                { 'Ы', 0b11011 },
                { 'Ь', 0b11100 },
                { 'Э', 0b11101 },
                { 'Ю', 0b11110 },
                { 'Я', 0b11111 }

};
        }

        public byte GetCode(char symbol)
        {
            if (_alphabet.TryGetValue(symbol, out byte code))
            {
                return code;
            }
            else
            {
                return 0;
            }
        }

        public int Sym2Bin(char sIn)
        {
            return sIn == '1' ? 1 : 0;
        }

        public bool IsSym(char sIn)
        {
            // Проверяем, содержится ли символ в алфавите
            return _alphabet.ContainsKey(sIn); 
        }

        public int[] Masg2Bin(string msgIn)
        {
            List<int> binaryResult = new List<int>();

            foreach (char ch in msgIn)
            {
                if (IsSym(ch)) // Если символ в алфавите 
                {
                    // Преобразуем код символа в двоичное представление
                    byte code = GetCode(ch); // Получаем числовой код символа
                    for (int i = 4; i >= 0; i--) // Каждый символ кодируется в 5 бит
                    {
                        binaryResult.Add((code >> i) & 1); // Берем i-й бит числа
                    }
                }
                else if (ch == '0' || ch == '1') // Если символ - бинарный
                {
                    binaryResult.Add(Sym2Bin(ch)); // Просто добавляем его как 0 или 1
                }
                else if (char.IsWhiteSpace(ch))
                {
                    continue;
                }
                else
                {
                    throw new ArgumentException($"Недопустимый символ: {ch}");
                }
            }

            return binaryResult.ToArray();
        }

        public string Bin2Msg(int[] binIn)
        {
            int binLength = binIn.Length;
            int fullGroups = binLength / 5; // Полные группы по 5 бит
            int remainingBits = binLength % 5; // Оставшиеся биты
            StringBuilder output = new StringBuilder();

            // Обработка полных групп по 5 бит
            for (int i = 0; i < fullGroups; i++)
            {
                int t = 0;
                for (int j = 0; j < 5; j++)
                {
                    t = 2 * t + binIn[i * 5 + j]; // Вычисляем значение символа
                }
                output.Append(GetSymbolByCode(t)); // Конвертируем в символ
            }

            // Обработка оставшихся бит (если есть)
            if (remainingBits > 0)
            {
                for (int k = 1; k <= remainingBits; k++)
                {
                    int index = fullGroups * 5 + (k - 1);
                    output.Append(binIn[index]); // Добавляем оставшиеся биты как числа
                }
            }

            return output.ToString();
        }


        public int SumCode(int code1, int code2)
        {
            return (code1 + code2) % 32; // Сложение и циклический сдвиг
        }

        public int SubtractCode(int code1, int code2)
        {
            return (code1 - code2 + 32) % 32; // Вычитание и циклический сдвиг
        }

        public char GetSymbolByCode(int code)
        {
            foreach (var letter in _alphabet)
            {
                if (letter.Value == code)
                {
                    return letter.Key;
                }
            }
            throw new ArgumentException($"Код '{code}' не соответствует ни одному символу.");
        }
    }
}