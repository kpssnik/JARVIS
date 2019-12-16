using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Speech.Recognition;
using System.Data.SQLite;
using System.Data;

namespace kpssJarvis
{
    class Jarvis : IDisposable
    {
        public string Name { get; }
        private SpeechRecognitionEngine engine;
        public bool IsWaitingForSummon { get; set; }

        public Jarvis(string name, Grammar grammar = null)
        {
            this.Name = name;
            this.engine = new SpeechRecognitionEngine(new CultureInfo("ru-ru"));
            this.engine.SetInputToDefaultAudioDevice();
            this.engine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Form1.jarvis_Summoned);
            this.IsWaitingForSummon = true;

            if (grammar != null) this.engine.LoadGrammar(grammar);
        }


        public void WaitForSummon()
        {
            do
            {
                engine.Recognize();
            } while (this.IsWaitingForSummon == true);

            Console.WriteLine("Слушаю");
            Console.Beep(350, 100);
            this.engine.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(Form1.jarvis_Summoned);
            this.engine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Form1.jarvis_CommandInserted);
        }

        public void ListenToCommand()
        {
            do
            {
                Console.WriteLine("\tlisten");
                engine.Recognize();
            } while (this.IsWaitingForSummon == false);

            this.engine.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(Form1.jarvis_CommandInserted);
            this.engine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Form1.jarvis_Summoned);
        }

        public void LoadGrammar(Grammar grammar)
        {
            this.engine.LoadGrammar(grammar);
        }
        public void UnloadGrammars()
        {
            this.engine.UnloadAllGrammars();
        }

        public void Dispose()
        {
            this.Dispose();
        }
    }



}
