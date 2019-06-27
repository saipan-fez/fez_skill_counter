using System.Drawing;

namespace FEZSkillCounter.Recognizer
{
    public interface IRecognizer<T>
    {
        T Recognize(Bitmap bitmap);
    }
}
