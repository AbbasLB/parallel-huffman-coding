import sys
import os
import random

allowed_characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.!? "

def generate_file(size_in_MB, file_type, destination_path):
    total_bytes = int(size_in_MB * 1024 * 1024)

    if file_type == 'uniformly_random':
        data = bytearray(ord(random.choice(allowed_characters)) for _ in range(total_bytes))
        #data = bytearray(os.urandom(total_bytes))
    elif file_type == 'all_same':
        byte_value = ord(random.choice(allowed_characters))
        #byte_value = random.randint(0, 255)
        data = bytearray([byte_value] * total_bytes)
    elif file_type == 'ten_chunks':
        chunk_size = total_bytes // 10
        data = bytearray()
        for i in range(10):
            #byte_value = random.randint(0, 255)
            byte_value = ord(random.choice(allowed_characters))
            data.extend(bytearray([byte_value] * chunk_size))
    else:
        raise ValueError("Invalid file type. Use 'uniformly_random', 'all_same', or 'ten_chunks'.")

    with open(destination_path, "wb") as file:
        file.write(data)

if __name__ == "__main__":
    if len(sys.argv) != 4:
        print("Usage: python script.py <size_in_MB> <file_type> <destination_path>")
        sys.exit(1)

    size_in_MB = float(sys.argv[1])
    file_type = sys.argv[2]
    destination_path = sys.argv[3]

    generate_file(size_in_MB, file_type, destination_path)
    print(f"{file_type} file generated successfully at {destination_path}.")