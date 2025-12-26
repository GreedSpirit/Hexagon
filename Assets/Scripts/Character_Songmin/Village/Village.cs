using TMPro;
using UnityEngine;

public class Village
{
    public VillageManager VillageManager { get; set; }
    public string Name { get; private set; }
    public Vector2 SpawnZone { get; private set; }
    public string Img { get; private set; }
    public string Bgm { get; private set; }

    public VillageData VillageData { get; set; }

    public Village(VillageData data, VillageManager manager)
    {
        VillageData = data;
        VillageManager = manager;
        Init();
    }
    

    private void Init()
    {        
        Name = DataManager.Instance.GetString(VillageData.Name).Korean.Trim('"');
        SpawnZone = new Vector2(VillageData.CharSpawnAreaX, VillageData.CharSpawnAreaY);        
        Img = VillageData.Img;
        Bgm = VillageData.Bgm;
    }    
}