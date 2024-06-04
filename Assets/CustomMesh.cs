using UnityEngine;
using System.Collections.Generic;

public class CustomMesh
{
    public VertexData[] vertices;
    public (int, int, int)[] triangles;

    Matrix4x4 translationMatrix, rotationMatrix, scaleMatrix;

    public Matrix4x4 transformMatrix => translationMatrix * rotationMatrix * scaleMatrix;

    Vector3 position;
    public Vector3 Position
    {
        get { 
            return position;
        }
        set {
            position = value;

            translationMatrix = Matrix4x4.identity;
            translationMatrix[0,3] = value.x;
            translationMatrix[1,3] = value.y;
            translationMatrix[2,3] = value.z;
        }
    }

    Vector3 scale;
    public Vector3 Scale
    {
        get {
            return scale;
        }
        set {
            scale = value;

            scaleMatrix[0,0] = value.x;
            scaleMatrix[1,1] = value.y;
            scaleMatrix[2,2] = value.z;
            scaleMatrix[3,3] = 1;
        }
    }

    Vector3 rotation;
    public Vector3 Rotation
    {
        get {
            return rotation;
        }
        set {
            rotation = value;

            float xSin = Mathf.Sin(value.x);
            float xCos = Mathf.Cos(value.x);
            float ySin = Mathf.Sin(value.y);
            float yCos = Mathf.Cos(value.y);
            float zSin = Mathf.Sin(value.z);
            float zCos = Mathf.Cos(value.z);

            Matrix4x4 xRot = new Matrix4x4(
                new Vector4(1, 0, 0, 0),
                new Vector4(0, xCos, -xSin, 0),
                new Vector4(0, xSin, xCos, 0),
                new Vector4(0, 0, 0, 1));
            Matrix4x4 yRot = new Matrix4x4(
                new Vector4(yCos, 0, ySin, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(-ySin, 0, yCos, 0),
                new Vector4(0, 0, 0, 1));
            Matrix4x4 zRot = new Matrix4x4(
                new Vector4(zCos, -zSin, 0, 0),
                new Vector4(zSin, zCos, 0, 0),
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 0, 0, 1));

            rotationMatrix = zRot * yRot * xRot;
        }
    }

    public CustomMesh(VertexData[] vertices, (int, int, int)[] triangles)
    {
        this.vertices = vertices;
        this.triangles = triangles;

        Position = Vector3.zero;
        Rotation = Vector3.zero;
        Scale = Vector3.one;
    }

    // based on https://www.desmos.com/3d/v6fxs4thwt
    public static CustomMesh Sphere(int vertRes, int radRes)
    {
        List<VertexData> vertices = new List<VertexData>();
        List<(int,int,int)> triangles = new List<(int,int,int)>();

        for(int V = 1; V < vertRes; V++)
        {
            float v = V * 2 * Mathf.PI / vertRes;
            float sinV = Mathf.Sin(v / 2f);
            float cosV = Mathf.Cos(v / 2f);

            for(int R = 0; R <= radRes; R++)
            {
                float r = R * 2 * Mathf.PI / radRes;
                vertices.Add(new VertexData(new Vector3(sinV * Mathf.Sin(r), sinV * Mathf.Cos(r), -cosV), new Vector3(), new Color(1f, 1f, 1f)));
            }
        }

        vertices.Add(new VertexData(new Vector3(0, 0, 1), new Vector3(), new Color(1f, 1f, 1f)));
        vertices.Add(new VertexData(new Vector3(0, 0, -1), new Vector3(), new Color(1f, 1f, 1f)));

        for(int v = 0; v < vertRes - 2; v++)
        {
            for(int r = 0; r < radRes; r++)
            {
                triangles.Add(((r + 1) % radRes + v * radRes, r + v * radRes, r + radRes * (v + 1)));
                triangles.Add(((r + 1) % radRes + v * radRes, r + v * radRes, (r + 1) % radRes + radRes * (v + 1)));
                triangles.Add(((r + 1) % radRes + vertices.Count - radRes - 2, r + vertices.Count - radRes - 2, vertices.Count - 2));
                triangles.Add((r, (r + 1) % radRes, vertices.Count - 1));
            }
        }

        var triNorms = new Vector3[triangles.Count];
        for(int i = 0; i < triangles.Count; i++)
        {
            (int a, int b, int c) = triangles[i];
            Vector3 v = vertices[b].Position - vertices[a].Position;
            Vector3 w = vertices[c].Position - vertices[a].Position;

            triNorms[i] = Vector3.Cross(v, w).normalized;
        }

        for(int i = 0; i < vertices.Count; i++)
        {
            Vector3 sumNormals = new Vector3();
            int numNormals = 0;
            for(int j = 0; j < triangles.Count; j++)
            {
                (int a, int b, int c) = triangles[j];
                
                if(a != i && b != i && c != i)
                    continue;

                sumNormals += triNorms[j];
                numNormals++;
            }
            
            vertices[i].Normal = sumNormals / numNormals;
        }

        return new CustomMesh(vertices.ToArray(), triangles.ToArray());
    }

    public static CustomMesh Cube = new CustomMesh(
        new VertexData[]{
            new VertexData(new Vector3(+0.5f, +0.5f, +0.5f), new Vector3(), new Color(1f, 1f, 1f)),
            new VertexData(new Vector3(-0.5f, +0.5f, +0.5f), new Vector3(), new Color(0f, 1f, 1f)),
            new VertexData(new Vector3(-0.5f, -0.5f, +0.5f), new Vector3(), new Color(0f, 0f, 1f)),
            new VertexData(new Vector3(+0.5f, -0.5f, +0.5f), new Vector3(), new Color(1f, 0f, 1f)),
            new VertexData(new Vector3(+0.5f, +0.5f, -0.5f), new Vector3(), new Color(1f, 1f, 0f)),
            new VertexData(new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(), new Color(0f, 1f, 0f)),
            new VertexData(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(), new Color(0f, 0f, 0f)),
            new VertexData(new Vector3(+0.5f, -0.5f, -0.5f), new Vector3(), new Color(1f, 0f, 0f)),
        },
        new (int, int, int)[]{
            (0, 1, 5),
            (4, 0, 5),

            (1, 2, 6),
            (5, 1, 6),

            (2, 3, 7),
            (6, 2, 7),

            (3, 0, 4),
            (7, 3, 4),

            (1, 0, 3),
            (2, 1, 3),

            (4, 5, 6),
            (4, 6, 7),
        }
    );
}

public class VertexData
{
    public Vector3 Position { get; set; }
    public Vector3 Normal { get; set; }
    public Color VertColor { get; set; }

    public VertexData(Vector3 pos, Vector3 normal, Color color)
    {
        Position = pos;
        Normal = normal;
        VertColor = color;
    }
}
