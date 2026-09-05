using UnityEngine;
using UnityEngine.Rendering;
public class Wall : SpawnedSpell
{
    public bool ScaleWall;
    private Vector3 _scaling, _position;
    private float _yInitialScale, _yInitialPosition;
    private float _scaleElapsed;

    public override void InitializeNoCaster(Team castingTeam, short castID, Transform parent, SpellData spellReference, byte[] payload)
    {
        base.InitializeNoCaster(castingTeam, castID, parent, spellReference, payload);
        Match.AddWall(_castID, this);
        _scaling = transform.localScale;
        _position = transform.localPosition;
        _yInitialPosition = _position.y - (_scaling.y / 2.0f);
        if (CasterID == 0)
        {
            ScaleWall = false;
        }
        if (ScaleWall)
        {
            _yInitialScale = _scaling.y;
            //transform.localScale = Vector3.zero;
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
            //transform.localPosition = new Vector3(_position.x, _position.y - ((_scaling.y / 2) * (1.0f - _scaleElapsed)), _position.z);
            transform.localPosition = new Vector3(_position.x, DetermineYPosition(), _position.z);
            _scaling.y = _yInitialScale * _scaleElapsed;
            transform.localScale = _scaling;
        }
    }
    private float DetermineYPosition()
    {
        return _yInitialPosition + (_scaleElapsed * (_yInitialScale / 2));
    }
}
