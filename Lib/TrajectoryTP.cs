using KeepCoding;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

class TrajectoryTP : TPScript<TrajectoryModule>
{
    public override IEnumerator ForceSolve()
    {
        Module.Display.Get<KMSelectable>().OnInteract();
        yield return YieldUntil(true, () => Module.Display.isActive);
        Module.Display.ResetButton.OnInteract();
        for (var i = 0; i < 3; i++)
        {
            for(var j = 0; j < (4 - Module.puzzle.possibleMovements[i])%4; j++)
            {
                Module.Display.ConfigButtons[i].OnInteract();
                yield return new WaitForSeconds(.1f);
            }
        }
        Module.Display.SubmitButton.OnInteract();
        Module.Display.Get<KMSelectable>().OnDefocus();
        yield return UntilSolve();
    }

    public override IEnumerator Process(string command)
    {
        yield return null;
        Module.isTP = true;
        string[] splitted = command
            .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(str => str.Trim().ToLower())
            .ToArray();

        if (splitted.Length == 0)
        {
            yield return SendToChatError("You must specify an argument.");
            yield break;
        }
        if (splitted.Length == 1)
        {
            IEnumerator c;
            switch(splitted[0])
            {
                case "reset":
                    c = DisplayAction(new KMSelectable[] { Module.Display.ResetButton });
                    break;
                case "submit":
                    yield return Module.puzzle.Check() ? Solve : Strike;
                    c = DisplayAction(new KMSelectable[] { Module.Display.SubmitButton });
                    break;
                default:
                    yield return SendToChatError("Invalid Argument.");
                    yield break;
            }
            while (c.MoveNext()) yield return c.Current;
            yield break;
        }
        if(splitted.Length == 3 && splitted[0] == "setspeed")
        {
            if(float.TryParse(splitted[1], out float xspeed) && float.TryParse(splitted[2], out float yspeed))
            {
                if(xspeed > 1 || yspeed > 1)
                {
                    yield return SendToChatError("Speeds must less than 1.");
                    yield break;
                }
                Module.Drawing.isTP = true;
                Module.Drawing.xSpeed = xspeed;
                Module.Drawing.ySpeed = yspeed;
                yield break;
            }
            yield return SendToChatError("Speeds must be numbers");
            yield break;
        }

        if (splitted.Length >= 2 && splitted[0] == "press")
        {
            if (splitted.Length > 12)
            {
                yield return SendToChatError("Maximum number of presses is 12.");
                yield break;
            }
            if(!splitted.Skip(1).All(input => input == "a" || input == "b" || input == "c"))
            {
                yield return SendToChatError("Buttons must be A, B or C");
                yield break;
            }
            var c = DisplayAction(splitted.Skip(1).Select(input => Module.Display.ConfigButtons[input.ToCharArray()[0] - 'a']).ToArray());
            while (c.MoveNext()) yield return c.Current;
        }
    }
    public IEnumerator DisplayAction(KMSelectable[] selects)
    {
        Module.Display.Get<KMSelectable>().OnInteract();
        yield return YieldUntil(true, () => Module.Display.isActive);
        yield return selects;
        Module.Display.Get<KMSelectable>().OnDefocus();
    }
}
