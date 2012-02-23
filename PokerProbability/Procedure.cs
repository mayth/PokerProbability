using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerProbability
{
    static class Procedure
    {
        const int NumberOfHand = 5;

        public static void SingleThread(int trialCount, bool saveToFile, bool saveProgress)
        {
            int fileNumber = 0;

            /***** Hands count Initialize *****/
            Dictionary<Hand, int> handsCount = new Dictionary<Hand, int>();
            foreach (Hand h in Enum.GetValues(typeof(Hand)).Cast<Hand>())
                handsCount[h] = 0;

            /***** Card pile Initialize *****/
            List<Card> cards = new List<Card>();
            foreach (Suit s in Enum.GetValues(typeof(Suit)).Cast<Suit>())
                for (int i = 1; i <= 13; i++)
                    cards.Add(new Card(s, i));

            StringBuilder builder = new StringBuilder();
            int displayEvery = (int)(trialCount * 0.1);
            const int fileOutputEvery = 50000;
            bool alwaysDisplay = trialCount < 100;
            bool saveOnce = trialCount < fileOutputEvery;

            // Random
            Random rand = new Random();

            DateTime started = DateTime.Now;

            /***** Directory create *****/
            string directoryName = string.Empty;
            if (saveToFile)
            {
                directoryName = started.ToString("yyyyMMdd-HHmmss");
                System.IO.Directory.CreateDirectory(directoryName);
            }

            /***** Trial *****/
            for (int n = 0; n < trialCount; n++)
            {
                /***** Take 5 cards from pile *****/
                List<Card> hand = new List<Card>(NumberOfHand);
                for (int i = 0; i < NumberOfHand; i++)
                    hand.Add(cards[Math.Abs(rand.Next() % cards.Count)]);

                /***** Check hands *****/
                // sort by number
                hand.Sort((p, q) => p.Number - q.Number);

                // hand initialize
                Dictionary<Hand, bool> madeHand = new Dictionary<Hand, bool>();
                foreach (Hand h in Enum.GetValues(typeof(Hand)).Cast<Hand>())
                    madeHand[h] = false;

                // check - X pair/X of a kind/full-house
                // http://oshiete.goo.ne.jp/qa/619584.html
                List<int> checkTemp = new List<int>();
                for (int i = 0; i < 14; i++)
                    checkTemp.Add(0);
                hand.ForEach(c =>
                {
                    if (c.Number == 1)
                    {
                        checkTemp[0]++;
                        checkTemp[13]++;
                    }
                    else
                        checkTemp[c.Number - 1]++;
                }
                );
                if (checkTemp.Any(p => p == 4))
                    madeHand[Hand.FourOfAKind] = true;
                if (checkTemp.Any(p => p == 3))
                    madeHand[Hand.ThreeOfAKind] = true;
                if (checkTemp.Any(p => p == 2))
                    madeHand[Hand.OnePair] = true;
                if (madeHand[Hand.ThreeOfAKind] && madeHand[Hand.OnePair])
                {
                    madeHand[Hand.ThreeOfAKind] = false;
                    madeHand[Hand.OnePair] = false;
                    madeHand[Hand.Fullhouse] = true;
                }
                if (!madeHand[Hand.Fullhouse] && checkTemp.Take(13).Where(q => q == 2).Count() == 2)
                {
                    madeHand[Hand.TwoPair] = true;
                    madeHand[Hand.OnePair] = false;
                }

                // check straight
                int straightCount = 0;
                checkTemp.ForEach(c =>
                {
                    if (c == 1)
                        straightCount++;
                    else
                        straightCount = 0;
                    if (straightCount >= 5)
                        madeHand[Hand.Straight] = true;
                }
                );
                // check Flash/RoyalStraightFlash
                madeHand[Hand.Flash] = hand.All(c => c.Suit == hand[0].Suit);
                if (madeHand[Hand.Straight] && madeHand[Hand.Flash])
                {
                    madeHand[Hand.Straight] = false;
                    madeHand[Hand.Flash] = false;
                    madeHand[Hand.StraightFlash] = true;
                }
                if (madeHand[Hand.StraightFlash] && checkTemp[13] == 1)
                {
                    madeHand[Hand.StraightFlash] = false;
                    madeHand[Hand.RoyalStraightFlash] = true;
                }

                Hand result;
                if (madeHand.All(kv => kv.Value == false))
                    result = Hand.NoPair;
                else
                    result = madeHand.Where(kv => kv.Value == true).First().Key;

                handsCount[result]++;
                string handString = string.Empty;
                hand.ForEach(c => handString += "[" + c.ToString() + "]");
                string resultLine = string.Format("{0}\t{1}\t{2}", n, handString, result);
                builder.AppendLine(resultLine);

                if (alwaysDisplay || n % displayEvery == 0)
                {
                    DateTime lapTime = DateTime.Now;
                    Console.WriteLine(
                        "{0} " + Environment.NewLine
                        + "  time:{1} # elapsed:{2}", resultLine, lapTime.ToString("G"), (lapTime - started).ToString());
                }
                if (!saveOnce && n % fileOutputEvery == 0)
                {
                    if (saveProgress)
                        System.IO.File.WriteAllText(string.Format(directoryName + System.IO.Path.DirectorySeparatorChar + "pp{0}.log", fileNumber++), builder.ToString());
                    builder.Clear();
                }
            }

            DateTime finished = DateTime.Now;

            if (saveToFile && saveOnce)
            {
                System.IO.File.WriteAllText(string.Format(directoryName + System.IO.Path.DirectorySeparatorChar + "pp{0}.log", fileNumber++), builder.ToString());
                builder.Clear();
            }

            Console.WriteLine("Finished");
            string probabilities = string.Empty;
            foreach (var kv in handsCount)
                probabilities += string.Format("{0}\t{1}\t{2} {3}", kv.Key.ToString().PadRight(20), kv.Value, ((double)kv.Value / (double)trialCount).ToString("P10"), Environment.NewLine);
            Console.WriteLine(probabilities);
            if (saveToFile)
                System.IO.File.WriteAllText(directoryName + System.IO.Path.DirectorySeparatorChar + "ppResult.txt",
                    "<Single-threaded>" + Environment.NewLine
                    + "Trial count: " + trialCount + Environment.NewLine
                    + "Started Date/Time: " + started.ToString("G") + Environment.NewLine
                    + "Finished Date/Time:" + finished.ToString("G") + Environment.NewLine
                    + "Process time: " + (finished - started).ToString() + Environment.NewLine
                    + "-- result --" + Environment.NewLine
                    + probabilities);

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        public static void MultiThread(int trialCount, bool saveToFile, bool saveProgress)
        {
            int fileNumber = 0;

            #region Stand-by
            /***** Hands count Initialize *****/
            Dictionary<Hand, int> handsCount = new Dictionary<Hand, int>();
            foreach (Hand h in Enum.GetValues(typeof(Hand)).Cast<Hand>())
                handsCount[h] = 0;

            /***** Card pile Initialize *****/
            List<Card> cards = new List<Card>();
            foreach (Suit s in Enum.GetValues(typeof(Suit)).Cast<Suit>())
                for (int i = 1; i <= 13; i++)
                    cards.Add(new Card(s, i));

            StringBuilder builder = new StringBuilder();
            int displayEvery = (int)(trialCount * 0.1);
            const int fileOutputEvery = 50000;
            bool alwaysDisplay = trialCount < 100;
            bool saveOnce = trialCount < fileOutputEvery;

            // Random
            Random rand = new Random();

            DateTime started = DateTime.Now;

            /***** Directory create *****/
            string directoryName = string.Empty;
            if (saveToFile)
            {
                directoryName = started.ToString("yyyyMMdd-HHmmss");
                System.IO.Directory.CreateDirectory(directoryName);
            }
            #endregion

            object drawSync = new object();
            object resultSync = new object();

            /***** Trial *****/
            Parallel.For(0, trialCount, n =>
            {
                /***** Take 5 cards from pile *****/
                List<Card> hand = new List<Card>(NumberOfHand);
                lock (drawSync)
                    for (int i = 0; i < NumberOfHand; i++)
                        hand.Add(cards[Math.Abs(rand.Next() % cards.Count)]);

                /***** Check hands *****/
                // sort by number
                hand.Sort((p, q) => p.Number - q.Number);

                // hand initialize
                Dictionary<Hand, bool> madeHand = new Dictionary<Hand, bool>();
                foreach (Hand h in Enum.GetValues(typeof(Hand)).Cast<Hand>())
                    madeHand[h] = false;

                // check - X pair/X of a kind/full-house
                // http://oshiete.goo.ne.jp/qa/619584.html
                List<int> checkTemp = new List<int>();
                for (int i = 0; i < 14; i++)
                    checkTemp.Add(0);
                hand.ForEach(c =>
                {
                    if (c.Number == 1)
                    {
                        checkTemp[0]++;
                        checkTemp[13]++;
                    }
                    else
                        checkTemp[c.Number - 1]++;
                }
                );
                if (checkTemp.Any(p => p == 4))
                    madeHand[Hand.FourOfAKind] = true;
                if (checkTemp.Any(p => p == 3))
                    madeHand[Hand.ThreeOfAKind] = true;
                if (checkTemp.Any(p => p == 2))
                    madeHand[Hand.OnePair] = true;
                if (madeHand[Hand.ThreeOfAKind] && madeHand[Hand.OnePair])
                {
                    madeHand[Hand.ThreeOfAKind] = false;
                    madeHand[Hand.OnePair] = false;
                    madeHand[Hand.Fullhouse] = true;
                }
                if (!madeHand[Hand.Fullhouse] && checkTemp.Take(13).Where(q => q == 2).Count() == 2)
                {
                    madeHand[Hand.TwoPair] = true;
                    madeHand[Hand.OnePair] = false;
                }

                // check straight
                int straightCount = 0;
                checkTemp.ForEach(c =>
                {
                    if (c == 1)
                        straightCount++;
                    else
                        straightCount = 0;
                    if (straightCount >= 5)
                        madeHand[Hand.Straight] = true;
                }
                );
                // check Flash/RoyalStraightFlash
                madeHand[Hand.Flash] = hand.All(c => c.Suit == hand[0].Suit);
                if (madeHand[Hand.Straight] && madeHand[Hand.Flash])
                {
                    madeHand[Hand.Straight] = false;
                    madeHand[Hand.Flash] = false;
                    madeHand[Hand.StraightFlash] = true;
                }
                if (madeHand[Hand.StraightFlash] && checkTemp[13] == 1)
                {
                    madeHand[Hand.StraightFlash] = false;
                    madeHand[Hand.RoyalStraightFlash] = true;
                }

                Hand result;
                if (madeHand.All(kv => kv.Value == false))
                    result = Hand.NoPair;
                else
                {
                    result = madeHand.Where(kv => kv.Value == true).First().Key;
                    int x = madeHand.Where(kv => kv.Value == true).Count();
                    if (x != 1)
                        Console.WriteLine("duplicate");
                }

                string handString = string.Empty;
                hand.ForEach(c => handString += "[" + c.ToString() + "]");
                string resultLine = string.Format("{0}\t{1}\t{2}", n, handString, result);

                lock (resultSync)
                {
                    handsCount[result]++;
                    builder.AppendLine(resultLine);
                }

                if (alwaysDisplay || n % displayEvery == 0)
                {
                    Console.WriteLine(resultLine);
                    DateTime lapTime = DateTime.Now;
                    Console.WriteLine("  time:{0} # elapsed:{1}", lapTime.ToString("G"), (lapTime - started).ToString());
                }
                if (!saveOnce && n % fileOutputEvery == 0)
                {
                    lock (resultSync)
                    {
                        if (saveProgress)
                            System.IO.File.WriteAllText(
                                string.Format(directoryName + System.IO.Path.DirectorySeparatorChar + "pp{0}.log", fileNumber++), builder.ToString());
                        builder.Clear();
                    }
                }
            });

            DateTime finished = DateTime.Now;

            if (saveToFile && saveOnce)
            {
                System.IO.File.WriteAllText(string.Format(directoryName + System.IO.Path.DirectorySeparatorChar + "pp{0}.log", fileNumber++), builder.ToString());
                builder.Clear();
            }

            Console.WriteLine("Finished");
            string probabilities = string.Empty;
            foreach (var kv in handsCount)
                probabilities += string.Format("{0}\t{1}\t{2} {3}", kv.Key.ToString().PadRight(20), kv.Value, ((double)kv.Value / (double)trialCount).ToString("P10"), Environment.NewLine);
            Console.WriteLine(probabilities);
            if (saveToFile)
                System.IO.File.WriteAllText(directoryName + System.IO.Path.DirectorySeparatorChar + "ppResult.txt",
                    "<Multi-threaded>" + Environment.NewLine
                    + "Trial count: " + trialCount + Environment.NewLine
                    + "Started Date/Time: " + started.ToString("G") + Environment.NewLine
                    + "Finished Date/Time:" + finished.ToString("G") + Environment.NewLine
                    + "Process time: " + (finished - started).ToString() + Environment.NewLine
                    + "-- result --" + Environment.NewLine
                    + probabilities);

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}
