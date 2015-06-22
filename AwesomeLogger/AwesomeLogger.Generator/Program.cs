using System;
using System.IO;
using System.Text;

namespace AwesomeLogger.Generator
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please specify file path to generate log.");
                Console.ReadKey();
                return 1;
            }

            var filePath = args[0];

            Console.WriteLine("AwesomeLogger Generator: {0}", filePath);
            Console.WriteLine("Start typing then press Enter to commit...");

            using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                using (var file = new StreamWriter(fileStream, Encoding.UTF8, 1024))
                {
                    // Reading input until empty line
                    string input;
                    while (!string.IsNullOrEmpty(input = Console.ReadLine()))
                    {
                        // Writing to log and flushing
                        file.WriteLine(input);
                        file.Flush();
                    }
                }
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            return 0;
        }
    }
}