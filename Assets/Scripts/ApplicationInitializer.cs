using System.Collections;
using Match3.Field;
using UnityEngine;

namespace Match3
{
    public class ApplicationInitializer : MonoBehaviour
    {
        [Header(@"Field settings")]
        [SerializeField] private int _fieldWidth;
        [SerializeField] private int _fieldHeight;
        [SerializeField] private CellGridPresenter cellGridPresenter;

        [Header(@"Random operations")]
        [SerializeField] private uint _numberRandomMoves;
        [SerializeField] private float _timeBetweenOperations = 0.01f;

        
        private void Awake()
        {
            var grid = new CellGrid( new Vector2Int( _fieldWidth, _fieldHeight ), new FieldGenerator() );
            
            cellGridPresenter.Init( grid );

            MakeRandomMoves( grid );
        }

        private void MakeRandomMoves( CellGrid cellGrid )
        {
            if ( _timeBetweenOperations <= 0 )
            {
                for ( int i = 0; i < _numberRandomMoves; i++ )
                {
                    MakeMoves( cellGrid );
                }
            }
            else
            {
                StartCoroutine( MoveIteration( _numberRandomMoves, cellGrid, _timeBetweenOperations ) );
            }
        }

        private IEnumerator MoveIteration( uint moveCount, CellGrid cellGrid, float pause )
        {
            uint moveCounter = moveCount;

            while ( moveCounter > 0 )
            {
                MakeMoves( cellGrid );
                yield return new WaitForSeconds( pause );
                moveCounter--;
            }
        }

        private void MakeMoves( CellGrid cellGrid )
        {
            int x = Random.Range( 1, _fieldWidth - 1);
            int y = Random.Range( 1, _fieldHeight - 1 );

            Vector2Int firstCellIndex = new Vector2Int( x, y );

            cellGrid.SelectCell( firstCellIndex );

            Vector2Int direction = Vector2Int.zero;
            switch ( Random.Range( 0, 3 ) )
            {
                case 0:
                    direction = Vector2Int.down;
                    break;
                
                case 1:
                    direction = Vector2Int.up;
                    break;
                
                case 2:
                    direction = Vector2Int.left;
                    break;
                
                case 3:
                    direction = Vector2Int.right;
                    break;
            }
            
            cellGrid.SelectCell( firstCellIndex + direction );
        }
    }
}