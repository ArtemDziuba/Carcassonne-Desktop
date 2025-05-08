using UnityEngine;
using UnityEngine.UI;

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
