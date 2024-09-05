namespace PolyGenerator.Models
{
    public class PointModel
    {
        public double X { get; set; }
        public double Y { get; set; }
        public PointModel(double x, double y)
        {
            X = x;
            Y = y;
        }
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (PointModel)obj;
            return X == other.X && Y == other.Y;
        }

        // Implementacja metody GetHashCode
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
    }
}
