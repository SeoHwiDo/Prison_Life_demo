using UnityEngine;

[System.Serializable]
public class ExpansionStep
{
    public string stepName;            // 단계 구분용 이름
    public GameObject internalArea;    // 활성화할 바닥/내부 영역
    public GameObject leftWall;        // 활성화할 왼쪽 벽
    public GameObject rightWall;       // 활성화할 오른쪽 벽
    public Transform backWallTarget;   // 해당 단계에서 BackWall이 이동할 위치(Transform)
}