using System;

namespace FEZSkillCounter.Recognizer
{
    interface IResettableRecognizer<T> : IRecognizer<T>
    {
        event EventHandler<T> Updated;

        void Reset();
    }
}
