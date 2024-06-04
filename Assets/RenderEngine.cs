using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderEngine : MonoBehaviour
{
    public RawImage Frame;

    public float FOV { get; private set; } = 60;
    public Vector3 CameraPosition { get; private set; } = new Vector3(0, -5, 0);
    // public Vector3 CameraOrientation { get; private set; } = new Vector3(0, 0, 0);

    public Vector3 ProjectionPlane { get; private set; }

    public int Width { get; private set; } = 160;
    public int Height { get; private set; } = 90;

    public Texture2D FrameBuffer { get; private set; }
    public Texture2D DepthBuffer { get; private set; }

    public static RenderEngine Instance { get; private set; }

    private List<CustomMeshRenderer> RenderQueue = new();

    public RenderEngine()
    {
        float focalLength = Width / (2f * Mathf.Tan(FOV * Mathf.Deg2Rad / 2f));
        ProjectionPlane = new Vector3(Width / 2, Height / 2, focalLength);
    }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        CreateTextures();
    }

    void Update()
    {
        RenderFrame();

        FrameBuffer.Apply();
        Frame.texture = FrameBuffer;
    }

    void CreateTextures()
    {
        FrameBuffer = new Texture2D(Width, Height);
        FrameBuffer.filterMode = FilterMode.Point;
        DepthBuffer = new Texture2D(Width, Height);
    }

    public void QueueRender(CustomMeshRenderer renderer)
    {
        for(int i = 0; i < RenderQueue.Count; i++)
        {
            if(renderer.Shader.RenderQueue() < RenderQueue[i].Shader.RenderQueue())
            {
                RenderQueue.Insert(i, renderer);
                return;
            }
        }
        
        RenderQueue.Add(renderer);
    }

    void RenderFrame()
    {
        FrameBuffer = new Texture2D(Width, Height);

        foreach(CustomMeshRenderer renderer in RenderQueue)
        {
            v2f[] interpolators = new v2f[renderer.Mesh.vertices.Length];
            Vector2[] projections = new Vector2[renderer.Mesh.vertices.Length];

            for(int i = 0; i < renderer.Mesh.vertices.Length; i++)
            {
                interpolators[i] = renderer.Shader.vert(renderer.Mesh.vertices[i]);
                projections[i] = ProjectVertex(renderer.Mesh.transformMatrix.MultiplyPoint3x4(interpolators[i].position));
            }

            foreach(var (t1, t2, t3) in renderer.Mesh.triangles)
            {
                Vector2 a = projections[t1];
                Vector2 b = projections[t2];
                Vector2 c = projections[t3];

                float abc = EdgeFunction(a, b, c);

                if(abc > 0)
                    continue;

                foreach(var (x, y) in RasterizeTriangle(a, b, c))
                {
                    Vector3 bary = Barycentric(a, b, c, x, y);

                    v2f interpolator = bary.x * interpolators[t1] + bary.y * interpolators[t2] + bary.z * interpolators[t3];

                    FrameBuffer.SetPixel(x, y, renderer.Shader.frag(interpolator));
                }
            }
        }
    }

    Vector2 ProjectVertex(Vector3 vert)
    {
        Vector3 camRelative = vert - CameraPosition;

        return (new Vector2(camRelative.x, camRelative.z) * ProjectionPlane.z / camRelative.y) + new Vector2(ProjectionPlane.x, ProjectionPlane.y);
    }

    float EdgeFunction(Vector2 a, Vector2 b, Vector2 c)
    {
        return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
    }

    (int, int)[] RasterizeTriangle(Vector2 a, Vector2 b, Vector2 c)
    {
        List<(int, int)> pixels = new();

        for(int x = (int)Mathf.Min(a.x, b.x, c.x); x <= Mathf.Max(a.x, b.x, c.x); x++)
        {
            for(int y = (int)Mathf.Min(a.y, b.y, c.y); y <= Mathf.Max(a.y, b.y, c.y); y++)
            {
                Vector2 p = new(x,y);
                float abp = EdgeFunction(a, b, p);
                float bcp = EdgeFunction(b, c, p);
                float cap = EdgeFunction(c, a, p);

                if(abp <= 0 && bcp <= 0 && cap <= 0)
                    pixels.Add((x, y));
            }
        }

        return pixels.ToArray();
    }

    Vector3 Barycentric(Vector2 a, Vector2 b, Vector2 c, int x, int y)
    {
        Vector3 o = new(
            ((b.y - c.y) * (x - c.x) + (c.x - b.x) * (y - c.y)) / ((b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y)),
            ((c.y - a.y) * (x - c.x) + (a.x - c.x) * (y - c.y)) / ((b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y)),
            1);
        o.z -= o.x + o.y;
        return o;
    }
}

public enum QueueType
{
    Skybox,
    Opaque,
    Transparent
}