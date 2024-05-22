using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;

public class HUDInfo : MonoBehaviour
{
    public Character heroCharacter;
    private Hero hero;

    public TextMeshProUGUI textType;
    public TextMeshProUGUI textHP;
    public TextMeshProUGUI textMovement;
    public TextMeshProUGUI textAttack;
    public TextMeshProUGUI textRange;
    public TextMeshProUGUI textDef;

    private void Start()
    {
        if (heroCharacter is Hero)
        {
            hero = (Hero)heroCharacter;
        }
        else
        {
            Debug.LogError("Character is not a Hero or Enemy_Basic");
        }
    }

    private void Update()
    {
        if (heroCharacter != null)
        {
            /*
            if (heroCharacter.heroSO != null)
            {
                textType.text = "Type: " + heroCharacter.heroSO.attributes.elementType;
                textHP.text = "HP: " + heroCharacter.currentHealth + " / " + heroCharacter.maxHealth;
                textMovement.text = "MOV: " + heroCharacter.moveDistance;
                textAttack.text = "ATK: " + heroCharacter.attackDamage;
                textRange.text = "A.Rng: " + heroCharacter.heroSO.attributes.attackRange;
                textDef.text = "Def: " + heroCharacter.defensePercentage;
            }
            else
            {
                Debug.LogError("Character doesn't have a HeroSO assigned!");
            } */
        }
    }
}
