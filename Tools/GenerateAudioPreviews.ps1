$source = @'
using System;
using System.IO;

public static class BlockArenaAudioPreview
{
    const int Rate = 44100;
    static readonly Random Random = new Random(7319);

    public static void Generate(string folder)
    {
        Directory.CreateDirectory(folder);
        Write(Path.Combine(folder, "01_Menu_Enerjik.wav"), Music(12, 142, false));
        Write(Path.Combine(folder, "02_Oyun_Arena.wav"), Music(12, 164, true));
        Write(Path.Combine(folder, "03_Engel_Tok.wav"), Obstacle());
        Write(Path.Combine(folder, "04_Kazanma_Seyirci_Hey.wav"), CrowdHey());
        Write(Path.Combine(folder, "05_Kaybetme_Seyirci_Ooo.wav"), CrowdOoo());
    }

    static float[] Music(int seconds, double bpm, bool arena)
    {
        int length = seconds * Rate;
        float[] data = new float[length];
        double beat = 60.0 / bpm;
        int[] menu = { 60, 64, 67, 72, 67, 64, 62, 67 };
        int[] game = { 57, 60, 64, 67, 64, 69, 67, 64 };
        int[] notes = arena ? game : menu;
        for (int i = 0; i < length; i++)
        {
            double t = i / (double)Rate;
            int step = (int)(t / (beat / 2.0));
            double local = (t % (beat / 2.0)) / (beat / 2.0);
            double freq = Midi(notes[step % notes.Length]);
            double lead = (Math.Sin(2 * Math.PI * freq * t) +
                           0.30 * Math.Sin(4 * Math.PI * freq * t)) *
                          Math.Exp(-3.2 * local) * 0.17;
            double bassFreq = Midi(notes[(step / 2) % notes.Length] - 12);
            double bass = Math.Sin(2 * Math.PI * bassFreq * t) *
                          Math.Exp(-2.0 * ((t % beat) / beat)) * 0.14;
            double beatPos = (t % beat) / beat;
            double kick = Math.Sin(2 * Math.PI * (70 - 30 * beatPos) * t) *
                          Math.Exp(-22 * beatPos) * (arena ? 0.30 : 0.23);
            double halfPos = (t % (beat / 2.0)) / (beat / 2.0);
            double hat = Noise() * Math.Exp(-45 * halfPos) * (arena ? 0.09 : 0.055);
            double clapPos = ((t + beat / 2.0) % beat) / beat;
            double clap = Noise() * Math.Exp(-35 * clapPos) * (arena ? 0.12 : 0.07);
            data[i] = Limit(lead + bass + kick + hat + clap);
        }
        FadeEdges(data, 0.03);
        return data;
    }

    static float[] Obstacle()
    {
        float[] data = new float[(int)(0.65 * Rate)];
        for (int i = 0; i < data.Length; i++)
        {
            double t = i / (double)Rate;
            double thud = Math.Sin(2 * Math.PI * (105 - 55 * t) * t) * Math.Exp(-9 * t) * 0.75;
            double stone = Noise() * Math.Exp(-18 * t) * 0.36;
            double knock = Math.Sin(2 * Math.PI * 240 * t) * Math.Exp(-23 * t) * 0.28;
            data[i] = Limit(thud + stone + knock);
        }
        return data;
    }

    static float[] CrowdHey()
    {
        float[] data = new float[(int)(1.55 * Rate)];
        for (int i = 0; i < data.Length; i++)
        {
            double t = i / (double)Rate;
            double total = 0;
            for (int voice = 0; voice < 11; voice++)
            {
                double start = 0.10 + voice * 0.009;
                double x = t - start;
                if (x < 0 || x > 0.48) continue;
                double f = 150 + voice * 8 + 35 * x;
                double env = Math.Min(1, x * 35) * Math.Exp(-4.7 * x);
                total += (Math.Sin(2 * Math.PI * f * x) +
                          0.45 * Math.Sin(4 * Math.PI * f * x) +
                          0.20 * Math.Sin(6 * Math.PI * f * x)) * env;
            }
            double cheer = Noise() * Math.Exp(-2.3 * Math.Max(0, t - 0.18)) * (t > 0.16 ? 0.11 : 0);
            data[i] = Limit(total * 0.055 + cheer);
        }
        return data;
    }

    static float[] CrowdOoo()
    {
        float[] data = new float[(int)(2.0 * Rate)];
        for (int i = 0; i < data.Length; i++)
        {
            double t = i / (double)Rate;
            double env = Math.Min(1, t * 6) * Math.Exp(-1.25 * t);
            double total = 0;
            for (int voice = 0; voice < 13; voice++)
            {
                double f = 175 - 28 * t + voice * 3.8;
                total += Math.Sin(2 * Math.PI * f * t + voice * 0.7) +
                         0.32 * Math.Sin(2 * Math.PI * f * 2 * t);
            }
            data[i] = Limit(total * 0.032 * env + Noise() * 0.025 * env);
        }
        return data;
    }

    static double Midi(int note) { return 440.0 * Math.Pow(2, (note - 69) / 12.0); }
    static double Noise() { return Random.NextDouble() * 2 - 1; }
    static float Limit(double value) { return (float)Math.Max(-0.96, Math.Min(0.96, value)); }

    static void FadeEdges(float[] data, double seconds)
    {
        int count = (int)(seconds * Rate);
        for (int i = 0; i < count; i++)
        {
            float gain = i / (float)count;
            data[i] *= gain;
            data[data.Length - 1 - i] *= gain;
        }
    }

    static void Write(string path, float[] samples)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
        {
            int bytes = samples.Length * 2;
            writer.Write(new char[] { 'R','I','F','F' });
            writer.Write(36 + bytes);
            writer.Write(new char[] { 'W','A','V','E','f','m','t',' ' });
            writer.Write(16); writer.Write((short)1); writer.Write((short)1);
            writer.Write(Rate); writer.Write(Rate * 2); writer.Write((short)2); writer.Write((short)16);
            writer.Write(new char[] { 'd','a','t','a' }); writer.Write(bytes);
            foreach (float sample in samples) writer.Write((short)(sample * short.MaxValue));
        }
    }
}
'@

Add-Type -TypeDefinition $source -Language CSharp
$output = Join-Path $PSScriptRoot '..\AudioPreviews'
[BlockArenaAudioPreview]::Generate([IO.Path]::GetFullPath($output))
