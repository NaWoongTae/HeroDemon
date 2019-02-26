using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapQuad : MonoBehaviour
{
    [SerializeField] Material[] materials;

    public void setGreen()
    {
        GetComponent<MeshRenderer>().material = materials[0];
    }

    public void setRed()
    {
        GetComponent<MeshRenderer>().material = materials[1];
    }
}
