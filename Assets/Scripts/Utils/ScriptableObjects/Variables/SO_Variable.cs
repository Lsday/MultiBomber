using UnityEngine;

public class SO_Variable<T> : ScriptableObject
{
    public delegate void OnVariableChangeDelegate(T newVal);
    public event OnVariableChangeDelegate OnChange;   

    private SO_Variable<T> originalReference;

    [SerializeField]
    private T _value;

    public T value
    {
        get
        {
            return _value;
        }
        set
        {
            if (_value.Equals(value)) return;
            _value = value;

            OnChange?.Invoke(_value);
        }
    }

    public void Refresh()
    {
        OnChange?.Invoke(_value);
    }

    public void OnValidate()
    {
        Refresh();
    }

    public void Reset()
    {
        if(originalReference != null)
        {
            value = originalReference.value;
        }
    }

    private void SetOriginalReference(SO_Variable<T> original)
    {
        originalReference = original;
    }

    protected SO_Variable<T> PrepareClone()
    {
        SO_Variable<T> clone = Instantiate(this) as SO_Variable<T>;
        clone.SetOriginalReference(this);
        return clone;
    }
}