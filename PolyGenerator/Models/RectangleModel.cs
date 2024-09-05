namespace PolyGenerator.Models
{
    public class RectangleModel
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Area { get; private set; }

        public RectangleModel(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Area = Width * Height;
        }
    }

}
