using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zashita_lab5
{
    internal sealed class Crypt
    {
        //Размер блока
        private const int BLOCK_SIZE = 8;
        private const int KEY_SIZE = 32;
        private const int KEY_BLOCK = 8;
        private const int RAUND = 32;

        private readonly byte[][] SBOX =
        {
            //            0     1     2     3     4     5     6     7     8     9     A     B     C     D     E     F
            new byte[] { 0x0C, 0x04, 0x06, 0x02, 0x0A, 0x05, 0x0B, 0x09, 0x0E, 0x08, 0x0D, 0x07, 0x00, 0x03, 0x0F, 0x01 },

            new byte[] { 0x06, 0x08, 0x02, 0x03, 0x09, 0x0A, 0x05, 0x0C, 0x01, 0x0E, 0x04, 0x07, 0x0B, 0x0D, 0x00, 0x0F },

            new byte[] { 0x0B, 0x03, 0x05, 0x08, 0x02, 0x0F, 0x0A, 0x0D, 0x0E, 0x01, 0x07, 0x04, 0x0C, 0x09, 0x06, 0x00 },

            new byte[] { 0x0C, 0x08, 0x02, 0x01, 0x0D, 0x04, 0x0F, 0x06, 0x07, 0x00, 0x0A, 0x05, 0x03, 0x0E, 0x09, 0x0B },

            new byte[] { 0x07, 0x0F, 0x05, 0x0A, 0x08, 0x01, 0x06, 0x0D, 0x00, 0x09, 0x03, 0x0E, 0x0B, 0x04, 0x02, 0x0C },

            new byte[] { 0x05, 0x0D, 0x0F, 0x06, 0x09, 0x02, 0x0C, 0x0A, 0x0B, 0x07, 0x08, 0x01, 0x04, 0x03, 0x0E, 0x00 },

            new byte[] { 0x08, 0x0E, 0x02, 0x05, 0x06, 0x09, 0x01, 0x0C, 0x0F, 0x04, 0x0B, 0x00, 0x0D, 0x0A, 0x03, 0x07 },

            new byte[] { 0x01, 0x07, 0x0E, 0x0D, 0x00, 0x05, 0x08, 0x03, 0x04, 0x0F, 0x0A, 0x06, 0x09, 0x0C, 0x0B, 0x02 }
        };

        //Название файла без расширения
        private readonly string FILE_NAME;

        public Crypt(string file_name)
        {
            FILE_NAME = file_name;
        }

        //Бинарное чтение файла
        private byte[] ReadFile(string file_ext)
        {
            List<byte> bytes = new List<byte>();
            using (BinaryReader reader = new BinaryReader(File.Open(FILE_NAME + file_ext, FileMode.Open)))
            {
                //Чтение до конца файла
                while(reader.PeekChar() > -1)
                    bytes.Add(reader.ReadByte());
            }
            return bytes.ToArray();
        }

        //Бинарная запись файла
        private void WriteFile(string file_ext, byte[] bytes)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(FILE_NAME + file_ext, FileMode.OpenOrCreate)))
            {
                writer.Write(bytes);
            }
        }

        //Разбиваем массив байт на блоки байт
        private byte[][] GetTextBlocks(byte[] bytes)
        {
            //Блоки должны быть одинаковой длины, поэтому при необходимости дополняем
            List<byte> tmpBytes = bytes.ToList();
            while (tmpBytes.Count % BLOCK_SIZE != 0)
                tmpBytes.Add(0);

            //На выходе должен получиться массив значений
            bytes = tmpBytes.ToArray();
            byte[][] result = new byte[bytes.Length / BLOCK_SIZE][];
            for (int i = 0; i < result.Length; i++)
                result[i] = new byte[BLOCK_SIZE];

            //Заполняем массив
            int iter = 0;

            for(int i = 0; i < result.Length; i++)
            {
                for (int j = 0; j < BLOCK_SIZE; j++)
                    result[i][j] = bytes[iter++];
                if (iter > bytes.Length)
                    break;
            }
            return result;
        }

        //Обрабатываем ключ, приводим к нужному формату
        private byte[][] CalculateKey(string key)
        {
            //приводим строку к массиву байт
            List<byte> tmp = new List<byte>();
            for (int i = 0; i < key.Count(); i++)
                tmp.Add(Convert.ToByte(key[i]));

            //проверяем подходит ли размер ключа под условие
            while (tmp.Count < KEY_SIZE)
                tmp.Add(0);

            byte[][] result = new byte[KEY_BLOCK][];
            for (int i = 0; i < KEY_BLOCK; i++)
                result[i] = new byte[KEY_SIZE / KEY_BLOCK];

            int iter = 0;

            for (int i = 0; i < KEY_BLOCK; i++)
                for (int j = 0; j < KEY_SIZE / KEY_BLOCK; j++)
                    result[i][j] = tmp[iter++];
            return result;
        }

        //Шифрование одного блока
        private byte[] CryptBlock(byte[] block, byte[] key)
        {
            //Определяем блоки r, l в беззнаковое целое
            uint blockL = BitConverter.ToUInt32(block, 0);
            uint blockR = BitConverter.ToUInt32(block, 4);
            uint newKey = BitConverter.ToUInt32(key, 0);

            //Выполняем первый шаг
            uint tmp = (uint)((blockL + newKey) % Math.Pow(2, 32));

            //Выполняем второй шаг
            uint tmp2 = 0;
            tmp2 ^= (SBOX[0][tmp & 0x0000000f]);
            tmp2 ^= (uint)(SBOX[1][((tmp & 0x000000f0) >> 4)] << 4);
            tmp2 ^= (uint)(SBOX[2][((tmp & 0x00000f00) >> 8)] << 8);
            tmp2 ^= (uint)(SBOX[3][((tmp & 0x0000f000) >> 12)] << 12);
            tmp2 ^= (uint)(SBOX[4][((tmp & 0x000f0000) >> 16)] << 16);
            tmp2 ^= (uint)(SBOX[5][((tmp & 0x00f00000) >> 20)] << 20);
            tmp2 ^= (uint)(SBOX[6][((tmp & 0x0f000000) >> 24)] << 24);
            tmp2 ^= (uint)(SBOX[7][((tmp & 0xf0000000) >> 28)] << 28);

            //Выполняем третий шаг
            tmp2 = tmp2 << 11;

            //Выполняем четвертый шаг
            tmp = blockR;
            blockR = blockR ^ tmp2;

            //Выполняем пятый шаг
            blockL = tmp;

            byte[] result = new byte[BLOCK_SIZE];
            Array.Copy(BitConverter.GetBytes(blockL), 0, result, 0, 4);
            Array.Copy(BitConverter.GetBytes(blockR), 0, result, 4, 4);
            return result;
        }

        //Функция шифрования текста
        public void CryptText(string key)
        {
            //Обработка ключа
            byte[][] myKey = CalculateKey(key);

            //Получаем блоки из текстового файла
            byte[][] blocks = GetTextBlocks(ReadFile(".txt"));

            //начинаем выполнять операцию по числу раундов
            for (int i = 0; i < RAUND; i++)
            {
                int keyIndex = (i < 24) ? i % 8 : 7 - (i % 8);
                //пронать по всем блокам
                for (int j = 0; j < blocks.GetLength(0); j++)
                {
                    blocks[j] = CryptBlock(blocks[j], myKey[keyIndex]);
                }
            }

            //Получаем результат
            byte[] result = new byte[blocks.Length * BLOCK_SIZE];
            int iter = 0;

            for (int i = 0; i < blocks.Length; i++)
                for (int j = 0; j < BLOCK_SIZE; j++)
                    result[iter++] = blocks[i][j];

            WriteFile(".enc", result);
        }
    }
}  