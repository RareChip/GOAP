using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using RareChip.AITools;
using UnityEngine;

namespace RareChip.Goap
{
    public class GoapAgent : MonoBehaviour
    {
        [SerializeField] private GoapConfig goapConfig;
        private bool visualConfig = true;

        [SerializeField, ConditionalField("visualConfig")]
        private List<GoalConfig> goalConfigs;

        [SerializeField, ConditionalField("visualConfig")]
        private List<ActionConfig> actionConfigs;

        [SerializeField,
         Tooltip("This makes actions cost a little more if they dont exactly satisfy the condition required."),
         Range(0f, 2f)]
        private float strangeMatchNerf;

        private readonly List<IGoal> goals = new();
        private readonly List<GoapAction> actions = new();

        private readonly Dictionary<string, Func<bool>> conditionData = new();
        private readonly Dictionary<string, Func<float>> variableData = new();

        private IContext context;
        private GoapPlanner planner;
        private Plan currentPlan;
        private GoapAction currentAction;

        private void Awake()
        {
            context = GetComponent<IContext>();
            if (context == null)
            {
                Debug.LogError($"No context on GOAP agent {gameObject.name}!");
                return;
            }
        }

        private void Start()
        {
            goapConfig?.ConfigureConditions(this, context);

            if (!visualConfig)
            {
                goapConfig?.ConfigureActions(this, context);
                goapConfig?.ConfigureGoals(this, context);
            }
            else
            {
                goals.AddRange(goalConfigs.Select(goal => goal.GetGoal(this)));
                actions.AddRange(actionConfigs.Select(action => action.GetAction(this)));
            }
            
            planner = new GoapPlanner(actions, this, strangeMatchNerf);
        }

        private void Update()
        {
            if (currentAction is { IsInterruptable: false, IsComplete: false })
            {
                currentAction?.Update(context);
                return;
            }

            Plan newPlan = planner.GetPlan(goals, actionConfigs, this);

            if (currentPlan == null || newPlan.planID != currentPlan.planID)
            {
                currentPlan = newPlan;
                //Debug.Log("New plan generated: " + newPlan);
                currentAction?.Stop(context);
                currentAction = currentPlan.plan[0];
                currentAction?.Start(context);
            }

            currentAction?.Update(context);
        }

        public void AddCondition(string conditionKey)
        {
            conditionData.Add(conditionKey, () => false);
        }

        public void AddCondition(string conditionKey, Func<bool> condition)
        {
            conditionData.Add(conditionKey, condition);
        }

        public void AddVariable(string variableKey, Func<float> variable)
        {
            variableData.Add(variableKey, variable);
        }

        public void SetCondition(string conditionKey, Func<bool> overridingBehavior)
        {
            if (conditionData.ContainsKey(conditionKey))
            {
                conditionData[conditionKey] = overridingBehavior;
            }
            else
            {
                Debug.LogError("Error in method SetCondition: ConditonKey does not exist in Agent!");
            }
        }

        public bool EvaluateCondition(GoapCondition condition)
        {
            switch (condition.conditionType)
            {
                case ConditionType.True:
                    return EvaluateCondition(condition.ConditionKey);
                case ConditionType.False:
                    return !EvaluateCondition(condition.ConditionKey);
                case ConditionType.Equals:
                    return Mathf.Approximately(GetVariable(condition.ConditionKey), condition.value);
                case ConditionType.NotEquals:
                    return !Mathf.Approximately(GetVariable(condition.ConditionKey), condition.value);
                case ConditionType.GreaterThan:
                    return GetVariable(condition.ConditionKey) > condition.value;
                case ConditionType.LessThan:
                    return GetVariable(condition.ConditionKey) < condition.value;
            }

            return false;
        }

        public bool EvaluateCondition(string conditionKey)
        {
            if (conditionData.ContainsKey(conditionKey))
            {
                return conditionData[conditionKey]();
            }

            Debug.LogError($"Error in method EvaluateCondition: ConditonKey [{conditionKey}] does not exist in Agent!");
            return false;
        }

        public float GetVariable(string variableKey)
        {
            if (variableData.ContainsKey(variableKey))
            {
                return variableData[variableKey]();
            }

            Debug.LogError("Error in method GetVariable: VariableKey does not exist in Agent!");
            return 0f;
        }

        private void OnDisable()
        {
            currentAction = null;
            currentPlan = null;
        }

        public void SetVariable(string variableKey, float value)
        {
            variableData[variableKey] = () => value;
        }
    }
}