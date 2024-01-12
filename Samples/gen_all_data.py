import subprocess
import os

#sizes = [1, 5, 10]
sizes = [1, 5, 10, 20, 50, 100, 200, 500, 1000]
types = ['uniformly_random', 'all_same', 'ten_chunks']
output_directory = "ToCompress"

# Create the output directory if it doesn't exist
os.makedirs(output_directory, exist_ok=True)

for size in sizes:
    for file_type in types:
        destination_path = os.path.join(output_directory, f"{file_type}_file_{size}MB.bin")
        subprocess.run(["python", "gen_data.py", str(size), file_type, destination_path])

