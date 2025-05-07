using System.Collections.Generic;
using UnityEngine;

public class Component : MonoBehaviour
{
    public GameObject Self {private set;  get; }
    public string Name {private set; get; }

    [SerializeField] public short ContactAmount {private set; get; }
    [SerializeField] public List<Contact> Contacts {private set; get; }

    void Awake()
    {
        Self = this.gameObject;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
