using UnityEngine;

namespace Magus
{
    public class GameInitializer : MonoBehaviour
    {
        private void Awake()
        {
            Game.Running = true;
            if (!MatchParams.ReturningFromMatch)
            {
                LoadPrefs();
                Game.Init();
            }
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ComponentRegister.UIPrefabManager.InstantiateLoginForm();
            if (!Game.MusicPlayerCreated)
            {
                Game.MusicPlayerCreated = true;
                ComponentRegister.UIPrefabManager.InstantiateMusicPlayer();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LoadPrefs()
        {

        }
    }
}

