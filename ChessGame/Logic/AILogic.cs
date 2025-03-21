using Microsoft.Xna.Framework.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ChessGame.Logic;

public class AiLogic
{
    
}

public class StockfishManager
{
    private readonly Process process = new Process();
    private SynchronizationContext context;
    private string outputData;
    private object thelock = new SpinLock();
    private List<string> moves;
    public void Execute(string path)
    {
        process.StartInfo = new ProcessStartInfo()
        {
            FileName = @path,
            UseShellExecute = false,
            CreateNoWindow = false,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };

        process.Start();
        Task.Run(() => ReadContiniously());
        Thread.Sleep(50); 
        WriteLine("ucinewgame");
        WriteLine("position startpos");
    }
    public string Search(string query)
    {
        while (true)
        {
            lock (thelock)
            {
                if (outputData.Contains(query))
                {
                    return outputData;
                }
            }
        }
    }

    public async void SetPosition(int[] positions)
    {
        Dictionary<int, char> notationtonumbers = new Dictionary<char, int>
        {
            { 'a', 0 },
            { 'b', 1 },
            { 'c', 2 },
            { 'd', 3 },
            { 'e', 4 },
            { 'f', 5 },
            { 'g', 6 },
            { 'h', 7 }
        }.ToDictionary(x => x.Value, x => x.Key);
        for (int i = 0; i < positions.Length; i += 4)
        {
            char[] temp = new char[4];
            temp[i] = positions[i].ToString().ToCharArray()[0];
            temp[i+1] = notationtonumbers[positions[i+1]];
            temp[i+2] = positions[i].ToString().ToCharArray()[0];
            temp[i+3] = notationtonumbers[positions[i+3]];
            moves.Add(string.Concat(temp));
        }
        WriteLine($"position startpos moves {String.Join(" ", moves)}");
    }

    public async Task<int[]> BestMoveWithPonder()
    {
        WriteLine("go depth 23");
        string task = await Task.Run(() => Search("bestmove"));
        task = task.Replace("ponder", "");
        task = task.Replace("bestmove", "");
        task = task.Replace(" ", "");
        Dictionary<char, int> notationtonumbers = new Dictionary<char, int>
        {
            { 'a', 0 },
            { 'b', 1 },
            { 'c', 2 },
            { 'd', 3 },
            { 'e', 4 },
            { 'f', 5 },
            { 'g', 6 },
            { 'h', 7 }
        };
        int[] result = new int[8];
        for (int i = 0; i < task.Length; i++)
        {
            if (int.TryParse(task[i].ToString(), out int result1) == false)
            {
                result[i] = notationtonumbers[task[i]];
            }
            else
            {
                result[i] = result1;
            }
        }

        return result;
    }
    public string Search(int length)
    {
        while (true)
        {
            lock (thelock)
            {
                if (outputData.Length == length)
                {
                    return outputData;
                }
            }
        }
    }
    public void ReadContiniously()
    {
        while (!process.HasExited)
        {
            lock (thelock) 
            {
                outputData = process.StandardOutput.ReadLine();
                Debug.WriteLine(outputData);
            }
        }
    }
    public async void WriteAsync(string text)
    {
        await process.StandardInput.WriteLineAsync(text);
        await process.StandardInput.FlushAsync();
    }
    public async void WriteLineAsync(string text)
    {
        await process.StandardInput.WriteLineAsync(text + Environment.NewLine);
        await process.StandardInput.FlushAsync();
    }
    public async void WriteLine(string text)
    {
        process.StandardInput.WriteLine(text + Environment.NewLine);
        process.StandardInput.Flush();
    }
    public void Write(string text)
    {
        process.StandardInput.WriteLine(text);
        process.StandardInput.Flush();
    }
}