using Akka.Actor;

namespace WinTail
{
    public static class TailCoordinatorActorMessages
    {
        /// <summary>
        /// Start tailing the file at user-specified path.
        /// </summary>
        public class StartTail
        {
            public string FilePath { get; private set; }

            public IActorRef ReporterActor { get; private set; }

            public StartTail(string filepath, IActorRef reporterActor)
            {
                FilePath = filepath;
                ReporterActor = reporterActor;
            }
        }

        /// <summary>
        /// Stop tailing the file at user-specified path.
        /// </summary>
        public class StopTail
        {
            public string FilePath { get; private set; }

            public StopTail(string filePath)
            {
                FilePath = filePath;
            }
        }
    }
}
