using SW.Src.Utils;

namespace SW.Src.Global;

public class SwSettings
{
    private const string Path = "game_settings.json";
    public bool InitialFullscreen = false;
    public float InitialMainVolume = 0.5f;
    public float InitialMusicVolume = 0.5f;
    public float InitialSfxVolume = 0.5f;
    public float InitialVoiceVolume = 0.5f;
    public bool PauseOnSubmenu = false;
    public float GameSpeed = 1;
    public float DamageTakenMultiplier = 1;
    public float DamageDealtMultiplier = 1;
    // Debug settings
    public float MinPitchShift = 1f;
    public float MaxPitchShift = 1f;
    public bool DebugDraw = false;
    public bool CreativeMode = false;
    public SwSettings(){}
    public bool TryLoad()
    {
        SwFs fs = new();
        if(!fs.TryReadJson(Path, out var node)) return false;
        SwJsonDb db = new(node);
        if(db.TryGetBool("fullscreen", out bool b))InitialFullscreen = b;
        if(db.TryGetNumber("main_volume", out float d))InitialMainVolume = d;
        if(db.TryGetNumber("music_volume", out d))InitialMusicVolume = d;
        if(db.TryGetNumber("sfx_volume", out d))InitialSfxVolume = d;
        if(db.TryGetNumber("voice_volume", out d))InitialVoiceVolume = d;
        if(db.TryGetBool("pause_on_submenu", out b))PauseOnSubmenu = b;
        if(db.TryGetNumber("game_speed", out d))GameSpeed = d;
        if(db.TryGetNumber("damage_taken_multiplier", out d))DamageTakenMultiplier = d;
        if(db.TryGetNumber("damage_dealt_multiplier", out d))DamageDealtMultiplier = d;
        if(db.TryGetNumber("min_pitch_shift", out d))MinPitchShift = d;
        if(db.TryGetNumber("max_pitch_shift", out d))MaxPitchShift = d;
        if(db.TryGetBool("debug_draw", out b))DebugDraw = b;
        if(db.TryGetBool("creative_mode", out b))CreativeMode = b;
        return true;
    }
    public bool Save()
    {
        SwFs fs = new();
        SwJsonDb db = new();
        db.TrySetPath("fullscreen", SwGlobal.IsFullscreen());
        db.TrySetPath("main_volume", SwGlobal.GetVolume(SwGlobal.AudioBus.Main));
        db.TrySetPath("music_volume", SwGlobal.GetVolume(SwGlobal.AudioBus.Music));
        db.TrySetPath("sfx_volume", SwGlobal.GetVolume(SwGlobal.AudioBus.Sfx));
        db.TrySetPath("voice_volume", SwGlobal.GetVolume(SwGlobal.AudioBus.Voice));
        db.TrySetPath("pause_on_submenu", PauseOnSubmenu);
        db.TrySetPath("game_speed", GameSpeed);
        db.TrySetPath("damage_taken_multiplier", DamageTakenMultiplier);
        db.TrySetPath("damage_dealt_multiplier", DamageDealtMultiplier);
        db.TrySetPath("min_pitch_shift", MinPitchShift);
        db.TrySetPath("max_pitch_shift", MaxPitchShift);
        db.TrySetPath("debug_draw", DebugDraw);
        db.TrySetPath("creative_mode", CreativeMode);
        return fs.TryWriteFile("user://" + Path, db.ToString());
    }
}
