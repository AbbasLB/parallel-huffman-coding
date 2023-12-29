using System.Collections;
using System.Text;

namespace Huffman_Project
{


    internal class Program
    {
        static void Main()
        {
            var huffmanEncoder = new SequentialHuffmanEncoder();

            var file = @"C:\Important\University-MOSIG\Mosig-M2\S1\Advanced Parallel Systems\Huffman Project\Samples\Sample 1.txt";
            var compressed_file = @"C:\Important\University-MOSIG\Mosig-M2\S1\Advanced Parallel Systems\Huffman Project\Samples\Sample 1_Compressed.txt";
            var decoded_file = @"C:\Important\University-MOSIG\Mosig-M2\S1\Advanced Parallel Systems\Huffman Project\Samples\Sample 1_Decoded.txt";

            var huffmanEncoded = huffmanEncoder.Compress(file, compressed_file);

            foreach (var encoded in huffmanEncoded)
            {
                Console.WriteLine($"{(char)encoded.Key}: {String.Join("", encoded.Value.Select(b => b ? '1' : '0'))}");
            }
        }

    }
}