using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

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

            //var fileToCompress = @"C:\Important\University-MOSIG\Mosig-M2\S1\Advanced Parallel Systems\Huffman Project\Samples\ToCompress\all_same_file_1MB.bin";

            //var compressedFile = @"C:\Important\University-MOSIG\Mosig-M2\S1\Advanced Parallel Systems\Huffman Project\Samples\Sequential_Compressed\all_same_file_1MB.bin";
            //var decodedFile = @"C:\Important\University-MOSIG\Mosig-M2\S1\Advanced Parallel Systems\Huffman Project\Samples\Sequential_Decoded\all_same_file_1MB.bin";
            //var compressedFileParallel = @"C:\Important\University-MOSIG\Mosig-M2\S1\Advanced Parallel Systems\Huffman Project\Samples\Parallel_Compressed\all_same_file_1MB.bin";
            //var decodedFileParallel = @"C:\Important\University-MOSIG\Mosig-M2\S1\Advanced Parallel Systems\Huffman Project\Samples\Parallel_Decoded\all_same_file_1MB.bin";

            ////args = new string[] { "compress_sequential", fileToCompress, compressedFile, "4" };
            //args = new string[] { "compress_parallel", fileToCompress, compressedFileParallel, "4" };
            ////args = new string[] { "decode_sequential", compressedFile, decodedFile, "4" };
            ////args = new string[] { "decode_parallel", compressedFileParallel, decodedFileParallel, "4" };

            string action = args[0];
            string sourcePath = args[1];
            string destPath = args[2];
            int compressedFilesCount = int.Parse(args[3]);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
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


            var time_taken = (double)stopwatch.ElapsedMilliseconds / 1000;
            double compression_ratio = -1;
            if (action.StartsWith("compress"))
            {
                double originalFileSize = new FileInfo(sourcePath).Length;
                double compressedFileSize = FilesHelper.GetFileOrDirectorySize(destPath);
                compression_ratio = originalFileSize / compressedFileSize;
            }


            var data = new
            {
                Filename = Path.GetFileName(sourcePath),
                Action = action,
                Time = time_taken,
                Threads = compressedFilesCount,
                CompressionRatio = compression_ratio
            };

            string jsonText = JsonSerializer.Serialize(data);
            Console.WriteLine(jsonText);
        }

    }
}