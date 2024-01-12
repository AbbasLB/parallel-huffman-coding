import argparse
import json
import matplotlib.pyplot as plt

def parse_args():
    parser = argparse.ArgumentParser(description='Generate compression and decompression plots.')
    parser.add_argument('input_file', type=str, help='Input file containing processing information')
    return parser.parse_args()

def get_size_from_filename(filename):
    # Example: uniformly_random_file_1MB.bin_1.compressed
    return float(filename.split('.')[0].split('_')[-1].replace('MB', ''))

def get_type_from_filename(filename):
    # Example: uniformly_random_file_1MB.bin_1.compressed
    if filename.startswith("uniformly_random"):
        return "uniformly_random"
    if filename.startswith("all_same"):
        return "all_same"
    if filename.startswith("ten_chunks"):
        return "ten_chunks"
    return "NOT_EXPECTED"
    

def process_file(input_file):
    compress_data = {'sequential': {}, 'parallel': {}}

    with open(input_file, 'r') as file:
        for line in file:
            if line.startswith('[Info]') or line.startswith('Warning') or not line.strip():
                continue
            print(line)
            info = json.loads(line)
            type = get_type_from_filename(info['Filename'])
            size_in_byte = get_size_from_filename(info['Filename'])
            compression_ratio = info['CompressionRatio']

            if info['Action'] == 'compress_sequential':
                if type not in compress_data['sequential']:
                    compress_data['sequential'][type] = []
                compress_data['sequential'][type].append((size_in_byte,compression_ratio ))
            elif info['Action'] == 'compress_parallel':
                if type not in compress_data['parallel']:
                    compress_data['parallel'][type] = []
                compress_data['parallel'][type].append((size_in_byte,compression_ratio))

    return compress_data



def plot_graph(data, title):
    plt.figure(figsize=(10, 6))
    plt.title(title)
    plt.xlabel('Size (MB)')
    plt.ylabel('Compression Ratio')

    for label, values in data.items():
        sorted_values = sorted(values)
        print(f'{label} {sorted_values}')
        x = [size for size, _ in sorted_values]
        y = [compression_ratio for _, compression_ratio in sorted_values]
        plt.plot(x, y, label=label , marker='x')

    plt.legend()
    plt.show()
    
def main():
    args = parse_args()
    compress_data = process_file(args.input_file)
    # {'sequential': {'all_same': [(100.0, 7.999996948243352), (10.0, 7.99996948253829), (1.0, 7.999694835859838), (200.0, 7.9999984741213845), (20.0, 7.999984741240041), (50.0, 7.999993896489031), (5.0, 7.999938965309408)], 'ten_chunks': [(100.0, 2.352939486917303), (10.0, 2.4999827147725275), (1.0, 2.6664954391604088), (200.0, 2.4999991357329496), (20.0, 2.6666578505065774), (50.0, 2.3529377973664447), (5.0, 2.352907385865897)], 'uniformly_random': [(100.0, 1.320013073539516), (10.0, 1.3200399772544076), (1.0, 1.319876644219271), (200.0, 1.3200129322937522), (20.0, 1.3200268493009968), (50.0, 1.3200237584537398), (5.0, 1.3200311698644311)]}, 'parallel': {}}
    print(compress_data)
    # Process and plot compression data
    compression_data = {}
    for label, values in compress_data.items():
        for file_type, compression_ratios in values.items():
            if file_type == "ten_chunks":
                compression_data[f'{label} {file_type}'] = compression_ratios


    plot_graph(compression_data, 'Compression Ratios')
if __name__ == '__main__':
    main()
