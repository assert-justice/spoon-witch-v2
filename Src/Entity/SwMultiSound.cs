using System.Collections.Generic;
using Godot;
using SW.Src.Global;

namespace SW.Src.Entity;

public partial class SwMultiSound : AudioStreamPlayer2D
{
    [Export] private AudioStream[] Streams = [];
    private readonly Queue<AudioStream> StreamList = [];
    private readonly List<AudioStream> TempStreams = [];
    private bool TryGetShuffled(out AudioStream audioStream)
    {
        // Puts sound effects in shuffled order such that no elements repeat back to back.
        audioStream = default;
        if(Streams.Length == 0)
        {
            SwStatic.LogError("No streams assigned");
            return false;
        }
        if(Streams.Length == 1)
        {
            audioStream = Streams[0];
            return true;
        }
        if(StreamList.Count > 1)
        {
            audioStream = StreamList.Dequeue();
            return true;
        }
        if(StreamList.Count == 0) StreamList.Enqueue(SwStatic.ChooseRandom(Streams, "Should be unreachable"));
        audioStream = StreamList.Dequeue();
        foreach (var stream in Streams)
        {
            if(stream == audioStream) continue;
            TempStreams.Add(stream);
        }
        while(TempStreams.Count > 0)
        {
            var item = SwStatic.ChooseRandom(TempStreams, "Should be unreachable");
            StreamList.Enqueue(item);
            TempStreams.Remove(item);
        }
        return true;
    }
    private void Play(AudioStream audioStream)
    {
        Stream = audioStream;
        Play();
    }
    public int GetSoundsCount(){return Streams.Length;}
    public void PlayShuffled()
    {
        if(!TryGetShuffled(out var stream)) return;
        Play(stream);
    }
    public void PlayRandom()
    {
        if(Streams.Length == 0)
        {
            SwStatic.LogError("No streams assigned");
            return;
        }
        Play(SwStatic.ChooseRandom(Streams, "Should be unreachable"));
    }
    public void Play(int idx)
    {
        if(idx < 0 || idx >= Streams.Length) SwStatic.LogError($"Invalid index for '{Name}': {idx}");
        else Play(Streams[idx]);
    }
}
