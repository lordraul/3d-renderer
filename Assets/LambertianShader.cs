using UnityEngine;

public class LambertianShader : Shader
{
    public Vector3 _Light;

    public override QueueType RenderQueue() => QueueType.Opaque;
    public override bool ZWrite() => true;
    public override bool ZTest() => true;

    public LambertianShader(Vector3 light)
    {
        _Light = light;
    }

    public override v2f vert(VertexData v)
    {
        v2f o = new();
        o.position = v.Position;
        o.normal = v.Normal;
        return o;
    }

    public override Color frag(v2f i)
    {
        float c = Mathf.Max(0, Vector3.Dot(i.normal.normalized, _Light));
        return new Color(c, c, c);
    }
}