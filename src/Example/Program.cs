using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ShellOut;

namespace Example
{
    using static Shell;

    public static class Program
    {
        public static void Main()
        {
            RunExample().Wait();
        }

        private static async Task RunExample()
        {
            var one = Run("ping", "localhost") | Run("findstr", "time");

            {
                Console.WriteLine("Testing ping...");
                await one.Execute();
            }


            // we can construct a pipeline. this is a first-class object so you can pass it around
            // and running it is reentrant
            var pipeline = Run("ping", "localhost") | Run("findstr" /* grep */, "time") | Run("findstr", "1");

            {
                Console.WriteLine("Printing to stdout:");
                Console.WriteLine("Pipeline is: {0}", pipeline);
                // by default it will write to stdout:
                await pipeline.Execute();
            }

            {
                var filename = "file.txt";
                Console.WriteLine("Printing to '{0}':", filename);
                // we can write to a file:
                var redirectToFile = pipeline > filename;
                Console.WriteLine("Pipeline is: {0}", redirectToFile);
                await redirectToFile.Execute();

                Console.WriteLine(File.ReadAllText(filename));
            }

            {
                var filename = "file2.txt";
                using (var fs = File.OpenWrite(filename))
                {
                    Console.WriteLine("Printing to '{0}'", filename);

                    // or a filestream:
                    var redirectToFileStream = pipeline > fs;
                    Console.WriteLine("Pipeline is: {0}", redirectToFileStream);
                    await redirectToFileStream.Execute();
                }
                Console.WriteLine(File.ReadAllText(filename));
            }

            using (var ms = new MemoryStream())
            {
                Console.WriteLine("Printing to MemoryStream:");

                // or indeed, *any* stream:
                var redirectToMemoryStream = pipeline > ms;
                Console.WriteLine("Pipeline is: {0}", redirectToMemoryStream);
                await redirectToMemoryStream.Execute();

                Console.WriteLine(Encoding.UTF8.GetString(ms.ToArray()));
            }

            using (var msIn = new MemoryStream(Encoding.UTF8.GetBytes("hello\nworld")))
            using (var msOut = new MemoryStream())
            {
                Console.WriteLine("Grepping \"hello\\nworld\" for \"or\":");

                // a demo of reading from and writing to memory streams at the same time, using an external program as a filter:

                var grep = Run("findstr", "or") < msIn > msOut;
                Console.WriteLine("Pipeline is: {0}", grep);
                await grep.Execute();

                Console.WriteLine(Encoding.UTF8.GetString(msOut.ToArray()));
            }

            Console.WriteLine("Done");
        }
    }
}
