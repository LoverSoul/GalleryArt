using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GA.GameScreen
{
    public class ButtonSelect : MonoBehaviour
    {
        [SerializeField] private GameObject selectedVisual;
        [SerializeField] private TMP_Text text;
        [SerializeField] private Color selectedColor;
        public void Select(bool isSelected) 
        {
            selectedVisual.SetActive(isSelected);
            text.color = isSelected ? selectedColor : Color.black;
        }
    }
}
