namespace Govan.Runners
{
    public interface IRunner
    {
        void ExecuteCommand(string workingFolder, string command, string arguments);
    }
}