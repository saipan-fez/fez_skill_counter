using System.Drawing;

namespace SkillUseCounter.Recognizer
{
    internal interface IRecognizer<T>
    {
        T Recognize(Bitmap bitmap);
    }
}
