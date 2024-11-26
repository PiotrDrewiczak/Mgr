namespace PolyGenerator.Models
{
    public class RectangleModel
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Area { get; private set; }
        public double CenterX { get; private set; }
        public double CenterY { get; private set; }

        public RectangleModel(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Area = width * height;
            CenterX = x + width / 2;
            CenterY = y + height / 2;
        }
    }
}