using UnityEngine;

public class WinScreen : MonoBehaviour
{
    public GameObject player1WinPanel;
    public GameObject player2WinPanel;
    public GameObject drawPanel;
    public void Setup(GameManager.EPlayPhaseResult result)
    {
        player1WinPanel.SetActive(result == GameManager.EPlayPhaseResult.Player1Win);
        player2WinPanel.SetActive(result == GameManager.EPlayPhaseResult.Player2Win);
        drawPanel.SetActive(result == GameManager.EPlayPhaseResult.Draw);
    }
}
