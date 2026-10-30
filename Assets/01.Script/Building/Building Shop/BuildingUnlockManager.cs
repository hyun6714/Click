using UnityEngine;
using System.Collections.Generic;

public class BuildingUnlockManager : MonoBehaviour
{
    [Header("모든 테크트리 노드 목록")]
    public List<BuildingUnlockData> allUnlockNodes;

    private GameTimeManager timeMgr;
    private CurrencyManager currencyMgr;
    private WeatherManager weatherMgr;
    private BuildingManager buildingMgr;

    private bool isInitialized = false;

    private void Awake()
    {
        buildingMgr = Object.FindFirstObjectByType<BuildingManager>();
        currencyMgr = Object.FindFirstObjectByType<CurrencyManager>();
        weatherMgr = Object.FindFirstObjectByType<WeatherManager>();
        buildingMgr = Object.FindFirstObjectByType<BuildingManager>();
    }


    private void Start()
    {
        SaveData saveData = SaveLoadManager.LoadGame();

        if (saveData != null && saveData.unlockedBuildingNodes != null && saveData.unlockedBuildingNodes.Count > 0)
        {
            LoadUnlockData(saveData);
        }
        else
        {
            foreach (var node in allUnlockNodes)
            {
                if (node == null)
                {
                    continue;
                }

                bool isStartNode = node.isDefaultStartNode;
                node.ResetNodeState(isStartNode);
            }
            InitializeNodes();
        }

        isInitialized = true;
    }

    public void InitializeNodes()
    {
        foreach (var node in allUnlockNodes)
        {
            if (node == null)
            {
                continue;
            }

            // 만약 첫 번째 노드이거나 수동으로 켜둔 노드라면 해금 처리
            if (node.isUnlocked)
            {
                UnlockAdjacentNodes(node);
            }
        }

        EventManager.TriggerBuildingUnlockUpdated();
    }

    // 플레이어가 해금 버튼을 눌렀을 때 호출할 함수
    public void TryUnlockNode(BuildingUnlockData node)
    {
        if (node.isUnlocked || !node.isAvailable)
        {
            Debug.Log("[해금 실패]");
            return;
        }

        bool hasEnough = EventManager.RequestCheckCurrency(CurrencyType.Gold, node.unlockCost);
        if (!hasEnough)
        {
            Debug.Log("[해금 실패] 재화가 부족합니다!");
            return;
        }

        bool success = EventManager.RequestUseCurrency(CurrencyType.Gold, node.unlockCost);
        if (!success)
        {
            return;
        }

        node.isUnlocked = true;
        Debug.Log($"[해금 성공] {node.nodeName} 해금 완료");

        UnlockAdjacentNodes(node);
        EventManager.TriggerBuildingUnlockUpdated();

        TriggerSave();
    }

    private void UnlockAdjacentNodes(BuildingUnlockData node)
    {
        if (node.upNode != null)
        {
            node.upNode.isAvailable = true;
        }

        if (node.downNode != null)
        {
            node.downNode.isAvailable = true;
        }

        if (node.leftNode != null)
        {
            node.leftNode.isAvailable = true;
        }

        if (node.rightNode != null)
        {
            node.rightNode.isAvailable = true;
        }
    }

    public void SaveUnlockData(SaveData data)
    {
        if (data == null)
        {
            return;
        }

        data.unlockedBuildingNodes.Clear();

        foreach (var node in allUnlockNodes)
        {
            if (node != null && node.isUnlocked)
            {
                data.unlockedBuildingNodes.Add(node.name);
            }
        }
    }

    public void LoadUnlockData(SaveData data)
    {
        if (data == null || data.unlockedBuildingNodes == null)
        {
            return;
        }

        foreach (var node in allUnlockNodes)
        {
            if (node == null)
            {
                continue;
            }
            node.ResetNodeState(node.isDefaultStartNode);
        }

        foreach (var nodeName in data.unlockedBuildingNodes)
        {
            BuildingUnlockData targetNode = allUnlockNodes.Find(n => n != null && n.name == nodeName);
            if (targetNode != null)
            {
                targetNode.isUnlocked = true;
            }
        }

        InitializeNodes();
        Debug.Log("[BuildingUnlockManager] 테크트리 해금 데이터 복원 완료");
    }

    public void TriggerSave()
    {
        if (!isInitialized)
        {
            return;
        }

        Debug.Log("[unlock] 저장");

        BuildingUnlockManager unlockMgr = this;

        SaveLoadManager.SaveGame(timeMgr, currencyMgr, buildingMgr, weatherMgr, unlockMgr);
    }
}