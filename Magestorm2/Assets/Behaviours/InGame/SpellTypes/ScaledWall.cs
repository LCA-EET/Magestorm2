using UnityEngine;

namespace Magus
{
    public class ScaledWall : Wall
    {
        private Vector3 _scaling, _position;
        private float _yInitialScale, _yInitialPosition;
        private float _scaleElapsed;
        private bool _scaleWall;
        public override void InitializeNoCaster(Team castingTeam, short castID, Transform parent, SpellData spellReference, byte[] payload)
        {
            base.InitializeNoCaster(castingTeam, castID, parent, spellReference, payload);
            _scaling = transform.localScale;
            _position = transform.localPosition;
            _yInitialPosition = _position.y - (_scaling.y / 2.0f);
            _scaleWall = CasterID != 0;
            if (_scaleWall)
            {
                _yInitialScale = _scaling.y;
                transform.localScale = new Vector3(_scaling.x, 0, _scaling.z);
            }
        }
        public override void Update()
        {
            base.Update();
            if (_scaleWall)
            {
                _scaleElapsed += Time.deltaTime;
                if (_scaleElapsed >= 1.0f)
                {
                    _scaleElapsed = 1.0f;
                    _scaleWall = false;
                }
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
}

