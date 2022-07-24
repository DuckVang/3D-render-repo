namespace ConsoleApp23
{
    using System;
    class Raster
    {





        static int planeDistance = 100;

        // Camera/View direction
        static Object.Vertex E = new Object.Vertex(0, 0, 100); 
        // Light direction
        static Object.Vertex P = new Object.Vertex(0, 0, 100);
        // dot product of E and P
        static Object.Vertex V = ObjectsTools.Normalize(ObjectsTools.GetDotProduct(E, P));





        float[,] zBuffer;
        byte[,] img;






        int trianglecount = 0;
        public byte[,] DrawObj(Object obj)
        {
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            zBuffer = new float[Program.renderWidth, Program.renderHeigth];
            img = new byte[Program.renderWidth, Program.renderHeigth];

            zBuffer = ObjectsTools.FillZbuffer(zBuffer, -1000);
            img = ObjectsTools.FillOutput(img, 32);


            //Object.Verticle[] verticles = obj.verticles.ToArray()/*new Object.Verticle[obj.verticles.Count]*/;
            //obj.verticles.CopyTo(verticles);
            Object.Vertex[] newVertices = new Object.Vertex[obj.vertices.Count];
            obj.vertices.CopyTo(newVertices);

            Object.Vertex[] newNormals = new Object.Vertex[obj.normals.Count];
            obj.normals.CopyTo(newNormals);
            int count = 0;
            foreach (var item in obj.faces)
            {
                img = DrawTriangle(img, item, newVertices, newNormals);
                //Console.Write(" " + count++ + " " + obj.faces.Count);
            }

            stopWatch.Stop();

            Console.CursorVisible = false;

            double time = Convert.ToDouble(stopWatch.ElapsedMilliseconds);
            

            Console.SetCursorPosition(2, 2);
            Console.Write(time);

            return img;
        }

        public byte[,] DrawTriangle(byte[,] img, Object.Face face, Object.Vertex[] vertices, Object.Vertex[] normals)
        {
            ////int[,] zBuffer = new int[renderWidth, renderHeigth];
            ////zBuffer = ObjectsTools.FilledZbuffer(zBuffer, -100);
            trianglecount++;
            byte[,] newImg = img;

            Object.Vertex[] P = new Object.Vertex[3];
            int count = 0;
            foreach (var vertexID in face.vertexIDs)
            {
                P[count++] = vertices[vertexID];
            }
            // Sort the points so that y0 <= y1 <= y2
            if (P[1].y < P[0].y) { ObjectsTools.SwapVerticles(ref P[1], ref P[0]); };
            if (P[2].y < P[0].y) { ObjectsTools.SwapVerticles(ref P[2], ref P[0]); };
            if (P[2].y < P[1].y) { ObjectsTools.SwapVerticles(ref P[2], ref P[1]); };

            float x0 = P[0].x, y0 = P[0].y, z0 = P[0].z;
            float x1 = P[1].x, y1 = P[1].y, z1 = P[1].z;
            float x2 = P[2].x, y2 = P[2].y, z2 = P[2].z;

            // Compute the x and y coordinates of the triangle edges
            // return array of x and y coordinates based on y 
            Object.Vertex[] x01 = ObjectsTools.Interpolate(x0, y0, x1, y1);
            Object.Vertex[] x12 = ObjectsTools.Interpolate(x1, y1, x2, y2);
            Object.Vertex[] x02 = ObjectsTools.Interpolate(x0, y0, x2, y2);

            // return array of z("x") and y coordinates based on y
            Object.Vertex[] z01 = ObjectsTools.Interpolate(z0, y0, z1, y1);
            Object.Vertex[] z12 = ObjectsTools.Interpolate(z1, y1, z2, y2);
            Object.Vertex[] z02 = ObjectsTools.Interpolate(z0, y0, z2, y2);

            // Concatenate the short sides
            Array.Resize(ref x01, x01.Length - 1);
            Array.Resize(ref z01, z01.Length - 1);

            //merge 2 short sides
            Object.Vertex[] x012 = new Object.Vertex[x01.Length + x12.Length];

            Array.Copy(x01, x012, x01.Length);
            Array.Copy(x12, 0, x012, x01.Length, x12.Length);

            Object.Vertex[] z012 = new Object.Vertex[z01.Length + z12.Length];

            Array.Copy(z01, z012, z01.Length);
            Array.Copy(z12, 0, z012, z01.Length, z12.Length);

            // Determine which is left and which is right
            Object.Vertex[] xLeft;
            Object.Vertex[] xRight;

            // zleft.x/zright.x is z - I was too lazy
            Object.Vertex[] zLeft;
            Object.Vertex[] zRight;

            int m = x012.Length / 2;
            if (x02[m].x < x012[m].x)
            {
                xLeft = x02;
                xRight = x012;

                zLeft = z02;
                zRight = z012;
            }
            else
            {
                xLeft = x012;
                xRight = x02;

                zLeft = z012;
                zRight = z02;
            }

            // compute brightness 
            Object.Vertex N = /*face.normalVector*/ ObjectsTools.Normalize(normals[face.normalID]);

            Object.Vertex dotProduct = ObjectsTools.GetDotProduct(N, V);
            float facingRatio = Math.Max(0, dotProduct.x + dotProduct.y + dotProduct.z);


            int level = (int)Math.Round(Math.Clamp(facingRatio / 1 * 255, 0, 255));

            // Draw the horizontal segments
            for (float i = y0; i < y2 ; i += 0.5f)
            {
                // zleft/zright.x is z - I was too lazy
                float zStart = zLeft[(int)Math.Round(i - y0)].x;
                float xStart = xLeft[(int)Math.Round(i - y0)].x;

                float zEnd = zRight[(int)Math.Round(i - y0)].x;
                float xEnd = xRight[(int)Math.Round(i - y0)].x;

                Object.Vertex[] zArray = ObjectsTools.Interpolate(zStart, xStart, zEnd, xEnd);

                for (float s = xStart; s < xEnd; s += 0.5f)
                {

                    // from cartisan coordination format to console

                    float x = s;
                    float y = i;
                    float z = zArray[(int)Math.Round(s - xStart)].x;

                    // orthographic 

                    float objXDistance = x /*+ E[0, 0]*/;
                    float objYDistance = y /*+ E[1, 0]*/;
                    float objZDistance = -z  /*planeDistance*/ +E.z ;

                    int xp = (int)Math.Round((planeDistance   * objXDistance )/objZDistance + Program.renderWidth / 2);
                    int yp = (int)Math.Round(-((planeDistance  * objYDistance)/ objZDistance) + Program.renderHeigth / 2);


                    // out of range
                    if (xp < 0 || yp >= Program.renderHeigth || xp >= Program.renderWidth || yp < 0) continue;
                    if ( z > zBuffer[xp, yp] /*&& zDepth < 0*/)
                    {

                        newImg[xp, yp] = (byte)Convert.ToInt32(Program.levels[level])/*(byte)21*/;
                        zBuffer[xp, yp] = z;
                    }



                    continue;

                }


            }

            return newImg;





        }




    }
}
