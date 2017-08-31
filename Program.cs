using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using CommandLine.Parsing;
namespace TextfileValidator
{
	class Program
	{
		static void Main(string[] args)
        {
            ValidateFile();
          //  MakeFile();
        }
        private static void MakeFile()
        {
            Console.WriteLine("Press any key to starting to write file");
            Console.ReadLine();
           
                string sInt = "2785443";
                string sBigInt = "987654321987";
                string sDecimal = "345.452334";
                string sChar = "Y";
                string sVarchar = "eric the man";
                string sDate = "07/07/2019";
                string sTimestamp = "07/07/2019";
                StringBuilder line = new StringBuilder();
                
                line.Append(sInt+"|"+sBigInt+"|"+sDecimal+"|"+sChar+"|"+sVarchar+"|"+
                    sDate+"|"+sTimestamp);

            using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(@"C:\src\validator\validator\DataFiles\out\eSample.txt", true))
            {

                for (int i = 0; i < 30000000; i++)
                {
                    string sline = line.ToString();

                    file.WriteLine(sline + "|" + i);
                    if(i % 1000000 == 0)
                    {
                        Console.Write("\rLine Count {0}", i);
                    }
                }
            }
            Console.WriteLine();
            Console.WriteLine("done writing");
            Console.ReadLine();


        }
        private static bool eValidate(string item, string type)
        {
            Validator v = new Validator();
            var result = v.IsValid(item, new ItemDefintion(type));
            return result;
        }
        private static void ValidateFile()
        {
            Validator v = new Validator(); System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            var options = new Options();
            //if (CommandLine.Parser.Default.ParseArguments(args, options))
            //{

            timer.Start();
            Console.WriteLine("reading file {0} delimiting by {1}", options.InputFile, Char.Parse(options.Delimiter));
            Console.WriteLine();
            string line; string[] items; string[] types; Int64 lineCount = 0; Int64 itemCount = 0; ItemDefintion def = new ItemDefintion();
            int trues = 0; int falses = 0; bool result; TimeSpan ts = new TimeSpan();
            //StreamReader r = new StreamReader(@"C:\Projects\Playground\Eric.Scott\TextfileValidator\bin\Debug\sampleDatafile.txt");
            StreamReader r2 = new StreamReader(options.DefinitionFilePath);
            string line2 = r2.ReadLine();
            r2.Close();
            types = line2.Split(new char[] { Char.Parse(options.Delimiter) });
            int numTypes = types.Count();
            StreamReader r = new StreamReader(options.InputFile);
            
            int iStartRow;
            if (Int32.TryParse(options.StartRow, out iStartRow))
            {
                while (iStartRow > 0)
                {
                    r.ReadLine();
                    iStartRow--;
                }
            }

            Console.WriteLine("processor count {0}:", Environment.ProcessorCount);
            TimeSpan ets = new TimeSpan(0, 0, 0, 0, 1);
            Console.WriteLine("there are {0} ticks per millisection", ets.Ticks);
            int proc = Environment.ProcessorCount;
           
            while ((line = r.ReadLine()) != null)
            {
                lineCount++;
                items = line.Split(new char[] { Char.Parse(options.Delimiter) });
                if(items.Count() != numTypes)
                {
                    Console.WriteLine("ERROR:  expected more or less items in line # {0}", lineCount);
                }

                for (int i = 0; i < items.Count(); i++)
                {
                    def.Type = types[i];
                    result = v.IsValid(items[i], def); // takes about 150 ticks to do this.  threading overhead will not speed ths up
                    result = true;
                    if (result)
                        trues++;
                    else
                    {
                        Console.WriteLine("failure:  row #{0}  item#{1} value: {2}  not a {3}", lineCount, i, items[i], types[i]);
                        Console.WriteLine("the line is");
                        Console.WriteLine(line);//r.Close();
                        break;
                        falses++;
                    }
                    //Console.WriteLine("Item: {0} validation test is: {1} for type {2}", items[i], v.IsValid(items[i], def), types[i]);
                    itemCount++;
                }

                if (lineCount % 1000000 == 0)
                {
                    ts = timer.Elapsed;
                    Console.Write("\rLine Count {0}:  Item Count {1}: Passes: {2} Failes: {3}  Run time {4} Avg/line {5}", lineCount, itemCount, trues.ToString(), falses.ToString(),
                        String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds), ts.Ticks / lineCount);
                   
                }
            }
            timer.Stop();
            r.Close();
            Console.WriteLine();
            Console.Write("Validation completed:");
            Console.WriteLine("Line Count {0}:  Item Count {1}: Passes: {2} Failes: {3}  Run time {4}", lineCount, itemCount, trues.ToString(), falses.ToString(),
                        String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds));
            //}
            //else
            //{
            //	Console.WriteLine("you are missing required arguments");
            //}
            //run();
            Console.ReadLine();
        }

        public static void run()
		{
			Validator v = new Validator();
			ItemDefintion id = new ItemDefintion() { Type = "int",  Precision = "4" };
			Console.WriteLine("validation answer is {0}",v.IsValid("3", id));

		}
	}


    class Options
    {
        [Option('f', "datafilepath", Required = true, HelpText = "The TEXT data file to be validated")]
        public string InputFile { get; set; }

        [Option('d', "defintionfilepath", Required = true, HelpText = "the definition file to be validated")]
        public string DefinitionFilePath { get; set; }

        [Option('r', "startrow", Required = false, HelpText = "What row of the file does the data start on.")]
        public string StartRow { get; set; }

        [Option('s', "delimiter", Required = true, HelpText = "the Delimiter or separator in the file. Note | need to be in quotes")]
        public string Delimiter { get; set; }

        //[Option('v', "verbose", DefaultValue = true, HelpText = "Prints all the messages to standard output.")]
        //public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("<>", "<>"),
                Copyright = new CopyrightInfo("<>", 2017),
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true
            };

            help.AddPreOptionsLine("<>");
            help.AddPreOptionsLine("Usage: app -pSomeone");
            help.AddOptions(this);
            return help;


        }
    }


}

