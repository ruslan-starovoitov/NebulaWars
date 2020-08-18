namespace Code.Scenes.BattleScene.Udp.Experimental
{
    public class InputMessageIdFactory
    {
        private int lastNumber;
        public int Create()
        {
            lastNumber++;
            return lastNumber;
        }
    }
}