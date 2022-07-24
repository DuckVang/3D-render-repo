using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
namespace ConsoleApp23
{
    class Program
    {

        public static int renderWidth = 500;
        public static int renderHeigth = 150;

        public static char[] levels;

        static Object obj;

        static Raster raster = new Raster();
        static Render render = new Render();

        static float differnceX;
        static float differnceY;


        static float scaleIncrease;
        static string chosenPath;
        static void Main()
        {


            ConsoleHelper.SetCurrentFont("Consolas", 16);
            Console.SetWindowSize(Console.LargestWindowWidth * 3 / 4, Console.LargestWindowHeight * 3 / 4);
            Console.SetBufferSize(Console.LargestWindowWidth * 3 / 4, Console.LargestWindowHeight * 3 / 4);
            Console.WriteLine(GraphicInterface.img);
            Console.ReadLine();


            int chose;
            string[] samples = Directory.GetFiles(".\\Resources\\samples");

            while (true)
            {
                Console.Clear();
                try
                {

                    for (int i = 0; i < samples.Length; i++)
                    {
                        Console.WriteLine(i + 1 + ":" + " " + Path.GetFileName(samples[i]).PadLeft(2));
                    }


                    Console.SetCursorPosition(1, Console.WindowHeight - 10);
                    Console.WriteLine("W - UP scale");
                    Console.WriteLine("S - DOWN scale");
                    Console.WriteLine("A,D - rotationX");
                    Console.WriteLine("T,G - rotationZ");
                    Console.WriteLine();

                    Console.WriteLine("choose:");
                    string choose = Console.ReadLine();
                    Int32.TryParse(choose, out chose);

                    chosenPath = samples[chose - 1];
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("try again");
                    Thread.Sleep(500);


                    continue;
                }
                break;
            }


            init();
            obj = LoadObj(chosenPath);

            float maxX = obj.vertices.Max(i => i.x);
            float minX = obj.vertices.Min(i => i.x);
            differnceX = Math.Abs(minX - maxX);

            float maxY = obj.vertices.Max(i => i.y);
            float minY = obj.vertices.Min(i => i.y);
            differnceY = Math.Abs(minY - maxY);

            scaleIncrease = 1;

            if (renderWidth / differnceX < renderHeigth / differnceY)
            {
                scaleIncrease = differnceX / renderWidth;
                obj.Scale(differnceX * renderWidth / 2 / differnceX * 100, false);
            }
            else
            {
                scaleIncrease = differnceY / renderHeigth;
                obj.Scale(differnceY * renderHeigth / 2 / differnceY * 100, false);
            }


            Thread input = new Thread(GetInput);
            input.Start();


            render.RenderImg(raster.DrawObj(obj));

            while (true)
            {

                renderWidth = Console.WindowWidth;
                renderHeigth = Console.WindowHeight;

                render.RenderImg(raster.DrawObj(obj));

            }


        }
        public static void GetInput()
        {
            while (true)
            {


                ConsoleKeyInfo key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.W:
                        obj.Scale(1.1f, false, true);
                        break;
                    case ConsoleKey.A:
                        obj.Rotate(-1);
                        break;
                    case ConsoleKey.S:
                        obj.Scale(0.9f, false, true);
                        break;
                    case ConsoleKey.D:
                        obj.Rotate(1);
                        break;
                    case ConsoleKey.T:
                        obj.Rotate(0, 1);
                        break;
                    case ConsoleKey.G:
                        obj.Rotate(0, -1);
                        break;
                    case ConsoleKey.Z:
                        obj.Rotate(0, 0, 1);
                        break;
                    case ConsoleKey.H:
                        obj.Rotate(0, 0, -1);
                        break;
                    default:
                        break;
                }


            }
        }


        public static void init()
        {
            Console.CursorVisible = false;

            ConsoleHelper.SetCurrentFont("Consolas", 5);
            Console.SetWindowSize(renderWidth, renderHeigth + 15);
            Console.SetBufferSize(renderWidth, renderHeigth + 15);

            levels = new char[256];
            using (StreamReader r = new StreamReader(@".\\Resources\\levels.txt"))
            {
                int index = 0;
                while (!r.EndOfStream && index < 256)
                {
                    string e1 = r.ReadLine();
                    levels[index] = Char.Parse(e1);
                    index++;
                }
            }
        }
        public static Object LoadObj(string file)
        {


            List<Object.Vertex> loadedNormals = new List<Object.Vertex>();

            Object obj = new Object();

            using (StreamReader r = new StreamReader(file))
            {
                while (!r.EndOfStream)
                {
                    string line = r.ReadLine();
                    if (line != "")
                    {
                        string[] data = line.Split(' ');
                        switch (data[0])
                        {
                            // v -3.996950 0.871093 5.478993
                            case "v":

                                Object.Vertex verticle = new Object.Vertex(float.Parse(data[1], CultureInfo.InvariantCulture), float.Parse(data[2], CultureInfo.InvariantCulture), float.Parse(data[3], CultureInfo.InvariantCulture));
                                obj.orgVertices.Add(verticle);
                                break;
                            // vn -1.0000 0.0000 0.0000
                            case "vn":

                                Object.Vertex normal = new Object.Vertex(float.Parse(data[1], CultureInfo.InvariantCulture), float.Parse(data[2], CultureInfo.InvariantCulture), float.Parse(data[3], CultureInfo.InvariantCulture));
                                obj.orgNormals.Add(normal);
                                break;
                            // f 1/1/1 2/2/1 4/3/1 3/4/1 : ignore vt (verticle texture)
                            case "f":

                                data = data.Skip(1).ToArray();

                                // index start from 1

                                List<int> connectID = new List<int>();

                                int normalID = 0;

                                foreach (var item in data)
                                {
                                    string[] parameters = item.Split('/');
                                    connectID.Add(-1 + int.Parse(parameters[0], CultureInfo.InvariantCulture));
                                    normalID = -1 + int.Parse(parameters[2], CultureInfo.InvariantCulture);
                                }

                                Object.Face triangle = new Object.Face(connectID[0], connectID[1], connectID[2], normalID);
                                obj.faces.Add(triangle);

                                break;

                            default:
                                break;
                        }


                    }





                }

            }


            obj.vertices = new List<Object.Vertex>(obj.orgVertices);
            obj.normals = new List<Object.Vertex>(obj.orgNormals);
            return (obj);

        }
    }
}