using Shouldly;
using UMCore.Matches.Players;

namespace UMCore.Tests.Setup.Builders;

public class TestPlayerControllerBuilder
{
    private readonly ActionsBuilder _actions = new();
    private readonly HandCardChoicesBuilder _handCardChoices = new();
    private readonly FighterChoicesBuilder _fighterChoices = new();
    private readonly NodeChoicesBuilder _nodeChoices = new();

    // static controllers
    public static TestPlayerController Crasher()
    {
        return new TestPlayerControllerBuilder()
            .ConfigActions(a => a
                .CrashMatch()
            )
            .Build();
    }

    public TestPlayerControllerBuilder ConfigActions(Action<ActionsBuilder> actions)
    {
        actions(_actions);
        return this;
    }

    public TestPlayerControllerBuilder ConfigHandCardChoices(Action<HandCardChoicesBuilder> choices)
    {
        choices(_handCardChoices);
        return this;
    }

    public TestPlayerControllerBuilder ConfigFighterChoices(Action<FighterChoicesBuilder> choices)
    {
        choices(_fighterChoices);
        return this;
    }

    public TestPlayerControllerBuilder ConfigNodeChoices(Action<NodeChoicesBuilder> choices)
    {
        choices(_nodeChoices);
        return this;
    }

    public TestPlayerController Build()
    {
        return new TestPlayerController()
        {
            Actions = _actions.Queue,
            HandCardChoices = _handCardChoices.Queue,
            FighterChoices = _fighterChoices.Queue,
            NodeChoices = _nodeChoices.Queue,
        };
    }

    public class ActionsBuilder
    {
        public Queue<TestPlayerController.PlayerAction> Queue { get; } = [];

        private ActionsBuilder Enqueue(TestPlayerController.PlayerAction action)
        {
            Queue.Enqueue(action);
            return this;
        }

        public ActionsBuilder DeclareWinner()
        {
            return Enqueue((match, player, options) =>
            {
                match.SetWinner(player);
                return (TestPlayerController.NEXT_ACTION, true);
            });
        }

        public ActionsBuilder CrashMatch()
        {
            return Enqueue((match, player, options) =>
            {
                throw new IntentionalCrashException("Requested crash from TestPlayerController");
            });
        }

        public ActionsBuilder Manoeuvre()
        {
            return Enqueue((match, player, options) =>
            {
                return (new ManoeuvreAction().Name(), true);
            });
        }

        public ActionsBuilder Scheme()
        {
            return Enqueue((match, player, options) => (new SchemeAction().Name(), true));
        }

        public ActionsBuilder Assert(Action<Asserts> action)
        {
            return Enqueue((match, player, options) =>
            {
                action(new Asserts(match, player, options));
                return (TestPlayerController.NEXT_ACTION, true);
            });

        }

        public class Asserts(TestMatch match, Player player, string[] options)
        {
            public Asserts CantScheme()
            {
                options.ShouldContain(new ManoeuvreAction().Name());
                return this;
            }
        }
    }

    public class HandCardChoicesBuilder
    {
        public Queue<TestPlayerController.HandCardChoice> Queue { get; } = new();

        public HandCardChoicesBuilder Nothing()
        {
            Queue.Enqueue((player, pIdx, options, hint) => null);
            return this;
        }

        public HandCardChoicesBuilder First()
        {
            Queue.Enqueue((player, pIdx, options, hint) => options.First());
            return this;
        }
    }

    public class FighterChoicesBuilder
    {
        public Queue<TestPlayerController.FighterChoice> Queue { get; } = new();

        public FighterChoicesBuilder ForEach<T>(IEnumerable<T> objs, Action<FighterChoicesBuilder, T> action)
        {
            foreach (var o in objs)
            {
                action(this, o);
            }
            return this;
        }

        public FighterChoicesBuilder First()
        {
            Queue.Enqueue((player, options, hint) => options.First());
            return this;
        }

        public FighterChoicesBuilder WithName(string name)
        {
            Queue.Enqueue((player, options, hint) => options.First(f => f.Name == name));
            return this;
        }
    }

    public class NodeChoicesBuilder
    {
        public Queue<TestPlayerController.NodeChoice> Queue { get; } = new();

        public NodeChoicesBuilder ForEach<T>(IEnumerable<T> objs, Action<NodeChoicesBuilder, T> action)
        {
            foreach (var o in objs)
            {
                action(this, o);
            }
            return this;
        }

        public NodeChoicesBuilder WithId(int id)
        {
            Queue.Enqueue((player, options, hint) => (options.First(n => n.Id == id), true));
            return this;
        }

        public NodeChoicesBuilder First()
        {
            Queue.Enqueue((player, options, hint) => (options.First(), true));
            return this;
        }

        public NodeChoicesBuilder AssertOptionsHasLength(int amount)
        {
            Queue.Enqueue((player, options, hint) => {
                options.Length.ShouldBe(amount);
                return (null, false);
            });
            return this;
        }
    }
}



[Serializable]
public class IntentionalCrashException(string message) 
    : Exception(message) {}