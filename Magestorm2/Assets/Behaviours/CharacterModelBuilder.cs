using System.Collections;
using UnityEngine;

public class CharacterModelBuilder : MonoBehaviour
{
    public Material OrderMaterial;
    public Material ChaosMaterial;
    public Material BalanceMaterial;

    public GameObject Male01;
    public GameObject Male02;
    public GameObject Male03;


    //public 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public GameObject BuildModel(byte control, Team team)
    {
        BitArray ba = new BitArray(control);

        if (ba[0]) // 0 Male, 1 Female
        {

        }
        if (ba[1]) // 0 Light Skin, 1 Dark Skin
        {

        }
        return null;
    }
}
