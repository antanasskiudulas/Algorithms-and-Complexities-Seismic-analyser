using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeismicAnalysis
{
    class MinMax
    {
        /// <summary>
        /// Linearly goes through all records looking for the highest and lowest record for given target property
        /// </summary>
        /// <typeparam name="T">Generic type for compiler to infer for the delegate</typeparam>
        /// <param name="region">Region's records to search through</param>
        /// <param name="targetProperty">for a property to find min max versions</param>
        public static void FindMinMax<T>(ref RecordCollection region, Func<SeismicRecord, T> targetProperty) where T : IComparable
        {
            //seed values to compare against
            T min = targetProperty(region.Records[0]);
            T max = targetProperty(region.Records[0]);
            //Indexes to retrieve the max and min records when the search is done
            int minIndex = 0; int maxIndex = 0;

            for (int i = 0; i < region.Records.currentCapacity; i++)
            {
                //when the current min is bigger than the next value in the array
                if (min.CompareTo(targetProperty(region.Records[i])) > 0)
                {
                    //set the current min to that lower value
                    min = targetProperty(region.Records[i]);
                    maxIndex = i; //record the most up-to-date min record's position
                }
                //when the current max is lower than the next value in the array
                else if (max.CompareTo(targetProperty(region.Records[i])) < 0)
                {
                    //set the current max to higher value
                    max = targetProperty(region.Records[i]);
                    minIndex = i;//record the most up-to-date max record's position
                }
            }

            //informs user for which parameter the min max request was and tells which column to look for.
            string paramName = targetProperty.Method.GetParameters()[0].Name;
            string maxOutput = RecordCollection.formatPrinter(paramName, region.Records[minIndex], targetProperty);
            string minOutput = RecordCollection.formatPrinter(paramName, region.Records[maxIndex], targetProperty);
            Console.WriteLine($"Minimum Value for {paramName.ToUpper()}:\n{minOutput}\n" +
            $"Maximum Value for {paramName.ToUpper()}:\n{maxOutput}");
        }
    }
}
