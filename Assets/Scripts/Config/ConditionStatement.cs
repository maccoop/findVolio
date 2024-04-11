using System;

[System.Serializable]
public enum Logical
{
    AND, OR, NOT
}

/// <summary>
/// string use Mod as string.Contains
/// </summary>
[System.Serializable]
public enum Condition
{
    Equals = 0, Smaller = 1, Larger = 2, Mod = 3, SmallerOrEquals = 4, LargerOrEquals = 5
}

[System.Serializable]
public enum Statement
{
    NotYet = 0, True = 1, False = 2
}

[System.Serializable]
public struct ConditionStatement
{
    public Logical logical;
    public Condition condition;

    public ConditionStatement(Logical logical, Condition condition)
    {
        this.logical = logical;
        this.condition = condition;
    }

    public Statement GetStatement<T>(T value1, T value2) where T : IConvertible
    {
        var provider = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
        switch (condition)
        {
            case Condition.Mod:
                {
                    var result = value1.ToByte(provider) % value2.ToByte(provider) == 0;
                    return GetResult(result);
                }
            case Condition.Smaller:
                {
                    var result = value1.ToByte(provider) < value2.ToByte(provider);
                    return GetResult(result);
                }
            case Condition.SmallerOrEquals:
                {
                    var result = value1.ToByte(provider) <= value2.ToByte(provider);
                    return GetResult(result);
                }
            case Condition.Larger:
                {
                    var result = value1.ToByte(provider) > value2.ToByte(provider);
                    return GetResult(result);
                }
            case Condition.LargerOrEquals:
                {
                    var result = value1.ToByte(provider) >= value2.ToByte(provider);
                    return GetResult(result);
                }
            case Condition.Equals:
                {
                    return GetResult(value1.Equals(value2));
                }
        }
        return Statement.NotYet;
    }

    public Statement GetStatement(string value1, string value2)
    {
        switch (condition)
        {
            case Condition.Mod:
                {
                    var result = value1.Contains(value2);
                    return GetResult(result);
                }
            case Condition.Smaller:
                {
                    var result = value1.Length < value2.Length;
                    return GetResult(result);
                }
            case Condition.Larger:
                {
                    var result = value1.Length >= value2.Length;
                    return GetResult(result);
                }
            case Condition.Equals:
                {
                    return GetResult(value1.Equals(value2));
                }
        }
        return Statement.NotYet;
    }

    private readonly Statement GetResult(bool result)
    {
        if (logical == Logical.OR && result)
            return Statement.True;
        if (logical == Logical.AND && !result)
            return Statement.False;
        if (logical == Logical.NOT && result)
            return Statement.False;
        return Statement.NotYet;
    }
}

[System.Serializable]
public class SingleCondition<T> where T : IConvertible
{
    public ConditionStatement condition;
    public T require;

    public Statement GetStatement(T value)
    {
        return condition.GetStatement<T>(value, require);
    }
    public bool GetResult(T value)
    {
        return condition.GetStatement<T>(value, require) != Statement.False;
    }
}

[System.Serializable]
public class ConditionArray<T> where T : IConvertible
{
    public SingleCondition<T>[] conditions;

    public bool GetResult(T value)
    {
        var result = true;
        foreach (var e in conditions)
        {
            if (e.GetStatement(value) == Statement.False)
            {
                result = false;
                break;
            }
        }
        return result;
    }
}
