namespace UMCore.Tests.Setup.Builders;

public class LoadoutTemplateBuilder
{
    private readonly DeckBuilder _deckBuilder;

    public LoadoutTemplateBuilder(string name)
    {
        _deckBuilder = new(this);

        Result = new()
        {
            Deck = [],
            Fighters = [],
            Name = name,
            StartsWithSidekicks = true,
        };
    }

    public static LoadoutTemplate Foo(string loadoutName = "Foo")
    {
        // TODO
        return new()
        {
            Name = loadoutName,
            StartsWithSidekicks = true,
            Fighters = [
                new() {
                    Amount = 1,
                    IsHero = true,
                    IsRanged = false,
                    Key = "Foo",
                    MaxHealth = 10,
                    Movement = 2,
                    Name = "Foo",
                    Script = FighterTemplateBuilder.DEFAULT_FIGHTER_SCRIPT,
                    StartingHealth = 10,
                    Text = ""
                }
            ],
            Deck = [
                new() {
                    Amount = 5,
                    Card = CardTemplateBuilder.DefaultScheme()
                },
            ]
        };
    }

    public LoadoutTemplate Result { get; }

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

        private static int _basicAttackIdx = 0;
        private static int _basicDefenseIdx = 0;
        private static int _basicSchemeIdx = 0;

        public DeckBuilder AddBasicAttack(int boost = 1, int amount = 1)
        {
            var idx = ++_basicAttackIdx;
            // TODO
            throw new NotImplementedException();
            return this;
        }

        public DeckBuilder AddBasicDefense(int boost = 1, int amount = 1)
        {
            var idx = ++_basicDefenseIdx;
            // TODO
            throw new NotImplementedException();
            return this;
        }
        
        public DeckBuilder AddBasicScheme(int boost = 1, int amount = 1)
        {
            var idx = ++_basicSchemeIdx;

            parent.Result.Deck.Add(new() {
                Amount = amount,
                Card = new() {
                    Key = $"scheme{idx}",
                    Name = $"scheme{idx}",
                    Type = "Scheme",
                    Value = null,
                    Boost = boost,
                    Text = "",
                    Script = DEFAULT_CARD_TEXT,
                    AllowedFighters = [],
                }
            });
            return this;
        }
           
    }
}