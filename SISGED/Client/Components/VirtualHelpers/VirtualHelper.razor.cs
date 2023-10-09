using Microsoft.AspNetCore.Components;
using System.Timers;
using Timer = System.Timers;

namespace SISGED.Client.Components.VirtualHelpers
{
    public partial class VirtualHelper : IDisposable
    {
        [Parameter]
        public string Message { get; set; } = default!;


        private Timer.Timer timer = default!;
        private int messageIndex = 0;
        private string letters = string.Empty;
        private string showMessageClass = "d-block";
        private bool showMessage = true;

        public void Dispose() => timer?.Dispose();

        protected override void OnInitialized()
        {
            StartTimer();
        }

        public void ChangeMessage(string message)
        {
            timer.Start();
            Message = message;
        }

        private void StartTimer()
        {
            timer = new(50);

            timer.Elapsed += TimerOnElapsed!;
            timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            letters = Message[..messageIndex];

            if(letters.Length == Message.Length)
            {
                messageIndex = 0;

                timer.Stop();
            }
            else messageIndex++;

            StateHasChanged();
        }

        private void HideHelperMessage()
        {
            showMessage = !showMessage;

            showMessageClass = showMessage ? "d-block" : "d-none";
        }
    }
}