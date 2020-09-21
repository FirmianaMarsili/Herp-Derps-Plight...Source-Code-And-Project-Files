﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;

[Serializable]
public class EnemyAction
{
    [BoxGroup("General Action Data", centerLabel: true)]
    [LabelWidth(150)]
    public string actionName;

    [BoxGroup("General Action Data")]
    [LabelWidth(150)]
    public ActionType actionType;

    [BoxGroup("General Action Data")]
    [LabelWidth(150)]
    public IntentImage intentImage;

    [BoxGroup("General Action Data")]
    [LabelWidth(150)]
    public int actionLoops = 1;

    [BoxGroup("General Action Data")]
    [LabelWidth(200)]
    public bool doThisOnFirstActivation;

    [BoxGroup("General Action Data")]
    [LabelWidth(200)]
    public bool canBeConsecutive;

    [BoxGroup("General Action Data")]
    [LabelWidth(200)]
    public bool prioritiseWhenRequirementsMet;

    [BoxGroup("General Action Data")]
    [LabelWidth(100)]
    public List<ActionRequirement> actionRequirements;

    [BoxGroup("General Action Data")]
    [LabelWidth(150)]
    public List<EnemyActionEffect> actionEffects;


  

}



[Serializable]
public class ActionRequirement
{
    public ActionRequirementType requirementType;
    [ShowIf("ShowReqValue")]
    public int requirementTypeValue;

    [ShowIf("ShowStatusRequired")]
    public PassiveIconDataSO passiveRequired;
    [ShowIf("ShowStatusRequired")]
    public int statusStacksRequired;

    public bool ShowStatusRequired()
    {
        if(requirementType == ActionRequirementType.HasPassiveTrait)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool ShowReqValue()
    {
        if (requirementType != ActionRequirementType.HasPassiveTrait)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
[Serializable]
public class EnemyActionEffect
{
    [VerticalGroup("General Properties")]
    [LabelWidth(150)]
    public AnimationEventData animationEventData;
    [VerticalGroup("General Properties")]
    [LabelWidth(150)]
    public ActionType actionType;

    // damage target
    [VerticalGroup("Damage Properties")]
    [ShowIf("ShowDamage")]
    [LabelWidth(150)]
    public int baseDamage;

    [VerticalGroup("Damage Properties")]
    [ShowIf("ShowDamage")]
    [LabelWidth(150)]
    public DamageType damageType;


    // Status properties
    [VerticalGroup("Status Properties")]
    [ShowIf("ShowStatus")]
    [LabelWidth(150)]
    public PassiveIconDataSO passiveApplied;

    [VerticalGroup("Status Properties")]
    [ShowIf("ShowStatus")]
    [LabelWidth(150)]
    public int passiveStacks;


    // Block properties
    [VerticalGroup("Block Properties")]
    [ShowIf("ShowBlock")]
    [LabelWidth(150)]
    public int blockGained;


    // Add card properties
    [VerticalGroup("Card Properties")]
    [ShowIf("ShowCard")]
    [LabelWidth(150)]
    public CardDataSO cardAdded;

    [VerticalGroup("Card Properties")]
    [ShowIf("ShowCard")]
    [LabelWidth(150)]
    public int copiesAdded;

    [VerticalGroup("Card Properties")]
    [ShowIf("ShowCard")]
    [LabelWidth(150)]
    public CardCollection collection;



    // Inspector bools
    public bool ShowDamage()
    {
        if(actionType == ActionType.AttackAllEnemies ||
            actionType == ActionType.AttackTarget)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ShowStatus()
    {
        if (actionType == ActionType.BuffAllAllies ||
            actionType == ActionType.BuffSelf ||
            actionType == ActionType.BuffTarget ||
            actionType == ActionType.DebuffAllEnemies ||
            actionType == ActionType.DebuffTarget)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool ShowBlock()
    {
        if (actionType == ActionType.DefendAllAllies ||
            actionType == ActionType.DefendSelf ||
            actionType == ActionType.DefendTarget)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool ShowCard()
    {
        if (actionType == ActionType.AddCard)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
public class AddBoxToDrawer<T> : OdinValueDrawer<T>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        SirenixEditorGUI.BeginBox();
        CallNextDrawer(label);
        SirenixEditorGUI.EndBox();
    }
}