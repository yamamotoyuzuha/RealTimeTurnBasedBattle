using UnityEngine;
using Cinemachine;

public class TurnCameraAngle : MonoBehaviour
{
    [Header("")]
    [SerializeField] private CharacterCameraSettings[] _characterCameraSettings;
    
    //TODO：これOffSetはいじらずにキャラクター側に新しいスクリプトを作成してやるほうがいいかも
    //TODO：そのスクリプトからカメラ切り替えを行うって感じでいいような気がする
    //TODO：そうなるとこのスクリプトの存在意義とは？？？
}
