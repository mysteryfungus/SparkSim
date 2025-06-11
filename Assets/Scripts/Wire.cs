using UnityEngine;
using GogoGaga.OptimizedRopesAndCables;
using System.Collections.Generic;
using DG.Tweening;

public class Wire : MonoBehaviour
{
    private Rope rope;
    private RopeMesh ropeMesh;
    public Contact start;
    public Contact end;
    [SerializeField] private float width;
    [SerializeField] private Transform midPoint;
    [SerializeField] private List<Material> wireMaterials;

    void Awake()
    {
        Debug.Log("Wire initialized");
    }
    
    public void SetStartPoint(Contact contact){
        start = contact;
        Debug.Log("Setting start point of a wire to " + start.Guid);
    }

    public void SetEndPoint(Contact contact){
        end = contact;
        Debug.Log("Setting end point of a wire to " + end.Guid);
        CreateRope();
    }

    public void RemoveConnection(){
        ConnectionManager.instance.RemoveConnection(new HashSet<Contact> { start, end });
        Debug.Log("Removing connection between " + start.Guid + " and " + end.Guid);
        Destroy(gameObject);
    }

    private Material GetMaterial(){
        WireColorWindow colorWindow = FindAnyObjectByType<WireColorWindow>(FindObjectsInactive.Include);
        if (colorWindow.mode == WireColorWindow.SelectedMode.Shuffle)
        {
            return wireMaterials[Random.Range(0, wireMaterials.Count)];
        }
        else
        {
            return wireMaterials[(int)colorWindow.mode - 1];
        }
            
    }

    private void SetWireMaterial(){
        ropeMesh.material = GetMaterial();
        GetComponent<MeshRenderer>().material = ropeMesh.material;
    }

    private void CreateRope(){
        midPoint.position = (start.gameObject.transform.position + end.gameObject.transform.position) / 2;
        rope = gameObject.AddComponent<Rope>();
        rope.SetMidPoint(midPoint);
        rope.ropeLength = 0.01f;
        rope.damping = 10f;
        rope.SetStartPoint(start.gameObject.transform);
        rope.SetEndPoint(end.gameObject.transform);
        ropeMesh = gameObject.AddComponent<RopeMesh>();
        ropeMesh.ropeWidth = 0.001f;
        SetWireMaterial();

        if(rope == null) Debug.Log("Rope is null!");
        if(ropeMesh == null) Debug.Log("RopeMesh is null!");
        
        DOTween.To(() => ropeMesh.ropeWidth, x => ropeMesh.ropeWidth = x, width, 0.25f);
    }
}
