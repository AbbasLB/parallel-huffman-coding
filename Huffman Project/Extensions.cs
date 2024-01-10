using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman_Project
{
    public static class Extensions
    {
        public static List<bool> ToListOfBool(this byte packedByte)
        {
            List<bool> booleanList = new List<bool>();

            for (int i = 0; i < 8; i++)
            {
                // Extract each bit from the byte and add to the list
                bool value = (packedByte & (1 << i)) != 0;
                booleanList.Add(value);
            }

            booleanList.Reverse(); // Reversing to maintain the original order

            return booleanList;
        }
        public static byte ExtractByte(this List<bool> data, int startingIndex)
        {
            var byteList = data.GetRange(startingIndex, 8);

            byte result = 0;
            // This assumes the array never contains more than 8 elements!
            int index = 8 - byteList.Count;

            // Loop through the array
            foreach (bool b in byteList)
            {
                // if the element is 'true' set the bit at that position
                if (b)
                    result |= (byte)(1 << (7 - index));

                index++;
            }

            return result;
        }

        public static string ToBinaryString(this List<bool> data)
        {
            return String.Join("", data.Select(d => d ? '1' : '0').ToArray());
        }
    }
}
