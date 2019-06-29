using System.Drawing;

namespace FEZSkillUseCounter.Recognizer
{
    internal interface IRecognizer<T>
    {
        T Recognize(Bitmap bitmap);
    }
}
