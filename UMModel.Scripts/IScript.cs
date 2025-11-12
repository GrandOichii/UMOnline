namespace UMModel.Scripts;

public interface IScript
{
    Task Run(UMContext ctx, string[] args);
}