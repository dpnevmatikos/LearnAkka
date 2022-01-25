# Learn Akka.NET

Lessons from https://github.com/petabridge/akka-bootcamp.


### Actor basics

Followed this [guide](https://github.com/petabridge/akka-bootcamp/blob/master/src/Unit-1/lesson2/README.md).

First steps in building a simple message sending application. 

Actors can be though of as people, members of a community. They can perform actions and
are open to communication via messages. Actors can do anything and be anything. 
The conceptual construct here seems similar to Object Oriented Programming, where
real world is split into objects, categorized into classes (entities), interacting
with each other through methods. 

Actors are created from and live within an `ActorSystem` which is their context and is
one per application.

```csharp
MyActorSystem = ActorSystem.Create("MyActorSystem");

var consoleWriterActor = MyActorSystem.ActorOf(
    Props.Create(() => new ConsoleWriterActor()));
```

Actors use messaging for communicating, and those messages are POCO classes, C# types.

```csharp
actor.Tell("continue");

protected override void OnReceive(object message)
{
    Console.Writeline(message as string);
    // Prints "continue"
}
```

In our small program so far two actors exist:
1. `ConsolReaderActor` which read from console
2. `ConsoleWriterActor` which writes to the console

`ConsoleReaderActor` has a reference on `ConsoleWriterActor` for `Tell`ing him when
input's been given.
Interesting things: `IActorRef` which as a reference (or a handle) to an actor and which is
**serializable and can be passed over network**.