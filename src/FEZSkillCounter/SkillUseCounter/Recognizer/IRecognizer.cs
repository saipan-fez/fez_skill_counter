using System.Drawing;

namespace FEZSkillUseCounter.Recognizer
{
    public interface IRecognizer<T>
    {
        T Recognize(Bitmap bitmap);
    }
}
