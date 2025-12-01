using TMPro;
using UnityEngine;
public class DeathGraphic : MonoBehaviour
{
    public GameObject Reticle;
    public GameObject ImageAndText;
    public TMP_Text TextString;

    private PeriodicAction _deathCheck;
    private bool _deathGraphicShown;

    private void Awake()
    {
        _deathCheck = new PeriodicAction(0.5f, DeathCheck, null);
    }
    private void Update()
    {
        _deathCheck.ProcessAction(Time.deltaTime);
    }
    private void DeathCheck()
    {
        bool pcIsAlive = ComponentRegister.PC.IsAlive;
        if (!pcIsAlive && !_deathGraphicShown)
        {
            ShowDeath(true);
        }
        else if(pcIsAlive && _deathGraphicShown)
        {
            ShowDeath(false);
        }
    }
    private void ShowDeath(bool show)
    {
        _deathGraphicShown = show;
        ImageAndText.SetActive(show);
        Reticle.SetActive(!show);
        string text = Language.BuildString(207, InputControls.KeyToString(InputControl.Action));
        TextString.text = text;
    }
}
