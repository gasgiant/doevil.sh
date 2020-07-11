using UnityEngine;

public enum CommandType
{
    Empty,
    Move,
    Rotate
}

[System.Serializable]
public struct Command
{
    public CommandType type;
    public Vector2Int dir;
    public int steps;

    public Command(CommandType type)
    {
        this.type = type;
        this.dir = Vector2Int.zero;
        this.steps = 0;
    }

    public Command(CommandType type, Vector2Int dir, int steps)
    {
        this.type = type;
        this.dir = dir;
        this.steps = steps;
    }
}

public enum OverrideType
{
    None,
    Skip,
    Invert,
    Repeat
}

[System.Serializable]
public class Override
{
    public OverrideType type;

    public Override(OverrideType type)
    {
        this.type = type;
    }

    public Command GetResult(Command command)
    {
        switch (type)
        {
            case OverrideType.Skip:
                return SkipResult(command);
            case OverrideType.Invert:
                return InvertResult(command);
            case OverrideType.Repeat:
                return RepeatResult(command);
        }
        return command;
    }

    Command SkipResult(Command command)
    {
        return new Command(CommandType.Empty);
    }

    Command InvertResult(Command command)
    {
        return new Command(command.type, -command.dir, command.steps);
    }

    Command RepeatResult(Command command)
    {
        return new Command(command.type, command.dir, command.steps * 2);
    }

}