using UnityEngine;

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

    public static CustomMesh Cube = new CustomMesh(
        new VertexData[]{
            new VertexData(new Vector3(+0.5f, +0.5f, +0.5f), new Vector3(), new Vector2(), new Color(1f, 1f, 1f)),
            new VertexData(new Vector3(-0.5f, +0.5f, +0.5f), new Vector3(), new Vector2(), new Color(0f, 1f, 1f)),
            new VertexData(new Vector3(-0.5f, -0.5f, +0.5f), new Vector3(), new Vector2(), new Color(0f, 0f, 1f)),
            new VertexData(new Vector3(+0.5f, -0.5f, +0.5f), new Vector3(), new Vector2(), new Color(1f, 0f, 1f)),
            new VertexData(new Vector3(+0.5f, +0.5f, -0.5f), new Vector3(), new Vector2(), new Color(1f, 1f, 0f)),
            new VertexData(new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(), new Vector2(), new Color(0f, 1f, 0f)),
            new VertexData(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(), new Vector2(), new Color(0f, 0f, 0f)),
            new VertexData(new Vector3(+0.5f, -0.5f, -0.5f), new Vector3(), new Vector2(), new Color(1f, 0f, 0f)),
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
    public Vector2 UV { get; set; }
    public Color VertColor { get; set; }

    public VertexData(Vector3 pos, Vector3 normal, Vector2 uv, Color color)
    {
        Position = pos;
        Normal = normal;
        UV = uv;
        VertColor = color;
    }
}
