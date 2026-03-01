using SW.Src.Utils;

namespace SW.Src.Global;

public class SwSettings
{
    private const string Path = "settings/game_settings.json";
    public bool Fullscreen = false;
    public float MainVolume = 0.5f;
    public float MusicVolume = 0.5f;
    public float SfxVolume = 0.5f;
    // public float VoiceVolume = 1;
    public bool PauseOnSubmenu = false;
    public float GameSpeed = 1;
    public float DamageTakenMultiplier = 1;
    public float DamageDealtMultiplier = 1;
    public SwSettings(){}
    public bool TryLoad()
    {
        SwFs fs = new();
        if(!fs.TryReadJson(Path, out var node)) return false;
        SwJsonDb db = new(node);
        if(db.TryGetNumber("main_volume", out float d))MainVolume = d;
        if(db.TryGetNumber("music_volume", out d))MusicVolume = d;
        if(db.TryGetNumber("sfx_volume", out d))SfxVolume = d;
        // if(db.TryGetNumber("voice_volume", out d))VoiceVolume = d;
        if(db.TryGetBool("pause_on_submenu", out bool b))PauseOnSubmenu = b;
        if(db.TryGetNumber("game_speed", out d))GameSpeed = d;
        if(db.TryGetNumber("damage_taken_multiplier", out d))DamageTakenMultiplier = d;
        if(db.TryGetNumber("damage_dealt_multiplier", out d))DamageDealtMultiplier = d;
        return true;
    }
    public bool Save()
    {
        SwFs fs = new();
        SwJsonDb db = new();
        db.TrySetPath("main_volume", MainVolume);
        db.TrySetPath("music_volume", MusicVolume);
        db.TrySetPath("sfx_volume", SfxVolume);
        // db.TrySetPath("voice_volume", VoiceVolume);
        db.TrySetPath("pause_on_submenu", PauseOnSubmenu);
        db.TrySetPath("game_speed", GameSpeed);
        db.TrySetPath("damage_taken_multiplier", DamageTakenMultiplier);
        db.TrySetPath("damage_dealt_multiplier", DamageDealtMultiplier);
        return fs.TryWriteFile(Path, db.ToString());
    }
}
