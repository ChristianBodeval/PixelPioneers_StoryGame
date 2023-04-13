using UnityEngine;

[CreateAssetMenu(fileName = "HasTalkedTo", menuName = "HasTalkedTo", order = 0)]
public class HasTalkedTo : ScriptableObject
{
    public bool[] hasTalkedToArr = new bool[5];
}