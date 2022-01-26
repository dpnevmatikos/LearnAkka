﻿using Akka.Actor;

namespace WinTail
{
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var MyActorSystem = ActorSystem.Create("MyActorSystem");

            var consoleWriterActor = MyActorSystem.ActorOf(
                Props.Create(() => new ConsoleWriterActor()),
                "consoleWriterActor");

            var validationActor = MyActorSystem.ActorOf(
                Props.Create(() => new ValidationActor(consoleWriterActor)),
                "validationActor");

            var consoleReaderActor = MyActorSystem.ActorOf(
                Props.Create(() => new ConsoleReaderActor(validationActor)),
                "consoleReaderActor");

            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            MyActorSystem.WhenTerminated.Wait();
        }
    }
}
