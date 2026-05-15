using UnityEngine;

public class ViewAll : AppliedEffect
{
    private int[] _masks;
    private void Awake()
    {
        _masks = new int[2];
    }
    public override void ApplyEffect(Avatar appliedTo)
    {
        base.ApplyEffect(appliedTo);
        if (_appliedToPlayer)
        {
            switch (MatchParams.MatchTeam)
            {
                case Team.Chaos:
                    _masks[0] = LayerManager.TeamLayerMask_Balance;
                    _masks[1] = LayerManager.TeamLayerMask_Order;
                    break;
                case Team.Balance:
                    _masks[0] = LayerManager.TeamLayerMask_Chaos;
                    _masks[1] = LayerManager.TeamLayerMask_Order;
                    break;
                case Team.Order:
                    _masks[0] = LayerManager.TeamLayerMask_Balance;
                    _masks[1] = LayerManager.TeamLayerMask_Chaos;
                    break;
                    
            }
            foreach (int mask in _masks)
            {
                ComponentRegister.Minimap.AddToCullingMask(mask);
            }
        }
    }

    public override void ReverseEffect()
    {
        base.ReverseEffect();
        if (_appliedToPlayer)
        {
            foreach (int mask in _masks)
            {
                ComponentRegister.Minimap.RemoveFromCullingMask(mask);
            }
        }
    }
}
