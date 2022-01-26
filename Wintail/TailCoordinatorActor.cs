using System;

using Akka.Actor;

using static WinTail.TailCoordinatorActorMessages;

namespace WinTail
{
    /// <summary>
    /// 
    /// </summary>
    public class TailCoordinatorActor : UntypedActor
    {
        public TailCoordinatorActor()
        { }

        protected override void OnReceive(object message)
        {
            if (message is StartTail) {
                var msg = message as StartTail;

                Context.ActorOf(
                    Props.Create(
                        () => new TailActor(msg.FilePath, msg.ReporterActor)),
                    "tailActor");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                10,
                TimeSpan.FromSeconds(30),
                x => {
                    // Maybe we consider ArithmeticException to not be application critical
                    // so we just ignore the error and keep going.
                    if (x is ArithmeticException) {
                        return Directive.Resume;
                    }

                    // Error that we cannot recover from, stop the failing actor
                    if (x is NotSupportedException) {
                        return Directive.Stop;
                    }

                    // In all other cases, just restart the failing actor
                    return Directive.Restart;
                });
        }
    }
}
