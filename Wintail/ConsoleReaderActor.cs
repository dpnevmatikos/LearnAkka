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
            if (message.Equals(StartCommand)) {
                PrintInstructions();
            }

            if (message is Messages.InputError) {
                _consoleWriterActor.Tell(message as Messages.InputError);
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

            if (string.IsNullOrWhiteSpace(message)) {
                // signal that the user needs to supply an input, as previously
                // received input was blank
                Self.Tell(new Messages.NullInputError("No input received."));

            } else if (message.Equals(ExitCommand, StringComparison.OrdinalIgnoreCase)) {
                Context.System.Terminate();

            } else {

                if (IsValid(message)) {
                    _consoleWriterActor.Tell(new Messages.InputSuccess("Valid Message"));

                    // continue reading messages from console
                    Self.Tell(new Messages.ContinueProcessing());
                } else {
                    Self.Tell(new Messages.ValidationError("Invalid: input had odd number of characters."));
                }
            }
        }

        /// <summary>
        /// Validates <see cref="message"/>.
        /// Currently says messages are valid if contain even number of characters.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static bool IsValid(string message)
        {
            var valid = message.Length % 2 == 0;

            return valid;
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