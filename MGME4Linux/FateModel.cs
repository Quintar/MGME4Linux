using System;
using System.Collections.Generic;

namespace Mythic_Game_Master_Emulator
{
    public class FateModel
    {
        private string question = "";
        public string Question
        {
            get { return question; }
            set { question = value; }
        }

        private string answer = "";
        public String Answer
        {
            get { return answer; }
        }

        private int chaosFactor = 4;
        public int ChaosFactor
        {
            get { return chaosFactor; }
            set { chaosFactor = value; }
        }

        private int odds = 4;

        public int Odds
        {
            get { return odds; }
            private set { odds = value; }
        }

        public List<string> Characters {  get; set; } = new List<string>();
        public List<string> Threads { get; set; } = new List<string>();

        private List<Odds> oddsList = new List<Odds>();
        private List<ChaosFactor> chaosFactors = new List<ChaosFactor>();
        private List<string> questionList = new List<string>();


        public List<ChaosFactor> ChaosFactors
            { get { return chaosFactors; } }

        public List<Odds> OddsList
            { get { return oddsList; } }

        public List<string> QuestionList
            { get { return questionList; } }

        public FateModel(string[] oddsTable)
        {
            oddsList = new List<Odds>();

            int i = 0;
            foreach(string odd in oddsTable)
            {
                oddsList.Add(new Odds() { value = i, odds = odd });
                i++;
            }


            chaosFactors = new List<ChaosFactor>()
            {
                new ChaosFactor() { chaosFactor = 1 },
                new ChaosFactor() { chaosFactor = 2 },
                new ChaosFactor() { chaosFactor = 3 },
                new ChaosFactor() { chaosFactor = 4 },
                new ChaosFactor() { chaosFactor = 5 },
                new ChaosFactor() { chaosFactor = 6 },
                new ChaosFactor() { chaosFactor = 7 },
                new ChaosFactor() { chaosFactor = 8 },
                new ChaosFactor() { chaosFactor = 9 }
            };
        }

        public bool IsLastRollRandomEvent
        {
            get
            {
                return new List<int>
                { 11, 22, 33, 44, 55, 66, 77, 88, 99, 100 }.Contains(LastDiceResult) &&
                ((LastDiceResult / 11) <= ChaosFactor + 1);
            }
        }


        public Fate GetFate(Odds odds,  ChaosFactor chaos)
        {
            return GetFate(odds.value, chaos.chaosFactor);
        }

        public Fate GetFate(int odds, int chaos)
        {
            int odd = odds;
            int ch = chaos;
            //                         -12*5                                      3*  5*  6*  8*  9*  10* 11* 12*
            int[] fChaos = new int[] { -60, -55, -50, -45, -40, -35, -25, -15, 0, 15, 25, 35, 40, 45, 50, 55, 60 };
            int factor = fChaos[8 + ch - odd];

            int baseFactor = 50 + factor;

            if (baseFactor > 99) baseFactor = 99;
            if (baseFactor < 1) baseFactor = 1;
            return new Fate() { Yes = baseFactor };
        }

        public FateResult GetFateResult(Odds odds, ChaosFactor chaos, int mod = 0)
        {
            return GetFateResult(odds.value, chaos.chaosFactor, mod);
        }

        public int RollDice { get {
                Random r = new Random();
                var x = r.Next(1, 101);
                if (x > 100) x = 100;
                return x;
            } }

        public int Roll1Dice { get {
                Random r = new Random();
                return r.Next(1, 11); } }

        public FateResult GetFateResult(int odds, int chaos, int mod = 0)
        {
            lastFateDiceResult = RollDice;
            Fate fate = GetFate(odds, chaos);
            if (lastFateDiceResult <= fate.EYes) return FateResult.ExceptionalYes;
            if (lastFateDiceResult <= fate.Yes) return FateResult.Yes;
            if (lastFateDiceResult >= fate.ENo) return FateResult.ExceptionalNo;
            if (lastFateDiceResult > fate.Yes) return FateResult.No;
            return new FateResult();
        }

        private int lastFateDiceResult = 0;
        public int LastDiceResult { get { return lastFateDiceResult; } }

        public string GetEventFrom(EventChances eventChances, int diceroll)
        {
            foreach (EventChance eventTable in eventChances.chances)
            {
                if (diceroll >= eventTable.min && diceroll <= eventTable.max) return eventTable.label;
            }
            return "INVALID";
        }


        private bool isin(int roll, int min, int max) { return roll >= min && roll <= max; }

        public void AddToAnswer(string text, bool replace = false)
        {
            if (Answer == "" || replace) answer = text;
            else answer += "\r\n" + text;
        }

        public void ClearAnswers() { answer = ""; }
    }

    public class EventChances
    {
        public EventChance[] chances { get; set; } = new EventChance[1];
    }

    public class EventChance
    {
        public int min { get; set; } = 0;
        public int max { get; set; } = 0;
        public string label { get; set; } = "";
    }

    public enum FateResult
    {
        ExceptionalNo = 0,
        No,
        Yes,
        ExceptionalYes
    }

    public class Fate
    {
        /** Below = Exception No */
        public int ENo { get { return (int)(100 - (0.2 * Yes)); } }
        /** At and Below = Yes */
        public int Yes { get; set; } = 50;
        /** Above = Exceptional Yes */
        public int EYes { get { return (int)(0.2 * Yes); } }
    }

    public class ChaosFactor
    {
        public int chaosFactor { get; set; }
    }

    public class Odds
    {
        public int value { get; set; }
        public string odds { get; set; } = "";
    }
}
