namespace UMModel.Models;

public class MatchConfig : UMCore.Matches.MatchConfig
{
    public required string Name { get; set; }

    public static MatchConfig[] GetDefaultData()
    {
        return [
            // new() {
            //     Name = "Seed 0 tester",
            //     RandomMatch = false,
            //     Seed = 0,
            //     InitialHandSize = Default1x1.InitialHandSize,
            //     ActionsPerTurn = Default1x1.ActionsPerTurn,
            //     MaxHandSize = Default1x1.MaxHandSize,
            //     ManoeuvreDrawAmount = Default1x1.ManoeuvreDrawAmount,
            //     RandomFirstPlayer = Default1x1.RandomFirstPlayer,
            //     FirstPlayerIdx = Default1x1.FirstPlayerIdx,
            //     ExhaustDamage = Default1x1.ExhaustDamage,
            //     TeamSize = Default1x1.TeamSize,
            //     TeamCount = Default1x1.TeamCount,
            // }, 
            new() {
                Name = "1 vs 1",
                RandomMatch = Default1x1.RandomMatch,
                Seed = Default1x1.Seed,
                InitialHandSize = Default1x1.InitialHandSize,
                ActionsPerTurn = Default1x1.ActionsPerTurn,
                MaxHandSize = Default1x1.MaxHandSize,
                ManoeuvreDrawAmount = Default1x1.ManoeuvreDrawAmount,
                RandomFirstPlayer = Default1x1.RandomFirstPlayer,
                FirstPlayerIdx = Default1x1.FirstPlayerIdx,
                ExhaustDamage = Default1x1.ExhaustDamage,
                TeamSize = Default1x1.TeamSize,
                TeamCount = Default1x1.TeamCount,
            },
            new() {
                Name = "2 vs 2",
                RandomMatch = Default1x1.RandomMatch,
                Seed = Default1x1.Seed,
                InitialHandSize = Default1x1.InitialHandSize,
                ActionsPerTurn = Default1x1.ActionsPerTurn,
                MaxHandSize = Default1x1.MaxHandSize,
                ManoeuvreDrawAmount = Default1x1.ManoeuvreDrawAmount,
                RandomFirstPlayer = Default1x1.RandomFirstPlayer,
                FirstPlayerIdx = Default1x1.FirstPlayerIdx,
                ExhaustDamage = Default1x1.ExhaustDamage,
                TeamSize = 2,
                TeamCount = 2,
            },
            new() {
                Name = "4 free for all",
                RandomMatch = Default1x1.RandomMatch,
                Seed = Default1x1.Seed,
                InitialHandSize = Default1x1.InitialHandSize,
                ActionsPerTurn = Default1x1.ActionsPerTurn,
                MaxHandSize = Default1x1.MaxHandSize,
                ManoeuvreDrawAmount = Default1x1.ManoeuvreDrawAmount,
                RandomFirstPlayer = Default1x1.RandomFirstPlayer,
                FirstPlayerIdx = Default1x1.FirstPlayerIdx,
                ExhaustDamage = Default1x1.ExhaustDamage,
                TeamSize = 1,
                TeamCount = 4,
            },
        ];
    } 
}