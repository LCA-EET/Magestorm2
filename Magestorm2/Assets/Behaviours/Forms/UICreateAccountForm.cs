using UnityEngine;

public class UICreateAccountForm : ValidatableForm
{


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssociateFormToButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void PassedValidation()
    {
        CloseForm();
    }

    public override void SetInstantiator(GameObject instantiator)
    {
        base.SetInstantiator(instantiator);
    }
}
