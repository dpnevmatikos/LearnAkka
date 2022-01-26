# Learn Akka.NET

Lessons from https://github.com/petabridge/akka-bootcamp.


### Actor basics

Followed this [guide](https://github.com/petabridge/akka-bootcamp/blob/master/src/Unit-1/lesson2/README.md).

First steps in building a simple message sending application. 

Actors can be thought of as people, members of a community. They can perform actions and
are open to communication via messages. Actors can do anything and be anything. 
The conceptual construct here seems similar to Object Oriented Programming, where
real world is split into objects, categorized into classes (entities), interacting
with each other through methods. 

Actors are created from and live within an `ActorSystem` which is their context and is
one per application. In fact, **Actors must be created through the `ActorSystem`** or they
will be unusable.

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
_Interesting_: `IActorRef` which as a reference (or a handle) to an actor and which is
**serializable and can be passed over network**.

### Using types as messages

Followed this [guide](https://github.com/petabridge/akka-bootcamp/blob/master/src/Unit-1/lesson2/README.md).

Actors can handle different types of messages by checking their types and perform respective
actions. 

```csharp
if (message is Messages.InputError) {
    // show error
} else if (message is Messages.InputSuccess) {
    // show success
}
```

_Interesting_: `Actor`s have a `Sender` property with the sender of a message.

### More basics on Actors

Actors are not exposed to us directly, but through intermediary `IActorRef`s. This does two things:
1. Allows the `ActorSystem` to handle all messaging (include message in an `Envelope` with metadata). Messages
are guaranteed to be delivered by the system.
2. It abstracts complexities on how to get it touch with the Actor and its whereabouts (location transparency)

**Actors can practically live in anywhere, in any machine and in any process.** We use a special address, 
called `ActorPath` to communicate with them. In addition we should **[name them](https://github.com/petabridge/akka-bootcamp/blob/master/src/Unit-1/lesson3/README.md#do-i-have-to-name-my-actors)** upon their creation for 
better visibility (logs). 

```csharp
var actor = MyActorSystem.ActorOf(
    Props.Create(() => new MyActorClass()), "myFirstActor");
```

As mentioned earlier, all Actors live within a `Context`, a reference of which is available through the
respective property and which gives access to `Parent`s and `Children` actors as well.

Creating Actors is always done using a `Props` object. `Props` are "recipes" for creating `Actor` instances,
are serializable (for sharing between machines) and can contain **deployment information**. `Props` **must** be
created using the static `Create` method and **NOT** by using the `new` keyword. 

```csharp
// typeof syntax (not recommended)
var props = Props.Create(typeof(MyActor));

// lamda syntax (recommended)
var props = Props.Create(() => new MyActor(), "name");

// generic syntax
var props = Props.Create<MyActor>();
```

### Supervision & Hierarchies

Followed this [guide](https://github.com/petabridge/akka-bootcamp/blob/master/src/Unit-1/lesson4/README.md). 

Actors are organized in **[actor hierarchies](https://github.com/petabridge/akka-bootcamp/blob/master/src/Unit-1/lesson4/README.md#actor-hierarchies)**. There are two reasons for this design: 
1. Atomize work, by splitting large work into smaller chunks
2. Resilience. Errors are contained. 

High-level actors are **supervisors** and can push risky operations to children actors. If a child fails then
the parent can chose how to handle the error. A child's failure will also prevent the system from crashing.

There are three main top level actors: 
1. the `/` actor: the base actor of the entire system (a.k.a _root guardian_). This actor supervises the two
other actors, `/system` and `/user`. It is the only actor **without a parent**.
2. the `/system` actor (a.k.a _systemic guardian_): implements framework level features and ensures that
the system shuts down in an orderly manner. It's a child of `/` actor.
3. the `/user` actor (a.k.a _guardian actor_): this is root of our custom built actor system. All the actors
we create are children of this actor. It's a child of `/` actor. 

Parent actors can only supervise their children (the level below them) only. Exceptions and any failure that might
occur within a child is sent to the parent as a message of type `Failure` which can be handled in a number
of ways, called _supervision directives_: 
1. **Restart** the child (default behaviour).
2. **Stop** the child which terminates the child actor completely.
3. **Escalate the error** to their parent actor
4. **Resume** processing, and ignore the error

Parents have two (built in) **supervision strategies**: 
1. One-For-One (default): the directive issued by the parent affects only the failing child.
2. All-For-One-Strategy: the directive issued by the parent affects all children.

Custom strategies can also be implemented per case: 
```csharp
protected override SupervisorStrategy SupervisorStrategy()
{
    return new OneForOneStrategy(
        maxNrOfRetries: 10,
        withinTimeRange: TimeSpan.FromSeconds(30),
        localOnlyDecider: ex =>
        {
            // ignore the error
            if (ex is ArithmeticException) {
                return Directive.Resume;
            }

            // escalate to the Parent's parent
            if (ex is InsanelyBadException) {
                return Directive.Escalate;
            }

            // stop the child
            if (ex is NotSupportedException) {
                return Directive.Stop;
            }

            // restart the child
            return Directive.Restart;
        });
}
```