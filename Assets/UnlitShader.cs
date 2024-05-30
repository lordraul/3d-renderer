using UnityEngine;

public class UnlitShader : Shader
{
    public Color _Color;

    public override QueueType RenderQueue() => QueueType.Opaque;
    public override bool ZWrite() => true;
    public override bool ZTest() => true;

    public UnlitShader(Color color)
    {
        _Color = color;
    }

    public override v2f vert(VertexData v)
    {
        v2f o = new();
        o.position = v.Position;
        o.color = v.VertColor;
        return o;
    }

    public override Color frag(v2f i)
    {
        return i.color;
    }
}