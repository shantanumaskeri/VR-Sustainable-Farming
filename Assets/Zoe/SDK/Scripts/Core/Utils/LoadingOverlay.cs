using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class LoadingOverlay : MonoBehaviour
{
    private bool fading;
    private float fade_timer;

    public float in_alpha = 1.0f;
    public float out_alpha = 0.0f;

    [SerializeField]
    Color from_color = Color.black;
    [SerializeField]
    Color to_color = Color.black;    
    private Material material;

    void Start()
    {
        ReverseNormals(gameObject);
        fade_timer = 0;
        material = gameObject.GetComponent<Renderer>().material;       
    }

    void Update()
    {
        if (!fading)
            return;

        fade_timer += Time.deltaTime;
        material.color = Color.Lerp(from_color, to_color, fade_timer);
        if (material.color == to_color)
        {
            fading = false;
            fade_timer = 0;
        }
    }

    public void FadeOut()
    {
        if (material==null)
        {
            material = this.gameObject.GetComponent<Renderer>().material;
        }
        // Fade the overlay to `out_alpha`.
        from_color.a = in_alpha;
        to_color.a = out_alpha;
        if (to_color != material.color)
        {
            fading = true;
        }
    }

    public void FadeIn()
    {
        if (material == null)
        {
            this.material = this.gameObject.GetComponent<Renderer>().material;
        }
        // Fade the overlay to `in_alpha`.
        from_color.a = out_alpha;
        to_color.a = in_alpha;
        if (to_color != material.color)
        {
            fading = true;
        }
    }

    public static void ReverseNormals(GameObject gameObject)
    {
        // Renders interior of the overlay instead of exterior.
        // Included for ease-of-use. 
        MeshFilter filter = gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
        if (filter != null)
        {
            Mesh mesh = filter.mesh;
            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -normals[i];
            }
            mesh.normals = normals;

            for (int m = 0; m < mesh.subMeshCount; m++)
            {
                int[] triangles = mesh.GetTriangles(m);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                mesh.SetTriangles(triangles, m);
            }
        }
    }
}
