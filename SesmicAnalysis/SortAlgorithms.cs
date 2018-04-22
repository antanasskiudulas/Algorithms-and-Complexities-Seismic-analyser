using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeismicAnalysis
{
    /// <summary>
    /// Contains all the sort methods utilized across the project
    /// </summary>
    class SortAlgorithms
    {
        /// <summary>
        /// Heap algorithm transform the records into a heap and sorts it logarithmically
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="region">The data to be sorted</param>
        /// <param name="properyField">Property to sort</param>
        public static void HeapSort<T>(RecordCollection region, Func<SeismicRecord, T> properyField) where T : IComparable
        {
            int HeapSize = region.Records.currentCapacity;
            int i;
            //starting from the middle
            for (i = (HeapSize) / 2; i >= 0; i--)
            {
                //create a max heap (sorted)
                Max_Heapify(region, HeapSize, i, properyField);
            }
            for (i = region.Records.currentCapacity - 1; i > 0; i--)
            {
                SeismicRecord temp = region.Records[i];
                region.Records[i] = region.Records[0];
                region.Records[0] = temp;
                HeapSize--;
                Max_Heapify(region, HeapSize, 0, properyField);
            }
        }

        /// <summary>
        /// sorts the heap into parent nodes higher than children
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="region">region to sort</param>
        /// <param name="HeapSize">the size of the array</param>
        /// <param name="Index">where to point</param>
        /// <param name="properyField">by which property to sort</param>
        private static void Max_Heapify<T>(RecordCollection region, int HeapSize, int Index, Func<SeismicRecord, T> properyField) where T : IComparable
        {
            int Left = (Index + 1) * 2 - 1;
            int Right = (Index + 1) * 2;
            int largest = 0;
            //sorting ascendingly
            if(region.SortOrder)
            {
                //when the left node from the middle is more than indexed node
                if (Left < HeapSize && properyField(region.Records[Left]).CompareTo(properyField(region.Records[Index])) > 0)
                    largest = Left; //set largest index
                else
                    largest = Index;
                if (Right < HeapSize && properyField(region.Records[Right]).CompareTo(properyField(region.Records[largest])) > 0)
                    largest = Right;
            }
            //sorting descendingly
            else
            {
                if (Left < HeapSize && properyField(region.Records[Left]).CompareTo(properyField(region.Records[Index])) < 0)
                    largest = Left;
                else
                    largest = Index;
                if (Right < HeapSize && properyField(region.Records[Right]).CompareTo(properyField(region.Records[largest])) < 0)
                    largest = Right;
            }

            if (largest != Index)
            {
                SeismicRecord temp = region.Records[Index];
                region.Records[Index] = region.Records[largest];
                region.Records[largest] = temp;
                Max_Heapify(region, HeapSize, largest, properyField);
            }
        }
    }
}
