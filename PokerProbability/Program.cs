using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerProbability
{
    class Program
    {
        static void Main()
        {
            int trialCount;
            bool saveToFile;
            bool saveProgress;

            #region Prompt: Save to file
            Console.WriteLine("Save to file? ([A]ll/[R]esult-only/[N]one)");
            while (true)
            {
                Console.Write(">> ");
                ConsoleKey key = Console.ReadKey().Key;
                Console.WriteLine();
                if (key == ConsoleKey.A)
                {
                    saveToFile = true;
                    saveProgress = true;
                    break;
                }
                else if (key == ConsoleKey.R)
                {
                    saveToFile = true;
                    saveProgress = false;
                    break;
                }
                else if (key == ConsoleKey.N)
                {
                    saveToFile = false;
                    saveProgress = false;
                    break;
                }
                else
                    Console.WriteLine("Invalid.");
            }
            #endregion

            #region Prompt: Trial count
            Console.WriteLine("Trial count");
            while (true)
            {
                Console.Write(">> ");
                string strTrialCount = Console.ReadLine();
                if (int.TryParse(strTrialCount, out trialCount) && trialCount > 0)
                    break;
                else
                    Console.WriteLine("Invalid value!");
            }
            #endregion

            Console.WriteLine("Thread mode ([S]ingle-threaded/[M]ulti-threaded");
            while (true)
            {
                Console.Write(">> ");
                ConsoleKey key = Console.ReadKey().Key;
                Console.WriteLine();
                if (key == ConsoleKey.S)
                {
                    Procedure.SingleThread(trialCount, saveToFile, saveProgress);
                    break;
                }
                else if (key == ConsoleKey.M)
                {
                    Procedure.MultiThread(trialCount, saveToFile, saveProgress);
                    break;
                }
                else
                    Console.WriteLine("Invalid key!");
            }
        }
    }
}
