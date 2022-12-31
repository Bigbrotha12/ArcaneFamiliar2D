using Characters;
using Environment;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleService
    {
        private enum BattleResult
        {
            None,
            WIN,
            LOSE,
        }

        public List<CombatantSO> Enemies { get; private set; }
        public int TurnCount { get; private set; }
        private Queue<ICombatAction> _playerActionQueue = new Queue<ICombatAction>();
        private Queue<List<ICombatAction>> _enemiesActionQueue = new Queue<List<ICombatAction>>();
        private Queue<ICombatAction> _turnActions = new Queue<ICombatAction>();

        public event Action OnStartTurn;
        public event Action OnReadyForAction;
        public event Action OnReadyEnemyAction;
        public delegate void BattleInfoConsumer(string info, List<string> action);
        public event BattleInfoConsumer OnBattleInfo;

        [SerializeField] private List<CombatantSO> testEnemies;

        public void InitiateBattle(List<CombatantSO> enemies, Transform location)
        {
            TurnCount = 0;
            Enemies = GameManager.Instance.Characters.CreateCombatant(enemies);
            //GetComponent<BattleStageManager>().EnterBattleMode(location, enemies);
            ActivateCombatants();
            InitiateTurn();
        }

        private void ActivateCombatants()
        {
            // Player's abilities
            GameManager.Instance.Player.OnCombatantDefeat += EliminateCombatant;
            foreach (AbilitySO ability in GameManager.Instance.Player.Abilities)
            {
                ability.Activate(GameManager.Instance.Player, GameManager.Instance.Player.Atlas.CurrentLocation.Plane);
            }
            
            // Enemies' abilities
            foreach (CombatantSO enemy in Enemies)
            {
                enemy.OnCombatantDefeat += EliminateCombatant;
                foreach (AbilitySO ability in enemy.Abilities)
                {
                    ability.Activate(enemy, GameManager.Instance.Player.Atlas.CurrentLocation.Plane);
                }
            }
        }

        private void EliminateCombatant(CombatantSO combatant)
        {
            if(combatant == GameManager.Instance.Player)
            {
                Debug.Log("Player Fainted.");
                EndBattle(BattleResult.LOSE);
            }
            else if(Enemies.Contains(combatant))
            {
                Debug.Log(combatant.ObjectName + " Fainted.");
                Enemies.Remove(combatant);
                if(Enemies.Count == 0) { EndBattle(BattleResult.WIN); }
                
            } else
            {
                Debug.LogError("Error: Unreferenced combatant. Possible memory leak.");
            }
        }

        private void InitiateTurn()
        {
            TurnCount++;
            OnStartTurn.Invoke();
            if (_playerActionQueue.Count == 0) { OnReadyForAction.Invoke(); }
            if (_enemiesActionQueue.Count == 0) { OnReadyEnemyAction.Invoke(); }
            PrepareTurnActions();
        }
     
        public void AddPlayerActions(List<ICombatAction> actions)
        {
            foreach (ICombatAction action in actions)
            {
                _playerActionQueue.Enqueue(action);
            }
            PrepareTurnActions();
        }

        public void AddEnemyActions(List<ICombatAction> actions)
        {
            _enemiesActionQueue.Enqueue(actions);
            PrepareTurnActions();
        }

        private void PrepareTurnActions()
        {
            if (_playerActionQueue.Count > 0 && _enemiesActionQueue.Count > 0)
            {
                List<ICombatAction> actionsToResolve = new List<ICombatAction>();
                actionsToResolve.Add(_playerActionQueue.Dequeue());
                actionsToResolve.AddRange(_enemiesActionQueue.Dequeue());
                actionsToResolve.Sort((ICombatAction action1, ICombatAction action2) => { return action1.Priority - action2.Priority; });

                foreach (ICombatAction action in actionsToResolve)
                {
                    _turnActions.Enqueue(action);
                }
                ExecuteNextAction();
            }
        }

        private void ExecuteNextAction()
        {
            if (_turnActions.Count > 0)
            {
                ICombatAction nextAction = _turnActions.Dequeue();
                if(nextAction.Combatant.AvailableActions.Contains(nextAction.Category))
                {
                    nextAction.DoAction();
                    OnBattleInfo.Invoke(nextAction.ActionName, nextAction.ActionResult);
                }
                else
                {
                    OnBattleInfo.Invoke("Failed Action", new List<string>() { nextAction.Combatant.ObjectName + " tried to do something but failed." });
                }
            }
            else
            {
                EndTurn();
            }
        }

        private void EndTurn()
        {
            Debug.Log("Turn ended.");
            InitiateTurn();
        }

        public void RunAway(CombatantSO runner)
        {
            if(runner == GameManager.Instance.Player)
            {
                Debug.Log("Player ran way.");
                EndBattle(BattleResult.None);
            } 
            else if (Enemies.Contains(runner))
            {
                Debug.Log(runner.ObjectName + " ran away.");
                EliminateCombatant(runner);
            }
            else
            {
                Debug.LogError("Error: Unreferenced combatant. Possible memory leak.");
            }
            
        }

        private void EndBattle(BattleResult result)
        {
            //GetComponent<BattleStageManager>().ExitBattleMode();

            // Clear player modifiers
            GameManager.Instance.Player.OnCombatantDefeat -= EliminateCombatant;
            foreach(Effect effect in GameManager.Instance.Player.ActiveEffects)
            {
                effect.Deactivate();
            }
           
            // Destroy enemy SOs
            Enemies.Clear();
        }

        public void InitTestBattle()
        {
            InitiateBattle(testEnemies, GameObject.Find("Character").transform);
        }
    }
}