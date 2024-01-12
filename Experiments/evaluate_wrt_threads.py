import subprocess
import os
import time
import shutil

def run_command(command):
    result = subprocess.run(command, capture_output=True, text=True)
    return result.stdout, result.stderr


def delete_path(path):
    """Delete a path, whether it's a file or a directory with files."""
    try:
        if os.path.isfile(path):
            # If it's a file, delete the file
            os.remove(path)
        elif os.path.isdir(path):
            # If it's a directory, delete the entire directory and its contents
            shutil.rmtree(path)
        else:
            print(f"Warning: {path} is neither a file nor a directory.")
    except Exception as e:
        print(f"Error while deleting {path}: {e}")
        
compress_dir = "ToCompress"


def compress_and_decompress_sequential(huffman_script_path, filename):
    
    file_to_compress_path = os.path.join(compress_dir, filename)
    dest_path_sequential_comp = os.path.join("Sequential_Compressed", f"{filename}.compressed")
    dest_path_sequential_decode = os.path.join("Sequential_Decoded", f"{filename}.decoded")
    
    compress_cmd = f"{huffman_script_path} compress_sequential {file_to_compress_path} {dest_path_sequential_comp} 1"
    decompress_cmd = f"{huffman_script_path} decode_sequential {dest_path_sequential_comp} {dest_path_sequential_decode} 1"

    print(f'[Info] Compress sequentially {filename}')
    # Compress sequentially
    stdout,stderr = run_command(compress_cmd)
    print(f'{stdout}')

    print(f'[Info] Decompress sequentially {filename}')
    # Decompress sequentially
    stdout,stderr = run_command(decompress_cmd)
    print(f'{stdout}')
    
    # Delete compressed file
    delete_path(dest_path_sequential_comp)
    # Delete decoded file
    delete_path(dest_path_sequential_decode)

def compress_and_decompress_parallel(huffman_script_path, filename, num_of_threads):
    for threads in range(1, num_of_threads + 1, 1):
        
        file_to_compress_path = os.path.join(compress_dir, filename)
        dest_path_sequential_comp = os.path.join("Parallel_Compressed", f"{filename}_{threads}.compressed")
        dest_path_sequential_decode = os.path.join("Parallel_Decoded", f"{filename}_{threads}.decoded")
 
        compress_cmd = f"{huffman_script_path} compress_parallel {file_to_compress_path} {dest_path_sequential_comp} {threads}"
        decompress_cmd = f"{huffman_script_path} decode_parallel {dest_path_sequential_comp} {dest_path_sequential_decode} {threads}"

        print(f'[Info] Compress parallel {threads} {filename}')
        # Compress in parallel
        stdout,stderr = run_command(compress_cmd)
        print(f'{stdout}')

        print(f'[Info] Compress parallel {threads} {filename}')
        # Decompress in parallel
        stdout,stderr = run_command(decompress_cmd)
        print(f'{stdout}')
        
        # Delete compressed file
        delete_path(dest_path_sequential_comp)
        # Delete decoded file
        delete_path(dest_path_sequential_decode)


# python evaluate_wrt_threads.py "..\Huffman Project\bin\Debug\net6.0\Huffman Project.exe" 8 > Results\threads_results.txt
if __name__ == "__main__":
    if len(os.sys.argv) != 3:
        print("Usage: python evaluate_wrt_threads.py <huffman_script_path> <max_threads>")
        os.sys.exit(1)

    huffman_script_path = os.sys.argv[1]
    max_threads = int(os.sys.argv[2])


    # Iterate over files starting with uniformly_random in the source directory
    for filename in os.listdir(compress_dir):
        if filename.startswith("uniformly_random"):
            print(f'[Info] Processing {filename}')

            # Compress and decompress sequentially
            compress_and_decompress_sequential(huffman_script_path, filename)

            # Compress and decompress in parallel with varying thread counts
            compress_and_decompress_parallel(huffman_script_path, filename, max_threads)

    print("[Info] Compression and decompression completed.")
