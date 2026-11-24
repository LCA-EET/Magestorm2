using UnityEngine;
using UnityEngine.UI;

namespace Magus
{
    public class Splash : MonoBehaviour
    {
        public Image Image;
        public Sprite[] Backgrounds;

        public void Awake()
        {
            Image.sprite = Backgrounds[UnityEngine.Random.Range(0, Backgrounds.Length)];
        }
    }
}
