using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effect/DOT")]
public class DotEffect : StatusEffectData
{

    public float DOTDamage;

    Coroutine cor;

    public void CharacterDOTApply(Character characterToDamage, float tik)
    {
        cor = characterToDamage.StartCoroutine(DOT(characterToDamage,tik));
    }

    public void CharacterDOTRemove(Character characterToDamage, float tik)
    {
        Debug.Log("DEAPPLY");
        characterToDamage.StopCoroutine(cor);
    }

    IEnumerator DOT(Character characterToDamage, float tik)
    {
        yield return new WaitForSeconds(tik);
        Debug.Log($"{characterToDamage} , {DOTDamage}");
        CharacterDOTApply( characterToDamage, tik);
    }
}
