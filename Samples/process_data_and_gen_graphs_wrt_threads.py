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
    

def process_file(input_file):
    compress_data = {'sequential': [], 'parallel': {}}
    decompress_data = {'sequential': [], 'parallel': {}}

    with open(input_file, 'r') as file:
        for line in file:
            if line.startswith('[Info]') or line.startswith('Warning') or not line.strip():
                continue
            print(line)
            info = json.loads(line)
            size_in_mb = get_size_from_filename(info['Filename'])
            if info['Action'] == 'compress_sequential':
                compress_data['sequential'].append((size_in_mb, info['Time']))
            elif info['Action'] == 'decode_sequential':
                decompress_data['sequential'].append((size_in_mb, info['Time']))
            elif info['Action'] == 'compress_parallel':
                thread = info['Threads']
                if thread not in compress_data['parallel']:
                    compress_data['parallel'][thread] = []
                compress_data['parallel'][thread].append((size_in_mb, info['Time']))
            elif info['Action'] == 'decode_parallel':
                thread = info['Threads']
                if thread not in decompress_data['parallel']:
                    decompress_data['parallel'][thread] = []
                decompress_data['parallel'][thread].append((size_in_mb, info['Time']))

    return compress_data, decompress_data


def plot_graph(data, title):
    plt.figure(figsize=(10, 6))
    plt.title(title)
    plt.xlabel('Size (MB)')
    plt.ylabel('Time (s)')

    for label, values in data.items():
        sorted_values = sorted(values)
        print(f'{label} {sorted_values}')
        x = [size for size, _ in sorted_values]
        y = [time for _, time in sorted_values]
        plt.plot(x, y, label=label , marker='x')

    plt.legend()
    plt.show()
    
def main():
    args = parse_args()
    compress_data, decompress_data = process_file(args.input_file)
    #format: {'sequential': [(1.0, 0.123), (5.0, 0.549)], 'parallel': {1: [(1.0, 0.13), (5.0, 0.557)], 4: [(1.0, 0.046), (5.0, 0.195)], 7: [(1.0, 0.035), (5.0, 0.133)]}}

    # Process and plot compression data
    compression_data = {}
    for label, values in compress_data.items():
        if label == 'parallel':
            for thread_count, thread_values in values.items():
                compression_data[f'Parallel {thread_count} Threads'] = thread_values
        else:
            compression_data['Sequential'] = values

    plot_graph(compression_data, 'Compression Performance')

    # Process and plot decompression data
    decompression_data = {}
    for label, values in decompress_data.items():
        if label == 'parallel':
            for thread_count, thread_values in values.items():
                decompression_data[f'Parallel {thread_count} Threads'] = thread_values
        else:
            decompression_data['Sequential'] = values

    plot_graph(decompression_data, 'Decompression Performance')
if __name__ == '__main__':
    main()
