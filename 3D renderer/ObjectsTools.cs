
using System;
using System.Collections.Generic;
using System.Linq;
namespace ConsoleApp23
{
    public static class ObjectsTools
    {
        #region Map
        public static float[,] FillZbuffer(float[,] zBuffer, float val)
        {

            for (int i = 0; i < zBuffer.GetLength(0); i++)
            {
                for (int s = 0; s < zBuffer.GetLength(1); s++)
                {
                    zBuffer[i, s] = val;
                }
            }
            return zBuffer;
        }
        public static byte[,] FillOutput(byte[,] output, byte val)
        {

            for (int i = 0; i < output.GetLength(0); i++)
            {
                for (int s = 0; s < output.GetLength(1); s++)
                {
                    output[i, s] = val;
                }
            }
            return output;

        }
        #endregion
        #region Rotation
        public static float[,] RotateByX(float degree)
        {
            degree *= 180 / (float)Math.PI;
            float sin = Convert.ToInt16(Math.Sin(degree));
            float cos = Convert.ToInt16(Math.Cos(degree));
            float[,] RotationXAxis = new float[,]
             {
             { 1,   0,   0},
             { 0, cos, sin},
             { 0,-sin, cos}
             };

            return RotationXAxis;
        }
        public static float[,] RotateByY(float degree)
        {
            degree *= 180 / (float)Math.PI;
            float sin = Convert.ToInt16(Math.Sin(degree));
            float cos = Convert.ToInt16(Math.Cos(degree));
            float[,] RotationYAxis = new float[,]
             {
             { cos, 0, sin},
             {0, 1, 0},
             {   -sin,   0, cos}
             };
            return RotationYAxis;
        }
        public static float[,] RotateByZ(float degree)
        {
            degree *= 180 / (float)Math.PI;
            float sin = Convert.ToInt16(Math.Sin(degree));
            float cos = Convert.ToInt16(Math.Cos(degree));
            float[,] RotationZAxis = new float[,]
             {
             { cos, sin, 0},
             {-sin, cos, 0},
             {   0,   0, 1}
             };
            return RotationZAxis;
        }

        #endregion

        public static Object.Vertex MatrixMultiply(Object.Vertex verticle, float[,] matrix)
        {
            Object.Vertex newVerticle;
            newVerticle.x = verticle.x * matrix[0, 0] + verticle.y * matrix[0, 1] + verticle.z * matrix[0, 2];
            newVerticle.y = verticle.x * matrix[1, 0] + verticle.y * matrix[1, 1] + verticle.z * matrix[1, 2];
            newVerticle.z = verticle.x * matrix[2, 0] + verticle.y * matrix[2, 1] + verticle.z * matrix[2, 2];
            return newVerticle;
        }

        #region Verticle methods 
        public static void SwapVerticles(ref Object.Vertex verticle1, ref Object.Vertex verticle2)
        {
            var tempswap = verticle1;
            verticle1 = verticle2;
            verticle2 = tempswap;
        }
      

        public static Object.Vertex[] Interpolate(float x1, float y1, float x2, float y2)
        {
            List<Object.Vertex> verticles = new List<Object.Vertex>();
            for (float y = y1; y < y2 + 1; y++)
            {
                Object.Vertex verticle = new Object.Vertex();
                verticle.y = y;
                verticle.x = (y - y1) / ((y2 - y1) / (x2 - x1)) + x1;

                verticles.Add(verticle);
                //System.Console.WriteLine($"x: {verticle.x} and y: {verticle.y}");
            }

            return verticles.ToArray();

        }
        public static Object.Vertex Normalize(Object.Vertex a)
        {
            float norm = (float)System.Math.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
            return (new Object.Vertex(a.x / norm, a.y / norm, a.z / norm));
        }
        public static Object.Vertex GetDotProduct(Object.Vertex a, Object.Vertex b)
        {

            return (new Object.Vertex(  a.x * b.x, a.y * b.y, a.z * b.z ));
        }
        #endregion 
       





    }


}
