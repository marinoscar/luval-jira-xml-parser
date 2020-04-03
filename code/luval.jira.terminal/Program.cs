using luval.jira.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace luval.jira.terminal
{
    class Program
    {
        /// <summary>
        /// Main entry point to the application
        /// </summary>
        /// <param name="args">Arguments</param>
        static void Main(string[] args)
        {
            /// Provides a way to parse the arguments <see cref="https://gist.github.com/marinoscar/d84265533b242a8a5e7eb74cdd50b7e5"/>
            var arguments = new ConsoleSwitches(args);

            RunAction(() =>
            {
                DoAction(arguments);
            }, true);
        }

        /// <summary>
        /// Executes an action on the application
        /// </summary>
        /// <param name="arguments"></param>
        static void DoAction(ConsoleSwitches arguments)
        {
            if (!arguments.SourceFile.Exists) throw new ArgumentException("File not provided");
            var xmlText = File.ReadAllText(arguments.SourceFile.FullName);
            xmlText = xmlText.Replace("&", "_");
            var xml = XElement.Parse(xmlText);
            var search = new Search(xml);
            var excelReport = new ExcelReport();;
            if (arguments.DestinationFile.Exists) arguments.DestinationFile.Delete();
            excelReport.DoReport(arguments.DestinationFile, search);
        }

        /// <summary>
        /// Runs the action and handles exceptions
        /// </summary>
        /// <param name="action">The action to execute</param>
        public static void RunAction(Action action, bool waitForKey = false)
        {
            var consoleTextColor = Console.ForegroundColor;
            try
            {
                action();
            }
            catch (Exception exception)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception);
                Console.WriteLine();
            }
            finally
            {
                Console.ForegroundColor = consoleTextColor;
                if (waitForKey)
                {
                    Console.WriteLine("Press any key to end");
                    Console.ReadKey();
                }
            }
        }
    }
}