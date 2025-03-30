using UnityEngine;

public class ValidateableObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void MarkInvalid(bool invalid)
    {

    }
    public virtual bool Validate()
    {
        return true;
    }
}
