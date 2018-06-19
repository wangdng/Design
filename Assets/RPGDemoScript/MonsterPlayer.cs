using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MonsterPlayer : CharactorBase
{
    private Vector2 joyStickData;
    Animator anim;

    private MonsterPlayerData playerData;

    public override void ProcessEvent(MsgBase msgbase)
    {
        switch (msgbase.MsgId)
        {
            case (ushort)CharactorDataEvent.joyStickBegin:
                {
                   
                    break;
                }
            case (ushort)CharactorDataEvent.joyStick:
                {
                    bool isIdle = anim.GetCurrentAnimatorStateInfo(0).IsName("Idle");
                    bool isRun = anim.GetCurrentAnimatorStateInfo(0).IsName("Run");
                    if (isIdle || isRun)
                    {
                        JoyStickMsg msg = (JoyStickMsg)msgbase;

                        joyStickData = msg.GetjoyStickData();

                        PlayerMove(joyStickData);
                    }
                    break;
                }
                case(ushort)CharactorDataEvent.joyStickEnd:
                {
                    moveMsg.ChangeMsgId((ushort)CharactorAnimEvent.Idle);
                    SendMessage(moveMsg);
                    break;
                }
            case (ushort)CharactorDataEvent.ReduceBlood:
                {
                    MsgFloat bloodMsg = (MsgFloat)msgbase;

                    playerData.ReduceBlood(bloodMsg.GetValue());

                    float curBlood = playerData.GetBlood();

                    float UIbloodReduce = curBlood / 100.0f;

                    MsgFloat UIbloodMsg = new MsgFloat();

                    UIbloodMsg.ChangeMsgId((ushort)UIEvent.BloodLose);

                    UIbloodMsg.SetValue(UIbloodReduce);

                    SendMessage(UIbloodMsg);

                    break;
                }
            case (ushort)CharactorDataEvent.MonsterPlayerDead:
                {
                    MsgBase deadMsg = new MsgBase((ushort)CharactorAnimEvent.Die);

                    SendMessage(deadMsg);
                    break;
                }
        }
    }

    Vector3 tmpRotationVector = Vector3.zero;

    Vector3 tmpMoveVector = Vector3.zero;

    CharacterController cc;

    RunMsg moveMsg;

    void PlayerMove(Vector2 data)
    {
        float dx = Mathf.Abs(data.x);
        float dy = Mathf.Abs(data.y);

        //求出此时joystick的半径做为行动的速度
        float speed = Mathf.Sqrt(dx * dx + dy * dy) ;

        //向动画MonsterAnim发送行走播放的权重
        moveMsg.ChangeMsgId((ushort)CharactorAnimEvent.Run);

        moveMsg.ChangeValue(speed);
       
        //实际上就是将joystick平铺在和人物一样的水平面上,那么joystick的x和y的分量就可以表示人物的x和z的分量
        tmpMoveVector.x = data.x;
        tmpMoveVector.z = data.y;

        //4是快慢调整的系数,可以随意配置
        cc.SimpleMove(tmpMoveVector * speed * 4);

        //获得joystick传过来的2维坐标然后求出正切(正切是唯一的,在0-360度范围内,所以这里用正切)
        float rad = Mathf.Atan2(data.x, data.y);

        //将弧度转换为角度
        float degree = rad * Mathf.Rad2Deg;

        //将角度赋值给模型的y轴
        tmpRotationVector.y = degree;

        //用四元素来控制模型的旋转
        transform.rotation = Quaternion.Euler(tmpRotationVector);

        //向动画MonsterAnim发送行走播放的权重
        SendMessage(moveMsg);
    }

    private void Awake()
    {
        msgIds = new ushort[]
            {
                 (ushort)CharactorDataEvent.joyStickBegin,
                (ushort)CharactorDataEvent.joyStick,
                (ushort) CharactorDataEvent.joyStickEnd,
                (ushort) CharactorDataEvent.ReduceBlood,
                 (ushort) CharactorDataEvent.MonsterPlayerDead,
            };

        RegisterSelf(this, msgIds);

        cc = GetComponent<CharacterController>();

        moveMsg = new RunMsg((ushort)CharactorAnimEvent.Run,0);

        playerData = new MonsterPlayerData(100);

        anim = GetComponent<Animator>();
    }
}
