using System;
using System.IO;
using Gtk;
using Mythic_Game_Master_Emulator;

namespace MGME4Linux
{
    public partial class MainWindowMGME : Gtk.Window
    {
        private MythicGameMasterEmulator gme;
        private FateModel fm;
        public MainWindowMGME() : base(Gtk.WindowType.Toplevel)
        {
            Build();
            loadMeaningTables();

            fm = new FateModel(loadOddsTable());
            gme = new MythicGameMasterEmulator(ref fm);

            loadGui();
            resetGUI();
        }

        protected void OnDeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
            a.RetVal = true;
        }


        private string[] loadOddsTable()
        {
            return File.ReadAllLines("odds.txt");
        }

        private void loadMeaningTables()
        {
            string[] MeaningTables = Directory.GetFiles(".", "MeaningTable*.txt");
            for (int i = 0; i < MeaningTables.Length; i++)
            {
                //MeaningTables[i] = MeaningTables[i].Replace(".\\", ""); //Windows
                MeaningTables[i] = MeaningTables[i].Replace("./", ""); //Linux
                MeaningTables[i] = MeaningTables[i].Replace(".txt", "");
                MeaningTables[i] = MeaningTables[i].Replace("MeaningTable", "");
            }

            foreach (String s in MeaningTables)
            {
                comboBoxMeaningTablesFiles1.InsertText(0, s);
                comboBoxMeaningTablesFiles2.InsertText(0, s);
            }
            comboBoxMeaningTablesFiles1.Active = 0;
            comboBoxMeaningTablesFiles2.Active = 0;

        }

        private void resetModelAndScroll()
        {
            resetGUI();
            TextBuffer b = textviewLog.Buffer;
            TextMark mtmp = b.GetMark("end");
            if (mtmp == null) b.CreateMark("end", b.EndIter, true);
            mtmp = b.GetMark("end");
            //textviewLog.ScrollToMark(mtmp, 0, false, 0, 0);
            textviewLog.ScrollToIter(b.EndIter, 0, true, 0, 0);
        }

        private string getFilename1
        {
            get
            {
                return "MeaningTable" + comboBoxMeaningTablesFiles1.ActiveText + ".txt";
            }
        }
        private string getFilename2
        {
            get
            {
                return "MeaningTable" + comboBoxMeaningTablesFiles2.ActiveText + ".txt";
            }
        }

        ListStore Characters = new ListStore(typeof(string));
        ListStore Threads = new ListStore(typeof(string));

        private void loadGui()
        {
            foreach (Odds s in fm.OddsList)
            {
                comboboxOdds.InsertText(0, s.odds);
            }

            TreeViewColumn characters = new TreeViewColumn();
            TreeViewColumn threads = new TreeViewColumn();
            CellRendererText cellCharacters = new CellRendererText();
            CellRendererText cellThreads = new CellRendererText();
            characters.PackStart(cellCharacters, true);
            characters.AddAttribute(cellCharacters, "text", 0);
            threads.PackStart(cellThreads, true);
            threads.AddAttribute(cellThreads, "text", 0);
            listCharacters.AppendColumn(characters);
            listThreads.AppendColumn(threads);
            listCharacters.Model = Characters;
            listThreads.Model = Threads;

        }

        private void resetGUI()
        {
            comboboxChaosFactor.Active = fm.ChaosFactor;
            comboboxOdds.Active = fm.Odds;
            textviewLog.Buffer.Text = fm.Answer;
            Characters.Clear();
            Threads.Clear();

            foreach (String s in fm.Characters) Characters.AppendValues(s);
            foreach (String s in fm.Threads) Threads.AppendValues(s);
        }


        protected void doAskFateQuestion(object sender, EventArgs e)
        {
            gme.GenerateAnswer(entryFateQuestion.Text, checkbuttonAddAnswer.Active);
            resetModelAndScroll();
        }

        protected void CharacterKeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key == Gdk.Key.Return && entryCharacter.Text.Length > 0)
            {
                fm.Characters.Add(entryCharacter.Text);
                entryCharacter.Text = "";
                resetGUI();
            }
        }

        protected void ThreadKeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key == Gdk.Key.Return && entryThread.Text.Length > 0)
            {
                fm.Threads.Add(entryThread.Text);
                entryThread.Text = "";
                resetGUI();
            }
        }

        protected void CharacterClearClicked(object sender, EventArgs e)
        {
            fm.Characters.Clear();
            resetGUI();
        }

        protected void ThreadClearClicked(object sender, EventArgs e)
        {
            fm.Threads.Clear();
            resetGUI();
        }

        protected void CharacterRollClicked(object sender, EventArgs e)
        {
            gme.RollCharacterFromList();
            resetGUI();
        }

        protected void ThreadRollClicked(object sender, EventArgs e)
        {
            gme.RollThreadFromList();
            resetGUI();
        }

        protected void GenerateMeaningClicked(object sender, EventArgs e)
        {
            gme.GenerateMeaningFrom(getFilename1, getFilename2);
            resetGUI();
        }

        protected void AnswersClearClicked(object sender, EventArgs e)
        {
            fm.ClearAnswers();
            resetGUI();
        }

        protected void RollPercentileClicked(object sender, EventArgs e)
        {
            gme.RollPercentileDice();
            resetGUI();
        }

        protected void RandomEventClicked(object sender, EventArgs e)
        {
            gme.GenerateRandomEvent();
            resetGUI();
        }

        protected void RollClicked(object sender, EventArgs e)
        {
            gme.RollAgainstChaosFactor();
            resetGUI();
        }


        protected void LoadAnswersActivated(object sender, EventArgs e)
        {
            FileChooserDialog fileChooser = new FileChooserDialog("Load Answers", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Load", ResponseType.Ok);
            if (fileChooser.Run() == (int)ResponseType.Ok && fileChooser.Filename != "")
            {
                gme.SetAnswersFrom(fileChooser.Filename);
                resetGUI();
            }
            fileChooser.Destroy();
        }

        protected void SaveAnswersActivated(object sender, EventArgs e)
        {
            FileChooserDialog fileChooser = new FileChooserDialog("Save Answers", this, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Ok);
            if (fileChooser.Run() == (int)ResponseType.Ok && fileChooser.Filename != "")
            {
                File.WriteAllText(fileChooser.Filename, gme.FateModel.Answer);
            }
            fileChooser.Destroy();
        }

        protected void LoadCharactersActivated(object sender, EventArgs e)
        {
            FileChooserDialog fileChooser = new FileChooserDialog("Load Characters", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Load", ResponseType.Ok);
            if (fileChooser.Run() == (int)ResponseType.Ok && fileChooser.Filename != "")
            {
                gme.SetCharactersFrom(fileChooser.Filename);
                resetGUI();
            }
            fileChooser.Destroy();
        }

        protected void SaveCharactersActivated(object sender, EventArgs e)
        {
            FileChooserDialog fileChooser = new FileChooserDialog("Save Characters", this, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Ok);
            if (fileChooser.Run() == (int)ResponseType.Ok && fileChooser.Filename != "")
            {
                File.WriteAllLines(fileChooser.Filename, gme.FateModel.Characters);
            }
            fileChooser.Destroy();
        }

        protected void LoadThreadsActivated(object sender, EventArgs e)
        {
            FileChooserDialog fileChooser = new FileChooserDialog("Load Threads", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Load", ResponseType.Ok);
            if (fileChooser.Run() == (int)ResponseType.Ok && fileChooser.Filename != "")
            {
                gme.SetThreadsFrom(fileChooser.Filename);
                resetGUI();
            }
            fileChooser.Destroy();
        }

        protected void SaveThreadsActivated(object sender, EventArgs e)
        {
            FileChooserDialog fileChooser = new FileChooserDialog("Save Threads", this, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Ok);
            if (fileChooser.Run() == (int)ResponseType.Ok && fileChooser.Filename != "")
            {
                File.WriteAllLines(fileChooser.Filename, gme.FateModel.Threads);
            }
            fileChooser.Destroy();
        }

        protected void ExitAppActivated(object sender, EventArgs e)
        {
            Application.Quit();
        }

        protected void CharacterListKeyRelease(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key == Gdk.Key.Delete && listCharacters.Selection.GetSelectedRows().Length > 0)
            {
                TreeIter tree = new TreeIter();
                listCharacters.Model.GetIter(out tree, listCharacters.Selection.GetSelectedRows()[0]);
                var os = listCharacters.Model.GetValue(tree, 0);
                gme.RemoveCharacter((string)os);
                resetGUI();
            }
        }

        protected void ThreadListKeyRelease(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key == Gdk.Key.Delete && listThreads.Selection.GetSelectedRows().Length > 0)
            {
                TreeIter tree = new TreeIter();
                listThreads.Model.GetIter(out tree, listThreads.Selection.GetSelectedRows()[0]);
                var os = listThreads.Model.GetValue(tree, 0);
                gme.RemoveThread((string)os);
                resetGUI();
            }
        }
    }
}
