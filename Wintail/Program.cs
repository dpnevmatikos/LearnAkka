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

            var tailCoordinatorActor = MyActorSystem.ActorOf(
                Props.Create(() =>
                    new TailCoordinatorActor()),
                "tailCoordinatorActor");

            var consoleWriterActor = MyActorSystem.ActorOf(
                Props.Create(() =>
                    new ConsoleWriterActor()),
                "consoleWriterActor");

            var fileValidatorActor = MyActorSystem.ActorOf(
                Props.Create(() =>
                    new FileValidatorActor(
                        consoleWriterActor, tailCoordinatorActor)),
                "fileValidatorActor");

            var validationActor = MyActorSystem.ActorOf(
                Props.Create(() =>
                    new ValidationActor(consoleWriterActor)),
                "validationActor");

            var consoleReaderActor = MyActorSystem.ActorOf(
                Props.Create(() =>
                    new ConsoleReaderActor(fileValidatorActor)),
                "consoleReaderActor");

            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            MyActorSystem.WhenTerminated.Wait();
        }
    }
}
