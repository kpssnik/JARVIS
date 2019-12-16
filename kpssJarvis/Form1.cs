using MetroFramework.Forms;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Data.SQLite;

namespace kpssJarvis
{

    public partial class Form1 : MetroForm
    {
        static Jarvis jarvis;
        static Thread jarvisThread;
        static float confidence;

        public static string dbPath = "JarvisDatabase.db";
        public static string progTable = "Programs";
        public static string cmdTable = "Commands";

        static SQLiteConnection sqlconnection;
        static SQLiteCommand sqlcommand;

        static Dictionary<string, string> programsDictionary = new Dictionary<string, string>()
        {
            {" ", " "},
            {"-", "-"}
        };
        static Dictionary<string, string> commandsDictionary = new Dictionary<string, string>()
        {
            {" ", " "},
            {"-", "-"}
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlconnection = new SQLiteConnection($"Data source={dbPath}");

            CheckOrCreateDB();

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                jarvisThread.Abort();
                Application.Exit();
            }
            catch (Exception) { }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            metroProgressBar1.Value = 0;

            InitDictionaries();
            confidence = 0.75f;
            jarvis = new Jarvis(GetJarvisNameFromDB().ToLower());

            GrammarBuilder grammar = new GrammarBuilder();
            grammar.Append(new Choices(commandsDictionary.Values.ToArray()));
            grammar.Append(new Choices(programsDictionary.Keys.ToArray()));

            jarvis.LoadGrammar(new Grammar(grammar));

            jarvisThread = new Thread(() =>
            {

                while (true)
                {
                    jarvis.WaitForSummon();
                    jarvis.ListenToCommand();
                }
            });
            jarvisThread.Start();

            metroProgressBar1.Value = 100;
            startButton.Enabled = false;

        }
        private void SettingsButton_Click(object sender, EventArgs e)
        {
            SettingsForm form = new SettingsForm();
            form.ShowDialog();
        }

        private string GetJarvisNameFromDB()
        {
            sqlconnection.Open();

            DataSet ds = new DataSet();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter($"SELECT start_listening FROM {cmdTable}", sqlconnection);
            adapter.Fill(ds);

            sqlconnection.Close();
            return ds.Tables[0].Rows[0][0].ToString();
        }
        private void CheckOrCreateDB()
        {
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
                sqlconnection.Open();

                #region PROGRAMS DICTIONARY CREATING

                string cmd = $"CREATE TABLE IF NOT EXISTS {progTable}(" +
                    $"id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    $"key TEXT NOT NULL UNIQUE," +
                    $"value TEXT NOT NULL);";

                sqlcommand = new SQLiteCommand(cmd, sqlconnection);
                sqlcommand.ExecuteNonQuery();

                Dictionary<string, string> defaultDict = new Dictionary<string, string>()
                {
                    { "калькулятор", "calc.exe" },
                    { "браузер" , "chrome.exe"}
                };

                foreach (var a in defaultDict)
                {
                    sqlcommand = new SQLiteCommand($"INSERT INTO Programs(key, value) VALUES ('{a.Key}', '{a.Value}')", sqlconnection);
                    sqlcommand.ExecuteNonQuery();
                }

                #endregion

                #region COMMAND DICTIONARY CREATING
                cmd = $"CREATE TABLE IF NOT EXISTS {cmdTable}(" +
                  $"open_prog TEXT," +
                  $"close_prog TEXT," +
                  $"start_listening TEXT," +
                  $"stop_listening TEXT," +
                  $"hide_tray TEXT," +
                  $"show_tray TEXT," +
                  $"shutdown_jarvis TEXT," +
                  $"shutdown_pc TEXT);";

                sqlcommand = new SQLiteCommand(cmd, sqlconnection);
                sqlcommand.ExecuteNonQuery();

                cmd = $"INSERT INTO Commands(open_prog, close_prog, start_listening, stop_listening, hide_tray, " +
                    $"show_tray, shutdown_jarvis, shutdown_pc) VALUES('запусти', 'закрой', 'джарвис', 'отбой', 'уйди', 'покажись', " +
                    $"'выключись', 'выключи комп')";

                sqlcommand = new SQLiteCommand(cmd, sqlconnection);
                sqlcommand.ExecuteNonQuery();
                #endregion

                sqlconnection.Close();

                MessageBox.Show("Command file is not found. Created new file with default parameters.");
            }
        }
        private void InitDictionaries()
        {

            sqlconnection.Open();

            // Programs Dictionary

            DataSet ds = new DataSet();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter($"SELECT * FROM {progTable}", sqlconnection);
            adapter.Fill(ds);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                programsDictionary.Add(row["key"].ToString(), row["value"].ToString());
            }

            // Commands Dictionary

            ds = new DataSet();
            adapter = new SQLiteDataAdapter($"SELECT * FROM {cmdTable}", sqlconnection);
            adapter.Fill(ds);

            foreach (DataColumn a in ds.Tables[0].Columns)
            {
                commandsDictionary.Add(a.ColumnName, ds.Tables[0].Rows[0][a.ColumnName].ToString());
            }


            sqlconnection.Close();
        }

        // Jarvis Events
        public static void jarvis_Summoned(object sender, SpeechRecognizedEventArgs e)
        {
            if (jarvis.IsWaitingForSummon == true && e.Result.Text == jarvis.Name && e.Result.Confidence > confidence)
            {
                jarvis.IsWaitingForSummon = false;
            }
        }
        public static void jarvis_CommandInserted(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > confidence)
            {
                string[] input = e.Result.Text.Split(' ');
                try
                {

                    if (input[0].Equals(commandsDictionary["open_prog"]))
                    {
                        Process.Start(programsDictionary[input[1]]);
                    }

                    else if (input[0].Equals(commandsDictionary["close_prog"]))
                    {
                        string kill = string.Empty;
                        if (programsDictionary[input[1]].Contains("\\")) kill = programsDictionary[input[1]].Split("\\".ToCharArray()).Last();
                        else kill = programsDictionary[input[1]];

                        foreach (var a in Process.GetProcessesByName(kill.Replace(".exe", ""))) a.Kill();
                    }

                    else if (input[0].Equals(commandsDictionary["stop_listening"]))
                    {
                        for (int i = 0; i < 2; i++) Console.Beep(300, 100);
                        jarvis.IsWaitingForSummon = true;
                    }

                    else if (input[0].Equals(commandsDictionary["shutdown_jarvis"]))
                    {
                        Console.Beep(350, 100);
                        Console.Beep(350, 100);
                        
                        foreach(var a in Process.GetProcessesByName(Assembly.GetEntryAssembly().GetName().Name))                   
                            a.Kill();
                        
                    }

                    else if (input[0].Equals(commandsDictionary["shutdown_pc"]))
                    {
                        DialogResult result = MessageBox.Show("Shutdown your PC?", "PC Shutdown", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (result.Equals(DialogResult.OK))
                        {
                            Process.Start("shutdown", "/s /t 0");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Invalid query\n" + ex.Message);
                }
            }
        }

       
    }
}
