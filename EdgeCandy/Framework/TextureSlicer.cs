using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using SFML.Graphics;
using SFML.Window;

namespace EdgeCandy.Framework
{
    public static class TextureSlicer
    {
        public static void SliceAndDice(Vector2f startPoint, Vector2f endPoint, Texture victim, out Texture sliceA,
                                        out Texture sliceB)
        {
            //startPoint = new Vector2f(ConvertUnits.ToDisplayUnits(startPoint.X),
            //                          ConvertUnits.ToDisplayUnits(startPoint.Y));
            //endPoint =   new Vector2f(ConvertUnits.ToDisplayUnits(endPoint.X),
            //                          ConvertUnits.ToDisplayUnits(endPoint.Y));

            var input = victim.CopyToImage();
            var outputA = new Image(input.Size.X, input.Size.Y, Color.Transparent);
            var outputB = new Image(input.Size.X, input.Size.Y, Color.Transparent);

            for (uint y = 0; y < input.Size.Y; y++)
            {
                for (uint x = 0; x < input.Size.X; x++)
                {
                    var pos = new Vector2f(x, y);
                    if (WhichSideOfLine(startPoint, endPoint, pos))
                        outputA.SetPixel(x, y, input.GetPixel(x, y));
                    else
                        outputB.SetPixel(x, y, input.GetPixel(x, y));
                }
            }
            
            sliceA = new Texture(outputA);
            sliceB = new Texture(outputB);
        }

        private static bool WhichSideOfLine(Vector2f a, Vector2f b, Vector2f c)
        {
            return ((b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X)) < 0;
        }
    }
}
