using UnityEngine;

public abstract class Shader
{
    public abstract QueueType RenderQueue();
    public abstract bool ZWrite();
    public abstract bool ZTest();

    public abstract v2f vert(VertexData v);

    public abstract Color frag(v2f i);
}

public struct v2f
{
    public Vector3 position;
    public Color color;
    public Vector3 normal;
    public Vector2 uv;

    public static v2f operator *(float n, v2f a)
    {
        a.position *= n;
        a.color *= n;
        a.normal *= n;
        a.uv *= n;
        return a;
    }

    public static v2f operator +(v2f a, v2f b)
    {
        a.position += b.position;
        a.color += b.color;
        a.normal += b.normal;
        a.uv += b.uv;
        return a;
    }
}
