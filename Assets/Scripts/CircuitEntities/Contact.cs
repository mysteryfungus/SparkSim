using System;
using UnityEngine;

public class Contact : MonoBehaviour
{
    public GameObject Self {private set;  get; }
    public string Name {private set; get; }
    public UInt32 ID {private set; get; }

    void Awake()
    {
        ID = IDManager.AssignId();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnDestroy()
    {
        IDManager.RemoveId(ID);
    }
}
