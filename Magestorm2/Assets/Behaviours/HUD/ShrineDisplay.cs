using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
public class ShrineDisplay : MonoBehaviour
{
    public TMP_Text Title;
    public TMP_Text Health;

    public void Awake()
    {
        ComponentRegister.ShrineDisplay = this;
        Toggle(false);
    }

    public void Toggle(bool show)
    {
        Title.gameObject.SetActive(show);
        Health.gameObject.SetActive(show);
    }

    public void Refresh(Shrine shrine)
    {
        Color toApply = Teams.GetTeamColor(shrine.GetTeam());
        Health.color = toApply;
        Title.color = toApply;
        Health.text = Language.GetBaseString(180) + " " + shrine.GetHealth().ToString();
        Title.text = Teams.GetTeamName(shrine.Team) + " " + Language.GetBaseString(170);
        Toggle(true);
    }
}
