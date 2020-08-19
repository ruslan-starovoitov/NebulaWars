using System.Threading;
using System.Threading.Tasks;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.Test2
{
    public class Test2Behaviour : MonoBehaviour
    {
        private int count;
        private CancellationTokenSource cancellationTokenSource;
        private readonly ILog log = LogManager.CreateLogger(typeof(Test2Behaviour));

        private void Start()
        {
            log.Debug(nameof(Start));
            cancellationTokenSource = new CancellationTokenSource();
            Thread thread = new Thread(ThreadWork);
            thread.Start();
        }

        private async void ThreadWork()
        {
            int counter = 1_000_000;
            while (true)
            {
                await Task.Delay(15, cancellationTokenSource.Token);
                log.Debug(counter++);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void Update()
        {
            ++count;
            log.Debug(count.ToString());
        }

        private void OnDestroy()
        {
            cancellationTokenSource.Cancel();
            log.Print();
        }
    }
}
