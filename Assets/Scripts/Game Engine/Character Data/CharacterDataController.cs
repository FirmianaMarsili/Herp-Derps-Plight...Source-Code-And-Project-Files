﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDataController : Singleton<CharacterDataController>
{
    [HideInInspector] public List<CharacterData> allPlayerCharacters = new List<CharacterData>();

    // Build Characters From Data + Save/Load Logic
    #region
    public void BuildAllCharactersFromCharacterTemplateList(IEnumerable<CharacterTemplateSO> characters)
    {
        foreach(CharacterTemplateSO template in characters)
        {
            allPlayerCharacters.Add(ConverCharacterTemplateToCharacterData(template));
        }
    }
    public void BuildMyDataFromSaveFile(SaveGameData saveFile)
    {
        allPlayerCharacters.Clear();
        
        foreach (CharacterData characterData in saveFile.characters)
        {
            allPlayerCharacters.Add(characterData);
        }
    }
    public void SaveMyDataToSaveFile(SaveGameData saveFile)
    {
        foreach(CharacterData character in allPlayerCharacters)
        {
            saveFile.characters.Add(character);
        }
    }
    #endregion

    // Data Conversion
    #region
    public CharacterData ConverCharacterTemplateToCharacterData(CharacterTemplateSO template)
    {
        Debug.Log("CharacterDataController.ConverCharacterTemplateToCharacterData() called...");

        CharacterData newCharacter = new CharacterData();

        newCharacter.myName = template.myName;
        newCharacter.health = template.health;
        newCharacter.maxHealth = template.maxHealth;

        newCharacter.stamina = template.stamina;
        newCharacter.initiative = template.initiative;
        newCharacter.dexterity = template.dexterity;
        newCharacter.draw = template.draw;
        newCharacter.power = template.power;

        newCharacter.deck = new List<CardData>();
        foreach(CardDataSO cso in template.deck)
        {
            newCharacter.deck.Add(CardController.Instance.BuildCardDataFromScriptableObjectData(cso, newCharacter));
        }

        newCharacter.modelParts = new List<string>();
        newCharacter.modelParts.AddRange(template.modelParts);

        newCharacter.passiveManager = new PassiveManagerModel();
        PassiveController.Instance.BuildPassiveManagerFromSerializedPassiveManager(newCharacter.passiveManager, template.serializedPassiveManager);

        newCharacter.itemManager = new ItemManagerModel();      
        ItemController.Instance.CopySerializedItemManagerIntoStandardItemManager(template.serializedItemManager, newCharacter.itemManager);
        //ItemController.Instance.CopyItemManagerDataIntoOtherItemManager(template.itemManager, newCharacter.itemManager);

        return newCharacter;
    }

    #endregion

    // Modify Character Stats
    #region
    public void SetCharacterHealth(CharacterData data, int newValue)
    {
        Debug.Log("CharacterDataController.SetCharacterHealth() called for '" +
            data.myName + "', new health value = " + newValue.ToString());

        data.health = newValue;
    }
    #endregion
}
