using RPiLCDShared.Services;
using System.Text;

namespace RPiLCDClientConsole.Services
{
    public class UpdateableService
    {
        protected StringBuilder stringBuilder= new StringBuilder();

        protected string TagifyString(string message, Tag tag)
        {
            stringBuilder.Clear();
            stringBuilder.Append(tag.TagOpen);
            stringBuilder.Append(message);
            stringBuilder.Append(tag.TagClose);

            return stringBuilder.ToString();
        }
    }
}
