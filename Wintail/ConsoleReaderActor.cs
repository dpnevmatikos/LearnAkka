using System;

using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for reading FROM the console. 
    /// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
    /// </summary>
    public class ConsoleReaderActor : UntypedActor
    {
        public const string ExitCommand = "exit";

        private IActorRef _consoleWriterActor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consoleWriterActor"></param>
        public ConsoleReaderActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected override void OnReceive(object message)
        {
            var input = Console.ReadLine();

            if (!string.IsNullOrEmpty(input) &&
                input.Equals(ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                // shut down the system (acquire handle to system via this actors context)
                Context.System.Terminate();

                return;
            }

            // send console input to the actor
            _consoleWriterActor.Tell(input);

            // This is what keeps the read loop going
            Self.Tell("continue");
        }
    }
}