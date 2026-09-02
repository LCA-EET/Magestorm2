using UnityEngine;
using UnityEngine.Rendering;
public class Wall : SpawnedSpell
{
    public bool ScaleWall;
    private Vector3 _scaling;
    private float _scaleElapsed;
    public override void Initialize(byte casterID, Team castingTeam, short castID, Transform parent, SpellData spellReference, byte[] payload)
    {
        base.Initialize(casterID, castingTeam, castID, parent, spellReference, payload);
        Match.AddWall(_castID, this);
        _scaling = transform.localScale;
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
            transform.localScale = _scaling * _scaleElapsed;
        }
    }
}
