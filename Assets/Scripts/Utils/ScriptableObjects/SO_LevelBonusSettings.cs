using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelBonusSettings", menuName = "SO Variable/Level Bonus Settings", order = 1)]
public class SO_LevelBonusSettings : ScriptableObject
{

    public struct BonusStock
    {
        public int count;
        public string name;
        [SerializeField]
        public BonusBehaviour<PlayerEntity> bonus;
    }


    [System.Serializable]
    public struct BonusDropSettings
    {
        public string name;

        [SerializeField]
        public BonusBehaviour<PlayerEntity> bonus;

        [Range(0,10)]
        public int exactCount;

        [Range(0f,1f)]
        public float proportion;

        [Range(0f, 5f)]
        public float playersProportion;

        public BonusDropSettings(string name ,int exactCount , float proportion, float playersProportion)
        {
            this.name = name;
            this.exactCount = exactCount;
            this.proportion = proportion;
            this.playersProportion = playersProportion;
            this.bonus = null;
        }
    }

    public List<BonusDropSettings> bonusSettings
    {
        get
        {
            return _bonusSettings;
        }
    }

    [SerializeField]
    private List<BonusDropSettings> _bonusSettings = new List<BonusDropSettings>();

    [Range(0,500)]
    public int boxesCount = 100;

    [Range(1,10)]
    public int playersCount = 1;

    [Range(0f, 1f)]
    public float bonusPercentage = 1f;

    void OnEnable()
    {
        // Awake
    }

    public BonusStock[] ComputeBonusList(int bonusCount , int playersCount)
    {
        //int total = Mathf.CeilToInt(boxesCount * bonusPercentage);

        BonusStock[] result = new BonusStock[_bonusSettings.Count];

        // get exact counts
        int totalExact = 0;
        for (int i = 0; i < _bonusSettings.Count; i++)
        {
            BonusDropSettings settings = _bonusSettings[i];
            result[i].name = settings.name;
            result[i].bonus = settings.bonus; // assign the bonus scriptable object to the result

            if (settings.exactCount > 0)
            {
                result[i].count = Mathf.CeilToInt(settings.exactCount * (settings.playersProportion > 0 ? settings.playersProportion * playersCount : 1));
                totalExact += result[i].count;
            }
        }

        if(totalExact > bonusCount)
        {
            int newtotal = 0;
            for (int i = 0; i < _bonusSettings.Count; i++)
            {
                if (result[i].count > 0)
                {
                    result[i].count = Mathf.CeilToInt(((float)result[i].count/totalExact) * bonusCount);
                    newtotal += result[i].count;
                }
            }
            totalExact = newtotal;
        }

        int totalProportionnal = bonusCount - totalExact;
        if(totalProportionnal > 0)
        {
            float floatTotal = 0;
            float[] proportions = new float[_bonusSettings.Count];
            for (int i = 0; i < _bonusSettings.Count; i++)
            {
                BonusDropSettings settings = _bonusSettings[i];
                if (settings.exactCount == 0 && settings.proportion > 0)
                {
                    proportions[i] = (settings.proportion * totalProportionnal * (settings.playersProportion > 0 ? settings.playersProportion * playersCount : 1));
                    floatTotal += proportions[i];
                }
            }

            for (int i = 0; i < _bonusSettings.Count; i++)
            {
                BonusDropSettings settings = _bonusSettings[i];
                if (settings.exactCount == 0 && settings.proportion > 0)
                {
                    result[i].count = Mathf.RoundToInt((proportions[i] / floatTotal) * totalProportionnal);
                }
            }
        }
        return result;
    }
}
