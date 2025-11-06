using System.Text.Json;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace UMCore.Tests.Setup.Builders;

public class LoadoutTemplateBuilder
{
    private readonly string _name;
    private readonly DeckBuilder _deckBuilder;

    public LoadoutTemplateBuilder(string name)
    {
        _name = name;
        _deckBuilder = new(this);

        Result = new()
        {
            Deck = [],
            Fighters = [],
            Name = name,
            StartsWithSidekicks = true,
            ChoosesSidekick = false,
        };
    }

    public static LoadoutTemplate FooBar(string loadoutName = "FooBar")
    {
        return new LoadoutTemplateBuilder(loadoutName)
            .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                .Build()
            )
            .AddFighter(new FighterTemplateBuilder("Bar", "Bar")
                .IsSidekick()
                .Build()
            )
            .Build();
    }

    public static LoadoutTemplate FooBarBaz(string loadoutName = "FooBarBaz")
    {
        return new LoadoutTemplateBuilder(loadoutName)
            .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                .Build()
            )
            .AddFighter(new FighterTemplateBuilder("Bar", "Bar")
                .IsSidekick()
                .Build()
            )
            .AddFighter(new FighterTemplateBuilder("Baz", "Baz")
                .IsSidekick()
                .Build()
            )
            .Build();
    }

    public static LoadoutTemplate FooBarBazQuux(string loadoutName = "FooBarBazQuux")
    {
        return new LoadoutTemplateBuilder(loadoutName)
            .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                .Build()
            )
            .AddFighter(new FighterTemplateBuilder("Bar", "Bar")
                .IsSidekick()
                .Build()
            )
            .AddFighter(new FighterTemplateBuilder("Baz", "Baz")
                .IsSidekick()
                .Build()
            )
            .AddFighter(new FighterTemplateBuilder("Quux", "Quux")
                .IsSidekick()
                .Build()
            )
            .Build();
    }

    public static LoadoutTemplate NHeroesMSidekicks(int n, int m)
    {
        var result = new LoadoutTemplateBuilder($"{n}heroes_{m}sidekicks");
        for (int i = 0; i < n; ++i)
        {
            result.AddFighter(new FighterTemplateBuilder($"Hero{i}", $"Hero{i}").Build());
        }
        for (int i = 0; i < m; ++i)
        {
            result.AddFighter(new FighterTemplateBuilder($"Sidekick{i}", $"Sidekick{i}").IsSidekick().Build());
        }
        return result.Build();
    }

    public static LoadoutTemplate Foo(string loadoutName = "Foo")
    {
        return new LoadoutTemplateBuilder(loadoutName)
            .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                .Build()
            )
            .Build();
    }

    private static LoadoutTemplate LoadLoadout(string path)
    {
        var data = File.ReadAllText(path);
        var result = JsonSerializer.Deserialize<LoadoutTemplate>(data)!;
        foreach (var card in result.Deck)
        {
            card.Card.Script = File.ReadAllText($"../../../../{card.Card.Script}");
        }

        foreach (var fighter in result.Fighters)
        {
            fighter.Script = File.ReadAllText($"../../../../{fighter.Script}");
        }
        return result;
    }

    public LoadoutTemplate Result { get; private set;  }

    public LoadoutTemplateBuilder Clear()
    {
        Result = new()
        {
            Deck = [],
            Fighters = [],
            Name = _name,
            StartsWithSidekicks = true,
            ChoosesSidekick = true,
        };
        return this;
    }

    public LoadoutTemplateBuilder Load(string path)
    {
        Clear();
        Result = LoadLoadout(path);
        return this;
    }

    public LoadoutTemplateBuilder ClearDeck()
    {
        _deckBuilder.Clear();
        return this;
    }

    public LoadoutTemplateBuilder ForReach<T>(IEnumerable<T> arr, Action<LoadoutTemplateBuilder, T> action)
    {
        foreach (var o in arr)
        {
            action(this, o);
        }
        return this;
    }

    public LoadoutTemplateBuilder ConfigDeck(Action<DeckBuilder> configFunc)
    {
        configFunc(_deckBuilder);
        return this;
    }

    public LoadoutTemplateBuilder AddFighter(FighterTemplate fighter)
    {
        Result.Fighters.Add(fighter);
        return this;
    }

    public LoadoutTemplate Build()
    {
        return Result;
    }

    public class DeckBuilder(LoadoutTemplateBuilder parent)
    {
        private static readonly string DEFAULT_CARD_TEXT = "function _Create() return UM.Build:Card():Build() end";
        private static readonly string SCHEME_CARD_DRAW_TEXT = """
        function _Create() 
            return UM.Build:Card()
                :Effect(
                    'Draw {0} card(s)',
                    UM.Effects:Draw(
                        UM.Select:Players():You():Build(),
                        UM.Number:Static({0})
                    )
                )
                :Build()
        end
        """;

        private static int _basicVersatileIdx = 0;
        private static int _basicAttackIdx = 0;
        private static int _basicDefenseIdx = 0;
        private static int _basicSchemeIdx = 0;

        public DeckBuilder Clear()
        {
            parent.Result.Deck.Clear();
            return this;
        }

        public DeckBuilder Add(LoadoutCardTemplate card)
        {
            parent.Result.Deck.Add(card);

            return this;
        }

        public DeckBuilder AddBasicValueCard(string cardType, int value, int boost = 1, int amount = 1, string? key = null)
        {
            if (cardType == "Attack") return AddBasicAttack(value, boost, amount, key);
            if (cardType == "Defense") return AddBasicDefense(value, boost, amount, key);
            if (cardType == "Versatile") return AddBasicVersatile(value, boost, amount, key);
            throw new ArgumentException($"Unsupported card type for {nameof(AddBasicValueCard)}: {cardType}");
        }

        public DeckBuilder AddBasicVersatile(int value, int boost = 1, int amount = 1, string? key = null, string[]? labels = null)
        {
            var idx = ++_basicVersatileIdx;
            parent.Result.Deck.Add(new()
            {
                Amount = amount,
                Card = new()
                {
                    Key = key ?? $"versatile{idx}",
                    Name = $"versatile{idx}",
                    Type = "Versatile",
                    Value = value,
                    Boost = boost,
                    Text = "",
                    Script = DEFAULT_CARD_TEXT,
                    AllowedFighters = [],
                    Labels = labels ?? [],
                    IncludedInDeckWithSidekick = null,
                }
            });
            return this;
        }

        public DeckBuilder AddBasicAttack(int value, int boost = 1, int amount = 1, string? key = null, string[]? labels = null)
        {
            var idx = ++_basicAttackIdx;
            parent.Result.Deck.Add(new()
            {
                Amount = amount,
                Card = new()
                {
                    Key = key ?? $"attack{idx}",
                    Name = $"attack{idx}",
                    Type = "Attack",
                    Value = value,
                    Boost = boost,
                    Text = "",
                    Script = DEFAULT_CARD_TEXT,
                    AllowedFighters = [],
                    Labels = labels ?? [],
                    IncludedInDeckWithSidekick = null,
                }
            });
            return this;
        }

        public DeckBuilder AddBasicDefense(int value, int boost = 1, int amount = 1, string? key = null, string[]? labels = null)
        {
            var idx = ++_basicDefenseIdx;
            parent.Result.Deck.Add(new()
            {
                Amount = amount,
                Card = new()
                {
                    Key = key ?? $"defense{idx}",
                    Name = $"defense{idx}",
                    Type = "Defense",
                    Value = value,
                    Boost = boost,
                    Text = "",
                    Script = DEFAULT_CARD_TEXT,
                    AllowedFighters = [],
                    Labels = labels ?? [],
                    IncludedInDeckWithSidekick = null,
                }
            });
            return this;
        }

        public DeckBuilder AddBasicScheme(int boost = 1, int amount = 1, string[]? labels = null)
        {
            var idx = ++_basicSchemeIdx;

            parent.Result.Deck.Add(new()
            {
                Amount = amount,
                Card = new()
                {
                    Key = $"scheme{idx}",
                    Name = $"scheme{idx}",
                    Type = "Scheme",
                    Value = null,
                    Boost = boost,
                    Text = "",
                    Script = DEFAULT_CARD_TEXT,
                    AllowedFighters = [],
                    Labels = labels ?? [],
                    IncludedInDeckWithSidekick = null,
                }
            });
            return this;
        }

        public DeckBuilder AddCardDrawScheme(int draw, int boost = 1, int amount = 1, string[]? labels = null)
        {
            var idx = ++_basicSchemeIdx;

            parent.Result.Deck.Add(new()
            {
                Amount = amount,
                Card = new()
                {
                    Key = $"scheme{idx}",
                    Name = $"scheme{idx}",
                    Type = "Scheme",
                    Value = null,
                    Boost = boost,
                    Text = "",
                    Script = string.Format(SCHEME_CARD_DRAW_TEXT, draw),
                    AllowedFighters = [],
                    Labels = labels ?? [],
                    IncludedInDeckWithSidekick = null,
                }
            });
            return this;
        }

    }

}