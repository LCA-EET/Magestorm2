using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitializer : MonoBehaviour
{
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
        Debug.Log("Sending Match Joined Packet");
        Game.ChatMode = false;
        Game.MenuMode = false;
        Game.SendJoinMatchPacket();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
