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

            try
            {
                using (
                var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 1048576, FileOptions.Asynchronous))
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }

            

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            return 0;
        }
    }
}