namespace sngp_server
{
    public struct Point
    {
        public int x;
        public int y;

        public static bool operator !=(Point a, Point b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public static bool operator ==(Point a, Point b)
        {
            return a.x == b.x && a.y == b.y;
        }
    }

}