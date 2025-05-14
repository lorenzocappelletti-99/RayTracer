namespace Trace;

public class Pcg
{
    public ulong State;
    public ulong Inc;

    public Pcg(ulong initialState = 42, ulong initialSeq = 54)
    {
        State = 0;
        Inc = (initialSeq << 1) | 1;
        Random();
        State += initialState;
        Random();
    }

    /// <summary>
    /// Returns a 32-bit unsigned pseudorandom number using PCG algorithm.
    /// </summary>
    /// <returns></returns>
    public uint Random()
    {
        var oldState = State;

        State = (oldState * 6364136223846793005 + Inc);

        var xorShifted = (uint)(((oldState >> 18) ^ oldState) >> 27);

        //this needs to be cast int AFTER the shift. Otherwise, it might shift badly.
        var rot = oldState >> 59;

        return (xorShifted >> (int)rot) | (xorShifted << ((-(int)rot) & 31));
    }

    /// <summary>
    /// converting to float 2^32 (0xffffffff) prevents function loss
    /// and actually gives a uniform number between 0 and 1 and not mostly 0 or 1.
    /// </summary>
    /// <returns></returns>
    public float Random_float()
    {
        return Random()/ (float)0xffffffff;
    }
    
}