using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Camera MinimapCamera;
    private float _minHeight = 50.0f;
    private float _maxHeight = 150.0f;
    private float _adjustmentSpeed = 75.0f;

    private void Awake()
    {
        ComponentRegister.Minimap = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switch (MatchParams.MatchTeam)
        {
            case Team.Chaos:
                AddToCullingMask(LayerManager.TeamLayerMask_Chaos);
                break;
            case Team.Balance:
                AddToCullingMask(LayerManager.TeamLayerMask_Balance);
                break;
            case Team.Order:
                AddToCullingMask(LayerManager.TeamLayerMask_Order);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (InputControls.MiniMapZoomIn)
        {
            ZoomIn();
        }
        if (InputControls.MiniMapZoomOut)
        {
            ZoomOut();
        }
    }
    private void ZoomIn()
    {
        float yAdjustment = _adjustmentSpeed * Time.deltaTime;
        float oldY = MinimapCamera.transform.localPosition.y;
        float newY = oldY - yAdjustment;
        if(newY < _minHeight)
        {
            newY = _minHeight;
        }
        MinimapCamera.transform.localPosition = new Vector3(0, newY, 0);
    }
    private void ZoomOut()
    {
        float yAdjustment = _adjustmentSpeed * Time.deltaTime;
        float oldY = MinimapCamera.transform.localPosition.y;
        float newY = oldY + yAdjustment;
        if (newY > _maxHeight)
        {
            newY = _maxHeight;
        }
        MinimapCamera.transform.localPosition = new Vector3(0, newY, 0);
    }
    public void AddToCullingMask(int mask)
    {
        MinimapCamera.cullingMask |= mask;
    }
    public void RemoveFromCullingMask(int mask)
    {
        MinimapCamera.cullingMask &= ~mask;
    }
}
