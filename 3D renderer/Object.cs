namespace ConsoleApp23
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public class Object
    {
        public List<Vertex> orgVertices = new List<Vertex>();
        public List<Vertex> orgNormals = new List<Vertex>();

        public List<Vertex> vertices;
        public List<Vertex> normals;

        public List<Face> faces = new List<Face>();

        float scaleIncrease = 100;
        float rotationX = 0;
        float rotationY = 0;
        float rotationZ = 0;
        public Object()
        {
            vertices = new List<Vertex>(orgVertices);
            normals = new List<Vertex>(orgNormals);

        }

        public struct Face
        {

            public int normalID;
            public int[] vertexIDs;
            public Face(int v1, int v2, int v3, int normalVector)
            {
                vertexIDs = new int[3];
                vertexIDs[0] = v1;
                vertexIDs[1] = v2;
                vertexIDs[2] = v3;

                normalID = normalVector;

            }
        }
        public struct Vertex
        {
            public float x, y, z;
            public Vertex(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;

            }
        }

        public void Scale(float scaleIncrease, bool addScale = true, bool manual = false)
        {


           
            float scale = 1;
            Vertex[] newVerticles;
           
            newVerticles = new Vertex[vertices.Count];
            vertices.CopyTo(newVerticles);
            if (manual)
            {
                scale = scale * scaleIncrease;


            }
            else
            {
                if (addScale)
                { this.scaleIncrease += scaleIncrease; }

                else this.scaleIncrease = scaleIncrease;
                scale = this.scaleIncrease / 100;


            }





            int count = 0;
            foreach (var verticle in newVerticles)
            {

                float x = verticle.x * scale;
                float y = verticle.y * scale;
                float z = verticle.z * scale;
                newVerticles[count] = new Vertex(x, y, z);
                count++;
            }
            vertices.Clear();
            vertices.AddRange(newVerticles);

        }
        public void Rotate(float xDeg = 0, float zDeg = 0, float yDeg = 0)
        {





            rotationX += xDeg;
            rotationY += yDeg;
            rotationZ += zDeg;

            Vertex[] newVertices = new Vertex[orgVertices.Count];
            orgVertices.CopyTo(newVertices);


            int count = 0;
            foreach (var vertex in newVertices)
            {

                float x = vertex.x;
                float y = vertex.y;
                float z = vertex.z;
                Vertex xRotation = ObjectsTools.MatrixMultiply(new Vertex(x, y, z), ObjectsTools.RotateByX(rotationX));
                Vertex xyRotation = ObjectsTools.MatrixMultiply(xRotation, ObjectsTools.RotateByZ(rotationY));
                Vertex xyzRotation = ObjectsTools.MatrixMultiply(xyRotation, ObjectsTools.RotateByY(rotationZ));
                Vertex newVertex = xyzRotation;
                newVertices[count] = newVertex;
                count++;

            }


            Vertex[] newNormals = new Vertex[orgNormals.Count];
            orgNormals.CopyTo(newNormals);
            count = 0;
            foreach (var normal in newNormals)
            {
                float x = normal.x;
                float y = normal.y;
                float z = normal.z;
                Vertex xRotation = ObjectsTools.MatrixMultiply(new Vertex(x, y, z), ObjectsTools.RotateByX(rotationX));
                Vertex xzRotation = ObjectsTools.MatrixMultiply(xRotation, ObjectsTools.RotateByZ(rotationZ));
                newNormals[count] = xzRotation;
                count++;
            }


            //Translate(origin.x, origin.y, origin.z);

            vertices.Clear();
            vertices.AddRange(newVertices);

            normals.Clear();
            normals.AddRange(newNormals);

            Scale(scaleIncrease, false);
        }
        public void Translate(float x = 0, float y = 0, float z = 0)
        {

            Vertex[] newVertices = new Vertex[vertices.Count];
            vertices.CopyTo(newVertices);
            int count = 0;
            foreach (var vertex in newVertices)
            {
                float newX = vertex.x + x;
                float newY = vertex.y + y;
                float newZ = vertex.z + z;

                newVertices[count] = new Vertex(newX, newY, newZ);
                count++;
            }


            Vertex[] newNormals = new Vertex[normals.Count];
            normals.CopyTo(newNormals);
            count = 0;
            foreach (var normal in newNormals)
            {
                float newX = normal.x + x;
                float newY = normal.y + y;
                float newZ = normal.z + z;

                newNormals[count] = new Vertex(newX, newY, newZ);
                count++;
            }

            vertices.Clear();
            vertices.AddRange(newVertices);

            normals.Clear();
            normals.AddRange(newNormals);
        }
        public void TranslateToOrigin()
        {
            Object.Vertex[] newVertices = new Object.Vertex[orgVertices.Count];
            orgVertices.CopyTo(newVertices);


            float maxX = newVertices.Max(i => i.x);
            float minX = newVertices.Min(i => i.x);

            float differenceX = Math.Abs(maxX) - Math.Abs(minX) / 2;

            float maxY = newVertices.Max(i => i.y);
            float minY = newVertices.Min(i => i.y);

            float differenceY = Math.Abs(maxY) - Math.Abs(minY) / 2;

            float maxZ = newVertices.Max(i => i.z);
            float minZ = newVertices.Min(i => i.z);

            float differenceZ = Math.Abs(maxZ) - Math.Abs(minZ) / 2;

            Translate(-differenceX, differenceY, differenceZ);

        }

    }
}
