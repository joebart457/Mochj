using Mochj._Tokenizer.Models;
using Mochj.IDE.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Environment = Mochj._Storage.Environment;

namespace Mochj.IDE._Tokenizer.Models
{
    public class RangedToken
    {
        public Token Token { get; set; }
        public TextRange TextRange { get; set; }
        public Environment Environment { get; set; }
        public TokenClassifierEnum Classifier { get; set; } = TokenClassifierEnum.Unkown;
        public string Message { get; set; }

        public override string ToString()
        {
            return Token.ToString();
        }

        public void DoHighlight()
        {
            if (Classifier == TokenClassifierEnum.Identifier)
            {
                this.TextRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Blue));
            }

            if (Classifier == TokenClassifierEnum.Function)
            {
                this.TextRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Green));
            }

            if (Classifier == TokenClassifierEnum.Namespace)
            {
                this.TextRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.AliceBlue));
            }

            if (Classifier == TokenClassifierEnum.Error)
            {
                Pen path_pen = new Pen(new SolidColorBrush(Colors.Red), 0.2);
                path_pen.EndLineCap = PenLineCap.Square;
                path_pen.StartLineCap = PenLineCap.Square;

                Point path_start = new Point(0, 1);
                BezierSegment path_segment = new BezierSegment(new Point(1, 0), new Point(2, 2), new Point(3, 1), true);
                PathFigure path_figure = new PathFigure(path_start, new PathSegment[] { path_segment }, false);
                PathGeometry path_geometry = new PathGeometry(new PathFigure[] { path_figure });

                DrawingBrush squiggly_brush = new DrawingBrush();
                squiggly_brush.Viewport = new Rect(0, 0, 6, 4);
                squiggly_brush.ViewportUnits = BrushMappingMode.Absolute;
                squiggly_brush.TileMode = TileMode.Tile;
                squiggly_brush.Drawing = new GeometryDrawing(null, path_pen, path_geometry);

                TextDecoration squiggly = new TextDecoration();
                squiggly.Pen = new Pen(squiggly_brush, 6);

                this.TextRange.ApplyPropertyValue(Inline.TextDecorationsProperty, new TextDecorationCollection(new List<TextDecoration> { squiggly }));

            }
        }
    }
}
