namespace Kinectitude.Core.Data
{
    public abstract class RepeatReader : ValueReader
    {
        protected  ValueReader Reader;

        internal override double GetDoubleValue() { return Reader.GetDoubleValue(); }
        internal override float GetFloatValue() { return Reader.GetFloatValue(); }
        internal override int GetIntValue() { return Reader.GetIntValue(); }
        internal override long GetLongValue() { return Reader.GetLongValue(); }
        internal override bool GetBoolValue() { return Reader.GetBoolValue(); }
        internal override string GetStrValue() { return Reader.GetStrValue(); }
        internal override PreferedType PreferedRetType() { return Reader.PreferedRetType(); }
    }
}
