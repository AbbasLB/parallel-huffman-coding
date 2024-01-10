using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Huffman_Project
{


    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Failed");
                Console.WriteLine("Usage: <script>  compress_sequential|decode_sequential|compress_parallel|decode_parallel source_path dest_path num_of_threads");
                return;
            }

            var fileToCompress = @"C:\Important\University-MOSIG\Mosig-M2\S1\Advanced Parallel Systems\Huffman Project\Samples\Sample_1.txt";

            var compressedFile = @"C:\Important\University-MOSIG\Mosig-M2\S1\Advanced Parallel Systems\Huffman Project\Samples\tmp\Sample 1_Compressed.txt";
            var decodedFile = @"C:\Important\University-MOSIG\Mosig-M2\S1\Advanced Parallel Systems\Huffman Project\Samples\tmp\Sample 1_Decoded.txt";
            var compressedFileParallel = @"C:\Important\University-MOSIG\Mosig-M2\S1\Advanced Parallel Systems\Huffman Project\Samples\tmp\Sample 1_Compressed_Parallel.txt";
            var decodedFileParallel = @"C:\Important\University-MOSIG\Mosig-M2\S1\Advanced Parallel Systems\Huffman Project\Samples\tmp\Sample 1_Decoded_Parallel.txt";


            string action = args[0];
            string sourcePath = args[1];
            string destPath = args[2];
            int compressedFilesCount = int.Parse(args[3]);

            switch (action)
            {
                case "compress_sequential":
                    new SequentialHuffmanEncoder().Compress(sourcePath, destPath);
                    break;
                case "decode_sequential":
                    new SequentialHuffmanEncoder().Decode(sourcePath, destPath);
                    break;
                case "compress_parallel":
                    new ParallelHuffmanEncoder(compressedFilesCount).Compress(sourcePath, destPath);
                    break;
                case "decode_parallel":
                    new ParallelHuffmanEncoder(compressedFilesCount).Decode(sourcePath, destPath);
                    break;
                default:
                    break;
            }
            Console.WriteLine("Success");
        }

    }
}