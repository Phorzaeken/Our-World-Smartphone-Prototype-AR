using System;
using Mapbox.Utils;
using OurWorld.Scripts.DataModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OurWorld.Scripts.Views.ParksList
{
    public class ParkDisplayElement : MonoBehaviour
    {

        [SerializeField] private TMP_Text _placeNameText, _distanceText;
        [SerializeField] private Button _interactionButton;

        private POIData _data;

        private Action<POIData> _onClickAction;


        #region Engine Methods
        private void OnEnable()
        {
            _interactionButton.onClick.AddListener(OnButtonPressed);
        }
        private void OnDisable()
        {
            _interactionButton.onClick.RemoveListener(OnButtonPressed);
        }
        #endregion

        public void Initialize(POIData data, Action<POIData> onClickAction)
        {
            _data = data;
            _onClickAction = onClickAction;
            SetVisuals(_data);
        }
        private void SetVisuals(POIData data)
        {
            _placeNameText.text = data.Name;
            var distanceText = data.Distance < 1 ? $"{(data.Distance * 1000):F0}m" : $"{data.Distance:F2}Km"; 
            _distanceText.text = distanceText;
        }
        private void OnButtonPressed()
        {
            _onClickAction?.Invoke(_data);
        }
    }
}