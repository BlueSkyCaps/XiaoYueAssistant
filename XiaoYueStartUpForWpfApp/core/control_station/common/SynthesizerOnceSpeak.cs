using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace XiaoYueStartUpForWpfApp.core.control_station.common
{
    class SynthesizerOnceSpeak
    {
        private PromptBuilder promptBuilder;
        private SpeechSynthesizer speechSynthesizer;
        public SynthesizerOnceSpeak(string textToSpeak)
        {
            speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.SpeakStarted += SpeechSynthesizer_SpeakStarted;
            speechSynthesizer.SpeakCompleted += SpeechSynthesizer_SpeakCompleted;
            // SpeechSynthesizer.SpeakProgress += SpeechSynthesizer_SpeakProgress;
            speechSynthesizer.SetOutputToDefaultAudioDevice();
            promptBuilder = new PromptBuilder();
            promptBuilder.Culture = new System.Globalization.CultureInfo("zh-CN");
            promptBuilder.AppendText(textToSpeak);
            speechSynthesizer.Rate = 0;
            speechSynthesizer.Volume = 100;
            speechSynthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
            speechSynthesizer.SpeakAsync(promptBuilder); // Async才能引发SpeakCompleted
        }

        private void SpeechSynthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            Console.WriteLine("the speak down");
            promptBuilder.ClearContent();
            speechSynthesizer.Dispose();
        }

        private void SpeechSynthesizer_SpeakStarted(object sender, SpeakStartedEventArgs e)
        {
            Console.WriteLine("the speak start");
        }

        private void SpeechSynthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e){}
    }
}
