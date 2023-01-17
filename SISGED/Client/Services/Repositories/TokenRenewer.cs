using SISGED.Client.Services.Contracts;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SISGED.Client.Services.Repositories
{
    public class TokenRenewer : IDisposable, ITokenRenewer
    {
        private readonly ILoginRepository _loginRepository;
        Timer? timer;

        public TokenRenewer(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        public void Start()
        {
            timer = new Timer();
            timer.Interval = 1000 * 60 * 60 * 1; //One Hour
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _loginRepository.ManageTokenRenew();
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
