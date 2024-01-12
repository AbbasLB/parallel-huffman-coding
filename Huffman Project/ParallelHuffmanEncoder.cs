using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Huffman_Project
{
    internal class ParallelHuffmanEncoder
    {
        private readonly int _compressPartsNumber;
        public ParallelHuffmanEncoder(int compressPartsNumber)
        {
            _compressPartsNumber = compressPartsNumber;
        }

        public void Compress(string fileToCompress, string compressedDirectory)
        {
            if (!File.Exists(fileToCompress))
                throw new FileNotFoundException(nameof(fileToCompress));

            if (Directory.Exists(compressedDirectory))
                Directory.Delete(compressedDirectory, true);
            Directory.CreateDirectory(compressedDirectory);

            //Console.WriteLine($"Started Read {DateTime.Now.ToString("hh:mm:ss.fff")}");
            var filePrefix = Path.Combine(compressedDirectory, "Part_");
            var inputBytes = File.ReadAllBytes(fileToCompress);
            //Console.WriteLine($"Finished Read {DateTime.Now.ToString("hh:mm:ss.fff")}");

            var chunkSize = inputBytes.Length / _compressPartsNumber + 1;

            int chunkIndex = 0;
            var threads = inputBytes.Chunk(chunkSize).Select(chunk =>
            {
                var thread = CompressChunk(chunk, filePrefix + chunkIndex);
                chunkIndex++;
                return thread;
            }).ToList();

            foreach (var thread in threads)
                thread.Join();

            //Console.WriteLine($"Finished Compress {DateTime.Now.ToString("hh:mm:ss.fff")}");
        }
        private Thread CompressChunk(byte[] chunk, string compressedFilePath)
        {
            var thread = new Thread(() =>
            {
                //Console.WriteLine($"Thread {compressedFilePath.Last()} Started Compress {DateTime.Now.ToString("hh:mm:ss.fff")}");
                var sequentialEncoder = new SequentialHuffmanEncoder();
                var compressedBits = sequentialEncoder.CompressBinary(chunk);
                //Console.WriteLine($"Thread {compressedFilePath.Last()} Started Write {DateTime.Now.ToString("hh:mm:ss.fff")}");
                FilesHelper.WriteBooleanListToFile(compressedBits, compressedFilePath);
            });
            thread.Start();
            return thread;
        }



        public void Decode(string directoryToDecode, string decodedFile)
        {
            if (!Directory.Exists(directoryToDecode))
                throw new FileNotFoundException(nameof(directoryToDecode));

            var files = Directory.GetFiles(directoryToDecode);

            var chunksToFill = Enumerable.Repeat(new List<byte>(), files.Length).ToList();
            int chunkIndex = 0;
            var threads = files.Select(file =>
            {
                var thread = DecompressFile(file, chunksToFill, chunkIndex);
                chunkIndex++;
                return thread;
            }).ToList();

            foreach (var thread in threads)
                thread.Join();

            var decodedBytes = chunksToFill.SelectMany(b => b);

            File.WriteAllBytes(decodedFile, decodedBytes.ToArray());

        }

        private Thread DecompressFile(string fileToDecode, List<List<byte>> chunksToFill, int index)
        {
            var thread = new Thread(() =>
            {
                var sequentialEncoder = new SequentialHuffmanEncoder();
                var inputBytes = FilesHelper.ReadBooleanListFromFile(fileToDecode);

                var decodedBytes = sequentialEncoder.DecodeBinary(inputBytes);
                chunksToFill[index] = decodedBytes;

            });
            thread.Start();
            return thread;
        }
    }
}
