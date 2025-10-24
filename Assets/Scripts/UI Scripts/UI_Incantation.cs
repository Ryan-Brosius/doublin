using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;

public class UI_Incantation : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI incantationText;
    private string incantation;
    LazyDependency<InputManager> _InputManager;
    LazyDependency<SpellDatabase> _SpellDatabase;

    private void Start()
    {
        if (incantationText == null) gameObject.GetComponent<TextMeshProUGUI>();
        _InputManager.Value.OnGrimoireIncant += DisplayIncantation;
        _InputManager.Value.OnStaffCast += ResetIncantation;
        _InputManager.Value.OnStaffCancelIncant += ResetIncantation;
        ResetIncantation();
    }

    public void DisplayIncantation(string c)
    {
        incantation += c;
        if (incantation.Length > 6)
        {
            ResetIncantation();
        }
        incantationText.text = incantation.ToUpper();
    }

    public void ResetIncantation()
    {
        incantation = "";
        incantationText.text = incantation.ToUpper();
    }
}
