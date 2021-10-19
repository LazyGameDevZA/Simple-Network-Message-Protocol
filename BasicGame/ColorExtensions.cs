namespace BasicGame
{
    using Microsoft.Xna.Framework;
    public static class ColorExtensions
    {
        public static void Invert(ref this Color color)
        {
            color.R = (byte)~color.R;
            color.G = (byte)~color.G;
            color.B = (byte)~color.B;
        }
    }
}