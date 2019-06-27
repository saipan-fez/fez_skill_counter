using System;

namespace FEZSkillUseCounter.Recognizer
{
    interface IResettableRecognizer<T> : IRecognizer<T>
    {
        event EventHandler<T> Updated;

        void Reset();
    }
}
