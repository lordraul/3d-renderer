using UnityEngine;

public class CustomMeshRenderer : MonoBehaviour
{
    public CustomMesh Mesh;
    public Shader Shader = new LambertianShader(new Vector3(1, 1, 1).normalized);
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale = Vector3.one;

    void Start()
    {
        Mesh = CustomMesh.Sphere(8, 16);
        RenderEngine.Instance.QueueRender(this);
    }

    void Update()
    {
        Mesh.Position = position;
        Mesh.Rotation = rotation * Mathf.Deg2Rad;
        Mesh.Scale = scale;
    }
}
