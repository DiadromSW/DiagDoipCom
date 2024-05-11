namespace DiagCom.Uds.Iterator
{
    public interface ISequenceIterator<T>
    {
        T First();
        T Next();
        bool IsDone();
        T CurrentItem { get; }
        int Current { get; set; }
    }
}
