using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class EventManager
{
    //UI 전용 현재 재화량 가져오기 
    public static event Action<CurrencyType> OnRequestCurrencyValue;
    //재화 얻을 시
    public static event Action<CurrencyType, int> OnCurrencyAdded;
    //재화 사용 시
    public static event Func<CurrencyType, int, bool> OnCurrencyUsed;
    //재화량 변화 시
    public static event Action<CurrencyType, int> OnCurrencyChanged;

    //상점에서 건물 선택 시 
    public static event Action<BuildingData> OnBuildingSelected;

    //클릭 최대지 가져옴
    public static event Action<int> OnMaxClickChanged;
    public static event Action OnRequestMaxClick;

    //건물 설치 시 
    public delegate void RequestPlaceBuildingHandler(Vector3Int cellPosition, BuildingData buildingData);
    public static event RequestPlaceBuildingHandler OnRequestPlaceBuilding;

    //철거 버튼 누를 때
    public static event Action OnDemolishButtonClicked;

    //건물 철거 팝업
    public static event Action OnBuildMenuPopupToggle;

    //건물 설치 팝업
    public static event Action<bool> OnBuilAddPopupToggle;

    //특정 좌표 건물 철거 
    public delegate void RequestDemolishHandler(Vector3Int cellPosition);
    public static event RequestDemolishHandler OnRequestDemolish;

    //재화 충분한지 확인 하는 이벤트 
    public static event Func<CurrencyType, int, bool> OnCheckCurrency;

    //날짜 변경 이벤트(하루 바뀔 떄마다)
    public delegate void DayChangedHandler(int day);
    public static event DayChangedHandler OnDayChanged;
    
    //일,시,분,초 상세 시간 변경
    public delegate void TimeDetailedChangedHandler(int day, float hour, float minute, float second);
    public static event TimeDetailedChangedHandler OnTimeDetailedChanged;

    //날짜 스킵버튼 이벤트
    public delegate void SkipButtonClickHandler();
    public static event SkipButtonClickHandler OnSkipButtonClicked;

    //자원 건물 생성
    public static event Action<Vector3Int, ProductionBuildingData> OnRequestSpawnResourceBuilding;

    //공격 건물 생성
    public static event Action<Vector3Int, AttackBuildingData> OnRequestSpawnTower;

    //함정 생성
    public static event Action<Vector3Int, TrapBuildingData> OnRequestSpawnTrapBuilding;

    //메인성 생성
    public static event Action<Vector3Int, BuildingData> OnRequestSpawnMainCastle;

    //몬스터 사망시
    public static event Action<EnemyType, GameObject> OnRequestReturnPool;

    //투사체 풀에 반납 
    public static event Action<GameObject, GameObject> OnRequestReturnProjectile;

    //투사체 생성 or 풀에서 꺼내기 
    public static event Func<GameObject, Vector3, Transform, int, float, GameObject> OnRequestSpawnProjectile;

    //건물 피격시
    public delegate void RequestDamageBuildingHandler(Vector3Int cellPosition, int damage);
    public static event RequestDamageBuildingHandler OnRequestDamageBuilding;

    //건물 등록 알림
    public static event Action<Vector3Int> OnBuildingRegistered;

    //건물 파괴시
    public static event Action<Vector3Int> OnBuildingUnregistered;

    //타일맵 참조
    public delegate void BuildingManagerReadyHandler(Tilemap tilemap);
    public static event BuildingManagerReadyHandler OnBuildingManagerReady;

    //동기화 용
    public static event Action OnRequestSyncBuildings;

    public static void RequestCurrencyValue(CurrencyType type) => OnRequestCurrencyValue?.Invoke(type);
    public static void TriggerBuildingSelected(BuildingData buildingData) => OnBuildingSelected?.Invoke(buildingData);
    public static void CurrencyAdded(CurrencyType type, int value) => OnCurrencyAdded?.Invoke(type, value);
    public static void CurrencyUsed(CurrencyType type, int value) => OnCurrencyUsed?.Invoke(type, value);
    public static void CurrencyChanged(CurrencyType type, int value) => OnCurrencyChanged?.Invoke(type, value);
    public static void MaxClickChanged(int maxAmount) => OnMaxClickChanged?.Invoke(maxAmount);
    public static void RequestMaxClick() => OnRequestMaxClick?.Invoke();
    public static void TriggerRequestPlaceBuilding(Vector3Int cellPosition, BuildingData buildingData) => OnRequestPlaceBuilding?.Invoke(cellPosition, buildingData);
    public static void TriggerDemolishButtonClicked() => OnDemolishButtonClicked?.Invoke();
    public static void TriggerBuildMenuPopupToggle() => OnBuildMenuPopupToggle?.Invoke();
    public static void TriggerBuildAddPopupToggle(bool isOpen) => OnBuilAddPopupToggle?.Invoke(isOpen);
    public static void TriggerRequestDemolish(Vector3Int cellPosition) => OnRequestDemolish?.Invoke(cellPosition);
    public static void TriggerDayChanged(int day) => OnDayChanged?.Invoke(day);
    public static void TriggerTimeDetailedChanged(int day, float hour, float minute, float second) => OnTimeDetailedChanged?.Invoke(day, hour, minute, second);
    public static void TriggerSkipButtonClicked() => OnSkipButtonClicked?.Invoke();
    public static void TriggerRequestSpawnTower(Vector3Int cellPos, AttackBuildingData attackData) => OnRequestSpawnTower?.Invoke(cellPos, attackData);
    public static void TriggerRequestSpawnResourceBuilding(Vector3Int cellPos, ProductionBuildingData prodData) => OnRequestSpawnResourceBuilding?.Invoke(cellPos, prodData);
    public static void TriggerRequestSpawnTrapBuilding(Vector3Int cellPos, TrapBuildingData trapData) => OnRequestSpawnTrapBuilding?.Invoke(cellPos, trapData);
    public static void TriggerRequestSpawnMainCastle(Vector3Int cellPos, BuildingData mainData) => OnRequestSpawnMainCastle?.Invoke(cellPos, mainData);
    public static void TriggerRequestReturnPool(EnemyType type, GameObject enemyObj) =>OnRequestReturnPool?.Invoke(type, enemyObj);
    public static void TriggerRequestReturnProjectile(GameObject prefabRef, GameObject projectileObj) => OnRequestReturnProjectile?.Invoke(prefabRef, projectileObj);
    public static void TriggerRequestDamageBuilding(Vector3Int cellPosition, int damage) => OnRequestDamageBuilding?.Invoke(cellPosition, damage);
    public static void TriggerBuildingRegistered(Vector3Int cellPosition) => OnBuildingRegistered?.Invoke(cellPosition);
    public static void TriggerBuildingUnregistered(Vector3Int cellPosition) => OnBuildingUnregistered?.Invoke(cellPosition);
    public static void TriggerBuildingManagerReady(Tilemap tilemap) => OnBuildingManagerReady?.Invoke(tilemap);
    public static void TriggerRequestSyncBuildings() => OnRequestSyncBuildings?.Invoke();


    //건물 비용차감 
    public static bool RequestUseCurrency(CurrencyType type, int value)
    {
        if (OnCurrencyUsed != null)
        {
            return OnCurrencyUsed.Invoke(type, value);
        }
        return false;
    }

    public static bool RequestCheckCurrency(CurrencyType type, int value)
    {
        if (OnCheckCurrency != null)
        {
            return OnCheckCurrency.Invoke(type, value);
        }
        return false;
    }

    public static GameObject TriggerRequestSpawnProjectile(GameObject prefab, Vector3 spawnPos, Transform target, int damage, float speed)
    {
        return OnRequestSpawnProjectile?.Invoke(prefab, spawnPos, target, damage, speed);
    }
}

