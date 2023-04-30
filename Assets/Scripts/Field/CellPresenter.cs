using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Match3.Field
{
    public sealed class CellPresenter : MonoBehaviour, IPointerClickHandler
    {
        public event Action< Vector2Int > OnClicked;

        [SerializeField] private Image _image;

        private Color _baseColor;
        private Color _selectedColor;

        private ICell _cell;

        public void Init( Vector2 initPosition, ICell cell, Vector2 cellSize )
        {
            if ( _cell != null )
            {
                _cell.OnSelectChanged -= OnSelectedChangedHandler;
                _cell.OnDestroy -= OnDestroyHandler;
            }

            _cell = cell;
            
            _cell.OnSelectChanged += OnSelectedChangedHandler;
            _cell.OnDestroy += OnDestroyHandler;
            
            var cellTransform = (RectTransform)transform;
            cellTransform.sizeDelta = cellSize;
            cellTransform.anchoredPosition = initPosition;

            _baseColor = _cell.Color;
            _selectedColor = _cell.Color;
            _selectedColor.a = 0.5f;
            _image.color = _cell.Color;

            gameObject.name = $@"Cell {_cell.Index.x}X{_cell.Index.y}";
            if ( _cell.IsDestroyed ) 
                OnDestroyHandler( );
        }

        private void OnDestroyHandler()
        {
            Color destroyedColor = _baseColor;
            destroyedColor.a = 0.2f;
            _image.color = destroyedColor;
        }

        private void OnSelectedChangedHandler( bool isSelected ) => _image.color = isSelected ? _selectedColor : _baseColor;

        public void OnPointerClick( PointerEventData eventData ) => OnClicked?.Invoke( _cell.Index );
    }
}