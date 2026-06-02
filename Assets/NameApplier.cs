using TMPro;
using UnityEngine;

public class NameApplier : MonoBehaviour
{
    public TMP_Text nameText;

    void Start()
    {
        nameText.text = PlayerPrefs.GetString("PlayerName", "Player");
    }
}