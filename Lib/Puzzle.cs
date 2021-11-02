#nullable enable

using KeepCoding;
using KModkitLib;
using System.Linq;
using UnityEngine;

internal class Puzzle
{
    State[] initStates = new State[3];
    Configuration[] configurations;

    internal State[] currentStates;
    internal int[] possibleMovements;
    ILog logger;

    internal Puzzle(ILog logger)
    {
        this.logger = logger;
        var configurationList = Enumerable.Range(0, 3).Select(i => new Configuration(i)).ToList();
        configurationList.Shuffle();
        configurations = configurationList.ToArray();
        
        logger.Log($"Configuration A: {configurations[0]}");
        logger.Log($"Configuration B: {configurations[1]}");
        logger.Log($"Configuration C: {configurations[2]}");
        var possibleAnswerState = State.RandomState();
        do
        {
            possibleMovements = Enumerable.Range(0, 3).Select(_ => UnityEngine.Random.Range(0, 4)).ToArray();
        } while (possibleMovements.Sum() < 2);

        for(int i = 0; i < 3; i++)
        {
            initStates[i] = possibleAnswerState;
        }

        for (int j = 0; j < 3; j++)
        {
            initStates = configurations[j].ApplyToStates(initStates, possibleMovements[j]);
        }

        currentStates = (State[])initStates.Clone();
        logger.Log($"Initial State Red: {initStates[0]}");
        logger.Log($"Initial State Blue: {initStates[1]}");
        logger.Log($"Initial State Green: {initStates[2]}");
        logger.Log($"Possible Movements: A*{(4 - possibleMovements[0]) % 4}, B*{(4 - possibleMovements[1]) % 4}, C*{(4 - possibleMovements[2]) % 4}");
        logger.Log($"State after possible movements: {possibleAnswerState}");
    }

    static string[] numToConfigLabel = new string[] { "A", "B", "C" };
    internal void ApplyConfiguration(int config)
    {
        logger.Log($"Pressed {numToConfigLabel[config]}");
        currentStates = configurations[config].ApplyToStates(currentStates);
        logger.Log($"State Red: {currentStates[0]}");
        logger.Log($"State Blue: {currentStates[1]}");
        logger.Log($"State Green: {currentStates[2]}");
    }

    internal void Reset()
    {
        logger.Log($"Resetting states to initial states.");
        currentStates = (State[])initStates.Clone();
    }

    internal bool Check()
    {
        return currentStates[0].Equals(currentStates[1]) && currentStates[1].Equals(currentStates[2]);
    }
}

internal struct Place {
    internal int x;
    internal int y;

    internal Place(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    internal static Place RandomPlace()
    {
        return new Place(UnityEngine.Random.Range(0, 4), UnityEngine.Random.Range(0, 4));
    }

    internal int ToIconIndex()
    {
        return x * 4 + y;
    }

}

internal struct State
{
    internal Place place;
    internal bool isClock;

    internal State (Place place, bool isClock)
    {
        this.place = place;
        this.isClock = isClock;
    }

    internal State ApplyMovement(int dx, int dy)
    {
        if (dx == 0 && dy == 0) return new State(place, !isClock);
        return new State(new Place((place.x + dx + 4) % 4, (place.y + dy + 4) % 4), isClock);
    }

    internal static State RandomState()
    {
        return new State(Place.RandomPlace(), UnityEngine.Random.Range(0, 2) == 0);
    }
    public override string ToString()
    {
        return $"({place.x + 1}, {place.y + 1}, {(isClock ? "CW" : "CCW")})";
    }
}

internal struct Configuration
{
    internal int[] dxs;
    internal int[] dys;

    internal Configuration(int surpress)
    {
        dxs = new int[3];
        dys = new int[3];
        bool isFirstAssigned = false;
        bool isFirstX = false;
        for (var i = 0; i < 3; i++)
        {
            if (i == surpress)
            {
                dxs[i] = 0;
                dys[i] = 0;
                continue;
            }
            if(isFirstAssigned)
            {
                if(isFirstX)
                {
                    dys[i] = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
                    dxs[i] = 0;
                } else
                {
                    dxs[i] = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
                    dys[i] = 0;
                }
                continue;
            }

            isFirstAssigned = true;
            if(UnityEngine.Random.Range(0,2) == 0)
            {
                dxs[i] = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
                dys[i] = 0;
                isFirstX = true;
            } else
            {
                dys[i] = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
                dxs[i] = 0;
                isFirstX = false;
            }
        }
    }

    internal State ApplyToState(State prev, int index)
    {
        return prev.ApplyMovement(dxs[index], dys[index]);
    }

    internal State[] ApplyToStates(State[] prevs)
    {
        var a = this;
        return prevs.Select((prev, i) => a.ApplyToState(prev, i)).ToArray();
    }

    internal State[] ApplyToStates(State[] prevs, int count)
    {
        for(int i = 0; i < count; i++)
        {
            prevs = ApplyToStates(prevs);
        }
        return prevs;
    }

    public override string ToString()
    {
        return $"Red ({ToString(0)}), Green({ToString(1)}), Blue({ToString(2)})";
    }

    string ToString(int i)
    {
        if (dxs[i] == 0 && dys[i] == 0) return "switch rotation";
        if (dxs[i] != 0) return $"x+={dxs[i]}";
        return $"y+={dys[i]}";
    }
}