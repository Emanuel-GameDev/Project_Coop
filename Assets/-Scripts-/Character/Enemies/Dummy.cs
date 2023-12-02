using System;
using UnityEngine;
using System.Collections.Generic;


public class Dummy : CharacterClass
{
    public float totalDamageReceived;
    private float _totalDamageReceived;
    public float TotalDamageReceived
    {
        get => _totalDamageReceived;
        set
        {
            _totalDamageReceived = value;
            totalDamageReceived = value;
        }
    }

    [Serializable]
    private struct DummyData
    {
        public float damageReceived;
        public IDamager dealer;
    }

    private List<DummyData> dummyData;


    public override void TakeDamage(DamageData data)
    {

        DummyData existingData = dummyData.Find(dataToFind => dataToFind.dealer == data.dealer);

        if (existingData.dealer != null)
        {
            existingData.damageReceived += data.damage;
        }
        else
        {
            DummyData newData = new DummyData
            {
                damageReceived = data.damage,
                dealer = data.dealer
            };
            dummyData.Add(newData);
        }

        TotalDamageReceived += data.damage;
        Debug.Log("Dummy subito " + totalDamageReceived + " danni");
        
    }

    public override void Inizialize(CharacterData characterData, Character character)
    {
        base.Inizialize(characterData, character);
        dummyData = new List<DummyData>();
        TotalDamageReceived = 0;
    }

}
