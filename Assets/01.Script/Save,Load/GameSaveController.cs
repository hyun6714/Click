using UnityEngine;

public class GameSaveController : MonoBehaviour
{
    [SerializeField] private GameTimeManager timeMgr;
    [SerializeField] private CurrencyManager currencyMgr;
    [SerializeField] private BuildingManager buildingMgr;
    [SerializeField] private WeatherManager weatherMgr;
    [SerializeField] private BuildingUnlockManager unlockMgr;

    public void TriggerSave()
    {
        SaveLoadManager.SaveGame(timeMgr, currencyMgr, buildingMgr, weatherMgr, unlockMgr);
        Debug.Log("[GameSaveController] 게임 저장");
    }
}