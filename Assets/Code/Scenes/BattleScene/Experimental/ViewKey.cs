namespace Code.Scenes.BattleScene.Experimental
{
    public struct ViewKey
    {
        public ViewKey(ViewTypeEnum viewTypeId, int width, int height)
        {
            Width = width;
            Height = height;
            ViewTypeId = viewTypeId;
        }

        public ViewTypeEnum ViewTypeId { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public bool Equals(ViewKey other)
        {
            return ViewTypeId == other.ViewTypeId && Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            return obj is ViewKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) ViewTypeId;
                hashCode = (hashCode * 397) ^ Width;
                hashCode = (hashCode * 397) ^ Height;
                return hashCode;
            }
        }
    }
}