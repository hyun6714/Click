using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnlockNode", menuName = "Building/Unlock Node Data")]
public class BuildingUnlockData : ScriptableObject
{
    [Header("기본 정보")]
    public string nodeName;
    public Sprite icon;
    public int unlockCost = 100; //해금 비용 (골드)

    [Header("노드 해금 시 열리는 건물 목록")]
    public List<BuildingData> targetBuildingDatas = new List<BuildingData>();

    [Header("해금 상태")]
    public bool isUnlocked = false;  // 이미 해금 완료했는지
    public bool isAvailable = false; // 인접 노드가 열려서 구매 시도가 가능한지 여부

    [Header("시작 노드 설정")]
    public bool isDefaultStartNode = false;

    [Header("상하좌우 인접 테크트리 노드")]
    public BuildingUnlockData upNode;
    public BuildingUnlockData downNode;
    public BuildingUnlockData leftNode;
    public BuildingUnlockData rightNode;

    // 게임 시작 시나 씬 로드 시 초기화용 함수
    public void ResetNodeState(bool isDefaultOpen)
    {
        isUnlocked = isDefaultOpen;
        isAvailable = isDefaultOpen; // 시작 노드라면 바로 구매 가능/해금 상태로 설정
    }
}