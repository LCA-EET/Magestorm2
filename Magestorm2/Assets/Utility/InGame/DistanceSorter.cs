using System.Collections.Generic;
using UnityEngine;

public class DistanceSorter : IComparer<IDistanced>
{
    private Transform _reference;
    private bool _ascending;
    public DistanceSorter(Transform reference, bool ascending) : this(reference)
    {
        _ascending = ascending;
    }

    public DistanceSorter(Transform reference)
    {
        SetReference(reference);
    }
    public void SetReference(Transform reference)
    {
        _reference = reference;
    }
    public void SetOrder(bool ascending)
    {

    }
    int IComparer<IDistanced>.Compare(IDistanced x, IDistanced y)
    {
        if (_ascending)
        {
            return x.DetermineDistance(_reference).CompareTo(y.DetermineDistance(_reference));
        }
        else
        {
            return y.DetermineDistance(_reference).CompareTo(x.DetermineDistance(_reference));
        }
    }
}
