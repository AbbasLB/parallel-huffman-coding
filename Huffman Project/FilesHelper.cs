using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Huffman_Project
{
    public static class FilesHelper
    {
        public static List<bool> ReadBooleanListFromFile(string filePath)
        {
            List<bool> readBooleanList = new List<bool>();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                byte currentByte;
                int bitPosition = 0;

                var readRes = fileStream.ReadByte();
                while (readRes != -1)
                {
                    currentByte = (byte)readRes;
                    for (int i = 0; i < 8; i++)
                    {
                        // Extract each bit from the byte and add to the list
                        bool value = (currentByte & (1 << (7 - bitPosition))) != 0;
                        readBooleanList.Add(value);
                        bitPosition++;

                        if (bitPosition == 8)
                        {
                            bitPosition = 0;
                        }
                    }
                    readRes = fileStream.ReadByte();
                }
            }

            return readBooleanList;
        }
        public static void WriteBooleanListToFile(List<bool> booleanList, string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            using (BufferedStream bufferedStream = new BufferedStream(fileStream))
            using (BinaryWriter writer = new BinaryWriter(bufferedStream))
            {

                byte currentByte = 0;
                int bitPosition = 0;

                foreach (bool value in booleanList)
                {
                    // Set the corresponding bit in the byte
                    if (value)
                    {
                        currentByte |= (byte)(1 << (7 - bitPosition));
                    }

                    bitPosition++;

                    // If 8 bits have been aggregated, write the byte to file and reset
                    if (bitPosition == 8)
                    {
                        writer.Write(currentByte);
                        currentByte = 0;
                        bitPosition = 0;
                    }
                }

                // If there are remaining bits, write the last byte padded with zeros
                if (bitPosition > 0)
                {
                    writer.Write(currentByte);
                }
            }
        }
    


    }
}
