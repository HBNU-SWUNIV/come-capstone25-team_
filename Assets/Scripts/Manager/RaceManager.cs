using Photon.Pun;
using UnityEngine;

public class RaceManager : MonoBehaviourPun
{
    [SerializeField] private int totalLaps = 3;   // 목표 랩 수
    private bool gameEnded = false;

    /// <summary>마스터가 직접 호출(또는 RPC로 요청받아 호출)</summary>
    public void DeclareWinner(int actorNumber)
    {
        if (gameEnded) return;

        Debug.Log($"[RaceManager] ▶ 플레이어 {actorNumber} 우승 선언");
        photonView.RPC(nameof(GameOver), RpcTarget.All, actorNumber);
        gameEnded = true;
    }

    /// <summary>클라이언트 → 마스터 : 우승 요청</summary>
    [PunRPC]
    private void RPC_RequestDeclareWinner(int actorNumber)
        => DeclareWinner(actorNumber);

    /// <summary>모든 클라이언트에서 실행 : 실제 게임 종료</summary>
    [PunRPC]
    private void GameOver(int winnerActorNumber)
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log($"[RaceManager] ▶ GameOver RPC 수신, 우승자: {winnerActorNumber}, Time.timeScale=0");
        Time.timeScale = 0f;
        
        // 자신이 우승자인지 확인
        bool isWinner = PhotonNetwork.LocalPlayer.ActorNumber == winnerActorNumber;
        
        // UIManager를 통해 게임 오버 UI 표시
        if (UIManager.instance != null)
        {
            UIManager.instance.GameoverUI(isWinner);
        }
        else
        {
            Debug.LogError("[RaceManager] UIManager.instance가 null입니다!");
        }
    }
}
