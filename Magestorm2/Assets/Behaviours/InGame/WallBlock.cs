using UnityEngine;
namespace Magus 
{ 
    public class WallBlock : MonoBehaviour
    {
        public BoxCollider BoxCollider;
        
        public void DisableCollider()
        {
            BoxCollider.enabled = false;
        }
    }
}

