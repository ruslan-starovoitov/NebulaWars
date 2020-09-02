namespace Code.Scenes.BattleScene.Udp.Experimental
{
    public class InputMessageIdFactory
    {
        private uint lastNumber;
        public uint Create()
        {
            lastNumber++;
            return lastNumber;
        }
    }
}