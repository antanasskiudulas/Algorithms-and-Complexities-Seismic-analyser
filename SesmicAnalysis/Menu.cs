using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Diagnostics;

namespace SeismicAnalysis
{
    /// <summary>
    /// To make the code more managable, it makes to seperate the multi-level menu's from main() to it's own class.
    /// The main job of this class is to carry out user choices
    /// </summary>
    static class Menu
    {
        /// <summary>
        /// The root menu is gateway to other sub-menu's depending on the path chosen by the user in the root menu.
        /// </summary>
        static public void RootMenu()
        {
            int userInput;
            do
            {
                //clearing the console when making back to root
                Console.Clear();
                Console.WriteLine("Seismic Data Manager - Antanas Skiudulas" +
                    "\nInstructions: The application is navigated using numbers i.e. to select first option, type 1.");
                Console.WriteLine("Please select which region you want to analyse" +
                    "\n1)Region 1" +
                    "\n2)Region 2" +
                    "\n3)Region 1 & 2"+
                    "\n4)Quit");
                userInput = short.Parse(Console.ReadLine());
                //since the choice here are regions, it makes sense to instantiate and initialize in the cases selected
                RecordCollection tempReg = new RecordCollection();
                RecordCollection tempReg2 = new RecordCollection();
                //Initializes certain region(s) depending on the input
                switch (userInput)
                {
                    case 1:
                        tempReg.InitializeCollections(1);
                        FieldMenu(tempReg);
                        break;
                    case 2:
                        tempReg2.InitializeCollections(2);
                        FieldMenu(tempReg2);
                        break;
                    case 3:
                        //intializing all these objects.
                        tempReg.InitializeCollections(1);
                        tempReg2.InitializeCollections(2);
                        //joining the regions into one array.
                        tempReg.JoinArrays(tempReg2);
                        FieldMenu(tempReg);
                        break;
                    case 4: Environment.Exit(0); break;
                    default:
                         Console.WriteLine("The region you've entered doesn't exist!");
                        break;
                }
            } while (userInput != 4); //loops until the input is a correct case or intention to quit
        }
        /// <summary>
        /// Once the region is initialized, then you want to choose region specific fields to operate upon
        /// </summary>
        /// <param name="regionData">The initialized instance of the selected region</param>
        static private void FieldMenu(RecordCollection regionData)
        {
            int userInput;
            do
            {
                Console.WriteLine("Which field do you want to perform an operation on?" +
                    "\n1)Years" +
                    "\n2)Months" +
                    "\n3)Days" +
                    "\n4)Times" +
                    "\n5)Magnitude" +
                    "\n6)Latitude" +
                    "\n7)Longitude" +
                    "\n8)Depth" +
                    "\n9)Region" +
                    "\n10)IRIS_ID" +
                    "\n11)Timestamp" +
                    "\n12)List current records"+
                    "\n13)Go back");
                userInput = short.Parse(Console.ReadLine());
                switch (userInput)
                {
                    //selected field should refer to OperationMenu with lambda expression to satisfy the delegate condition and the region's data
                    case 1: OperationMenu(regionData, year => year.Year); break;
                    case 2: OperationMenu(regionData, month => int.Parse(month.Month[0])); break; //month integer equivalents (for sorting and searching)
                    case 3: OperationMenu(regionData, day => day.Day); break;
                    case 4: OperationMenu(regionData, time => time.Time); break;
                    case 5: OperationMenu(regionData, magnitude => magnitude.Magnitude); break;
                    case 6: OperationMenu(regionData, latitude => latitude.Latitude); break;
                    case 7: OperationMenu(regionData, longitude => longitude.Longitude); break;
                    case 8: OperationMenu(regionData, depth => depth.Depth); break;
                    case 9: OperationMenu(regionData, region => region.Region); break;
                    case 10: OperationMenu(regionData, iris_id => iris_id.Iris_id); break;
                    case 11: OperationMenu(regionData, timestamp => timestamp.Timestamp); break;
                    case 12: regionData.Records.ListRecords(); break;//if the user wants to view the current state of the records before performing any operation
                    case 13: break;
                    default:
                    Console.WriteLine("Such field option doesn't exist!");
                    break;
                }
            } while (userInput != 13); //loops until the input is a correct case or intention to quit
        }
        /// <summary>
        /// The operation menu contains all the operations that you can perform on a given field
        /// </summary>
        /// <typeparam name="T">To reduce redundancy, I've specified a generic type method to ensure properties of the SesmicRecord
        /// such as int, double, TimeSpan are inferred by the compiler</typeparam>
        /// <param name="region">region data to operate on</param>
        /// <param name="propertyField">A selected property (column) to perform the required operations on</param>
        static private void OperationMenu<T>(RecordCollection region, Func<SeismicRecord, T> propertyField) where T : IComparable
        {
            int userInput;
            do
            {
                Console.WriteLine("Which of the following you want to perform?" +
                    "\n1)Heap Sort and display corresponding values" +
                    "\n2)Binary Search by a field" +
                    "\n3)Find Max and minimum value" +
                    "\n4)Go back!");
                userInput = short.Parse(Console.ReadLine());
                //paramName; a name of currently passed propertField parameter i.e. month => month.Month is month
                string paramName = propertyField.Method.GetParameters()[0].Name;
                switch (userInput)
                {
                    case 1:
                        //sort in the chosen order
                        Console.WriteLine("1)Ascending" +
                                        "\n2)Descending");
                        int sortInput = short.Parse(Console.ReadLine());
                        switch (sortInput)
                        {
                            //sets the region's desired sort order before sorting
                            case 1: region.SortOrder = true; break;
                            case 2: region.SortOrder = false; break;
                            default: Console.WriteLine("No such sorting option exists!"); break;
                        }
                        //when order is set, sort the region records by the property
                        SortAlgorithms.HeapSort(region, propertyField);
                        region.Records.ListRecords();//list the records when it's sorted
                        Console.WriteLine("There are currently: {0} records\n", region.Records.currentCapacity);
                        break;
                    case 2:
                        //how the found records to be displayed
                        Console.WriteLine("1)Display corresponding values" +
                                        "\n2)Display only selected field");
                        int searchInput = short.Parse(Console.ReadLine());
                        switch (searchInput)
                        {
                            //sets the decision on how to display the records
                            case 1: region.CorrespondingFields = true; break;
                            case 2: region.CorrespondingFields = false; break;
                            default: Console.WriteLine("No such sorting option exists!"); break;
                        }
                        Console.WriteLine($"Enter your search value for {paramName}:");
                        region.SortOrder = true;
                        SortAlgorithms.HeapSort(region, propertyField);
                        //calls a generic method for the users input
                        TypeSafeSearch(region, propertyField);
                        break;
                    case 3:
                        //the static method for finding minimum maximum for current field
                        MinMax.FindMinMax(ref region, propertyField);
                        break;
                    case 4: break;
                    default:
                         Console.WriteLine("Such operation doesn't exist!");
                        break;
                }
            } while (userInput != 4); //loops until the input is a correct case or intention to quit
        }

        /// <summary>
        /// Decide upon which type of input to ask user based on the type of property being passed as arg
        /// </summary>
        /// <typeparam name="T">Generic type for the delegate's 'out TResult'</typeparam>
        /// <param name="region">Region record data</param>
        /// <param name="propertyField">record's property the search operation to be performed</param>
        static public void TypeSafeSearch<T>(RecordCollection region, Func<SeismicRecord, T> propertyField) 
            where T : IComparable
        {
            //paramName; a name of currently passed propertField parameter i.e. month => month.Month is month
            string paramName = propertyField.Method.GetParameters()[0].Name;
            //retrieves the type from a selected record property
            Type propertyType = propertyField(region.Records[0]).GetType();
            //if the current property integer type, then the user is searching by either year, month or day etc.
            if (propertyType == typeof(int))
            {
                int key;
                //months can be searched by string name or integer equivalent, so it makes sense to see if the current parameter is of name month
                if (paramName == "month")
                {
                    //then notify user of possible search inputs
                    Console.WriteLine("You're searching for month; you can use numbers (1-12) or words like june");
                    //a variable to test what type of input the user provided
                    string tempInput = Console.ReadLine();
                    //if it's true that you can't parse the user input as an integer, it's a string
                    if (!int.TryParse(tempInput, out key))
                        //take the string equivalent for the month input and parse it to months integer for the key
                        key = DateTime.ParseExact(tempInput, "MMMM", CultureInfo.InvariantCulture).Month;
                }
                else
                {
                    //if input is parasable as an int, then parse it as ant integer and assign it to the key
                    Console.WriteLine($"You're searching for {paramName}; expected input is integer");
                    key = int.Parse(Console.ReadLine());
                }
                //after acquiring the key from the condition statement, it's passed to the search algorithm.
                SearchAlgorithms.BinarySearchAll(key, 0, region.Records.currentCapacity - 1, ref region, propertyField);
            }
            //when the property is of string type
            else if (propertyType == typeof(string))
            {
                //ask user for string input for the key
                Console.WriteLine($"You're searching for {paramName}; expected input is string (lower or upper-case)");
                string key = Console.ReadLine().ToUpper();
                SearchAlgorithms.BinarySearchAll(key, 0, region.Records.currentCapacity - 1, ref region, propertyField);
            }
            //when the property is of TimeSpan, it's clear the user wants to search by the Time property
            else if (propertyType == typeof(TimeSpan))
            {
                //Parse the the input as a TimeSpan type
                Console.WriteLine($"You're searching for {paramName}; expected input is HH:MM:SS");
                TimeSpan key = TimeSpan.Parse(Console.ReadLine());
                SearchAlgorithms.BinarySearchAll(key, 0, region.Records.currentCapacity - 1, ref region, propertyField);
            }
            //when property of type double (magnituded, latitude etc)
            else if (propertyType == typeof(double))
            {
                //Parse the input as a double
                Console.WriteLine($"You're searching for {paramName}; expected input is double (DDD.DDD or -DDD.DDDD)");
                double key = double.Parse(Console.ReadLine());
                SearchAlgorithms.BinarySearchAll(key, 0, region.Records.currentCapacity - 1, ref region, propertyField);
            }

        }
    }
}
