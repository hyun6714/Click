using UnityEngine;
using UnityEngine.Tilemaps;
public enum BuildingCategory
{
    Main, //메인
    Resource, // 자원
    Attack, // 공격
    Trap // 함정
}

[CreateAssetMenu(fileName = "NewBuilding", menuName = "Game/Building Data")]
public class BuildingData : ScriptableObject
{
    [Header("기본 정보")]
    public string buildingName;       // 건물의 이름
    public TileBase buildingTile;     // 타일맵 에셋
    public BuildingCategory category;

    [Header("건설 자원")]
    public int treeCost;
    public int rockCost;

    [Header("설치 설정")]
    public int sizeWidth = 2;         // 건물이 차지하는 가로 칸 수
    public int sizeHeight = 2;        // 건물이 차지하는 세로 칸 수

    [Header("건물 HP")]
    public int maxBuildingHp = 1;


    [Header("테크트리 해금 연동")]
    public BuildingUnlockData unlockData;

    public bool isUnlocked
    {
        get
        {
            if (unlockData == null)
            {
                return true;
            }
            return unlockData.isUnlocked;
        }
    }
}
