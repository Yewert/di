using System.Drawing;

namespace TagsCloudVisualization
{
    public struct WordCloudElement
    {
        public string Name { get; }
        public Rectangle Rectangle { get; }
        public Font Font { get; }

        public WordCloudElement(string name, Rectangle rectangle, Font font)
        {
            Name = name;
            Rectangle = rectangle;
            Font = font;
        }
    }
}