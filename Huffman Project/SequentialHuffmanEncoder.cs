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


        public void Compress(string fileToCompress, string compressedFile)
        {
            if (!File.Exists(fileToCompress))
                throw new FileNotFoundException(nameof(fileToCompress));

            var inputBytes = File.ReadAllBytes(fileToCompress);

            var compressedBits = CompressBinary(inputBytes);

            FilesHelper.WriteBooleanListToFile(compressedBits, compressedFile);
        }

        public List<bool> CompressBinary(byte[] bytesToCompress)
        {
            var frequencies = ExtractFrequencies(bytesToCompress);

            var huffmanCodes = GenerateHuffmanCodes(frequencies);

            var compressedBits = GenerateHeader(huffmanCodes);
            compressedBits.AddRange(CompressBytes(bytesToCompress, huffmanCodes));

            //add ending bit
            compressedBits.Add(true);
            return compressedBits;
        }

        public void Decode(string fileToDecode, string decodedFile)
        {
            if (!File.Exists(fileToDecode))
                throw new FileNotFoundException(nameof(fileToDecode));

            var inputBytes = FilesHelper.ReadBooleanListFromFile(fileToDecode);

            var decodedBytes = DecodeBinary(inputBytes);

            File.WriteAllBytes(decodedFile, decodedBytes.ToArray());
        }

        public List<byte> DecodeBinary(List<bool> bytesToDecode)
        {
            (var headerLength, var huffmanCodes) = ReadHeader(bytesToDecode);
            RemoveBinaryPadding(bytesToDecode);

            return DecodeBytes(bytesToDecode, headerLength, huffmanCodes);
        }

        private (int HeaderLength, Dictionary<string, byte> HuffmanCodes) ReadHeader(List<bool> file)
        {
            var huffmanCodes = new Dictionary<string, byte>();

            var tableEntriesCount = (int)file.ExtractByte(0);
            for (int i = 0; i < tableEntriesCount; i++)
            {
                var originalByte = file.ExtractByte(8 + 24 * i);
                var huffmanEncoded = file.GetRange(16 + 24 * i, 16);
                RemoveBinaryPadding(huffmanEncoded);
                huffmanCodes.Add(huffmanEncoded.ToBinaryString(), originalByte);
            }
            var headerLength = 8 + tableEntriesCount * 24;
            return (headerLength, huffmanCodes);
        }

        private void RemoveBinaryPadding(List<bool> bits)
        {
            var endingBitPos = bits.LastIndexOf(true);
            bits.RemoveRange(endingBitPos, bits.Count - endingBitPos);
        }
        private List<bool> CompressBytes(byte[] data, Dictionary<byte, List<bool>> huffmanCodes)
        {
            var compressed = new List<bool>(data.Length * 8 );
            foreach (var curByte in data)
                compressed.AddRange(huffmanCodes[curByte]);
            return compressed;
        }

        private List<byte> DecodeBytes(List<bool> data, int headerLength, Dictionary<string, byte> huffmanCodes)
        {
            var decoded = new List<byte>();
            string accumulating = "";
            foreach (var curBit in data.Skip(headerLength))
            {
                accumulating += curBit ? '1' : '0';
                if (huffmanCodes.TryGetValue(accumulating, out var decodedByte))
                {
                    decoded.Add(decodedByte);
                    accumulating = "";
                }
            }
            return decoded;
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
