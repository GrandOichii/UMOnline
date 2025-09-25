using UMCore.Matches.Players;

namespace UMCore;

public interface IHasData<T>
{
    public T GetData(Player player);
}

public interface IHasSetupData<T>
{
    public T GetSetupData();
}