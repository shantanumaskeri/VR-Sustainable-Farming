using UnityEngine;
public class S_Transform
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
    public S_Transform GTransform;

    public S_Transform() { }
    public S_Transform(Transform _transform)
    {
        Position = _transform.position;
        Rotation = _transform.rotation;
        Scale = _transform.localScale;
    }
    public S_Transform(Vector3 _postion, Quaternion _rotation, Vector3 _scale)
    {
        Position = _postion;
        Rotation = _rotation;
        Scale = _scale;
    }
}