using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildingInfoUI : MonoBehaviour
{
    [Header("UI 참조")]
    public GameObject infoPanel;
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text detailStatText; // 공격력 및 사거리 또는 채집속도 및 범위

    [Header("사거리 시각화 참조")]
    public GameObject rangeIndicatorPrefab; // 사거리 표시용 프리팹
    private GameObject currentRangeIndicator;
    public Tilemap referenceTilemap;

    private Vector3Int currentSelectedCell;

    private void Awake()
    {
        HidePanel();

        if (rangeIndicatorPrefab != null && currentRangeIndicator == null)
        {
            currentRangeIndicator = Instantiate(rangeIndicatorPrefab);
            currentRangeIndicator.SetActive(false);
        }
    }

    public void ShowBuildingInfo(Vector3Int cellPos, BuildingData data, int currentHp)
    {
        currentSelectedCell = cellPos;
        infoPanel.SetActive(true);

        nameText.text = data.buildingName;
        hpText.text = $"체력: {currentHp} / {data.maxBuildingHp}";

        float rangeValue = 0f;

        if (data is ClickBuildingData clickData)
        {
            detailStatText.text = $"최대 클릭 증가: +{clickData.maxClickBonus}";
        }
        else if (data is ProductionBoostBuildingData boostData)
        {
            detailStatText.text = $"일일 생산 보너스: +{boostData.dailyProductionBonus}";
        }
        else if (data is AttackBuildingData attackData)
        {
            detailStatText.text = $"공격력: {attackData.attackDamage}\n" +
                                  $"공격 속도: {attackData.attackSpeed}초\n" +
                                  $"사거리: {attackData.attackRange}";

            rangeValue = attackData.attackRange;
        }

        else if (data is ProductionBuildingData prodData)
        {
            detailStatText.text = $"채집 대상: {prodData.targetResourceType}\n" +
                                  $"채집 주기: {prodData.gatherInterval}초\n" +
                                  $"채집 범위: {prodData.gatherRange}타일";

            rangeValue = prodData.gatherRange;
        }
        else if (data is TrapBuildingData trapData)
        {
            detailStatText.text = $"피해량: {trapData.damage}\n" +
                                  $"피해 범위: {trapData.triggerRange}";
        }
        else if (data is BarricadeBuildingData barricadeData)
        {
            detailStatText.text = $"체력: {barricadeData.maxHealth}";
        }
        else if (data is FirepitData fireData)
        {
            detailStatText.text = $"온기 범위: {fireData.effectRadius}";
            rangeValue = fireData.effectRadius;
        }

        else
        {
            detailStatText.text = "";
        }

        ShowRangeIndicator(cellPos, rangeValue);
    }

    private void ShowRangeIndicator(Vector3Int cellPos, float range)
    {
        if (currentRangeIndicator == null || referenceTilemap == null)
        {
            return;
        }

        if (range <= 0)
        {
            currentRangeIndicator.SetActive(false);
            return;
        }

        Vector3 worldPos = referenceTilemap.GetCellCenterWorld(cellPos);
        currentRangeIndicator.transform.position = worldPos;

        float scaleSize = range * 2f;
        currentRangeIndicator.transform.localScale = new Vector3(scaleSize, scaleSize * 0.5f, 1f);

        currentRangeIndicator.SetActive(true);
    }

    public void HidePanel()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }

        if (currentRangeIndicator != null)
        {
            currentRangeIndicator.SetActive(false);
        }
    }
}