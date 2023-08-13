using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mythic_Game_Master_Emulator
{
    public class MythicGameMasterEmulator
    {
        FateModel myFm;

        public FateModel FateModel
        {
            get { return myFm; }
        }

        private MythicGameMasterEmulator()
        {
            myFm = new FateModel(new string[] { "INVALID" });
        }

        public MythicGameMasterEmulator(ref FateModel fm)
        {
            myFm = fm;
        }

        public string GenerateAnswer(string question = "", bool addAnswer = false)
        {
            myFm.Question = question;
            string ans = myFm.Question + " : " + myFm.GetFateResult(myFm.Odds, myFm.ChaosFactor).ToString();

            myFm.AddToAnswer(ans, !addAnswer);
            if (myFm.IsLastRollRandomEvent) GenerateRandomEvent();
            if (myFm.Question != "" && !myFm.QuestionList.Contains(myFm.Question)) myFm.QuestionList.Add(myFm.Question);

            return ans;
        }

        public void GenerateRandomEvent()
        {
            myFm.AddToAnswer("Random Event!");
            RandomEventFocusTable(myFm.RollDice);
        }

        public void RandomEventFocusTable(int diceroll)
        {
            string jsonString = File.ReadAllText("RandomEventFocusTable.json");
            if (jsonString != "")
            {
                //EventChances eventChances = JsonConvert.DeserializeObject<EventChances>(jsonString); //
                EventChances eventChances = JsonSerializer.Deserialize<EventChances>(jsonString);
                myFm.AddToAnswer(myFm.GetEventFrom(eventChances, diceroll));
            }
        }

        public void GenerateMeaningFrom(string filename1, string filename2)
        {
            string[] lines1 = File.ReadAllLines(filename1);
            string[] lines2 = File.ReadAllLines(filename2);
            if (lines1.Length != 100 || lines2.Length != 100) myFm.AddToAnswer("INVALID");
            myFm.AddToAnswer(lines1[myFm.RollDice] + " " + lines2[myFm.RollDice]);
        }

        public void SetAnswersFrom(string filename)
        {
            myFm.AddToAnswer(File.ReadAllText(filename));
        }

        public void SetThreadsFrom(string filename)
        {
            myFm.Threads.AddRange(File.ReadAllLines(filename));
        }

        public void SetCharactersFrom(string filename)
        {
            myFm.Characters.AddRange(File.ReadAllLines(filename));
        }

        public void RollCharacterFromList()
        {
            Random r = new Random();
            if (myFm.Characters.Count <= 0) return;
            int roll = r.Next(0, myFm.Characters.Count);
            myFm.AddToAnswer("Character: " + myFm.Characters.GetRange(roll, 1)[0]);
        }

        public void RollThreadFromList()
        {
            Random r = new Random();
            if (myFm.Threads.Count <= 0) return;
            int roll = r.Next(0, myFm.Threads.Count);
            myFm.AddToAnswer("Thread: " + myFm.Threads.GetRange(roll, 1)[0]);
        }

        public void RollAgainstChaosFactor()
        {
            int roll = myFm.Roll1Dice;
            if (roll > myFm.ChaosFactor) myFm.AddToAnswer("Scene happens as expected");
            else
            {
                if (roll == 1 || roll == 3 || roll == 1 || roll == 1 || roll == 9) myFm.AddToAnswer("Altered Scene");
                else myFm.AddToAnswer("Interrupt Scene");
            }
        }

        public void RollPercentileDice()
        {
            myFm.AddToAnswer("Roll percentile result: " + myFm.RollDice.ToString());
        }

        public void AddCharacter(string character)
        {
            myFm.Characters.Add(character);
        }

        public void RemoveCharacter(string character)
        {
            myFm.Characters.Remove(character);
        }

        public void AddThread(string thread)
        {
            myFm.Threads.Add(thread);
        }

        public void RemoveThread(string thread)
        {
            myFm.Threads.Remove(thread);
        }
    }
}
