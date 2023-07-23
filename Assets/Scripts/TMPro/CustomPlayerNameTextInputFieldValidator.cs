using UnityEngine;
using System;

namespace TMPro
{
    [Serializable]
    [CreateAssetMenu(fileName = "InputValidator - PlayerName.asset", menuName = "TextMeshPro/Input Validators/Player Name")]
    public class CustomPlayerNameTextInputFieldValidator : TMP_InputValidator
    {
        public override char Validate(ref string text, ref int pos, char ch)
        {
            if (char.IsLetter(ch) || char.IsDigit(ch))
            {
                text += ch;
                pos += 1;
                return ch;
            }
            return (char)0;
        }
    }
}