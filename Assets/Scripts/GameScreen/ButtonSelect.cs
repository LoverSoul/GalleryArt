using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GA.GameScreen
{
    public class ButtonSelect : MonoBehaviour
    {
        [SerializeField] private GameObject selectedVisual;

        public void Select(bool isSelected) 
        {
            selectedVisual.SetActive(isSelected);
        }
    }
}
