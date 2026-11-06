using UnityEngine;
namespace Magus
{

    public class DisciplineSelectView : ScrollSelectView
    {

        public SpellSelectView SpellSelectView;

        public override void Start()
        {
            base.Start();
        }

        protected override void ProcessSelection()
        {
            SpellSelectView.PopulateOptions(_selectedOption);
        }
    }

}