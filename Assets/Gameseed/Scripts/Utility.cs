using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayerState { PlayerIddle, PlayerMoving, PlayerDodging, PlayerAttack, PlayerAction }
public static class Utility
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
    public static bool CheckHitable(string from, string to)
    {
        bool result = false;
        switch (from)
        {
            case "Player":
                switch (to)
                {
                    case "Enemy":
                        result = true;
                        break;
                    default:
                        result = false;
                        break;
                }
                break;
            case "Enemy":
                switch (to)
                {
                    case "Player":
                        result = true;
                        break;
                    default:
                        result = false;
                        break;
                }
                break;
        }
        return result;
    }
}
