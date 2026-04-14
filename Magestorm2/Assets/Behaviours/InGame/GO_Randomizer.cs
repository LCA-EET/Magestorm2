using UnityEngine;

public class GO_Randomizer : MonoBehaviour 
{
    public GameObject[] Candidates;

    private void Awake()
    {
        int index = SharedFunctions.RandomInt(0, Candidates.Length - 1);
        Candidates[index].gameObject.SetActive(true);
    }
}
