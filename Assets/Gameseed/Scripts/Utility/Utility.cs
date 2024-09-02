using UnityEngine;

public enum PlayerState { PlayerIddle, PlayerMoving, PlayerDodging, PlayerAttack, PlayerAction }
public enum EnemyState { EnemyWaiting, EnemyChasePlayer, EnemyBackToOriginPos, EnemyAttack, EnemyOnDamage, EnemyPatrol }
public static class Utility
{
    private static Matrix4x4 _isoMatrix(Vector3 rot) => Matrix4x4.Rotate(Quaternion.Euler(rot));
    public static Vector3 ToIso(this Vector3 input, Vector3 rot) => _isoMatrix(rot).MultiplyPoint3x4(input);
    public static bool CheckHitable(string from, string to)
    {
        bool result = false;
        switch (from)
        {
            case "Player Attack":
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
            case "Enemy Attack":
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
