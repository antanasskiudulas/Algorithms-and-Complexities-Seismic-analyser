using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeismicAnalysis
{
    /// <summary>
    /// Contains all the search algorithms utilized across the project
    /// </summary>
    class SearchAlgorithms
    {
        /// <summary>
        /// BinarySearch looks for matching records in the sorted array and prints all matching records
        /// </summary>
        /// <typeparam name="T">Generic type for the TResult in targetProperty</typeparam>
        /// <typeparam name="F">Generic type for the key that will be searched (Type T == Type F in this program)</typeparam>
        /// <param name="key">The reference to generic key type</param>
        /// <param name="region">Which array the operation to be performed on</param>
        /// <param name="foundVals">Reference to array outside the method to store matched keys</param>
        /// <param name="targetProperty">Property that the search will be conducted on (month, day etc)</param>
        /// <param name="left">Lowest index boundry in the array</param>
        /// <param name="right">Highest index boundry in the array</param>
        private static void BinarySearch<T,F>(F key, int left, int right, 
            ref RecordCollection region, ref RecordCollection foundVals, Func<SeismicRecord, T> targetProperty) 
            where T : IComparable
            where F : IComparable
        {
            //during recursion, if left boundry exceeds right, return void
            if (left > right)
                return;
            int mid = (left + right) / 2; //calibrating current middle position in the array
            if (region.SortOrder == true)//checking the order in which the array is sorted in ascended way
            {
                if (key.CompareTo(targetProperty(region.Records[mid])) < 0)// when key is < mid record's key
                    BinarySearch(key, left, mid - 1, ref region, ref foundVals, targetProperty);//call with updated right boundry
                else if (key.CompareTo(targetProperty(region.Records[mid])) > 0)// when key is > mid record's key
                    BinarySearch(key, mid + 1, right, ref region, ref foundVals, targetProperty);//call with updated left boundry
            }
            else//when the array is sorted in descending order
            {
                //same operation as above, but changed boundries to account for descending order
                if (key.CompareTo(targetProperty(region.Records[mid])) < 0)
                    BinarySearch(key, mid + 1, right, ref region, ref foundVals, targetProperty);
                else if (key.CompareTo(targetProperty(region.Records[mid])) > 0)
                    BinarySearch(key, left, mid - 1, ref region, ref foundVals, targetProperty);
            }
            //when the key is matched
            if (key.Equals(targetProperty(region.Records[mid])))
            {
                //add record to the array of matched keys
                foundVals.Records.AddRecord(region.Records[mid]);
                //branch out and search for other values in both halves of current mid record (in case they're between the current matched record)
                BinarySearch(key, left, mid - 1, ref region, ref foundVals, targetProperty);
                BinarySearch(key, mid + 1, right, ref region, ref foundVals, targetProperty);
            }
        }
        /// <summary>
        /// Helper method to print out all the found results in the BinarySearch
        /// </summary>
        /// <typeparam name="T">Generic type for the TResult in targetProperty</typeparam>
        /// <typeparam name="F">Generic type for the key that will be searched (Type T == Type F in this program)</typeparam>
        /// <param name="key">The reference to generic key type</param>
        /// <param name="region">Which array the operation to be performed on</param>
        /// <param name="targetProperty">Property that the search will be conducted on (month, day etc)</param>
        /// <param name="left">Lowest index boundry in the array</param>
        /// <param name="right">Highest index boundry in the array</param>
        public static void BinarySearchAll<T, F>(F key, int left, int right,
            ref RecordCollection region, Func<SeismicRecord, T> targetProperty)
            where T : IComparable
            where F : IComparable
        {
            //array to capture the matched keys from the binary search
            RecordCollection foundVals = new RecordCollection();
            BinarySearch(key, left, right, ref region, ref foundVals, targetProperty);
            //if the array of found records is not empty
            if (foundVals.Records.currentCapacity != 0)
            {
                //if the record should be displayed with the corresponding fields
                if(region.CorrespondingFields == true)
                    foundVals.Records.ListRecords();
                else//if only the value for the record should be displaye
                {
                    //capturing the name of the lambda parameter in the delegate to determine which record field's format to return the key in
                    string paramName = targetProperty.Method.GetParameters()[0].Name;
                    //setting the string to the formatted key
                    string formatOutput = RecordCollection.formatPrinter(paramName, foundVals.Records[0], targetProperty);
                    //for each record present in foundVals
                    for (int i = 0; i < foundVals.Records.currentCapacity; i++)
                    {
                        Console.WriteLine(formatOutput);//output the record
                    }
                }
            }
            else //if the array is emptry, it means the BinarySearch yielded no results
                Console.WriteLine($"'{key}' was not found among the records!");

            //search yield information for the key
            Console.WriteLine($"Found records for '{key}' : {foundVals.Records.currentCapacity} records");
        }
    }
}
