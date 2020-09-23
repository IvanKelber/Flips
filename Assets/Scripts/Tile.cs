using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;

public class Tile : MonoBehaviour, ITappable
{

    private Color color = Color.white;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;    
    private float cellWidth = 1;
    private float cellHeight = 1;

    private BoxCollider collider;

    [SerializeField]
    private Vector2GameEvent onTapped;

    public Vector3 Position {
        get {
            return transform.position;
        }
        set {
            transform.position = value;
        }
    }

    private Vector2Int index;
    public Vector2Int Index {
        get {
            return index;
        }
        set {
            index = value;
        }
    }

    public Material material;

    [HideInInspector]
    public AudioSource audioSource;

    private void Awake()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        collider = gameObject.AddComponent<BoxCollider>();
        audioSource = GetComponent<AudioSource>();
        if(audioSource == null) {
            Debug.LogWarning("Audio source is not attached to tile prefab");
        }
    }

    public void Initialize(float cellWidth, float cellHeight, Color color, Vector2Int index) {
        this.cellWidth = cellWidth;
        this.cellHeight = cellHeight;
        this.color = color;
        Index = index;
    }

    private void Start() {
        Render(transform.position);
        collider.size = new Vector3(cellWidth, cellHeight, 1);
    }

    public void Destroy() {
        Destroy(this.gameObject);
    }
 
    public void SetColor(Color color) {
        this.color = color;
        meshRenderer.material.SetColor("_MainColor", color);
    }
    
    public Color GetColor() {
        return this.color;
    }

    public void Render(Vector3 position)
    {
        meshRenderer.sharedMaterial = material;
        meshRenderer.material.SetColor("_MainColor", color);
        meshFilter.mesh = new Mesh();

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        mesh.vertices = GetVertices(position);
        mesh.triangles = GetTriangles();
        Vector3[] normals = new Vector3[4]
        {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
        };
        mesh.uv = uv;
    }
    private Vector3[] GetVertices(Vector3 position)
    {
        Vector3[] vertices = new Vector3[4]
        {
                transform.InverseTransformPoint(new Vector3(position.x - cellWidth/2, position.y - cellHeight/2, 1)),
                transform.InverseTransformPoint(new Vector3(position.x + cellWidth/2, position.y - cellHeight/2, 1)),
                transform.InverseTransformPoint(new Vector3(position.x - cellWidth/2, position.y + cellHeight/2, 1)),
                transform.InverseTransformPoint(new Vector3(position.x + cellWidth/2, position.y + cellHeight/2, 1))
        };
        return vertices;
    }

    private int[] GetTriangles()
    {
        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        return tris;
    }

    public void OnTap() {
        onTapped.Raise(new Vector2(Index.x, Index.y));
    }

    public override string ToString() {
        return "Tile: " + Index + ", " + color;
    }
}
