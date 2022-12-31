using UnityEngine;
using Characters;
using System.Collections.Generic;
using System.Collections;

public class BattleStageManager : MonoBehaviour
{
    [System.Serializable]
    public class BattleStage
    {
        public GameObject Stage;
        public Transform PlayerPosition;
        public Transform MainFamiliarPosition;
        public Transform SupportFamiliarPosition;
        public List<Transform> EnemiesPosition;
    }
    private List<GameObject> _battleActors = new List<GameObject>();
    [SerializeField] private List<BattleStage> _stages;
    [SerializeField] private GameObject _battleHUD;
    [SerializeField] private GameObject _explorationHUD;
    private BattleStage _closestStage;

    public void EnterBattleMode(Transform playerPosition, List<CombatantSO> enemies)
    {
        _closestStage = FindClosestStage(playerPosition);
        if(_closestStage is null) 
        {
            Debug.LogError("Error: Invalid stage selection.");
            return;
        }
        StartCoroutine("BattleScreenTransitionIn", enemies);
    }

    public IEnumerator BattleScreenTransitionIn(List<CombatantSO> enemies)
    {
        GameManager.Instance.UInterface.StartFade(false, 1.0f);
        yield return new WaitForSeconds(1.0f);
        _closestStage.Stage.SetActive(true);
        ActivateBattleHUD(true);
        PlaceActors(enemies);
        GameManager.Instance.UInterface.StartFade(true, 1.0f);
    }

    public IEnumerator BattleScreenTransitionOut()
    {
        GameManager.Instance.UInterface.StartFade(false, 1.0f);
        yield return new WaitForSeconds(1.0f);
        ActivateBattleHUD(false);
        RemoveActors();
        _closestStage.Stage.SetActive(false);
        GameManager.Instance.UInterface.StartFade(true, 1.0f);
    }

    public void PlaceActors(List<CombatantSO> enemies)
    {
        // Instantiate player side
        PlayerSO player = GameManager.Instance.Player;
        if(_closestStage.PlayerPosition is null || _closestStage.MainFamiliarPosition is null || _closestStage.SupportFamiliarPosition is null)
        {
            Debug.LogError("Error: Invalid position data. Check Stage List and Object names match.");
            RemoveActors();
            return;
        }

        _battleActors.Add(GameObject.Instantiate(player.Prefab, _closestStage.PlayerPosition));
        _battleActors.Add(GameObject.Instantiate(player.Familiars.MainFamiliar.Prefab, _closestStage.MainFamiliarPosition));
        _battleActors.Add(GameObject.Instantiate(player.Familiars.SupportFamiliar.Prefab, _closestStage.SupportFamiliarPosition));
        
        // Instantiate enemy side
        if(enemies is null) { Debug.LogError("Error: Enemy list received is null."); return; }
        for (int i = 0; i < enemies.Count && i < _closestStage.EnemiesPosition.Count; i++)
        {
            if(_closestStage.EnemiesPosition[i] is null)
            {
                Debug.LogError("Error: Invalid position data. Check Stage List and Object names match.");
                RemoveActors();
                return;
            }
            _battleActors.Add(GameObject.Instantiate(enemies[i].Prefab, _closestStage.EnemiesPosition[i]));
        }
    }

    public void RemoveActors()
    {
        if(_battleActors is not null)
        {
            foreach (GameObject actor in _battleActors)
            {
                Destroy(actor);
            }
            _battleActors.Clear();
        }
    }

    public BattleStage FindClosestStage(Transform position)
    {
        (BattleStage stage, float distance) closestStage = (null, 0);
        foreach (BattleStage stage in _stages)
        {
            if(closestStage.stage is null) 
            { 
                closestStage.stage = stage;
                closestStage.distance = Vector3.Distance(position.position, closestStage.stage.PlayerPosition.position);
                continue;
            }

            float distance = Vector3.Distance(position.position, stage.PlayerPosition.position);
            if(distance < closestStage.distance)
            {
                closestStage.stage = stage;
                closestStage.distance = distance;
            }
        }
        
        return closestStage.stage;

    }

    public void ActivateBattleHUD(bool active)
    {
        _battleHUD.SetActive(active);
        _explorationHUD.SetActive(!active);
    }

    public void ExitBattleMode()
    {
        StartCoroutine("BattleScreenTransitionOut");
    }
}