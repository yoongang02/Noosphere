using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class InputFieldStructure
{
   public string inputId;
   public string questionText;
   public string correctAnswer;
   public string inputWrong;
   public string inputCorrect;

   public bool isSolved = false;
}
