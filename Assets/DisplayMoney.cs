using TMPro;
using UnityEngine;

public class DisplayMoney : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    public SizeManager player;

    void Update()
    {
        coinText.text = "Coins: " + player.coins;
    }
  
}
