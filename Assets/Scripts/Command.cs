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
    public int repeats;

    public Command(CommandType type)
    {
        this.type = type;
        this.dir = Vector2Int.zero;
        this.repeats = 0;
    }

    public Command(CommandType type, Vector2Int dir, int repeats)
    {
        this.type = type;
        this.dir = dir;
        this.repeats = repeats;
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
        return new Command(command.type, -command.dir, command.repeats);
    }

    Command RepeatResult(Command command)
    {
        return new Command(command.type, command.dir, command.repeats * 2);
    }

}