using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance;

    public GameObject gameOverUI;
    public TMPro.TextMeshProUGUI gameOverText;

    private bool gameEnded = false;

    private void Awake()
    {
        Instance = this;
    }

    [PunRPC]
    public void EndGame(string loserPlayer)
    {
        if (gameEnded) return;
        gameEnded = true;

        // Kim kazandı?
        string winner = loserPlayer == PhotonNetwork.NickName ?
                        "Rakip" :
                        PhotonNetwork.NickName;

        Debug.Log($"🏁 OYUN BİTTİ!");

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);

            if (gameOverText != null)
                gameOverText.text = $"<size=50> OYUN BİTTİ</size>";
        }
    }
}
