using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman_Project
{
    public class MinHeap<T> where T : IComparable<T>
    {
        T[] heap;
        int curSize;
        public int Count => curSize;

        public MinHeap(int maxSize)
        {
            heap = new T[maxSize];
            curSize = 0;
        }


        public void Add(T element)
        {
            if (curSize == heap.Length)
                throw new Exception("Can't add new element to full heap");
            int pos = curSize++;
            heap[pos] = element;
            HeapifyUp(pos);
        }


        public T Top()
        {
            if (curSize == 0)
                throw new ArgumentOutOfRangeException("Heap contains no elements");
            return heap[0];
        }


        public T PopTop()
        {
            var top = Top();
            Swap(0, curSize - 1);
            heap[--curSize] = default;
            HeapifyDown(0);
            return top;
        }


        private int Left(int pos)
        {
            var left = 2 * pos + 1;
            return (left < curSize && left >= 0) ? left : -1;
        }
        private int Right(int pos)
        {
            var right = 2 * pos + 2;
            return (right < curSize && right >= 0) ? right : -1;
        }

        private int Parent(int pos)
        {
            var parent = (pos - 1) / 2;
            return (parent < curSize && parent >= 0) ? parent : -1;
        }

        private void Swap(int pos1, int pos2)
        {
            T tmp = heap[pos1];
            heap[pos1] = heap[pos2];
            heap[pos2] = tmp;
        }
        private void HeapifyUp(int pos)
        {
            var parentPos = Parent(pos);
            if (parentPos == -1)
                return;
            if (heap[parentPos].CompareTo(heap[pos]) > 0)
            {
                Swap(pos, parentPos);
                HeapifyUp(parentPos);
            }
            else return;
        }

        private void HeapifyDown(int pos)
        {
            if (pos == -1)
                return;

            int left = Left(pos);
            int right = Right(pos);
            int minimal = pos;

            if (left != -1 && heap[left].CompareTo(heap[minimal]) < 0)
                minimal = left;

            if (right != -1 && heap[right].CompareTo(heap[minimal]) < 0)
                minimal = right;

            if (minimal != pos)
            {
                Swap(minimal, pos);
                HeapifyDown(minimal);
            }
        }

    }
}
