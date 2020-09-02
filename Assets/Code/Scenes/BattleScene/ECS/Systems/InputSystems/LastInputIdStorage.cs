namespace Code.Scenes.BattleScene.ECS.Systems.InputSystems
{
    public class LastInputIdStorage:ILastInputIdStorage
    {
        private uint lastInputId;
        public void SetLastInputId(uint lastInputIdArg)
        {
            lastInputId = lastInputIdArg;
        }
        public uint GetLastInputId()
        {
            return lastInputId;
        }
    }
}