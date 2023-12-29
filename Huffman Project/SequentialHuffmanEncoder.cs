using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman_Project
{
    internal class SequentialHuffmanEncoder
    {


        public Dictionary<byte, List<bool>> Compress(string fileToCompress, string compressedFile)
        {
            if (!File.Exists(fileToCompress))
                throw new FileNotFoundException(nameof(fileToCompress));

            var inputBytes = File.ReadAllBytes(fileToCompress);

            var frequencies = ExtractFrequencies(inputBytes);

            var huffmanCodes = GenerateHuffmanCodes(frequencies);

            var compressedBits = GenerateHeader(huffmanCodes);
            compressedBits.AddRange(CompressBytes(inputBytes, huffmanCodes));

            //add padding bit
            compressedBits.Add(true);

            FilesHelper.WriteBooleanListToFile(compressedBits, compressedFile);

            return huffmanCodes;
        }
        public Dictionary<byte, List<bool>> Decode(string fileToDecode, string decodedFile)
        {
            if (!File.Exists(fileToDecode))
                throw new FileNotFoundException(nameof(fileToDecode));

            var inputBytes = FilesHelper.ReadBooleanListFromFile(fileToDecode);

            var frequencies = ExtractFrequencies(inputBytes);

            var huffmanCodes = GenerateHuffmanCodes(frequencies);

            var compressedBits = GenerateHeader(huffmanCodes);
            compressedBits.AddRange(CompressBytes(inputBytes, huffmanCodes));

            //add padding bit
            compressedBits.Add(true);

            FilesHelper.WriteBooleanListToFile(compressedBits, decodedFile);

            return huffmanCodes;
        }

        private (int HeaderLength, Dictionary<byte, List<bool>> HuffmanCodes) ReadHeader(List<bool> file)
        {

        }

        private void ExtractCodesHelper(HuffmanNode<byte> root, List<bool> binary, Dictionary<byte, List<bool>> codesDict)
        {
            if (root == null)
                return;

            if (root.Character.HasValue)
                codesDict.Add(root.Character.Value, binary);

            var leftBitArray = binary.ToList();
            leftBitArray.Add(true);

            var rightBitArray = binary.ToList();
            rightBitArray.Add(false);

            ExtractCodesHelper(root.Left, leftBitArray, codesDict);
            ExtractCodesHelper(root.Right, rightBitArray, codesDict);

        }
        private void ExtractCodes(HuffmanNode<byte> root, Dictionary<byte, List<bool>> codesDict)
        {
            if (root != null && root.Left == null && root.Right == null)
            {
                codesDict.Add(root.Character.Value, new List<bool>() { false });
            }
            else ExtractCodesHelper(root, new List<bool>(), codesDict);


        }

        private Dictionary<byte, List<bool>> GenerateHuffmanCodes(Dictionary<byte, ulong> frequencies)
        {
            var minHeap = new MinHeap<HuffmanNode<byte>>(frequencies.Count);

            foreach (var freq in frequencies)
                minHeap.Add(new HuffmanNode<byte>(freq.Key, freq.Value));

            while (minHeap.Count != 1)
            {
                var left = minHeap.PopTop();
                var right = minHeap.PopTop();

                var top = new HuffmanNode<byte>(null, left.Occurences + right.Occurences);

                top.Left = left;
                top.Right = right;

                minHeap.Add(top);
            }

            var huffmanEncoded = new Dictionary<byte, List<bool>>();
            ExtractCodes(minHeap.Top(), huffmanEncoded);
            return huffmanEncoded;

        }

        private List<bool> CompressBytes(byte[] data, Dictionary<byte, List<bool>> huffmanCodes)
        {
            var compressed = new List<bool>(data.Length * 8);
            foreach (var curByte in data)
                compressed.AddRange(huffmanCodes[curByte]);
            return compressed;
        }



        private List<bool> GenerateHeader(Dictionary<byte, List<bool>> huffmanCodes)
        {
            var header = new List<bool>();

            var length = huffmanCodes.Count;
            var lengthAsByte = (byte)length;

            header.AddRange(lengthAsByte.ToListOfBool());

            //for each entry in the huffman table, we will use 3 bytes 
            foreach (var pair in huffmanCodes)
            {
                var charAsBinary = pair.Key.ToListOfBool();
                header.AddRange(charAsBinary);
                header.AddRange(pair.Value);

                //end bit
                header.Add(true);

                var paddingLen = 24 - (charAsBinary.Count + pair.Value.Count + 1);
                for (int i = 0; i < paddingLen; i++)
                    header.Add(false);
            }
            return header;
        }

        private Dictionary<byte, ulong> ExtractFrequencies(IEnumerable<byte> bytes)
        {
            var frequencies = new Dictionary<byte, ulong>();
            foreach (var b in bytes)
            {
                if (!frequencies.ContainsKey(b))
                    frequencies.Add(b, 0);
                frequencies[b]++;
            }
            return frequencies;
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
