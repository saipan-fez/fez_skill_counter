using System;

namespace FEZSkillUseCounter.Recognizer
{
    internal interface IResettableRecognizer<T> : IRecognizer<T>
    {
        event EventHandler<T> Updated;

        void Reset();
    }
}
