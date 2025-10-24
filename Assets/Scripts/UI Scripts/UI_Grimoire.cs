using UnityEngine;

public class UI_Grimoire : MonoBehaviour
{
    LazyDependency<GoblinStateManager> _GoblinStateManager;

    private void Start()
    {
        _GoblinStateManager.Value.CurrentGoblinState.Subscribe(ToggleGrimoire);
        gameObject.SetActive(false);
    }

    private void ToggleGrimoire(GoblinState goblinState)
    {
        if (goblinState == GoblinState.BookTop)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
