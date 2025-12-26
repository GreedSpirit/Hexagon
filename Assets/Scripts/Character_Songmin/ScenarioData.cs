using System;

public enum Scenario_Type
{
    intro, dungeon, stage, boss, ending, None
}

public enum Sin_Type
{
    Pride, Envy, Greed, Wrath, Sloth, Gluttony, Lust, None
}

public enum Trigger_Type
{
    gamestart, dungeonenter, stageenter, prebattle, preseal, clear, None
}

public enum Event_Type
{
    img_show, img_hide, shake, img_change, None
}

public enum Image_Effect
{
    fade_in, fade_out, None
}

public class ScenarioData : CSVLoad, TableKey
{
    public string Id { get; set; }
    public Scenario_Type Scenario_Type { get; set; }
    public Sin_Type Sin_Type { get; set; }
    public Trigger_Type Trigger_Type { get; set; }
    public int Play_Order { get; set; }
    public Event_Type Event_Type { get; set; }
    public string Image_ID { get; set; }
    public Image_Effect Image_Effect { get; set; }
    public string Npc { get; set; } //캐릭터 이름
    public string Character_Image { get; set; }
    public string Background { get; set; }
    public string Dialogue { get; set; }
    public string Next_Flag { get; set; }

    int TableKey.Id
    {
        get { return Play_Order; }
    }
    string TableKey.Key
    {
        get { return Id; }
    }

    public void LoadFromCsv(string[] values)
    {
        // 0: Id
        Id = values[0];

        // 1 : Scenario_Type
        if (Enum.TryParse(values[1], out Scenario_Type type1))
        {
            Scenario_Type = type1;
        }
        else
        {
            Scenario_Type = Scenario_Type.None;
        }

        // 2 : Sin_Type
        if (Enum.TryParse(values[2], out Sin_Type type2))
        {
            Sin_Type = type2;
        }
        else
        {
            Sin_Type = Sin_Type.None;
        }

        // 3 : Trigger
        if (Enum.TryParse(values[3], out Trigger_Type type3))
        {
            Trigger_Type = type3;
        }
        else
        {
            Trigger_Type = Trigger_Type.None;
        }

        // 4 : Play_Order
        if (int.TryParse(values[4], out int order))
        {
            Play_Order = order;
        }
        else
        {
            Play_Order = 0;
        }

        // 5 : Event
        if (Enum.TryParse(values[5], out Event_Type type4))
        {
            Event_Type = type4;
        }
        else
        {
            Event_Type = Event_Type.None;
        }

        // 6 : Image_ID
        Image_ID = values[6];

        // 7 : Image_Effect
        if (Enum.TryParse(values[7], out Image_Effect type5))
        {
            Image_Effect = type5;
        }
        else
        {
            Image_Effect = Image_Effect.None;
        }

        // 8 : Npc
        Npc = values[8];

        // 9 : Character_Image
        Character_Image = values[9];

        // 10 : Background
        Background = values[10];

        // 11 : Dialogue
        Dialogue = values[11];
    }
}
