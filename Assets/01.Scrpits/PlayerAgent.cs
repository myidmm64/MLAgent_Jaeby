using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PlayerAgent : Agent
{
    [SerializeField]
    private float _normalSpeed = 4f;
    [SerializeField]
    private float _slowSpeed = 2f;

    private Animator _animator = null;
    private Rigidbody2D _rigid = null;

    private MoveState _moveState = MoveState.None;

    public override void Initialize()
    {
        _animator = transform.Find("Renderer").GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody2D>();
        _moveState = MoveState.Normal;
    }

    public override void OnEpisodeBegin()
    {
    }

    public override void CollectObservations(VectorSensor sensor)
    {
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        _moveState = actions.DiscreteActions.Array[0] == 1 ? MoveState.Slow : MoveState.Normal;

        float speed = _normalSpeed;
        if(_moveState == MoveState.Slow)
            speed = _slowSpeed;

        _rigid.velocity = new Vector2(actions.ContinuousActions[0], actions.ContinuousActions[1]).normalized * speed;
        int hori = Mathf.RoundToInt(actions.ContinuousActions[0]);
        _animator.SetInteger("Horizontal", hori);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuous = actionsOut.ContinuousActions;
        continuous.Array[0] = Input.GetAxisRaw("Horizontal");
        continuous.Array[1] = Input.GetAxisRaw("Vertical");

        var discreate = actionsOut.DiscreteActions;
        discreate.Array[0] = Input.GetKey(KeyCode.LeftShift) ? 1 : 0;
    }
}

public enum MoveState
{
    None,
    Normal,
    Slow
}
