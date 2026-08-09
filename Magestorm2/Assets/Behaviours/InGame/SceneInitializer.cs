using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitializer : MonoBehaviour
{
    public Scene SceneGO;
    public Material NeutralBiased;
    public Material BalanceBiased;
    public Material OrderBiased;
    public Material ChaosBiased;
    private void Awake()
    {
        if (!Game.Running)
        {
            SceneManager.LoadScene("Pregame");
        }
        else
        {
            ComponentRegister.SceneInitializer = this;
            Match.Reinitialize();
            InputControls.Init();
        }
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Teams.Init();
        Game.Init();
        Language.Init();
        LayerManager.Init();
        PoolManager.InitializePools();
        AssignPools();
        AssignActivatables();
        Debug.Log("Sending Match Joined Packet");
        Game.ChatMode = false;
        Game.MenuMode = false;
        Game.SendJoinMatchPacket();
    }
    private void AssignPools()
    {
        ManaPool[] mp = SceneGO.GetComponentsInChildren<ManaPool>();
        Array.Sort(mp);
        for (byte b = 0; b < mp.Length; b++)
        {
            mp[b].RegisterPool(b);
        }
    }
    private void AssignActivatables()
    {
        ActivateableObject[] ao = SceneGO.GetComponentsInChildren<ActivateableObject>();
        Array.Sort(ao);
        for (byte b = 0; b < ao.Length; b++)
        {
            ao[b].RegisterObject(b);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
