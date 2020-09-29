namespace Plugins.submodules.SharedCode.Systems
{
    public interface ILastProcessedInputIdStorage
    {
        uint? Get(ushort tmpPlayerId);
    }
}