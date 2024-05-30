using UnityEngine;

public class CustomMeshRenderer : MonoBehaviour
{
    public CustomMesh Mesh = CustomMesh.Cube;
    public Shader Shader = new UnlitShader(Color.white);
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale = Vector3.one;

    void Start()
    {
        RenderEngine.Instance.QueueRender(this);
    }

    void Update()
    {
        Mesh.Position = position;
        Mesh.Rotation = rotation * Mathf.Deg2Rad;
        Mesh.Scale = scale;
    }
}
