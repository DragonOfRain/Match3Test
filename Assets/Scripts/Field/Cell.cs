using System;
using UnityEngine;

namespace Match3.Field
{
    public sealed class Cell : ICell
    {
        public event Action< bool > OnSelectChanged;
        public event Action OnDestroy;

        public Vector2Int Index { get; set; }
        public Color Color { get; private set; }

        public bool IsSelected
        {
            get => _isSelected;
            private set
            {
                if ( _isSelected == value ) return;
                
                _isSelected = value;
                OnSelectChanged?.Invoke( _isSelected );
            }
        }

        public bool IsDestroyed { get; private set; }

        private bool _isSelected;

        public Cell( Vector2Int initIndex, Color color )
        {
            Index = initIndex;
            Color = color;
        }

        public void SetSelected( bool isSelected ) => IsSelected = isSelected;

        public void Destroy()
        {
            IsDestroyed = true;
            OnDestroy?.Invoke();
        }
    }
}

