using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ConsoleApp1
{
    class TelegraphAlphabet
    {
        private Dictionary<char, byte> _alphabet;

        public TelegraphAlphabet()
        {
            _alphabet = new Dictionary<char, byte>
{
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
                throw new ArgumentException($"Символ '{symbol}' не найден в алфавите.");
            }
        }

        public int SumCode(int code1, int code2)
        {
            if (code1 + code2 > 31)
            {
                return code1 + code2 - 31;
            }
            else
            {
                return code1 + code2;
            }
        }

        public int SubtractCode(int code1, int code2)
        {
            if (code1 - code2 < 1)
            {
                return 31 - (code2 - code1);
            }
            else
            {
                return code1 - code2;
            }
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