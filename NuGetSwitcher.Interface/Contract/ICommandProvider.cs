namespace NuGetSwitcher.Interface.Contract
{
    public interface ICommandProvider
    {
        void Route(string command);
    }
}