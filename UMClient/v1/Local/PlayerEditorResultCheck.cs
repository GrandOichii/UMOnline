using System.Collections.Generic;

public interface IPlayerEditorResultCheck
{
    string Check(List<PlayerEditorResult> pers);
}

public class SameDeckPlayerEditorResultCheck : IPlayerEditorResultCheck
{
    public string Check(List<PlayerEditorResult> pers)
    {
        HashSet<string> usedDeckNames = [];
        foreach (var per in pers)
        {
            if (!usedDeckNames.Add(per.Loadout.Name))
            {
                return $"Deck with name {per.Loadout.Name} is used by 2 or more players";
            }
        }

        return null;
    }
}

public class SameNamePlayerEditorResultCheck : IPlayerEditorResultCheck
{
    public string Check(List<PlayerEditorResult> pers)
    {
        HashSet<string> usedNames = [];
        foreach (var per in pers)
        {
            if (!usedNames.Add(per.Loadout.Name))
            {
                return $"Player name {per.Name} is used by 2 or more players";
            }
        }

        return null;
    }
}

// TODO add more checks