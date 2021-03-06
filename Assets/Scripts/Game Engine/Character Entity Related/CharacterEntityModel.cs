﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEntityModel 
{
    [Header("General Properties")]
    public string myName;
    public AudioProfileType audioProfile;
    public Allegiance allegiance;
    public Controller controller;
    public LivingState livingState;

    [Header("Core Stats Properties")]
    public int power;
    public int dexterity;
    public int energy;
    public int stamina;
    public int initiative;
    public int draw;

    [Header("Health + Block Properties")]
    public int health;
    public int maxHealth;
    public int block;

    [Header("Resistance Properties")]
    public int basePhysicalResistance;
    public int baseMagicResistance;

    [Header("Model Component References")]
    [HideInInspector] public PassiveManagerModel pManager;

    [Header("Item Data References")]
    [HideInInspector] public ItemManagerModel iManager;

    [Header("View Components Properties ")]
    public LevelNode levelNode;
    public CharacterEntityView characterEntityView;

    [Header("Data References")]
    [HideInInspector] public EnemyDataSO enemyData;
    [HideInInspector] public CharacterData characterData;

    [Header("Card Properties")]
    [HideInInspector] public List<Card> drawPile = new List<Card>();
    [HideInInspector] public List<Card> discardPile = new List<Card>();
    [HideInInspector] public List<Card> hand = new List<Card>();
    [HideInInspector] public List<Card> expendPile = new List<Card>();

    [Header("Misc Combat Properties")]
    [HideInInspector] public bool hasLostHealthThisCombat = false;
    [HideInInspector] public int currentInitiativeRoll;
    [HideInInspector] public bool hasActivatedThisTurn;
    [HideInInspector] public int nextActivationCount = 1;
    [HideInInspector] public bool hasMovedOffStartingNode = false;
    [HideInInspector] public int meleeAttacksPlayedThisActivation = 0;

    [Header("Enemy Specific Properties")]
    [HideInInspector] public CharacterEntityModel currentActionTarget;
    [HideInInspector] public EnemyAction myNextAction;
    [HideInInspector] public List<EnemyAction> myPreviousActionLog = new List<EnemyAction>();
    [HideInInspector] public TargettingPathReadyState targettingPathReadyState;
    
}

public enum TargettingPathReadyState
{
    NotReady = 0,
    Ready = 1,
}

