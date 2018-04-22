using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeismicAnalysis
{
    //type safe-array's to mimic Lists, and further extend the reusability of the code in the project.
    public class ArrayList<T>
    {
        private T[] Records = new T[0];
        public int currentCapacity;
        private int maxCapacity;

        public void AddRecord(T record)
        {
            //checking to see if current capacity approached the limit!
            if(currentCapacity == maxCapacity)
            {
                //If max capacity is 0, make it 4, else double it (list behaviour)
                maxCapacity = maxCapacity == 0 ? 4 : maxCapacity * 2;
                //creating temp array with resized array
                T[] copy = new T[maxCapacity];
                //Copies current records into temp resized array at different capacity
                Array.Copy(Records, copy, currentCapacity);
                //now sets the main record holding array to the new double-sized array with same records
                Records = copy;
            }
            Records[currentCapacity] = record;
            currentCapacity++;
        }

        public T this[int i]//atribute index for the method (no need to include it in class)
        {
            //when retrieving record, if the index is out of bounds, an exception should be thrown.
            //this operation is performed when tretrieving or setting a record
            get
            {
                if (i < 0 || i >= currentCapacity)
                    throw new IndexOutOfRangeException();
                //else return the record
                return Records[i];
            }
            set
            {
                if (i < 0 || i >= currentCapacity)
                    throw new IndexOutOfRangeException();
                //else set the record to the array
                Records[i] = value;
            }
        }

        /// <summary>
        /// simple method to print out all the values in the region's array
        /// </summary>
        public void ListRecords()
        {
            Console.WriteLine("{0,4} {1,10} {2,2} {3,9} {4,9} {5,8} {6,9} {7,7} {8,29} {9,7} {10,8}",
                "Year", "Month", "Day", "Time", "Magnitude", "Latitude", "Longitude", "Depth", "Region", "Iris_id", "Timestamp");
            foreach (T entry in Records)
            {
                if (entry != null) //to avoid the method listing empty arrays
                    Console.WriteLine($"{entry}");
            }
            Console.WriteLine();
        }
    }
}
