using System;
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


    public override void TakeDamage(float damage, IDamager dealer)
    {
        DummyData existingData = dummyData.Find(data => data.dealer == dealer);

        if (existingData.dealer != null)
        {
            existingData.damageReceived += damage;
        }
        else
        {
            DummyData newData = new DummyData
            {
                damageReceived = damage,
                dealer = dealer
            };
            dummyData.Add(newData);
        }

        TotalDamageReceived += damage;
    }

    public override void Inizialize(CharacterData characterData, Character character)
    {
        base.Inizialize(characterData, character);
        dummyData = new List<DummyData>();
        TotalDamageReceived = 0;
    }

}
