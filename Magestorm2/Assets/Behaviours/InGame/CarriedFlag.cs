using UnityEngine;
public class CarriedFlag : MonoBehaviour
{
    public GameObject Chaos, Balance, Order;
    private void Awake()
    {
        if (!MatchParams.IsCTF)
        {
            Destroy(gameObject);
        }
    }
    private void HideFlags()
    {
        Chaos.SetActive(false);
        Balance.SetActive(false);
        Order.SetActive(false);
    }
    public void ShowFlag(Team flag)
    {
        HideFlags();
        switch (flag)
        {
            case Team.Balance:
                Balance.SetActive(true);        
                break;
            case Team.Order:
                Order.SetActive(true);
                break;
            case Team.Chaos:
                Chaos.SetActive(true);
                break;
        }
    }
}
