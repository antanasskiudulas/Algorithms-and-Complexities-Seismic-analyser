using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace SeismicAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            //repeat until a valid option is selected
            bool menuLoop = true;
            while (menuLoop)
            {
                //wrap all calls by this method into try/catch block
                try
                {
                    Menu.RootMenu();
                    menuLoop = false;
                }
                catch (FormatException e)//most common errors to occur are format exceptions in this program
                {
                    Console.WriteLine($"Seems like your input wasn't of the type that's expected: {e.Message}");
                    Console.ReadKey();
                }
                catch (Exception e)//everything else
                {
                    Console.WriteLine(e.Message);
                    Console.ReadKey();
                }
            }
        }
    }
    /// <summary>
    /// A definition for a single Sesmic record
    /// </summary>
    public class SeismicRecord
    {
        //the expected properties for a single record from the sesmic data
        private int year;
        public int Year
        {
            get { return year; }
            set { year = value; }
        }
        private string[] month = new string[2];//store month's number equivelent(for sorting) and the month name
        public string[] Month
        {
            get { return month; }
            set { month = value; }
        }
        private int day;
        public int Day
        {
            get { return day; }
            set { day = value; }
        }
        private TimeSpan time;
        public TimeSpan Time
        {
            get { return time; }
            set { time = value; }
        }
        private double magnitude;
        public double Magnitude
        {
            get { return magnitude; }
            set { magnitude = value; }
        }
        private double latitude;
        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }
        private double longitude;
        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }
        private double depth;
        public double Depth
        {
            get { return depth; }
            set { depth = value; }
        }
        private string region;
        public string Region
        {
            get { return region; }
            set { region = value; }
        }
        private int iris_id;
        public int Iris_id
        {
            get { return iris_id; }
            set { iris_id = value; }
        }
        private int timestamp;
        public int Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        public override string ToString()
        {
            //when the object is printed, this line of string will be returned. $ is faster method than the equivelent.
            return string.Format("{0,4} {1,11} {2,2} {3,9} {4,9:N3} {5,8:N3} {6,9:N3} {7,7:N3} {8,29} {9,7} {10,8}", 
                Year,Month[1],Day,Time,Magnitude,Latitude,Longitude,Depth,Region,Iris_id,Timestamp );
        }
    }
    /// <summary>
    /// A definition for collection of sesmic records onto which i want to perform operations
    /// extra properties in this class are there to alter search and sort algorithms
    /// </summary>
    public class RecordCollection
    {
        //custom created data structure from ArrayList class
        private ArrayList<SeismicRecord> records = new ArrayList<SeismicRecord>();
        public ArrayList<SeismicRecord> Records
        {
            get { return records; }
            set { records = value; }
        }
        //order in which the records to be displayed when sorted (ascending = true, else descending)
        private bool sortOrder;
        public bool SortOrder
        {
            get { return sortOrder; }
            set { sortOrder = value; }
        }
        //choice between displaying corresponding fields to search field (true) or just displaying the values
        private bool correspondingFields;
        public bool CorrespondingFields
        {
            get { return correspondingFields; }
            set { correspondingFields = value; }
        }

        /// <summary>
        /// Initializes the ArrayList from the I/O files, does so by reading selected region files into string[] dictionary,
        /// then loops through each file name string key, access string[] value by row index and set the temp record through setter dictionary
        /// once looped though each file, the temp record is fully initialized, so it's added to the array
        /// </summary>
        /// <param name="dataSet">Allows user to choose which region to initialize</param>
        public void InitializeCollections(int dataSet)
        {
            /* a look up dictionary for the initializer
            Key is the parsed variable from the txt file, Action is a simple delegate which takes 2 arguments;
            SesmicRecord and String to perform property assignment operation */
            var setters = new Dictionary<string, Action<SeismicRecord, string>>
            {
                /*using lambda expressions to assign values to keys.
                in parenthesis i call for anonymoius lambda operator (type SesmicRecord) with a value argument */
                ["Year"] = (record, value) => record.Year = int.Parse(value), //in this half It initialize the corresponding field of the anonymous property
                ["Month"] = (record, value) => {
                    //Parses month strings to their integer equivalent into first array sloth for month to sort of give integer weight for the associated string
                    record.Month[0] = DateTime.ParseExact(value.TrimEnd(' '), "MMMM", CultureInfo.InvariantCulture)
                    .Month.ToString();//trimmed the end spaces, otherwise it won't recognize it's a month
                    record.Month[1] = value.TrimEnd(' ').ToUpper();//sets the string equivalent to the month
                },
                ["Day"] = (record, value) => record.Day = int.Parse(value),
                ["Time"] = (record, value) => record.Time = DateTime.ParseExact(value, "HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay,
                ["Magnitude"] = (record, value) => record.Magnitude = double.Parse(value),
                ["Latitude"] = (record, value) => record.Latitude = double.Parse(value),
                ["Longitude"] = (record, value) => record.Longitude = double.Parse(value),
                ["Depth"] = (record, value) => record.Depth = double.Parse(value),
                ["Region"] = (record, value) => record.Region = value,
                ["IRIS_ID"] = (record, value) => record.Iris_id = int.Parse(value),
                ["Timestamp"] = (record, value) => record.Timestamp = int.Parse(value),
                //conversion of values is necessary as the stream reads everything as a string.
            };

            DirectoryInfo dataColumns = new DirectoryInfo(@"SesmicData\");
            FileInfo[] Files = dataColumns.GetFiles($"*{dataSet}.txt");
            Console.WriteLine($"Initializing region {dataSet}\n");
            Dictionary<string, string[]> lines = new Dictionary<string, string[]>();

            /*reading files is a very slow operation, so it's best to read all the text from the file and put it in an array
            the string array can later be indexed by row to create a record which will be added to an array */
            foreach (FileInfo file in Files)
            {
                lines.Add(file.Name, File.ReadAllLines(file.FullName));
            }

            int row = 0;
            /*the condition compares current row with the overall length of string array entries
            array is quadratic, so it's safe to assume all the files will have same (n) entries*/
            while (row < lines[$"Day_{dataSet}.txt"].Length)
            {
                //temprory record to store values from string arrays
                SeismicRecord temp = new SeismicRecord();
                //looping through each key in the dictionary
                foreach (KeyValuePair<string,string[]> file in lines)
                {
                    /* retrieves the file name index (in this case it's to get the length) at a point _1 or _2 is deteccted
                    Day_1 will return pos. 3 (the pos. at which $"_{dataSet}" detected is at 3 */
                    int pos = file.Key.IndexOf($"_{dataSet}");
                    //delegate 
                    Action<SeismicRecord, string> fieldSetter;
                    //if the pos -1 then indexOf couldn't retrieve the position, or if trying to retrieve the said substring from setter returns false
                    if (pos < 0 || !setters.TryGetValue(file.Key.Substring(0, pos), out fieldSetter))
                    {
                        //if either condition is true, skip this iteration
                        continue;
                    }
                    else
                        //if the substring is retrievable initialize the temp with the value in a current indexed row
                        fieldSetter(temp, file.Value[row]);
                }
                /*once the foreach loop is completed, the temp will have all of its fields initialized
                 so the next step is to add the temp to the array and increment the index for next row*/
                Records.AddRecord(temp);
                row++;
            }
        }

        /// <summary>
        /// To join the current instance of Record collection to another instance
        /// </summary>
        /// <param name="target">Target array you wish to combine with the source</param>
        public void JoinArrays(RecordCollection target)
        {
            //loops through each record in the target and adds it to the current records.
            for (int i = 0; i < target.Records.currentCapacity; i++)
            {
                Records.AddRecord(target.Records[i]);
            }
            SortAlgorithms.HeapSort(this, c=>c.Timestamp);
        }
        public static string formatPrinter<T>(string paramName, SeismicRecord record, Func<SeismicRecord,T> property)
        {
            //unique cases where formatting has to differ
            switch (paramName)
            {
                case "magnitude": return string.Format("{0:N3}", property(record));
                case "longitude": return string.Format("{0:N3}", property(record));
                case "latitude": return string.Format("{0:N3}", property(record));
                case "depth": return string.Format("{0:N3}", property(record));
                case "month": return string.Format("{0}", record.Month[1]);
                default: return string.Format("{0}", property(record));
            }
        }
    }
}
