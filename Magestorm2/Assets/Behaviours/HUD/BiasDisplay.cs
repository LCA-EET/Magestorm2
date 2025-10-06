using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

public class BiasDisplay : MonoBehaviour
{
    public TMP_Text Title;
    public TMP_Text Power;
    public TMP_Text Bias;

    public void Awake()
    {
        ComponentRegister.BiasDisplay = this;
        Toggle(false);
    }

    public void Refresh(ManaPool pool)
    {
        Color toApply = Teams.GetTeamColor(pool.GetTeam());
        Power.color = toApply;
        Bias.color = toApply;
        Title.color = toApply;
        Power.text = Language.GetBaseString(172) + " " + pool.GetPoolPower().ToString();
        Bias.text = Language.GetBaseString(173) + " " + pool.GetBiasAmount().ToString();
        Title.text = Teams.GetTeamName(pool.GetTeam()) + " " + Language.GetBaseString(171);
        Toggle(true);
    }
    public void Toggle(bool show)
    {
        Title.gameObject.SetActive(show);
        Power.gameObject.SetActive(show);
        Bias.gameObject.SetActive(show);
    }
}
