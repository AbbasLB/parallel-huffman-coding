using System.Text;

namespace Huffman_Project
{
    internal class ProgramText
    {
        static void ExtractCodes(HuffmanNode<char> root, string binary, Dictionary<char, string> codesDict)
        {
            if (root == null)
                return;
            if (root.Character.HasValue)
                codesDict.Add(root.Character.Value, binary);
            ExtractCodes(root.Left, binary + "1", codesDict);
            ExtractCodes(root.Right, binary + "0", codesDict);

        }

        static Dictionary<char, string> HuffmanEncode(Dictionary<char, ulong> frequencies)
        {
            var minHeap = new MinHeap<HuffmanNode<char>>(frequencies.Count);

            foreach (var freq in frequencies)
                minHeap.Add(new HuffmanNode<char>(freq.Key, freq.Value));

            while (minHeap.Count != 1)
            {
                var left = minHeap.PopTop();
                var right = minHeap.PopTop();

                var top = new HuffmanNode<char>(null, left.Occurences + right.Occurences);

                top.Left = left;
                top.Right = right;

                minHeap.Add(top);
            }

            var huffmanEncoded = new Dictionary<char, string>();
            ExtractCodes(minHeap.Top(), "", huffmanEncoded);
            return huffmanEncoded;

        }
        static Dictionary<char, ulong> ExtractFrequencies(string text)
        {
            var frequencies = new Dictionary<char, ulong>();
            foreach (var c in text)
            {
                if (!frequencies.ContainsKey(c))
                    frequencies.Add(c, 0);
                frequencies[c]++;
            }
            return frequencies;
        }
        static string EncodeToBinaryAsString(string originalText, Dictionary<char, string> huffmanEncoded)
        {
            var sb = new StringBuilder();
            foreach (var c in originalText)
                sb.Append(huffmanEncoded[c]);
            return sb.ToString();
        }
        static void SaveBinaryStringToFile(string binaryString, string filePath)
        {
            // Convert binary string to bytes
            int numOfBytes = binaryString.Length / 8;
            byte[] bytes = new byte[numOfBytes];

            for (int i = 0; i < numOfBytes; i++)
            {
                string byteString = binaryString.Substring(i * 8, 8);
                bytes[i] = Convert.ToByte(byteString, 2);
            }

            // Write bytes to file
            File.WriteAllBytes(filePath, bytes);
        }

        static void EncodeFile(string fileToEncode, string dstFile)
        {
            var text = "aabbbbbcccccccccddddddddddddeeeeeeeeeeeeeffffffffffffffff";


        }

        static string GenText()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 256; i++)
                sb.Append((char)i);
            return sb.ToString();
        }

        static void Main_Text()
        {
            var text = GenText();
            var frequencies = ExtractFrequencies(text);

            var huffmanEncoded = HuffmanEncode(frequencies);
            foreach (var encoded in huffmanEncoded)
            {
                Console.WriteLine($"{encoded.Key}: {encoded.Value}");
            }
            var binAsString = EncodeToBinaryAsString(text, huffmanEncoded);
            Console.WriteLine($"Encoded: {binAsString}");

            var oldSize = text.Length;
            var newSize = binAsString.Length / 8;
            Console.WriteLine($"Old Length={oldSize} NewLength={newSize} CompressionRatio={(double)newSize / oldSize}");

        }
        private class HuffmanNode<T> : IComparable<HuffmanNode<T>> where T : struct
        {
            public T? Character;
            public ulong Occurences;
            public HuffmanNode<T> Left, Right;
            public HuffmanNode(T? character, ulong occ)
            {
                Left = Right = null;
                this.Character = character;
                this.Occurences = occ;
            }

            public int CompareTo(HuffmanNode<T>? other)
            {
                return this.Occurences.CompareTo(other?.Occurences);
            }
        }
    }
}