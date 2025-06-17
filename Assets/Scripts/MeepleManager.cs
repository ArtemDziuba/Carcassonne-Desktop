using UnityEngine;
using UnityEngine.UI;

// Клас, що відповідає за контроль міплами
public class MeepleManager : MonoBehaviour
{
    public Button placeMeepleBtn;
    public MeepleSpawner meepleSpawner;

    private void Start()
    {
        placeMeepleBtn.onClick.AddListener(() =>
        {
            meepleSpawner.StartPlacingMeeple();
        });
    }
}
