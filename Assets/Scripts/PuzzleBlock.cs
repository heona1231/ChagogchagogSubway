//using UnityEngine;

//public class PuzzleBlock : MonoBehaviour
//{
//    [Header("Current")]
//    [SerializeField] private Vector2 currentPosition;
//    [SerializeField] private float currentRotation;

//    [Header("Answer")]
//    [SerializeField] private Vector2 correctPosition;
//    [SerializeField] private float correctRotation;

//    public bool IsCorrectPlaced()
//    {
//        bool isPositionCorrect = Vector2.Distance(currentPosition, correctPosition) < 0.1;
//        bool isRotationCOrrect = Mathf.Abs(currentRotation - correctRotation) < 1f;

//        return isPositionCorrect && isRotationCOrrect;
//    }
//}
