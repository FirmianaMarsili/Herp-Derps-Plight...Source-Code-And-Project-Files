﻿using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Collections;

public class LootController : Singleton<LootController>
{
    // Properties + Component References
    #region
    [Header("General Properties")]
    private LootResultModel currentLootResultData;
    private CharacterData currentCharacterSelection;

    [Header("Current Pity Timer Properties")]
    private int timeSinceLastEpic = 0;
    private int timeSinceLastRare = 0;
    private int timeSinceLastUpgrade = 0;

    [Header("Parent & Canvas Group Components")]
    [SerializeField] private GameObject visualParent;
    [SerializeField] private CanvasGroup visualParentCg;
    [SerializeField] private GameObject frontPageParent;
    [SerializeField] private CanvasGroup frontPageCg;
    [SerializeField] private GameObject chooseCardScreenParent;
    [SerializeField] private CanvasGroup chooseCardScreenCg;
    [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

    [Header("Card & Panel Components")]
    [SerializeField] private LootTab[] cardLootTabs;
    [SerializeField] private LootScreenCardViewModel[] lootCardViewModels;
    [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

    [Header("Character Deck Quick View References")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private CardInfoPanel[] cardPanels;
    [SerializeField] private CardViewModel previewCardVM;
    [SerializeField] private CanvasGroup previewCardCg;
    [SerializeField] private RectTransform[] rectRebuilds;
    [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

    [Header("XP Reward Screen Components")]
    public GameObject xpRewardScreenVisualParent;
    public CanvasGroup xpRewardScreenCg;
    public RewardCharacterBox[] rewardCharacterBoxes;


    public LootResultModel CurrentLootResultData
    {
        get { return currentLootResultData; }
        private set { currentLootResultData = value; }
    }
    public bool LootScreenIsActive()
    {
        return visualParent.activeSelf;
    }
    #endregion

    // Build Views
    #region
    public void CloseAndResetAllViews()
    {
        HideChooseCardScreen();
        HideFrontPageView();
        HideMainLootView();

        foreach(LootTab lt in cardLootTabs)
        {
            HideCardLootTab(lt);
        }
    }
    public void BuildLootScreenElementsFromLootResultData()
    {
        // Enable Choose card buttons
        for (int i = 0; i < CharacterDataController.Instance.AllPlayerCharacters.Count; i++)
        {
            ShowCardLootTab(cardLootTabs[i]);
            cardLootTabs[i].descriptionText.text = "Gain new card: " + 
                TextLogic.ReturnColoredText(CharacterDataController.Instance.AllPlayerCharacters[i].myName, TextLogic.neutralYellow);
        }
    }
    public void BuildChooseCardScreenCardsFromData(List<CardData> cardData)
    {
        for (int i = 0; i < cardData.Count; i++)
        {
            // Build card views
            CardController.Instance.BuildCardViewModelFromCardData(cardData[i], lootCardViewModels[i].cardViewModel);

            // Cache the card data
            lootCardViewModels[i].myDataRef = cardData[i];
        }
    }
    private void BuildCardInfoPanels(List<CardData> data)
    {
        // Disable + Reset all card info panels
        for (int i = 0; i < cardPanels.Length; i++)
        {
            cardPanels[i].gameObject.SetActive(false);
            cardPanels[i].copiesCount = 0;
            cardPanels[i].cardDataRef = null;
        }

        // Rebuild panels
        for (int i = 0; i < data.Count; i++)
        {
            CardInfoPanel matchingPanel = null;
            foreach (CardInfoPanel panel in cardPanels)
            {
                if (panel.cardDataRef != null && panel.cardDataRef.cardName == data[i].cardName)
                {
                    matchingPanel = panel;
                    break;
                }
            }

            if (matchingPanel != null)
            {
                matchingPanel.copiesCount++;
                matchingPanel.copiesCountText.text = "x" + matchingPanel.copiesCount.ToString();
            }
            else
            {
                cardPanels[i].gameObject.SetActive(true);
                cardPanels[i].BuildCardInfoPanelFromCardData(data[i]);
            }
        }
    }
    public void BuildAndShowCardViewModelPopup(CardData data)
    {
        previewCardCg.gameObject.SetActive(true);
        CardData cData = CardController.Instance.GetCardDataFromLibraryByName(data.cardName);
        CardController.Instance.BuildCardViewModelFromCardData(cData, previewCardVM);
        previewCardCg.alpha = 0;
        previewCardCg.DOFade(1f, 0.25f);
    }
    public void HidePreviewCard()
    {
        previewCardCg.gameObject.SetActive(false);
        previewCardCg.alpha = 0;
    }
    #endregion

    // Show, Hide And Transisition Screens
    #region
    public void FadeInMainLootView()
    {
        visualParentCg.alpha = 0;
        visualParent.SetActive(true);
        visualParentCg.DOFade(1f, 0.35f);
    }
    public void FadeOutMainLootView(Action onCompleteCallback)
    {
        visualParentCg.alpha = 1;
        Sequence s = DOTween.Sequence();
        s.Append(visualParentCg.DOFade(0f, 0.5f));
        s.OnComplete(() =>
        {
            if (onCompleteCallback != null)
            {
                onCompleteCallback.Invoke();
            }
        });
    }
    public void HideMainLootView()
    {
        visualParent.SetActive(false);
    }
    public void ShowFrontPageView()
    {
        frontPageCg.alpha = 0;
        frontPageParent.SetActive(true);
        frontPageCg.DOFade(1f, 0.35f);
    }
    public void HideFrontPageView()
    {
        frontPageParent.SetActive(false);
    }
    public void ShowChooseCardScreen()
    {
        chooseCardScreenCg.alpha = 0;
        chooseCardScreenParent.SetActive(true);
        chooseCardScreenCg.DOFade(1f, 0.35f);
    }
    public void HideChooseCardScreen()
    {
        chooseCardScreenParent.SetActive(false);

        // Reset card scales
        foreach(LootScreenCardViewModel card in lootCardViewModels)
        {
            card.ResetSelfOnEventComplete();
        }
    }
    public void ShowCardLootTab(LootTab tab)
    {
        tab.gameObject.SetActive(true);
    }
    public void HideCardLootTab(LootTab tab)
    {
        tab.gameObject.SetActive(false);
    }
    #endregion

    // Generate Loot Result Logic
    #region
    public void SetAndCacheNewLootResult()
    {
        CurrentLootResultData = GenerateNewLootResult();
    }
    public LootResultModel GenerateNewLootResult()
    {
        LootResultModel newLoot = new LootResultModel();

        for(int i = 0; i < CharacterDataController.Instance.AllPlayerCharacters.Count; i++)
        {
            newLoot.allCharacterCardChoices.Add(new List<CardData>());
            newLoot.allCharacterCardChoices[i] = GenerateCharacterCardLootChoices(CharacterDataController.Instance.AllPlayerCharacters[i]);
        }


        return newLoot;
    }
    #endregion

    // Get Lootable Cards Logic
    #region
    private List<CardData> GenerateCharacterCardLootChoices(CharacterData character)
    {
        // Set up + get all valid lootable cards
        List<CardData> listReturned = new List<CardData>();
        List<CardData> validLootableCards = GetAllValidLootableCardsForCharacter(character);

        // Seprate common, rare and epic cards
        List<CardData> commonCards = new List<CardData>();
        List<CardData> rareCards = new List<CardData>();
        List<CardData> epicCards = new List<CardData>();

        // Organise cards by rarity into loot buckets
        for(int i = 0; i < validLootableCards.Count; i++)
        {
            if(validLootableCards[i].rarity == Rarity.Common)
            {
                commonCards.Add(validLootableCards[i]);
            }
            else if (validLootableCards[i].rarity == Rarity.Rare)
            {
                rareCards.Add(validLootableCards[i]);
            }
            else if (validLootableCards[i].rarity == Rarity.Epic)
            {
                epicCards.Add(validLootableCards[i]);
            }
        }        

        // Get 3 random + different cards 
        for (int i = 0; i < 3; i++)
        {
            // Roll for rarity (5% Epic, 15% Rare, 80% Common probabilities)
            int roll = RandomGenerator.NumberBetween(1, 100);
            CardData cardSelected = null;

            // Check pity timers first
            if (timeSinceLastEpic >= GlobalSettings.Instance.epicPityTimer && epicCards.Count > 0)
            {
                Debug.Log("LootController epic card pity timer exceeded, forcing epic card loot reward!");
                epicCards.Shuffle();
                cardSelected = epicCards[0];
                listReturned.Add(cardSelected);
                epicCards.Remove(cardSelected);
                timeSinceLastEpic = 0;
                timeSinceLastRare++;
            }
            else if (timeSinceLastRare >= GlobalSettings.Instance.rarePityTimer && rareCards.Count > 0)
            {
                Debug.Log("LootController rare card pity timer exceeded, forcing rare card loot reward!");
                rareCards.Shuffle();
                cardSelected = rareCards[0];
                listReturned.Add(cardSelected);
                rareCards.Remove(cardSelected);
                timeSinceLastRare = 0;
                timeSinceLastEpic++;
            }

            // Epic
            else if(epicCards.Count > 0 && IsNumberBetweenValues(roll, GlobalSettings.Instance.epicCardLowerLimitProbability, GlobalSettings.Instance.epicCardUpperLimitProbability))
            {
                epicCards.Shuffle();
                cardSelected = epicCards[0];
                listReturned.Add(cardSelected);
                epicCards.Remove(cardSelected);
                timeSinceLastEpic = 0;
                timeSinceLastRare++;
            }

            // Rare
            else if (rareCards.Count > 0 && IsNumberBetweenValues(roll, GlobalSettings.Instance.rareCardLowerLimitProbability, GlobalSettings.Instance.rareCardUpperLimitProbability))
            {
                rareCards.Shuffle();
                cardSelected = rareCards[0];
                listReturned.Add(cardSelected);
                rareCards.Remove(cardSelected);
                timeSinceLastRare = 0;
                timeSinceLastEpic++;
            }

            // Common
            else
            {
                commonCards.Shuffle();
                cardSelected = commonCards[0];
                listReturned.Add(cardSelected);
                commonCards.Remove(cardSelected);

                timeSinceLastRare++;
                timeSinceLastEpic++;
            }

            // Roll for upgrade + Check pity timer
            int upgradeRoll = RandomGenerator.NumberBetween(1, 100);
            if((upgradeRoll >= 1 && upgradeRoll <= 3) ||
                timeSinceLastUpgrade >= GlobalSettings.Instance.upgradePityTimer)
            {
                // Hit a successful roll, upgrade the card
                CardData upgradedVersion = CardController.Instance.FindUpgradedCardData(cardSelected);

                if(upgradedVersion != null)
                {
                    listReturned.Remove(cardSelected);
                    listReturned.Add(upgradedVersion);
                    timeSinceLastUpgrade = 0;
                }
                else
                {
                    timeSinceLastUpgrade++;
                }
            }

            else
            {
                timeSinceLastUpgrade++;
            }
        }

        return listReturned;
    }
    public List<CardData> GetAllValidLootableCardsForCharacter(CharacterData character)
    {
        List<CardData> validLootableCards = new List<CardData>();

        // Get a list of all possible lootable cards
        foreach (CardData card in CardController.Instance.AllCards)
        {
            if (IsCardLootableByCharacter(character, card) && card.upgradeLevel == 0)
            {
                validLootableCards.Add(card);
            }
        }

        return validLootableCards;
    }
    private bool IsCardLootableByCharacter(CharacterData character, CardData card)
    {
        bool bReturned = false;

        foreach(TalentPairingModel tp in character.talentPairings)
        {
            // Does the character have the required talent school?
            if (tp.talentSchool == card.talentSchool)
            {
                bReturned = true;
                break;
            }
        }

        return bReturned;
    }
    #endregion

    // On Click Events
    #region
    public void OnLootTabButtonClicked(LootTab buttonClicked)
    {        
        if(buttonClicked.TabType == LootTabType.CardReward)
        {
            // Get the card reward buttons index for using later
            int index = 0;
            for (int i = 0; i < cardLootTabs.Length; i++)
            {
                if (cardLootTabs[i] == buttonClicked)
                {
                    index = i;
                    break;
                }
            }

            // Get the predetermined card loot result for the character
            List<CardData> cardChoices = CurrentLootResultData.allCharacterCardChoices[index];

            // Cache the character, so we know which character to reward a card to if player chooses one
            currentCharacterSelection = CharacterDataController.Instance.AllPlayerCharacters[index];
            characterNameText.text = currentCharacterSelection.myName;

            // Build choose card view models
            BuildChooseCardScreenCardsFromData(cardChoices);
            BuildCardInfoPanels(currentCharacterSelection.deck);

            // Hide front screen, show choose card screen
            HideFrontPageView();
            ShowChooseCardScreen();

            // Rebuild Deck quick view lay out
            foreach (RectTransform t in rectRebuilds)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(t);
            }
        }
        

    }
    public void OnLootCardViewModelClicked(LootScreenCardViewModel cardClicked)
    {
        // Add card to character deck
        CharacterDataController.Instance.AddCardToCharacterDeck(currentCharacterSelection, CardController.Instance.CloneCardDataFromCardData(cardClicked.myDataRef));

        // Close choose card window,  reopen front screen
        HideChooseCardScreen();
        ShowFrontPageView();

        // TO DO: find a better way to find the matching card tab
        // hide add card to deck button
        HideCardLootTab(cardLootTabs[CharacterDataController.Instance.AllPlayerCharacters.IndexOf(currentCharacterSelection)]);


    }
    public void OnChooseCardScreenBackButtonClicked()
    {
        HideChooseCardScreen();
        ShowFrontPageView();
    }
    public void OnContinueButtonClicked()
    {
        if (VisualEventManager.Instance.EventQueue.Count == 0)
        {
            EventSequenceController.Instance.HandleLoadNextEncounter();
        }
    }

    #endregion

    // Misc Roll Stuff
    #region
    public bool IsNumberBetweenValues(int number, int lowerLimit, int higherLimit)
    {
        if(number >= lowerLimit && number <= higherLimit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    // Save + Load Logic
    #region
    public void SaveMyDataToSaveFile(SaveGameData saveFile)
    {
        saveFile.currentLootResult = CurrentLootResultData;
        saveFile.timeSinceLastRare = timeSinceLastRare;
        saveFile.timeSinceLastEpic = timeSinceLastEpic;
        saveFile.timeSinceLastUpgrade = timeSinceLastUpgrade;
    }
    public void BuildMyDataFromSaveFile(SaveGameData saveFile)
    {
        CurrentLootResultData = saveFile.currentLootResult;
        timeSinceLastRare = saveFile.timeSinceLastRare;
        timeSinceLastEpic = saveFile.timeSinceLastEpic;
        timeSinceLastUpgrade = saveFile.timeSinceLastUpgrade;
    }
    #endregion

    // Post Combat Mini XP Event Logic
    #region
    public void PlayNewXpRewardVisualEvent(List<PreviousXpState> pxpData, List<XpRewardData> xpRewardDataSet)
    {
        StartCoroutine(PlayNewXpRewardVisualEventCoroutine(pxpData, xpRewardDataSet));
    }
    private IEnumerator PlayNewXpRewardVisualEventCoroutine(List<PreviousXpState> pxsData, List<XpRewardData> xpRewardDataSet)
    {
        FadeInXpRewardScreen();
        yield return new WaitForSeconds(0.5f);     

        // Disable + Reset reward boxes
        foreach(RewardCharacterBox b in rewardCharacterBoxes)
        {
            b.visualParent.SetActive(false);
        }

        int index = 0;

        // Build character box starting states
        foreach (CharacterData character in CharacterDataController.Instance.AllPlayerCharacters)
        {
            // Find the characters matching psx data
            foreach(PreviousXpState psx in pxsData)
            {
                if(psx.characterRef == character)
                {
                    foreach(XpRewardData xrd in xpRewardDataSet)
                    {
                        if(xrd.characterRef == character)
                        {
                            BuildRewardCharacterBoxStartingViewState(rewardCharacterBoxes[index], character, psx, xrd);
                            break;
                        }
                    }              
                }
            }

            index++;
        }

        // for each character
        // play rolling number anim for total xp
        // set combat 
    }
    private void FadeInXpRewardScreen(float speed = 0.5f)
    {
        xpRewardScreenVisualParent.SetActive(true);
        xpRewardScreenCg.alpha = 0;
        xpRewardScreenCg.DOFade(1, speed);
    }
    private void BuildRewardCharacterBoxStartingViewState(RewardCharacterBox box, CharacterData character, PreviousXpState pxs, XpRewardData xrd)
    {
        // Enable view
        box.visualParent.SetActive(true);
        box.flawlessParent.SetActive(false);

        // Calculate total xp gained for total xp text
        int totalXpGainedInt = xrd.totalXpGained;

        // Build ucm 
        CharacterModelController.BuildModelFromStringReferences(box.ucm, character.modelParts);

        // Build level and combat type texts
        box.currentLevelText.text = pxs.previousLevel.ToString();        
        box.combatTypeText.text = TextLogic.SplitByCapitals(xrd.encounterType.ToString());

        if(xrd.encounterType == EncounterType.BasicEnemy)
        {
            box.combatTypeRewardText.text = GlobalSettings.Instance.basicCombatXpReward.ToString();
        }
        else if (xrd.encounterType == EncounterType.EliteEnemy)
        {
            box.combatTypeRewardText.text = GlobalSettings.Instance.eliteCombatXpReward.ToString();
        }
        else if (xrd.encounterType == EncounterType.BossEnemy)
        {
            box.combatTypeRewardText.text = GlobalSettings.Instance.bossCombatXpReward.ToString();
        }

        // Check flawless
        if(xrd.flawless == true)
        {
            box.flawlessParent.SetActive(true);
            box.flawlessAmountText.text = GlobalSettings.Instance.noDamageTakenXpReward.ToString();
            totalXpGainedInt += GlobalSettings.Instance.noDamageTakenXpReward;
        }

        // Set total xp gained text
        box.totalXpText.text = "+" + totalXpGainedInt.ToString() + " XP";

        // Set xp bar start view state
        UpdateXpBarPosition(box.xpBar, pxs.previousXp, pxs.previousMaxXp);

        // Play xp bar animation
        PlayUpdateXpBarAnimation(character, box, pxs);

    }
    private void UpdateXpBarPosition(Slider xpBar, int currentXp, int maxXp)
    {
        // Convert health int values to floats
        float currentXpFloat = currentXp;
        float currentMaxXpFloat = maxXp;
        float xpBarFloat = currentXpFloat / currentMaxXpFloat;

        // Modify health bar slider + health texts
        xpBar.value = xpBarFloat;
    }
    private void PlayUpdateXpBarAnimation(CharacterData data, RewardCharacterBox box, PreviousXpState pxs)
    {
        StartCoroutine(PlayUpdateXpBarAnimationCoroutine( data, box, pxs));
    }
    private IEnumerator PlayUpdateXpBarAnimationCoroutine(CharacterData data, RewardCharacterBox box, PreviousXpState pxs)
    {
        // Should we play level up bar animation?
        if (pxs.previousLevel < data.currentLevel)
        {
            int maxXpPoint = pxs.previousMaxXp;
            int currentXpPoint = pxs.previousXp;

            // Gradually move xp slider to end of bar 
            while (currentXpPoint < maxXpPoint)
            {
                UpdateXpBarPosition(box.xpBar, currentXpPoint, maxXpPoint);
                currentXpPoint++;
                yield return null;
            }

            // Hit the end of bar, do level gained visual stuff
            box.currentLevelText.text = data.currentLevel.ToString();

            // Move xp slider to final position after xp overflow
            maxXpPoint = data.currentMaxXP;
            currentXpPoint = 0;
            while (currentXpPoint < data.currentXP)
            {
                UpdateXpBarPosition(box.xpBar, currentXpPoint, maxXpPoint);
                currentXpPoint++;
                yield return null;
            }
        }
        else
        {
            int maxXpPoint = data.currentMaxXP;
            int currentXpPoint = pxs.previousXp;

            // Gradually move xp slider to new pos
            while (currentXpPoint < data.currentXP)
            {
                UpdateXpBarPosition(box.xpBar, currentXpPoint, maxXpPoint);
                currentXpPoint++;
                yield return null;
            }
        }
    }
    #endregion
}


