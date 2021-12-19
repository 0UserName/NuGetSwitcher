using NuGetSwitcher.Interface.Entity.Enum;

namespace NuGetSwitcher.Interface.Provider.Command.Contract
{
    public interface ICommandProvider
    {
        /// <summary>
        /// 
        /// </summary>
        void Route(Mode command);
    }
}