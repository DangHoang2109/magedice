namespace Cosina
{
    public static class Random
    {
        private static System.Random _r;

        static Random()
        {
            _r = new System.Random();
        }

        /// <summary>
        /// min <= x < max
        /// </summary>
        public static int Next(int min, int max)
        {
            return _r.Next(min, max);
        }

        /// <summary>
        /// min <= x <= max
        /// <para>
        /// because of casting double to float
        /// </para>
        /// </summary>
        public static float Next(float min, float max)
        {
            return (float)_r.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// min < x < max
        /// </summary>
        public static double Next(double min, double max)
        {
            return _r.NextDouble() * (max - min) + min;
        }
    }
}