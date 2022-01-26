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
        public const string StartCommand = "start";

        private IActorRef _validationActor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consoleWriterActor"></param>
        public ConsoleReaderActor(IActorRef validationActor)
        {
            _validationActor = validationActor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected override void OnReceive(object message)
        {
            if (message.Equals(StartCommand)) {
                PrintInstructions();
            }

            GetAndValidateInput();
        }

        /// <summary>
        /// Reads input from console, validates it, then signals appropriate response
        /// (continue processing, error, success, etc.).
        /// </summary>
        private void GetAndValidateInput()
        {
            var message = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(message) &&
              message.Equals(ExitCommand, StringComparison.OrdinalIgnoreCase)) {
                Context.System.Terminate();

                return;
            }

            _validationActor.Tell(message);
        }

        /// <summary>
        /// 
        /// </summary>
        private void PrintInstructions()
        {
            Console.WriteLine("Write whatever you want into the console!");
            Console.WriteLine("Some entries will pass validation, and some won't...\n\n");
            Console.WriteLine("Type 'exit' to quit this application at any time.\n");
        }
    }
}