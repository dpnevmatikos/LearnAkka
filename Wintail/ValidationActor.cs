using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// 
    /// </summary>
    public class ValidationActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consoleWriterActor"></param>
        public ValidationActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected override void OnReceive(object message)
        {
            var msg = message as string;

            if (string.IsNullOrWhiteSpace(msg)) {
                _consoleWriterActor.Tell(
                    new Messages.NullInputError("Null or empty string"));
                Continue();

                return;
            }

            if (!IsValid(msg)) {
                _consoleWriterActor.Tell(
                    new Messages.ValidationError("Invalid: input had odd number of characters."));
            } else {
                _consoleWriterActor.Tell(
                    new Messages.InputSuccess("Message was valid!"));
            }

            Continue();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Continue()
        {
            Sender.Tell(new Messages.ContinueProcessing());
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
    }
}
