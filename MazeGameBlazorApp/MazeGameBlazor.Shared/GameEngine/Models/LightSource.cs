using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGameBlazor.Shared.GameEngine.Models
{
    public class LightSource
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Radius { get; set; }

        public LightSource(int x, int y, int radius)
        {
            X = x;
            Y = y;
            Radius = radius;
        }
    }
}
