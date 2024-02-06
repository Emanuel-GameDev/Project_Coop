using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DummyCharacter : EnemyCharacter
{
    public float totalDamageReceived;
    private float _totalDamageReceived;
    private bool attackMode = false;
    private bool AttackMode
    {
        get => attackMode;
        set { attackMode = value; animator.SetBool("Attack", value); }
    }
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

    public void ChangeBehaviour()
    {
        AttackMode = !AttackMode;
    }

    protected override void InitialSetup()
    {
        base.InitialSetup();
        dummyData = new List<DummyData>();
        TotalDamageReceived = 0;
    }

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

        //guardo la condition

        if (data.condition != null)
            data.condition.AddCondition(this);

        Debug.Log($"Dummy ha subito [{data.damage}] danni con condition [{data.condition}] da [{data.dealer.GetType()}] \nIl totale dei danni subiti ammonta a [{totalDamageReceived}]");
    }
}
