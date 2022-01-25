using System;

using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for serializing message writes to the console.
    /// (write one message at a time, champ :)
    /// </summary>
    public class ConsoleWriterActor : UntypedActor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected override void OnReceive(object message)
        {
            var input = message as string;

            // make sure we got a message
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Please provide an input.\n");
                Console.ResetColor();

                return;
            }

            // if message has even # characters, display in red; else, green
            var even = input.Length % 2 == 0;

            var color = even ? ConsoleColor.Red : ConsoleColor.Green;

            var alert = even ?
                $"Your string had an even # of characters.\n({input})\n" :
                $"Your string had an odd # of characters.\n({input})\n";

            Console.ForegroundColor = color;
            Console.WriteLine(alert);
            Console.ResetColor();
        }
    }
}
