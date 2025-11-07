using Shouldly;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;
using UMCore.Matches.Tokens;

namespace UMCore.Tests.Setup.Builders;

public class TestPlayerControllerBuilder
{
    private readonly ActionsBuilder _actions = new();
    private readonly HandCardChoicesBuilder _handCardChoices = new();
    private readonly FighterChoicesBuilder _fighterChoices = new();
    private readonly NodeChoicesBuilder _nodeChoices = new();
    private readonly AttackChoicesBuilder _attackChoices = new();
    private readonly StringChoicesBuilder _stringChoices = new();
    private readonly PathChoicesBuilder _pathChoices = new();
    private readonly TokenChoicesBuilder _tokenChoices = new();

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

    public TestPlayerControllerBuilder ConfigAttackChoices(Action<AttackChoicesBuilder> actions)
    {
        actions(_attackChoices);
        return this;
    }

    public TestPlayerControllerBuilder ConfigTokenChoices(Action<TokenChoicesBuilder> actions)
    {
        actions(_tokenChoices);
        return this;
    }

    public TestPlayerControllerBuilder ConfigStringChoices(Action<StringChoicesBuilder> actions)
    {
        actions(_stringChoices);
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
    
    public TestPlayerControllerBuilder ConfigPathChoices(Action<PathChoicesBuilder> choices)
    {
        choices(_pathChoices);
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
            AttackChoices = _attackChoices.Queue,
            StringChoices = _stringChoices.Queue,
            PathChoices = _pathChoices.Queue,
            TokenChoices = _tokenChoices.Queue,
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

        public ActionsBuilder NTimes(int n, Action<ActionsBuilder> action)
        {
            for (int i = 0; i < n; ++i)
                action(this);
            return this;
        }


        public ActionsBuilder DeclareWinner()
        {
            return Enqueue((match, player, options) =>
            {
                match.SetWinner(player);
                return Task.FromResult((TestPlayerController.NEXT_ACTION, true));
            });
        }

        public ActionsBuilder DealDamage(string fighterNameOrKey, int amount, bool byKey = false)
        {
            return Enqueue(async (match, player, options) =>
            {
                
                var fighter = byKey
                    ? match.Fighters.First(f => f.Template.Key == fighterNameOrKey)
                    : match.Fighters.First(f => f.Name == fighterNameOrKey)
                ;
                await fighter.ProcessDamage(amount);
                return (TestPlayerController.NEXT_ACTION, true);
            });
        }

        public ActionsBuilder PlaceToken(string tokenName, int nodeId)
        {
            return Enqueue(async (match, player, options) =>
            {
                var node = match.Map.Nodes.First(n => n.Id == nodeId);
                var token = match.Tokens.Get(tokenName);
                await node.PlaceToken(token);
                return (TestPlayerController.NEXT_ACTION, true);
            });
        }

        public ActionsBuilder Mill(int amount)
        {
            return Enqueue(async (match, player, options) =>
            {
                await player.Mill(amount);
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
                return Task.FromResult((new ManoeuvreAction().Name(), true));
            });
        }

        public ActionsBuilder Attack()
        {
            return Enqueue((match, player, options) =>
            {
                return Task.FromResult((new AttackAction().Name(), true));
            });
        }

        public ActionsBuilder Scheme()
        {
            return Enqueue((match, player, options) => Task.FromResult((new SchemeAction().Name(), true)));
        }

        public ActionsBuilder Assert(Action<Asserts> action)
        {
            return Enqueue((match, player, options) =>
            {
                action(new Asserts(match, player, options));
                return Task.FromResult((TestPlayerController.NEXT_ACTION, true));
            });
        }

        public class Asserts(TestMatch match, Player player, string[] options)
        {
            public Asserts CantScheme()
            {
                options.ShouldNotContain(new SchemeAction().Name());
                return this;
            }

            public Asserts CantAttack()
            {
                options.ShouldNotContain(new AttackAction().Name());
                return this;
            }

            public Asserts CanAttack()
            {
                options.ShouldContain(new AttackAction().Name());
                return this;
            }
        }
    }

    public class HandCardChoicesBuilder
    {
        public Queue<TestPlayerController.HandCardChoice> Queue { get; } = new();

        private HandCardChoicesBuilder Enqueue(TestPlayerController.HandCardChoice choice)
        {
            Queue.Enqueue(choice);
            return this;
        }

        public HandCardChoicesBuilder NTimes(int n, Action<HandCardChoicesBuilder> action)
        {
            for (int i = 0; i < n; ++i)
                action(this);
            return this;
        }

        public HandCardChoicesBuilder Nothing()
        {
            return Enqueue((player, pIdx, options, hint) => Task.FromResult<(MatchCard?, bool)>((null, true)));
        }

        public HandCardChoicesBuilder First()
        {
            Queue.Enqueue((player, pIdx, options, hint) => Task.FromResult<(MatchCard?, bool)>((options.First(), true)));
            return this;
        }

        public HandCardChoicesBuilder Assert(Action<Asserts> action)
        {
            return Enqueue((player, pIdx, options, hint) =>
            {
                action(new Asserts(player, pIdx, options, hint));
                return Task.FromResult<(MatchCard?, bool)>((null, false));
            });
        }

        public class Asserts(Player player, int playerHandIdx, MatchCard[] options, string hint) : GeneralAsserts(player)
        {
            
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

        public FighterChoicesBuilder InNodeWithId(int id)
        {
            Queue.Enqueue((player, options, hint) => player.Match.Map.Nodes.Single(n => n.Id == id).Fighter!);
            return this;
        }
    
        public FighterChoicesBuilder NTimes(int n, Action<FighterChoicesBuilder> action)
        {
            for (int i = 0; i < n; ++i)
                action(this);
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

        public NodeChoicesBuilder NTimes(int n, Action<NodeChoicesBuilder> action)
        {
            for (int i = 0; i < n; ++i)
                action(this);
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
            Queue.Enqueue((player, options, hint) =>
            {
                options.Length.ShouldBe(amount);
                return (null, false);
            });
            return this;
        }
    }

    public class AttackChoicesBuilder
    {
        public Queue<TestPlayerController.AttackChoice> Queue { get; } = new();

        private AttackChoicesBuilder Enqueue(TestPlayerController.AttackChoice choice)
        {
            Queue.Enqueue(choice);
            return this;
        }

        public AttackChoicesBuilder First()
        {
            return Enqueue((player, options) => (options.First(), true));
        }

        public AttackChoicesBuilder FirstByFighterWithName(string name)
        {
            return Enqueue((player, options) => (options.First(a => a.Fighter.Name == name), true));
        }

        public AttackChoicesBuilder FirstTargetingFighterWithName(string name)
        {
            return Enqueue((player, options) => (options.First(a => a.Target.Name == name), true));
        }

        public AttackChoicesBuilder ByFighterInNodeWithId(int nodeId)
        {
            return Enqueue((player, options) => {
                var nodes = player.Match.Map.Nodes;
                var fighter = nodes.First(n => n.Id == nodeId).Fighter;

                return (options.First(a => a.Fighter == fighter), true);
            });
        }

        public AttackChoicesBuilder Assert(Action<Asserts> action)
        {
            return Enqueue((player, options) =>
            {
                action(new Asserts(player, options));
                return (null, false);
            });
        }

        public class Asserts(Player player, AvailableAttack[] options)
        {
            public Asserts OptionsCount(int amount)
            {
                options.Length.ShouldBe(amount);
                return this;
            }

            public Asserts CanAttack(string attackerKey, string defenderKey, string attackCardKey)
            {
                var attack = options.FirstOrDefault(a =>
                    a.AttackCard.Template.Key == attackCardKey &&
                    a.Fighter.Template.Key == attackerKey &&
                    a.Target.Template.Key == defenderKey
                );
                attack.ShouldNotBeNull();
                return this;
            }

            public Asserts CanAttackOnly(string attackerKey, string defenderKey, string attackCardKey)
            {
                options.All(a =>
                    a.AttackCard.Template.Key == attackCardKey &&
                    a.Fighter.Template.Key == attackerKey &&
                    a.Target.Template.Key == defenderKey
                ).ShouldBeTrue();
                return this;
            }
        }
    }

    public class StringChoicesBuilder
    {
        public Queue<TestPlayerController.StringChoice> Queue { get; } = new();

        private StringChoicesBuilder Enqueue(TestPlayerController.StringChoice choice)
        {
            Queue.Enqueue(choice);
            return this;
        }

        public StringChoicesBuilder NTimes(int n, Action<StringChoicesBuilder> action)
        {
            for (int i = 0; i < n; ++i)
                action(this);
            return this;
        }

        public StringChoicesBuilder Yes()
        {
            return Enqueue((player, options, hint) => ("Yes", true));
        }

        public StringChoicesBuilder No()
        {
            return Enqueue((player, options, hint) => ("No", true));
        }

        public StringChoicesBuilder First()
        {
            return Enqueue((player, options, hint) => (options.First(), true));
        }

        public StringChoicesBuilder Choose(string v)
        {
            return Enqueue((player, options, hint) => (v, true));
        }

        public StringChoicesBuilder Assert(Action<Asserts> action)
        {
            return Enqueue((player, options, hint) =>
            {
                action(new Asserts(player, options, hint));
                return (null, false);
            });
        }

        public class Asserts(Player player, string[] options, string hint)
        {
            public Asserts OptionsCount(int amount)
            {
                options.Length.ShouldBe(amount);
                return this;
            }

            public Asserts EquivalentTo(string[] opts)
            {
                options.ShouldBeEquivalentTo(opts);
                return this;
            }
        }
    }

    public class PathChoicesBuilder
    {
        public Queue<TestPlayerController.PathChoice> Queue { get; } = new();

        private PathChoicesBuilder Enqueue(TestPlayerController.PathChoice choice)
        {
            Queue.Enqueue(choice);
            return this;
        }

        public PathChoicesBuilder First()
        {
            return Enqueue((player, options, hint) => (options.First(), true));
        }
        
        public PathChoicesBuilder FirstStopsAtId(int id)
        {
            return Enqueue((player, options, hint) => (options.First(p => p.Nodes.Last().Id == id), true));
        }

        public PathChoicesBuilder NTimes(int n, Action<PathChoicesBuilder> action)
        {
            for (int i = 0; i < n; ++i)
                action(this);
            return this;
        }

        public PathChoicesBuilder Assert(Action<Asserts> action)
        {
            return Enqueue((player, options, hint) =>
            {
                action(new Asserts(player, options, hint));
                return (null, false);
            });
        }

        public class Asserts(Player player, UMCore.Matches.Path[] options, string hint)
        {
            public Asserts CanReachNodeWithId(int id)
            {
                // options.Any(p => p.Nodes.FirstOrDefault(n => n.Id == id) is not null).ShouldBeTrue();
                options.Any(p => p.Nodes.Last().Id == id).ShouldBeTrue();

                return this;
            }

            public Asserts CantReachNodeWithId(int id)
            {
                options.Any(p => p.Nodes.FirstOrDefault(n => n.Id == id) is not null).ShouldBeFalse();
                return this;
            }
            
            public Asserts OptionsCount(int amount)
            {
                options.Length.ShouldBe(amount);
                return this;
            }
        }
    }

    public class TokenChoicesBuilder
    {
        public Queue<TestPlayerController.TokenChoice> Queue { get; } = new();

        private TokenChoicesBuilder Enqueue(TestPlayerController.TokenChoice choice)
        {
            Queue.Enqueue(choice);
            return this;
        }

        public TokenChoicesBuilder First()
        {
            return Enqueue((player, options, hint) => (options.First(), true));
        }
        
        public TokenChoicesBuilder InNodeWithId(int id)
        {
            return Enqueue((player, options, hint) => (options.First(o => o.Node.Id == id), true));
        }
        
        public TokenChoicesBuilder NTimes(int n, Action<TokenChoicesBuilder> action)
        {
            for (int i = 0; i < n; ++i)
                action(this);
            return this;
        }

        public TokenChoicesBuilder Assert(Action<Asserts> action)
        {
            return Enqueue((player, options, hint) =>
            {
                action(new Asserts(player, options, hint));
                return (null, false);
            });
        }

        public class Asserts(Player player, PlacedToken[] options, string hint)
        {
            public Asserts OptionsCount(int amount)
            {
                options.Length.ShouldBe(amount);
                return this;
            }
        }
    }
}

public class GeneralAsserts(Player player)
{
    // public void CombatCardsAreHidden()
    // {
    //     var combat = player.Match.Combat;
    //     combat.ShouldNotBeNull();
    //     combat.AttackCard.
    // }
}

[Serializable]
public class IntentionalCrashException(string message)
    : Exception(message)
{ }
