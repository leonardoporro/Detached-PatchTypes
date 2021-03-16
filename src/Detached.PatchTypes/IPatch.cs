namespace Detached.PatchTypes
{
    public interface IPatch
    {
        void Reset();

        bool IsSet(string name);
    } 
}