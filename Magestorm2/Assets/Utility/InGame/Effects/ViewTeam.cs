public class ViewTeam : AppliedEffect
{
    private int _mask = 0;
    public override void ApplyEffect(Avatar appliedTo)
    {
        base.ApplyEffect(appliedTo);
        if (_appliedToPlayer)
        {
            switch (_applier.PlayerTeam)
            {
                case Team.Balance:
                    _mask = LayerManager.TeamLayerMask_Balance;
                    break;
                case Team.Order:
                    _mask = LayerManager.TeamLayerMask_Order;
                    break;
                case Team.Chaos:
                    _mask = LayerManager.TeamLayerMask_Chaos;
                    break;
            }
            if(_mask != 0)
            {
                ComponentRegister.Minimap.AddToCullingMask(_mask);
            }    
        }
    }
    public override void ReverseEffect()
    {
        base.ReverseEffect();
        if (_appliedToPlayer)
        {
            if (_mask != 0)
            {
                ComponentRegister.Minimap.RemoveFromCullingMask(_mask);
            }
        }
    }
}
