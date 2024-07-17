using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EN2DE_chatGPT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenAIService apiService;
        string output = "";
        public MainWindow()
        {
            InitializeComponent();

            var apiKey = Environment.GetEnvironmentVariable("OPEN_AI_API_KEY") ?? throw new KeyNotFoundException("No api key found");
            apiService = new OpenAIService(new OpenAiOptions
            {
                ApiKey = apiKey
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string sourceText = GetSourceText();
            Task.Run(async () => await callChatGPT(sourceText)).Wait();
            targetFileAddress.Text = output;
        }

        private string GetSourceText()
        {
            using StreamReader reader = new(sourceFileAddress.Text);

            return reader.ReadToEnd();
        }

        private async Task callChatGPT(string sourceText)
        {
            var completionResult = await apiService.ChatCompletion.CreateCompletion
                (
                new OpenAI.ObjectModels.RequestModels.ChatCompletionCreateRequest
                {
                    Messages = new List<ChatMessage>
                    { 
                        ChatMessage.FromSystem("You are a translator, translating from English to German for an Enterprice Resource Planning system"),
                        ChatMessage.FromUser(sourceText)
                    },
                    Model = Models.Gpt_3_5_Turbo_Instruct
                }
                );

            if (completionResult.Successful)
            {
                output = completionResult.Choices.First().Message.Content ?? "empty...";
            }
            else
            {
                output = completionResult.Error!.Message ?? "Error???";
            }
        }
    }
}
