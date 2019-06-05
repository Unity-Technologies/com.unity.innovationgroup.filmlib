// The casts to object in the below code are an unfortunate necessity due to
// C#'s restriction against a where T : Enum constraint. (There are ways around
// this, but they're outside the scope of this simple illustration.)

/*
 * The traditional way to do this is to use the Flags attribute on an enum:

    [Flags]
    public enum Names
    {
        None = 0,
        Susan = 1,
        Bob = 2,
        Karen = 4
    }
Then you'd check for a particular name as follows:

    Names names = Names.Susan | Names.Bob;

    // evaluates to true
    bool susanIsIncluded = (names & Names.Susan) != Names.None;

    // evaluates to false
    bool karenIsIncluded = (names & Names.Karen) != Names.None;
    This would allow me to rewrite the above code as:

    Names names = Names.Susan | Names.Bob;

    bool susanIsIncluded = FlagsHelper.IsSet(names, Names.Susan);

    bool karenIsIncluded = FlagsHelper.IsSet(names, Names.Karen);
Note I could also add Karen to the set by doing this:

FlagsHelper.Set(ref names, Names.Karen);
And I could remove Susan in a similar way:

FlagsHelper.Unset(ref names, Names.Susan);
*As Porges pointed out, an equivalent of the IsSet method above already exists in .NET 4.0: Enum.HasFlag. The Set and Unset methods don't appear to have equivalents, though; so I'd still say this class has some merit.
* 
*/

public static class BitFlags
{
    public static bool IsSet<T>(T flags, T flag) where T : struct
    {
        int flagsValue = (int)(object)flags;
        int flagValue = (int)(object)flag;

        return (flagsValue & flagValue) != 0;
    }

    public static void Set<T>(ref T flags, T flag) where T : struct
    {
        int flagsValue = (int)(object)flags;
        int flagValue = (int)(object)flag;

        flags = (T)(object)(flagsValue | flagValue);
    }

    public static void Unset<T>(ref T flags, T flag) where T : struct
    {
        int flagsValue = (int)(object)flags;
        int flagValue = (int)(object)flag;

        flags = (T)(object)(flagsValue & (~flagValue));
    }
}

