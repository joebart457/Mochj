using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.IDE.Components.MochjEditor.Constants
{
    internal static class StatusIndicatorConstants
    {
        public const string CodeAnalysisReady = "Ready";
        public const string CodeAnalysisWorking = "Working...";

        public static class Brushes
        {
            public static readonly SolidColorBrush CodeAnalysisReady = new SolidColorBrush(Colors.DimGray);
            public static readonly SolidColorBrush CodeAnalysisWorking = new SolidColorBrush(Colors.Goldenrod);
        }

    }
}
