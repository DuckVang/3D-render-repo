using System;
using System.IO;
using System.Linq;

namespace ConsoleApp23
{
    class Render
    {

        Stream s = Console.OpenStandardOutput();

        byte[,] lastRender;
        int countRender = 0;
        public void RenderImg(byte[,] img)
        {
            countRender++;
            // anti-epilepsy mode
            if (lastRender == null) lastRender = img;

            if (lastRender.Cast<byte>().SequenceEqual(img.Cast<byte>())) return;
            else lastRender = img;
            //


            Console.Clear();

            byte[] bufImg = new byte[Program.renderWidth * Program.renderHeigth];

            for (int x = 0; x < img.GetLength(0); x++)
            {
                for (int y = 0; y < img.GetLength(1); y++)
                {
                    bufImg[x + y * Program.renderWidth] = img[x, y];
                }
            }

            s.Write(bufImg, 0, bufImg.Length);

        }


    }
}
