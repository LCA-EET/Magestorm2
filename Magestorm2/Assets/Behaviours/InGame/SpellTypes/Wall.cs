using UnityEngine;
using UnityEngine.Rendering;
public class Wall : SpawnedSpell
{
    public bool ScaleWall;
    private Vector3 _scaling, _position;
    private float _scaleElapsed;
    public override void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference, byte[] payload)
    {
        base.Initialize(casterID, castingTeam, castID, parent, spellReference, payload);
        Match.AddWall(_castID, this);
        _scaling = transform.localScale;
        _position = transform.localPosition;
        if (ScaleWall)
        {
            transform.localScale = Vector3.zero;
        }
    }
    private void IncreaseWallScale()
    {

    }
    public void DestroyWall()
    {
        Destroy(gameObject);
    }
    public override void Update()
    {
        base.Update();
        if (ScaleWall)
        {
            _scaleElapsed += Time.deltaTime;
            if(_scaleElapsed > 1.0f)
            {
                _scaleElapsed = 1.0f;
            }
            transform.localPosition = new Vector3(_position.x, _position.y - ((_scaling.y / 2) * (1.0f - _scaleElapsed)), _position.z);
            transform.localScale = _scaling * _scaleElapsed;
        }
    }
}
